using MediatR;
using Newtonsoft.Json;
using Skurk.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Skurk.Core.Shared.Mediator
{
    public interface IQuery<TResponse> : IRequest<TResponse>
    {
        public Task<TResponse> Send(HttpClient client, RouteFinder routeFinder, CancellationToken ct = default);
    }

    public interface ICommand<TResponse> : IRequest<TResponse>
    {
        public Task<TResponse> Send(HttpClient client, RouteFinder routeFinder, CancellationToken ct = default);
    }

    internal static class RequestHelper
    {
        public static string BuildQueryString<T>(T obj) where T : class
        {
            var props = obj.GetType().GetProperties();

            return $"?{string.Join('&', props.Where(x => x.GetValue(obj) != null).Select(x => $"{HttpUtility.UrlEncode(x.Name)}={HttpUtility.UrlEncode(x.GetValue(obj)!.ToString())}"))}";
        }

        public static async Task<QueryResult<TResponse>> SendQueryStringExpectQuery<TResponse>(string url, string queryString, Func<string, CancellationToken, Task<HttpResponseMessage>> callback, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(url))
            {
                return QueryResult<TResponse>.Fail("An error occured",
                     QueryFailureHttpStatusCode.InternalServerError,
                     new InvalidOperationException("Route is not registered in assembly"));
            }

            HttpResponseMessage res;
            try
            {
                res = await callback.Invoke(url + queryString, ct);
            }
            catch (Exception e)
            {
                return QueryResult<TResponse>.Fail("Error contacting the server", QueryFailureHttpStatusCode.InternalServerError, e);
            }

            try
            {
                var deserializedResult = JsonConvert.DeserializeObject<QueryResult<TResponse>>(await res.Content.ReadAsStringAsync(ct));
                if (deserializedResult != null)
                {
                    return deserializedResult;
                }
                else
                {
                    throw new InvalidOperationException("Response data isn't parsable");
                }
            }
            catch (Exception e)
            {
                var statusCode = Enum.IsDefined(typeof(QueryFailureHttpStatusCode), (int)res.StatusCode) ? (QueryFailureHttpStatusCode)res.StatusCode : QueryFailureHttpStatusCode.InternalServerError;
                return QueryResult<TResponse>.Fail("Unrecognizable object format", statusCode, e);
            }
        }

        public static async Task<CommandResult<TResponse>> SendQueryStringExpectCommand<TResponse>(string url, string queryString, Func<string, CancellationToken, Task<HttpResponseMessage>> callback, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(url))
            {
                return CommandResult<TResponse>.Fail("An error occured",
                     CommandFailureHttpStatusCode.InternalServerError,
                     new InvalidOperationException("Route is not registered in assembly"));
            }

            HttpResponseMessage res;
            try
            {
                res = await callback.Invoke(url + queryString, ct);
            }
            catch (Exception e)
            {
                return CommandResult<TResponse>.Fail("Error contacting the server", CommandFailureHttpStatusCode.InternalServerError, e);
            }

            try
            {
                var deserializedResult = JsonConvert.DeserializeObject<CommandResult<TResponse>>(await res.Content.ReadAsStringAsync(ct));
                if (deserializedResult != null)
                {
                    return deserializedResult;
                }
                else
                {
                    throw new InvalidOperationException("Response data isn't parsable");
                }
            }
            catch (Exception e)
            {
                var statusCode = Enum.IsDefined(typeof(CommandFailureHttpStatusCode), (int)res.StatusCode) ? (CommandFailureHttpStatusCode)res.StatusCode : CommandFailureHttpStatusCode.InternalServerError;
                return CommandResult<TResponse>.Fail("Unrecognizable object format", statusCode, e);
            }
        }

        public static async Task<CommandResult<TResponse>> SendBodyExpectCommand<TResponse>(string url, StringContent stringContent, Func<string, StringContent, CancellationToken, Task<HttpResponseMessage>> callback, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(url))
            {
                return CommandResult<TResponse>.Fail("An error occured",
                     CommandFailureHttpStatusCode.InternalServerError,
                     new InvalidOperationException("Route is not registered in assembly"));
            }

            HttpResponseMessage res;
            try
            {
                res = await callback.Invoke(url, stringContent, ct);
            } catch (Exception e)
            {
                return CommandResult<TResponse>.Fail("Error contacting the server", CommandFailureHttpStatusCode.InternalServerError, e);
            }
            

            try
            {
                var deserializedResult = JsonConvert.DeserializeObject<CommandResult<TResponse>>(await res.Content.ReadAsStringAsync(ct));
                if (deserializedResult != null)
                {
                    return deserializedResult;
                }
                else
                {
                    throw new InvalidOperationException("Response data isn't parsable");
                }
            }
            catch (Exception e)
            {
                var statusCode = Enum.IsDefined(typeof(CommandFailureHttpStatusCode), (int)res.StatusCode) ? (CommandFailureHttpStatusCode)res.StatusCode : CommandFailureHttpStatusCode.InternalServerError;
                return CommandResult<TResponse>.Fail("Unrecognizable object format", statusCode, e);
            }
        }
    }

    public record Postable<TResponse> : ICommand<CommandResult<TResponse>>
    {
        public async Task<CommandResult<TResponse>> Send(HttpClient client, RouteFinder routeFinder, CancellationToken ct = default)
        {
            var url = routeFinder.RequestRoutes.First(x => GetType().Name == x.Key).Value;
            var content = new StringContent(JsonConvert.SerializeObject(this));
            return await RequestHelper.SendBodyExpectCommand<TResponse>(url,
                content,
                new Func<string, StringContent, CancellationToken, Task<HttpResponseMessage>>(client.PostAsync),
                ct);
        }
    }

    public record Putable<TResponse> : ICommand<CommandResult<TResponse>>
    {
        public async Task<CommandResult<TResponse>> Send(HttpClient client, RouteFinder routeFinder, CancellationToken ct = default)
        {
            var url = routeFinder.RequestRoutes.First(x => GetType().Name == x.Key).Value;
            var content = new StringContent(JsonConvert.SerializeObject(this));
            return await RequestHelper.SendBodyExpectCommand<TResponse>(url,
                content,
                new Func<string, StringContent, CancellationToken, Task<HttpResponseMessage>>(client.PutAsync),
                ct);
        }
    }

    public record Deletable<TResponse> : ICommand<CommandResult<TResponse>>
    {
        public async Task<CommandResult<TResponse>> Send(HttpClient client, RouteFinder routeFinder, CancellationToken ct = default)
        {
            var url = routeFinder.RequestRoutes.First(x => GetType().Name == x.Key).Value;
            var queryString = RequestHelper.BuildQueryString(this);
            return await RequestHelper.SendQueryStringExpectCommand<TResponse>(url,
                queryString,
                new Func<string, CancellationToken, Task<HttpResponseMessage>>(client.DeleteAsync),
                ct);
        }
    }

    public record Getable<TResponse> : IQuery<QueryResult<TResponse>>
    {
        public async Task<QueryResult<TResponse>> Send(HttpClient client, RouteFinder routeFinder, CancellationToken ct = default)
        {
            var url = routeFinder.RequestRoutes.First(x => GetType().Name == x.Key).Value;
            var queryString = RequestHelper.BuildQueryString(this);
            return await RequestHelper.SendQueryStringExpectQuery<TResponse>(url,
                queryString,
                new Func<string, CancellationToken, Task<HttpResponseMessage>>(client.GetAsync),
                ct);
        }
    }

    public interface IQueryHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
    {

    }

    public interface ICommandHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : ICommand<TResponse>
    {

    }

    public enum CommandFailureHttpStatusCode
    {
        /// <summary>Http status code used when bad input data is received.</summary>
        BadRequest = 400,
        /// <summary>Http status code used when the user tries accessing forbidden resources.</summary>
        Forbidden = 403,
        /// <summary>Http status code used when the server has an internal error.</summary>
        InternalServerError = 500,
    }

    public enum CommandSuccessHttpStatusCode
    {
        /// <summary>Http status code used when a reqest was successful.</summary>
        OK = 200,
        /// <summary>Http status code used when a new resource was created successfully</summary>
        Created = 201,
    }

    public enum QueryFailureHttpStatusCode
    {
        /// <summary>Http status code used when the user tries accessing forbidden resources.</summary>
        Forbidden = 403,
        /// <summary>Http status code used when the server has an internal error.</summary>
        InternalServerError = 500,
    }

    public enum QuerySuccessHttpStatusCode
    {
        /// <summary>Http status code used when a reqest was successful.</summary>
        OK = 200,
        /// <summary>Http status code used when a process was executed correctly, but contained no content.</summary>
        NoContent = 204,
    }

    public abstract record MediatorResult<TResponse, TSuccessStatusCodes, TFailureStatusCodes>
    {
        public TResponse Value { get; protected set; } = default!;
        public string FailureReason { get; protected set; } = string.Empty;
        public HttpStatusCode HttpStatusCode { get; protected set; }
        public bool IsSuccess => string.IsNullOrEmpty(FailureReason);

        protected Exception _storedException { get; set; } = null!;

        public Exception ConsumeError()
        {
            var ex = _storedException;
            _storedException = null!;
            return ex;
        }

        public static implicit operator bool(MediatorResult<TResponse, TSuccessStatusCodes, TFailureStatusCodes> result)
        {
            return result.IsSuccess;
        }
    }

    public record CommandResult<TResponse> : MediatorResult<TResponse, CommandSuccessHttpStatusCode, CommandFailureHttpStatusCode>
    {
        public static CommandResult<TResponse> Fail(string reason, CommandFailureHttpStatusCode statusCode, Exception exception) => new CommandResult<TResponse>
        {
            HttpStatusCode = (HttpStatusCode)statusCode,
            FailureReason = reason,
            _storedException = exception,
        };

        protected static CommandResult<TResponse> Success(TResponse value, CommandSuccessHttpStatusCode statusCode) => new CommandResult<TResponse>
        {
            Value = value,
            HttpStatusCode = (HttpStatusCode)statusCode
        };
    }

    public record QueryResult<TResponse> : MediatorResult<TResponse, QuerySuccessHttpStatusCode, QueryFailureHttpStatusCode>
    {
        public static QueryResult<TResponse> Fail(string reason, QueryFailureHttpStatusCode statusCode, Exception exception) => new QueryResult<TResponse>
        {
            HttpStatusCode = (HttpStatusCode)statusCode,
            FailureReason = reason,
            _storedException = exception,
        };

        protected static QueryResult<TResponse> Success(TResponse value, QuerySuccessHttpStatusCode statusCode) => new QueryResult<TResponse>
        {
            Value = value,
            HttpStatusCode = (HttpStatusCode)statusCode
        };
    }
}
