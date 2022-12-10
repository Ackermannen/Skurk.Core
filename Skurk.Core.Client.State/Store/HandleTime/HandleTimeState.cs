using Fluxor;
using Skurk.Core.Shared.Week;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Client.State.Store.HandleTime
{
    [FeatureState]
    public record HandleTimeState
    {
        private HandleTimeState()
        {
        }

        public bool IsLoadingDate { get; set; }

        public DateOnly Date { get; set; } = DateOnly.MinValue;

        public WeekDto Week { get; set; } = new WeekDto();

    }
}
