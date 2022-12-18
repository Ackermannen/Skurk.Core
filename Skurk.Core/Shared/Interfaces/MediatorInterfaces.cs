using MediatR;
using Skurk.Core.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Shared.Interfaces
{
    public interface IMediatorRequest<TResponse> : IRequest<TResponse>
    {
        public Task<TResponse> Send(HttpClient client, string route, object content, CancellationToken ct = default);
    }

    public interface IQuery<TResponse> : IMediatorRequest<TResponse>
    {
        
    }

    public interface ICommand<TResponse> : IMediatorRequest<TResponse>
    {
    }

    public interface IQueryHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IQuery<TResponse>
    {

    }

    public interface ICommandHandler<in TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : ICommand<TResponse>
    {

    }
}
