using Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tradefact.Data;
using Tradefact.Utilities.DataImport.Model;

namespace Tradefact.Utilities.DataImport
{
    partial class ReferenceDataService : IReferenceDataService
    {
        private readonly ILogger<ReferenceDataService> _logger;
        private readonly TradefactDbContext _context;

        public ReferenceDataService(ILoggerFactory loggerFactory, TradefactDbContext context)
        {
            _logger = loggerFactory.CreateLogger<ReferenceDataService>();
            _context = context;
        }


        public void ImportCarriers()
        {
            List<Carrier> existing = _context.Carriers.ToList();

            var file = Path.Combine(Directory.GetCurrentDirectory(), "referencedata", "carriers.json");
            string carrierdata = System.IO.File.ReadAllText(file);
            CarriersRootObject carriers = JsonConvert.DeserializeObject<CarriersRootObject>(carrierdata);
            foreach (CarrierDataItem c in carriers.items)
            {
                if (!existing.Any(q => q.SCAC == c.scac))
                {

                    _context.Carriers.Add(new Carrier { ShipmentTypeId = Core.Enums.ShipmentTypeEnum.SEA, SCAC = c.scac, Name = c.name, Description = c.description, CarrierGroup = c.carrier_group, Regions = c.regions });
                }
                else
                {
                    Carrier carrier = existing.FirstOrDefault(q => q.SCAC == c.scac);
                    carrier.Company = c.company;
                }
            }
            int rowsAffected = _context.SaveChanges();
        }
        public void ImportContainers()
        {
            List<ContainerType> existing = _context.ContainerTypes.ToList();

            var file = Path.Combine(Directory.GetCurrentDirectory(), "referencedata", "containers.json");
            string data = System.IO.File.ReadAllText(file);
            List<ContainerType> containers = JsonConvert.DeserializeObject<List<ContainerType>>(data);
            foreach (ContainerType c in containers)
            {
                if (!existing.Any(q => q.Code == c.Code))
                {

                    _context.Add(c);
                }
            }
            int rowsAffected = _context.SaveChanges();
        }
        public void ImportCurrencies()
        {
            List<Currency> existing = _context.Currency.ToList();

            var file = Path.Combine(Directory.GetCurrentDirectory(), "referencedata", "currencies.json");
            string data = System.IO.File.ReadAllText(file);
            List<CurrencyItem> currencyItems = JsonConvert.DeserializeObject<List<CurrencyItem>>(data);
            foreach (CurrencyItem c in currencyItems)
            {
                if (!existing.Any(q => q.CurrencyCode == c.Alphabetic_Code))
                {
                    Currency cu = new Currency
                    {
                        CurrencyCode = c.Alphabetic_Code,
                        Description = c.Currency,
                        CurrencyName = c.Currency

                    };
                    _context.Currency.Add(cu);
                }
            }
            int rowsAffected = _context.SaveChanges();
        }
        public void ImportExtendedCurrencyData()
        {
            List<Currency> existing = _context.Currency.ToList();

            List<CurrencyCountryInfo> found_countries = new List<CurrencyCountryInfo>();


            var file = Path.Combine(Directory.GetCurrentDirectory(), "referencedata", "ExtendedCurrencyData.json");
            string data = System.IO.File.ReadAllText(file);
            ExtendedCurrencyInformation extended_currency_data = JsonConvert.DeserializeObject<ExtendedCurrencyInformation>(data);



            foreach (ExtendedCurrency c in extended_currency_data.Currencies)
            {
                for (int i = 0; i < c.CountryKeywords.Count; i += 3)
                {
                    try
                    {
                        CurrencyCountryInfo country = new CurrencyCountryInfo { Code2 = c.CountryKeywords[i + 1].ToUpper(), Code3 = c.CountryKeywords[i + 2].ToUpper(), Name = c.CountryKeywords[i] };
                        c.Countries.Add(country);

                        if (!found_countries.Any(q => q.Code2 == country.Code2))
                        {
                            found_countries.Add(country);
                        }
                    }
                    catch (Exception exi)
                    {

                        throw exi;
                    }

                }
            }


            bool hasChanges = false;
            List<Country> tradefact_countries = _context.Countries.ToList();

            foreach (var found_country in found_countries)
            {
                Country c = tradefact_countries.FirstOrDefault(q => q.Code2 == found_country.Code2);
                if (c != null && c.Code3 != found_country.Code3)
                {
                    c.Code3 = found_country.Code3;
                    c.Name = found_country.Name;
                    hasChanges = true;
                }
            }
            if (hasChanges)
            {
                int rowsAffected = _context.SaveChanges();
            }

            List<Currency> tradefact_currencies = _context.Currency.ToList();

            //using (WebClient client = new WebClient())
            //{
            //    client.Headers.Add("user-agent", " Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0");
            //    foreach (ExtendedCurrency c in extended_currency_data.Currencies)
            //    {
            //        string url = $"https://wise.com/public-resources/assets/flags/rectangle/{c.Code.ToLower()}.png";

            //        client.DownloadFile(new Uri(url), @$"C:\Tradefact\CurrencyFlags\{c.Code}.png");
            //    }
            //}


            foreach (ExtendedCurrency c in extended_currency_data.Currencies)
            {
                Currency ce = tradefact_currencies.FirstOrDefault(q => q.CurrencyCode == c.Code);
                if (ce != null)
                {
                    ce.Active = true;
                    ce.Symbol = c.Symbol;
                    ce.CurrencyName = ce.Description = c.Name;
                    hasChanges = true;
                    ce.Countries = new List<CurrencyCountry>();
                    foreach (var cy in c.Countries)
                    {
                        if (tradefact_countries.Any(q => q.Code2 == cy.Code2))
                        {
                            ce.Countries.Add(new CurrencyCountry { Active = true, CurrencyId = ce.CurrencyId, CountryCode = cy.Code2 });
                        }
                        else
                        {
                            Console.WriteLine($"No Match: {cy.Code2} {cy.Name} ");
                        }
                    }
                }
            }
            if (hasChanges)
            {
                int rowsAffected = _context.SaveChanges();
            }

        }

