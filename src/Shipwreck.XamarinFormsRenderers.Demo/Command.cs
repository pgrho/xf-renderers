using System;
using System.Windows.Input;

namespace Shipwreck.XamarinFormsRenderers.Demo;

public sealed class Command : ICommand
{
    private Action _Action;

    public Command(Action action)
    {
        _Action = action;
    }

    event EventHandler ICommand.CanExecuteChanged
    {
        add { }
        remove { }
    }

    bool ICommand.CanExecute(object parameter) => true;

    public void Execute(object parameter)
        => _Action();
}
