using Fluxor;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Client.State.Store.Orders
{
    public static class OrderReducers
    {
        [ReducerMethod]
        public static OrdersState ReduceGoToPageAction(OrdersState state, GoToPageAction action) => state with
        {
            Page = action.Page,
        };

        [ReducerMethod]
        public static OrdersState ReduceChangePageSizeAction(OrdersState state, ChangePageSizeAction action) => state with
        {
            PageSize = action.Size,
        };

        [ReducerMethod]
        public static OrdersState AppendOrdersToListAction(OrdersState state, ReplaceItemsInOrderList action) 
        {
            var oldList = state.Orders.ToList();
            var removeByIndex = action.EndIndex <= oldList.Count;
            oldList.RemoveRange(action.StartIndex, removeByIndex ? action.EndIndex : oldList.Count - 1);

            return state with
            {
                Orders = oldList.Concat(action.Orders).ToImmutableList(),
            };
        } 
    }
}
