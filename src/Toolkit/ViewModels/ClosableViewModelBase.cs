using Aldwych.AvaloniaToolkit.Abstract;
using Aldwych.AvaloniaToolkit.Services;
using System.ComponentModel;
using ReactiveUI;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleToAttribute("Aldwych.Mdi")]
namespace Aldwych.AvaloniaToolkit.ViewModels
{
    public class ClosableViewModelBase : ViewModelBase
    {
        public bool IsDirty
        {
            get => _isDirty;
            protected set => this.RaiseAndSetIfChanged(ref _isDirty, value);
        }

        public string Title
        {
            get => _title;
            protected set => this.RaiseAndSetIfChanged(ref _title, value);
        }


        public ClosableViewModelBase(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }


        public virtual void Close(CancelEventArgs e)
        {
            OnClosing(e);
            //Allow for inherited vms to do things on close.
        }

        protected virtual void OnClosing(CancelEventArgs e)
        {
            if (IsDirty && _dialogService != null)
            {
                if (!_dialogService.Confirm("Close Window", "All the changes made in this window will be lost. Are you sure you want to close?"))
                {
                    e.Cancel = true;
                }
            }
        }

        internal void SetOwner(IHasClose owner) => _owner = owner;


        protected IDialogService _dialogService;
        private IHasClose _owner;
        private string _title;
        private bool _isDirty;
    }
}
