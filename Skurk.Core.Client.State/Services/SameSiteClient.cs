using MediatR;
using MudBlazor;
using Newtonsoft.Json;
using Skurk.Core.Client.State.Store.Common;
using Skurk.Core.Shared.Common;
using Skurk.Core.Shared.Interfaces;
using System.Net;
using System.Security.Cryptography;

namespace Skurk.Core.Client.State.Services
{
    public class SameSiteClient
    {
        private readonly HttpClient _client;
        private readonly RouteFinder _routeFinder;
        private readonly ISnackbar _snackbar;
        private readonly CommonState _state;


        public SameSiteClient(HttpClient client, RouteFinder routeFinder, ISnackbar snackbar, CommonState state)
        {
            _client = client;
            _routeFinder = routeFinder;
            _snackbar = snackbar;
            _state = state;
        }

        public async Task<RequestResult<R>> Send<R>(IMediatorRequest<RequestResult<R>> request, CancellationToken ct = default!)
        {

            string url;
            string msg = string.Empty;
            Severity svr;
            try
            {
                url = _routeFinder.RequestRoutes[request.GetType().Name];
            }
            catch
            {
                msg = "An error occured";
                svr= Severity.Error;
                _snackbar.Add(msg, svr);
                return RequestResult<R>.Fail(msg,
                        HttpStatusCode.InternalServerError,
                        new InvalidOperationException("Route is not registered in assembly"));
            }

            RequestResult<R> res = await request.Send(_client, url, ct);

            switch (res.HttpStatusCode)
            {
                case HttpStatusCode.OK:
                    msg = "Executed successfully";
                    svr = Severity.Success;
                    break;
                case HttpStatusCode.Unauthorized:
                    svr = Severity.Warning;
                    break;
                case HttpStatusCode.Forbidden:
                    svr= Severity.Warning;
                    break;
                case HttpStatusCode.BadRequest:
                    svr = Severity.Error;
                    break;
                case HttpStatusCode.InternalServerError:
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
                case HttpStatusCode.NotFound:
                    svr = Severity.Warning;
                    _state.IsOnline = false;
                    break;
                default:
                    msg = "Odd status received";
                    svr = Severity.Error;
                    break;
            }

            if(res.IsSuccess)
            {
                _snackbar.Add(msg, svr);
            }
            else
            {
                _snackbar.Add(res.FailureReason, svr);
            }
            

            return res;
        }
    }
}
