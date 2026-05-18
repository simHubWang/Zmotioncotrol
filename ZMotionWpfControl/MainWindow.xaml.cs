using System.Windows;
using System.Windows.Input;
using ZMotionWpfControl.Services;
using ZMotionWpfControl.ViewModels;

namespace ZMotionWpfControl;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel(new MockMotionController());
        DataContext = _viewModel;
    }

    private void JogPositiveButton_Down(object sender, MouseButtonEventArgs e)
    {
        ExecuteCommand(_viewModel.JogPositiveCommand);
    }

    private void JogNegativeButton_Down(object sender, MouseButtonEventArgs e)
    {
        ExecuteCommand(_viewModel.JogNegativeCommand);
    }

    private void JogButton_MouseUp(object sender, MouseButtonEventArgs e)
    {
        ExecuteCommand(_viewModel.StopJogCommand);
    }

    private void JogButton_MouseLeave(object sender, MouseEventArgs e)
    {
        ExecuteCommand(_viewModel.StopJogCommand);
    }

    private static void ExecuteCommand(ICommand command)
    {
        if (command.CanExecute(null))
        {
            command.Execute(null);
        }
    }
}
