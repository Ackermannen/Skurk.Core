using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Shared.Week
{
    public class WeekDto
    {
        public DateOnly StartDay { get; set; }
        public DateOnly EndDay { get; set; }
        public List<TimeTaskDto> TimeTasks { get; set; } = new();
    }
}
