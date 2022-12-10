using Skurk.Core.Shared.Week;
using Skurk.Core.Shared.Week.Commands;
using Skurk.Core.Shared.Week.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Client.State.Store.HandleTime
{
    public record SetDateAction(GetWeekQuery Query);
    public record AddRowAction();
    public record DeleteRowAction(DeleteTimeTaskCommand Command);
    public record ChangeWeekAction(WeekDto Week);
}
