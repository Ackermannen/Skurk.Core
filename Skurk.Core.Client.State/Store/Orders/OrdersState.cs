using Fluxor;
using Skurk.Core.Shared.Orders;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Client.State.Store.Orders
{
    [FeatureState]
    public record OrdersState
    {
        private OrdersState()
        {
        }

        public int Page { get; set; }
        public int PageSize { get; set; }
        public ImmutableList<OrderDto> Orders { get; set; }
    }
}
