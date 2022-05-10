using Core.Models;
using FunctionApp.Integration.External.Model.Typeform;
using FunctionApp.Integration.External.Typeform.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Tradefact.Data;

namespace FunctionApp.Integration.External.Typeform
{
    public partial class TypeFormHooks
    {
        private readonly TradefactDbContext _context;
        //NewsWeekFieldId
        private static string week_identifier = Environment.GetEnvironmentVariable("NewsWeekFieldId", EnvironmentVariableTarget.Process);

        public TypeFormHooks(TradefactDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [FunctionName(nameof(ShippingNewsWebHook))]
        public async Task<IActionResult> ShippingNewsWebHook(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger logger)
        {
            if (req.Headers.TryGetValue("Typeform-Signature", out var signature))
            {
                string payload = await new StreamReader(req.Body).ReadToEndAsync();

                if (IsValidTypeformSignature(payload, signature, secret))
                {
                    try
                    {
                        Response response = JsonConvert.DeserializeObject<Response>(payload);

                        int weekNo = (response.FormResponse.FormAnswers.FirstOrDefault(x => x.Field.Id == week_identifier).Number > 0) ? response.FormResponse.FormAnswers.FirstOrDefault(x => x.Field.Id == week_identifier).Number : DateTime.Now.GetIso8601WeekOfYear();
                        ShippingNewsModel news = new ShippingNewsModel { WeekNo = weekNo };
                        foreach (var f in response.FormResponse.FormDefinition.Fields.Where(q=>q.Id != week_identifier))
                        {
                            FormAnswer answer = response.FormResponse.FormAnswers.FirstOrDefault(x => x.Field.Id == f.Id);
                            if (!String.IsNullOrWhiteSpace(answer?.Text))
                            {
                                news.Sections.Add(new ShippingNewsSection(f.Ref, f.Title, answer.Text));
                            }
                        }

                        NewsFeed feed = new NewsFeed { Id = Guid.NewGuid(), WeekNo = news.WeekNo, Sections = new List<NewsSection>() };
                        foreach (var section in news.Sections)
                        {
                            if (section.Items != null)
                            {
                                NewsSection ns = new NewsSection
                                {
                                    NewsFeedId = feed.Id,
                                    Title = section.Title,
                                    Reference = section.Reference,
                                    Items = section.Items.Select((item, index) => new NewsItem { NewsFeedId = feed.Id, NewsSectionReference = section.Reference, SeqNo = index, Text = item.Trim() }).ToList()
                                };
                                feed.Sections.Add(ns);
                            }
                        }

                        _context.NewsFeeds.Add(feed);
                        _ = await _context.SaveChangesAsync();

                        return new OkObjectResult("OK");
                    }
                    catch (Exception ex)
                    {
                        return new BadRequestResult();
                    }
                }
            }
            return new BadRequestResult();
        }

    }

    public static class DateExtensions
    {
        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetIso8601WeekOfYear(this DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
    }

}

