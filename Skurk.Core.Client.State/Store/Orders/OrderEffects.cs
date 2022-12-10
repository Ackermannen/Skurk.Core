using Fluxor;
using Skurk.Core.Client.State.Services;
using Skurk.Core.Shared.Orders;
using Skurk.Core.Shared.Orders.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Client.State.Store.Orders
{
    internal class OrderEffects
    {
        private readonly SameSiteClient _client;
        private readonly IState<OrdersState> _state;

        public OrderEffects(SameSiteClient client, IState<OrdersState> state)
        {
            _client = client;
            _state = state;
        }

        [EffectMethod]
        public async Task HandleGoToPageActionAction(GoToPageAction action, IDispatcher dispatcher)
        {
            var startIndex = action.Page * (_state.Value.PageSize - 1);
            var endIndex = action.Page * _state.Value.PageSize;
            //var missingItems = (action.Page * _state.Value.PageSize) - (_state.Value.Page * _state.Value.PageSize);
            //If we had positive index in page gap and items are missing from list.
            if (startIndex > _state.Value.Orders.Count || endIndex > _state.Value.Orders.Count)
            {
                List<OrderDto> newPages = new();
                //If we jump pages, i.e. 1 to 3, add empty items as to not destroy pagination.
                var mockItemCount = startIndex - _state.Value.Orders.Count;
                if (mockItemCount > 0)
                {
                    newPages.AddRange(new List<OrderDto>(new OrderDto[mockItemCount]));
                }

                var pagesFromServer = await _client.Send(new GetPaginatedOrdersQuery
                {
                    Page = action.Page,
                    PageSize = _state.Value.PageSize,
                });

                newPages.AddRange(pagesFromServer);

                dispatcher.Dispatch(new ReplaceItemsInOrderList(startIndex, endIndex, newPages));
            } else
            {

            }
        }
    }
}
