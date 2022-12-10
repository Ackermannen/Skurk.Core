using MediatR;
using MudBlazor;
using Skurk.Core.Shared.Interfaces;
using Skurk.Core.Shared.Mediator;
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

        public async Task<R> Send<R>(IMediatorRequest<CommandResult<R>> request, CancellationToken ct = default!)
        {
            var res = await request.Send(_client, _routeFinder, ct);
            if(res.IsSuccess)
            {
                _snackbar.Add(res.HttpStatusCode == HttpStatusCode.Created ? "Created" : "Success", Severity.Success);
            } 
            else
            {
                _snackbar.Add(res.FailureReason, res.HttpStatusCode == HttpStatusCode.BadRequest ? Severity.Warning : Severity.Error);
            }
            return res.Value;
        }

        public async Task<R> Send<R>(IMediatorRequest<QueryResult<R>> request, CancellationToken ct = default!)
        {
            var res = await request.Send(_client, _routeFinder, ct);
            if (res.IsSuccess)
            {
                _snackbar.Add(res.HttpStatusCode == HttpStatusCode.NoContent ? "No item(s)" : "Success", Severity.Success);
            }
            else
            {
                _snackbar.Add(res.FailureReason, Severity.Error);
            }
            return res.Value;
        }
    }
}
