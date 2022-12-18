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
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Web;

namespace Skurk.Core.Shared.Common
{
    public static class RequestHelper
    {
        public static string BuildQueryString<T>(T obj) where T : class
        {
            var props = obj.GetType().GetProperties();

            return $"?{string.Join('&', props.Where(x => x.GetValue(obj) != null).Select(x => $"{HttpUtility.UrlEncode(x.Name)}={HttpUtility.UrlEncode(x.GetValue(obj)!.ToString())}"))}";
        }

        private static async Task<RequestResult<TResponse>> ParseData<TResponse>(HttpResponseMessage res, CancellationToken ct)
        {
            try
            {
                var deserializedResult = JsonConvert.DeserializeObject<JsonResponseValue>(await res.Content.ReadAsStringAsync(ct));
                if (deserializedResult != default)
                {
                    var deserierializedValue = JsonConvert.DeserializeObject<TResponse>(deserializedResult.Value);
                    if (deserierializedValue != null)
                    {
                        return RequestResult<TResponse>.Success(deserierializedValue, res.StatusCode);
                    }
                    else
                    {
                        throw new InvalidOperationException("Response values aren't parsable");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Response data isn't parsable");
                }
            }
            catch (Exception e)
            {
                return RequestResult<TResponse>.Fail("Unrecognizable object format", res.StatusCode, e);
            }
        }

        public static async Task<RequestResult<TResponse>> SendAsQuery<TResponse>(string url, string queryString, Func<string, CancellationToken, Task<HttpResponseMessage>> callback, CancellationToken ct = default)
        {
            HttpResponseMessage res;
            try
            {
                res = await callback.Invoke(url + queryString, ct);
            }
            catch (WebException e)
            {
                return RequestResult<TResponse>.Fail("No internet connection", HttpStatusCode.NotFound, e);
            }
            catch (Exception e)
            {
                return RequestResult<TResponse>.Fail("Error contacting the server", HttpStatusCode.InternalServerError, e);
            }

            return await ParseData<TResponse>(res, ct);
        }

        public static async Task<RequestResult<TResponse>> SendAsBody<TResponse>(string url, StringContent stringContent, Func<string, StringContent, CancellationToken, Task<HttpResponseMessage>> callback, CancellationToken ct = default)
        {
            HttpResponseMessage res;
            try
            {
                res = await callback.Invoke(url, stringContent, ct);
            }
            catch (WebException e)
            {
                return RequestResult<TResponse>.Fail("No internet connection", HttpStatusCode.NotFound, e);
            }
            catch (Exception e)
            {
                return RequestResult<TResponse>.Fail("Error contacting the server", HttpStatusCode.InternalServerError, e);
            }

            return await ParseData<TResponse>(res, ct);
        }
    }

    public abstract record Postable<TResponse> : ICommand<RequestResult<TResponse>>
    {
        public async Task<RequestResult<TResponse>> Send(HttpClient client, string route, CancellationToken ct = default)
        {

            var content = new StringContent(JsonConvert.SerializeObject(this));
            return await RequestHelper.SendAsBody<TResponse>(route,
                content,
                new Func<string, StringContent, CancellationToken, Task<HttpResponseMessage>>(client.PostAsync),
                ct);
        }
    }

    public abstract record Putable<TResponse> : ICommand<RequestResult<TResponse>>
    {
        public async Task<RequestResult<TResponse>> Send(HttpClient client, string route, CancellationToken ct = default)
        {

            var content = new StringContent(JsonConvert.SerializeObject(this));
            return await RequestHelper.SendAsBody<TResponse>(route,
                content,
                new Func<string, StringContent, CancellationToken, Task<HttpResponseMessage>>(client.PutAsync),
                ct);
        }
    }

    public abstract record Deletable<TResponse> : ICommand<RequestResult<TResponse>>
    {
        public async Task<RequestResult<TResponse>> Send(HttpClient client, string route, CancellationToken ct = default)
        {

            var queryString = RequestHelper.BuildQueryString(this);
            return await RequestHelper.SendAsQuery<TResponse>(route,
                queryString,
                new Func<string, CancellationToken, Task<HttpResponseMessage>>(client.DeleteAsync),
                ct);
        }
    }

    public abstract record Getable<TResponse> : IQuery<RequestResult<TResponse>>
    {
        public async Task<RequestResult<TResponse>> Send(HttpClient client, string route, CancellationToken ct = default)
        {

            var queryString = RequestHelper.BuildQueryString(this);
            return await RequestHelper.SendAsQuery<TResponse>(route,
                queryString,
                new Func<string, CancellationToken, Task<HttpResponseMessage>>(client.GetAsync),
                ct);
        }
    }

    internal record JsonResponseValue
    {
        public string Value { get; set; } = default!;
    }

    public record RequestResult<TResponse>
    {
        [JsonIgnore]
        public TResponse Value { get; set; } = default!;

        [JsonProperty("value")]
        private string _stringifiedValue => JsonConvert.SerializeObject(Value);
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

        public static RequestResult<TResponse> Fail(string reason, HttpStatusCode statusCode, Exception exception) => new()
        {
            HttpStatusCode = statusCode,
            FailureReason = reason,
            _storedException = exception,
        };

        public static RequestResult<TResponse> Success(TResponse value, HttpStatusCode statusCode) => new()
        {
            Value = value,
            HttpStatusCode = statusCode
        };
    }
}
