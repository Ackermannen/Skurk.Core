using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Client.State.Store.Common
{
    public class CommonState : StateBase
    {
        public bool IsOnline { get; set; } = true;
    }
}
