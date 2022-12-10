using Skurk.Core.Shared.Orders;

namespace Skurk.Core.Client.State.Store.Orders
{
    public record GoToPageAction(int Page);
    public record ChangePageSizeAction(int Size);
    public record ReplaceItemsInOrderList(int StartIndex, int EndIndex, IList<OrderDto> Orders);
}
