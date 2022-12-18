using Skurk.Core.Client.State.Services;
using Skurk.Core.Shared.Common;
using Skurk.Core.Shared.Week;
using Skurk.Core.Shared.Week.Queries;

namespace Skurk.Core.Client.State.Store.HandleTime
{
    public class HandleTimeState : StateBase
    {
        public bool IsLoadingWeek { get; set; }

        public DateOnly Date { get; set; } = DateOnly.MinValue;
        public WeekDto? Week { get; set; }
    }
}
