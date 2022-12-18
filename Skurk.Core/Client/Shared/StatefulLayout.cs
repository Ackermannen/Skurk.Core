using Microsoft.AspNetCore.Components;
using Skurk.Core.Client.State.Store;
using System.Threading;

namespace Skurk.Core.Client.Shared
{
    public partial class StatefulLayout : LayoutComponentBase, IDisposable
    {
        private StateBase[] _state = new StateBase[0];
        private CancellationTokenSource _source = new CancellationTokenSource();
        public CancellationToken CancellationToken => _source.Token;
        public void Dispose()
        {
            //foreach (var stateItem in _state)
            //{
            //    stateItem.PropertyChanged -= InvokeStateHasChanged;
            //}
            _source?.Cancel();
            _source?.Dispose();
        }

        public void RegisterState(params StateBase[] state)
        {
            _state = state;
            //foreach(var stateItem in _state)
            //{
            //    stateItem.PropertyChanged += InvokeStateHasChanged;
            //}
        }

        private void InvokeStateHasChanged(bool? persist)
        {
            //StateHasChanged();
        }
    }
}
