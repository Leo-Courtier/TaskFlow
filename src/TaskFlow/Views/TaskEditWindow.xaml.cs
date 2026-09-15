using System;
using System.Windows;
using TaskFlow.Models;
using TaskFlow.Services;
using TaskFlow.ViewModels;

namespace TaskFlow.Views
{
    public partial class TaskEditWindow : Window
    {
        private readonly TaskEditViewModel _viewModel;

        public TaskEditWindow(TaskItem task)
        {
            InitializeComponent();

            _viewModel = new TaskEditViewModel(task);
            DataContext = _viewModel;

            // On veut prevenir l'utilisateur quand les donnees sont enregistrees.
            TaskRepository.DataChanged += OnRepositoryDataChanged;
        }

        private void OnRepositoryDataChanged(object sender, EventArgs e)
        {
            MessageBox.Show("Les données ont été enregistrées.", "TaskFlow");
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.Apply())
            {
                DialogResult = true;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
