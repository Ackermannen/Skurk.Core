using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Shared.Mediator
{
    interface IQuery<TResponse> : IRequest<QueryResult<TResponse>> 
    {

    }

    interface ICommand<TResponse> : IRequest<CommandResult<TResponse>>
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

    //public interface IPost<> : ICommand<>

    public abstract class MediatorResult<TResponse, TSuccessStatusCodes, TFailureStatusCodes>
    {
        public TResponse Value { get; protected set; } = default!;
        public string FailureReason { get; protected set; } = string.Empty;
        public HttpStatusCode HttpStatusCode { get; protected set; }
        public bool IsSuccess => string.IsNullOrEmpty(FailureReason);

        protected Exception _storedException { get; set; } = null!;
        protected abstract MediatorResult<TResponse, TSuccessStatusCodes, TFailureStatusCodes> Success(TResponse value, TSuccessStatusCodes statusCode);
        public abstract MediatorResult<TResponse, TSuccessStatusCodes, TFailureStatusCodes> Fail(string reason, TFailureStatusCodes statusCode, Exception exception);

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

    public class CommandResult<TResponse> : MediatorResult<TResponse, CommandSuccessHttpStatusCode, CommandFailureHttpStatusCode>
    {
        public override CommandResult<TResponse> Fail(string reason, CommandFailureHttpStatusCode statusCode, Exception exception) => new CommandResult<TResponse>
        {
            HttpStatusCode = (HttpStatusCode)statusCode,
            FailureReason = reason,
            _storedException = exception,
        };

        protected override CommandResult<TResponse> Success(TResponse value, CommandSuccessHttpStatusCode statusCode) => new CommandResult<TResponse>
        {
            Value = value,
            HttpStatusCode = (HttpStatusCode)statusCode
        };
    }

    public class QueryResult<TResponse> : MediatorResult<TResponse, QuerySuccessHttpStatusCode, QueryFailureHttpStatusCode>
    {
        public override QueryResult<TResponse> Fail(string reason, QueryFailureHttpStatusCode statusCode, Exception exception) => new QueryResult<TResponse>
        {
            HttpStatusCode = (HttpStatusCode)statusCode,
            FailureReason = reason,
            _storedException = exception,
        };

        protected override QueryResult<TResponse> Success(TResponse value, QuerySuccessHttpStatusCode statusCode) => new QueryResult<TResponse>
        {
            Value = value,
            HttpStatusCode = (HttpStatusCode)statusCode
        };
    }
}