        private class CurrencyItem
        {
            public string Alphabetic_Code { get; set; }
            public string Currency { get; set; }
        }

        private IEnumerable<CountryData> ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            foreach (string fileName in fileEntries)
                yield return ProcessFile(fileName);

            yield break;
        }

        // Insert logic for processing found files here.
        private CountryData ProcessFile(string path)
        {
            string readText = File.ReadAllText(path);
            CountryData country = JsonConvert.DeserializeObject<CountryData>(readText);

            return country;
        }

        public void ImportPorts()
        {
            List<Country> existing_countries = _context.Countries.ToList();
            try
            {
                IEnumerable<CountryData> countries = ProcessDirectory("C:\\Tradefact\\ports");
                foreach (var country in countries)
                {
                    Country c = new Country
                    {
                        Code2 = country.Code,
                        Name = country.Name,
                        Locations = new List<Location>()
                    };

                    List<Location> existing_locations = _context.Locations.Where(q=>q.CountryCode == c.Code2).ToList();


                    foreach (PortLocation l in country.Locations)
                    {
                        Location loc = new Location
                        {
                            LocCode = Regex.Replace(l.LocCode, @"\s+", ""),
                            Name = l.Name,
                            Port = l.Port,
                            Airport = l.Airport,
                            Rail = l.Rail,
                            IATA = l.IATA,
                            CountryCode = c.Code2
                        };

                        if (!existing_locations.Any(q=>q.LocCode == loc.LocCode))
                        {

                            if (l.Position != null)
                            {
                                loc.Position = new GeographicPosition
                                {
                                    Latitude = (double)l.Position.Latitude,
                                    Longitude = (double)l.Position.Longitude
                                };
                            }

                            if (!existing_locations.Any(q => q.LocCode == loc.LocCode))
                            {
                                existing_locations.Add(loc);
                                _context.Locations.Add(loc);
                            }
                        }
                    }
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }

        }


    }

    public interface IReferenceDataService
    {
        void ImportCarriers();

        void ImportContainers();

        void ImportCurrencies();

        void ImportPorts();

        void ImportExtendedCurrencyData();

        void ImportThomasPinkProductData();
    }
}
