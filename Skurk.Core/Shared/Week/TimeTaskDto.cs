using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Shared.Week
{
    public class TimeTaskDto
    {
        public required Guid Id { get; set; }
        public required Guid TaskId { get; set; }
        public required float[] Times { get; set; }
    }
}
