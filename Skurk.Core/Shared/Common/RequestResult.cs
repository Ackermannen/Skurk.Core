using MediatR;
using Newtonsoft.Json;
using Skurk.Core.Shared.Enums;
using Skurk.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Skurk.Core.Shared.Common
{
    internal static class RequestHelper
    {
        public static string BuildQueryString<T>(T obj) where T : class
        {
            var props = obj.GetType().GetProperties();

            return $"?{string.Join('&', props.Where(x => x.GetValue(obj) != null).Select(x => $"{HttpUtility.UrlEncode(x.Name)}={HttpUtility.UrlEncode(x.GetValue(obj)!.ToString())}"))}";
        }

        public static async Task<RequestResult<TResponse>> SendAsQuery<TResponse>(string url, string queryString, Func<string, CancellationToken, Task<HttpResponseMessage>> callback, CancellationToken ct = default)
        {
            //int lastSlash = url.LastIndexOf('/');
            //url = lastSlash > -1 ? url.Substring(0, lastSlash) : url;

            HttpResponseMessage res;
            try
            {
                res = await callback.Invoke(url + queryString, ct);
            }
            catch (Exception e)
            {
                return RequestResult<TResponse>.Fail("Error contacting the server", FailureHttpStatusCode.InternalServerError, e);
            }

            try
            {
                var deserializedResult = System.Text.Json.JsonSerializer.Deserialize<RequestResult<TResponse>>(await res.Content.ReadAsStreamAsync(ct));
                if (deserializedResult != default)
                {
                    deserializedResult.HttpStatusCode = res.StatusCode;
                    return deserializedResult;
                }
                else
                {
                    throw new InvalidOperationException("Response data isn't parsable");
                }
            }
            catch (Exception e)
            {
                var statusCode = Enum.IsDefined(typeof(FailureHttpStatusCode), (int)res.StatusCode) ? (FailureHttpStatusCode)res.StatusCode : FailureHttpStatusCode.InternalServerError;
                return RequestResult<TResponse>.Fail("Unrecognizable object format", statusCode, e);
            }
        }

        public static async Task<RequestResult<TResponse>> SendAsBody<TResponse>(string url, StringContent stringContent, Func<string, StringContent, CancellationToken, Task<HttpResponseMessage>> callback, CancellationToken ct = default)
        {
            HttpResponseMessage res;
            try
            {
                res = await callback.Invoke(url, stringContent, ct);
            }
            catch (Exception e)
            {
                return RequestResult<TResponse>.Fail("Error contacting the server", FailureHttpStatusCode.InternalServerError, e);
            }


            try
            {
                var deserializedResult = JsonConvert.DeserializeObject<RequestResult<TResponse>>(await res.Content.ReadAsStringAsync(ct));
                if (deserializedResult != null)
                {
                    deserializedResult.HttpStatusCode = res.StatusCode;
                    return deserializedResult;
                }
                else
                {
                    throw new InvalidOperationException("Response data isn't parsable");
                }
            }
            catch (Exception e)
            {
                var statusCode = Enum.IsDefined(typeof(FailureHttpStatusCode), (int)res.StatusCode) ? (FailureHttpStatusCode)res.StatusCode : FailureHttpStatusCode.InternalServerError;
                return RequestResult<TResponse>.Fail("Unrecognizable object format", statusCode, e);
            }
        }
    }

    public record Postable<TResponse> : ICommand<RequestResult<TResponse>>
    {
        public async Task<RequestResult<TResponse>> Send(HttpClient client, RouteFinder routeFinder, CancellationToken ct = default)
        {
            string url;
            try
            {
                url = routeFinder.RequestRoutes[GetType().Name];
            }
            catch
            {
                return RequestResult<TResponse>.Fail("An error occured",
                        FailureHttpStatusCode.InternalServerError,
                        new InvalidOperationException("Route is not registered in assembly"));
            }

            var content = new StringContent(JsonConvert.SerializeObject(this));
            return await RequestHelper.SendAsBody<TResponse>(url,
                content,
                new Func<string, StringContent, CancellationToken, Task<HttpResponseMessage>>(client.PostAsync),
                ct);
        }
    }

    public record Putable<TResponse> : ICommand<RequestResult<TResponse>>
    {
        public async Task<RequestResult<TResponse>> Send(HttpClient client, RouteFinder routeFinder, CancellationToken ct = default)
        {
            string url;
            try
            {
                url = routeFinder.RequestRoutes[GetType().Name];
            }
            catch
            {
                return RequestResult<TResponse>.Fail("An error occured",
                        FailureHttpStatusCode.InternalServerError,
                        new InvalidOperationException("Route is not registered in assembly"));
            }

            var content = new StringContent(JsonConvert.SerializeObject(this));
            return await RequestHelper.SendAsBody<TResponse>(url,
                content,
                new Func<string, StringContent, CancellationToken, Task<HttpResponseMessage>>(client.PutAsync),
                ct);
        }
    }

    public record Deletable<TResponse> : ICommand<RequestResult<TResponse>>
    {
        public async Task<RequestResult<TResponse>> Send(HttpClient client, RouteFinder routeFinder, CancellationToken ct = default)
        {
            string url;
            try
            {
                url = routeFinder.RequestRoutes[GetType().Name];
            }
            catch
            {
                return RequestResult<TResponse>.Fail("An error occured",
                        FailureHttpStatusCode.InternalServerError,
                        new InvalidOperationException("Route is not registered in assembly"));
            }

            var queryString = RequestHelper.BuildQueryString(this);
            return await RequestHelper.SendAsQuery<TResponse>(url,
                queryString,
                new Func<string, CancellationToken, Task<HttpResponseMessage>>(client.DeleteAsync),
                ct);
        }
    }

    public record Getable<TResponse> : IQuery<RequestResult<TResponse>>
    {
        public async Task<RequestResult<TResponse>> Send(HttpClient client, RouteFinder routeFinder, CancellationToken ct = default)
        {
            var url = routeFinder.RequestRoutes.First(x => GetType().Name == x.Key).Value;
            var queryString = RequestHelper.BuildQueryString(this);
            return await RequestHelper.SendAsQuery<TResponse>(url,
                queryString,
                new Func<string, CancellationToken, Task<HttpResponseMessage>>(client.GetAsync),
                ct);
        }
    }

    public record RequestResult<TResponse>
    {
        public TResponse Value { get; set; } = default!;
        public string FailureReason { get; protected set; } = string.Empty;
        [JsonIgnore] public HttpStatusCode HttpStatusCode { get; set; }
        public bool IsSuccess => string.IsNullOrEmpty(FailureReason);
        [JsonIgnore] protected Exception _storedException { get; set; } = null!;

        public Exception ConsumeError()
        {
            var ex = _storedException;
            _storedException = null!;
            return ex;
        }

        public static implicit operator bool(RequestResult<TResponse> result)
        {
            return result.IsSuccess;
        }

        public static RequestResult<TResponse> Fail(string reason, FailureHttpStatusCode statusCode, Exception exception) => new RequestResult<TResponse>
        {
            HttpStatusCode = (HttpStatusCode)statusCode,
            FailureReason = reason,
            _storedException = exception,
        };

        public static RequestResult<TResponse> Success(TResponse value, SuccessHttpStatusCode statusCode) => new RequestResult<TResponse>
        {
            Value = value,
            HttpStatusCode = (HttpStatusCode)statusCode
        };
    }
}
