using Microsoft.AspNetCore.Components;
using Skurk.Core.Client.State.Store;

namespace Skurk.Core.Client.Shared
{
    public class StatefulComponent : ComponentBase, IDisposable
    {
        private StateBase[] _state = new StateBase[0];
        private CancellationTokenSource _source = new CancellationTokenSource();
        public CancellationToken CancellationToken => _source.Token;
        public void Dispose()
        {
            foreach (var stateItem in _state)
            {
                stateItem.PropertyChanged -= InvokeStateHasChanged;
            }
            _source?.Cancel();
            _source?.Dispose();
        }

        public void RegisterState(params StateBase[] state)
        {
            _state = state;
            foreach (var stateItem in _state)
            {
                stateItem.PropertyChanged += InvokeStateHasChanged;
            }
        }

        private void InvokeStateHasChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            StateHasChanged();
        }
    }
}
