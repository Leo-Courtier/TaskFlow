using System.Windows;
using System.Windows.Input;
using TaskFlow.Models;
using TaskFlow.ViewModels;

namespace TaskFlow.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void TasksList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TaskItem selected = TasksList.SelectedItem as TaskItem;
            if (selected == null)
            {
                return;
            }

            // Les utilisateurs se plaignaient du bouton Editer, on a mis le double-clic en attendant.
            TaskEditWindow window = new TaskEditWindow(selected);
            window.Owner = this;
            window.ShowDialog();
        }
    }
}
