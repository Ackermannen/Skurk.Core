using Skurk.Core.Shared.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Shared.Week.Commands
{
    public record DeleteTimeTaskCommand : Deletable<bool>
    {
        public Guid Id { get; set; }
    }
}
