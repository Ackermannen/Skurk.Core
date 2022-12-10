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

        private float[] _times;
        public required float[] Times 
        { 
            get { return _times; } 
            set {
                // Rounds the Time to the nearest quarter.
                _times = value.Select(x => (float) Math.Round((decimal)x * 4, MidpointRounding.ToEven) / 4).ToArray();
            } 
        }
    }
}
