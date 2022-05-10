using Microsoft.Extensions.Logging;
using Polly;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tradefact.Services.Mailgun.Services
{
    //public class MailgunService: IEmailSender
    //{
    //    private readonly ILogger<MailgunService> _logger;
    //    // private readonly CargoSmartOptions _options;
    //    private readonly IAsyncPolicy _policy;

    //    private readonly IMailgunAPI _client;

    //    //public MailgunService(CargoSmartOptions options, ILogger<MailgunService> logger, IMailgunAPI api)
    //    //{
    //    //    _options = options;
    //    //    _client = api;


    //    //    _policy = Policy
    //    //        .Handle<ApiException>(ex => ex.StatusCode == HttpStatusCode.RequestTimeout)
    //    //        .RetryAsync(_options.Retries, async (exception, retryCount) =>
    //    //        {
    //    //            _logger.LogDebug("Mailgun Service: Retrying, waiting first");
    //    //            await Task.Delay(500).ConfigureAwait(false);
    //    //        });
    //    //}

    //    //public async Task<CargoSmartScheduleResponse> GetScheduledFlightsByRoute(string departurePortCode, string arrivalPortCode, string departureFrom, string searchDuration, bool enableNearbySchedules = false) =>
    //    //    await _client.GetRouteSchedules(departurePortCode, arrivalPortCode, departureFrom, searchDuration, enableNearbySchedules).ConfigureAwait(false);


    //    //public async Task<TransitSchedule> GetScheduleAsync(TransitScheduleRequest request, CancellationToken cancellationToken = default)
    //    //{
    //    //    string departuredate = request.ScheduleDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
    //    //    try
    //    //    {

    //    //        CargoSmartScheduleResponse response = (request.Carriers != null && request.Carriers.Any()) switch
    //    //        {
    //    //            true => await _policy
    //    //            .ExecuteAsync(async () => await GetScheduledFlightsByRoute(request.PortOfLoading, request.PortOfDischarge, string.Join(",", request.Carriers), departuredate, _options.EnableNearbySchedules).ConfigureAwait(false))
    //    //            .ConfigureAwait(false),
    //    //            _ => await _policy
    //    //            .ExecuteAsync(async () => await GetScheduledFlightsByRoute(request.PortOfLoading, request.PortOfDischarge, departuredate, _options.SearchDuration.ToString(), _options.EnableNearbySchedules).ConfigureAwait(false))
    //    //            .ConfigureAwait(false)
    //    //        };

    //    //        TransitSchedule ts = new TransitSchedule();


    //    //        return ts;
    //    //    }
    //    //    catch (ValidationApiException validationException)
    //    //    {
    //    //        // handle validation here by using validationException.Content,
    //    //        // which is type of ProblemDetails according to RFC 7807

    //    //        // If the response contains additional properties on the problem details,
    //    //        // they will be added to the validationException.Content.Extensions collection.
    //    //    }
    //    //    catch (ApiException exception)
    //    //    {
    //    //        // other exception handling
    //    //    }
    //    //    return null;


    //    //}
    //}

}
