using MediatR;
using MudBlazor;
using Skurk.Core.Shared.Common;
using Skurk.Core.Shared.Interfaces;
using System.Net;

namespace Skurk.Core.Client.State.Services
{
    internal class SameSiteClient
    {
        private readonly HttpClient _client;
        private readonly RouteFinder _routeFinder;
        private readonly ISnackbar _snackbar;

        public SameSiteClient(HttpClient client, RouteFinder routeFinder, ISnackbar snackbar)
        {
            _client = client;
            _routeFinder = routeFinder;
            _snackbar = snackbar;
        }

        public async Task<R> Send<R>(IMediatorRequest<RequestResult<R>> request, CancellationToken ct = default!)
        {
            var res = await request.Send(_client, _routeFinder, ct);

            string msg;
            Severity svr;
            switch(res.HttpStatusCode)
            {
                case HttpStatusCode.OK:
                    msg = "Executed successfully";
                    svr = Severity.Success;
                    break;
                case HttpStatusCode.Unauthorized:
                    msg = "Unathorization needed";
                    svr = Severity.Warning;
                    break;
                case HttpStatusCode.Forbidden:
                    msg = "Permission missing";
                    svr= Severity.Warning;
                    break;
                case HttpStatusCode.BadRequest:
                    msg = "Request rejected";
                    svr = Severity.Error;
                    break;
                case HttpStatusCode.InternalServerError:
                    msg = "Server error";
                    svr = Severity.Error;
                    break;
                case HttpStatusCode.Created:
                    msg = "Created successfully";
                    svr = Severity.Success;
                    break;
                case HttpStatusCode.NoContent:
                    msg = "No content available";
                    svr = Severity.Info;
                    break;
                default:
                    msg = "Odd status received";
                    svr = Severity.Info;
                    break;
            }
            _snackbar.Add(msg, svr);
            return res.Value;
        }
    }
}
