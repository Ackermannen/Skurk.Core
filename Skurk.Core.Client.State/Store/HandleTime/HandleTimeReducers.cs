using Fluxor;
using Skurk.Core.Client.State.Store.Orders;
using Skurk.Core.Shared.Week;
using Skurk.Core.Shared.Week.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Client.State.Store.HandleTime
{
    public static class HandleTimeReducers
    {

        [ReducerMethod]
        public static HandleTimeState ReduceSetDateAction(HandleTimeState state, SetDateAction action) => state with
        {
            Date = action.Query.Date,
        };

        [ReducerMethod]
        public static HandleTimeState ReduceAddRowAction(HandleTimeState state, AddRowAction action) 
        {
            var oldWeekObj = state.Week;
            oldWeekObj.TimeTasks.Add(new TimeTaskDto
            {
                Id = Guid.NewGuid(),
                TaskId = Guid.Empty,
                Times = new float[oldWeekObj.EndDay.DayNumber - oldWeekObj.StartDay.DayNumber]
            });

            return state with
            {
                Week = oldWeekObj,
            };
        }

        [ReducerMethod]
        public static HandleTimeState ReduceDeleteRowAction(HandleTimeState state, DeleteRowAction action)
        {
            var oldWeekObj = state.Week;
            oldWeekObj.TimeTasks.Remove(oldWeekObj.TimeTasks.First(x => x.Id == action.Command.Id));

            return state with
            {
                Week = oldWeekObj,
            };
        }

    }
}
