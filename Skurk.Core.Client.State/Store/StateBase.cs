using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Client.State.Store
{
    public abstract class StateBase : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
