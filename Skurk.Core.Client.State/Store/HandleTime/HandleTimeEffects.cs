using Fluxor;
using Skurk.Core.Client.State.Services;
using Skurk.Core.Client.State.Store.Orders;
using Skurk.Core.Shared.Orders.Queries;
using Skurk.Core.Shared.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Client.State.Store.HandleTime
{
    internal class HandleTimeEffects
    {
        private readonly SameSiteClient _client;
        private readonly IState<HandleTimeState> _state;

        public HandleTimeEffects(SameSiteClient client, IState<HandleTimeState> state)
        {
            _client = client;
            _state = state;
        }

        [EffectMethod]
        public async Task HandleSetDateAction(SetDateAction action, IDispatcher dispatcher)
        {
            var newWeek = await _client.Send(action.Query);
            dispatcher.Dispatch(new ChangeWeekAction(newWeek));
        }
    }
}
