using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TaskFlow.Commands;
using TaskFlow.Converters;
using TaskFlow.Models;
using TaskFlow.Services;
using TaskFlow.Views;

namespace TaskFlow.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private List<TaskItem> _allTasks;
        private List<TaskItem> _tasks;

        private string _searchText;
        private string _stateFilter;
        private TaskItem _selectedTask;
        private string _statusMessage;

        private int _total;
        private int _done;
        private int _late;
        private int _progress;

        public MainViewModel()
        {
            _stateFilter = "Tous";
            _searchText = "";
            _statusMessage = "Pret.";

            _allTasks = TaskRepository.Load();
            _tasks = _allTasks;

            RecomputeStats();

            NewCommand = new RelayCommand(ExecuteNew);
            EditCommand = new RelayCommand(ExecuteEdit, HasSelection);
            DeleteCommand = new RelayCommand(ExecuteDelete, HasSelection);
            SaveCommand = new RelayCommand(ExecuteSave);
        }

        public ICommand NewCommand { get; set; }

        public ICommand EditCommand { get; set; }

        public ICommand DeleteCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public List<TaskItem> Tasks
        {
            get { return _tasks; }
        }

        public List<string> StateFilters
        {
            get
            {
                List<string> filters = new List<string>();
                filters.Add("Tous");
                filters.Add("A faire");
                filters.Add("En cours");
                filters.Add("Bloquee");
                filters.Add("Terminee");
                return filters;
            }
        }

        public TaskItem SelectedTask
        {
            get { return _selectedTask; }
            set
            {
                _selectedTask = value;
                OnPropertyChanged("SelectedTask");
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged("SearchText");
                ApplyFilter();
            }
        }

        public string StateFilter
        {
            get { return _stateFilter; }
            set
            {
                _stateFilter = value;
                OnPropertyChanged("StateFilter");
                ApplyFilter();
            }
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                OnPropertyChanged("StatusMessage");
            }
        }

        public int Total
        {
            get { return _total; }
        }

        public int Done
        {
            get { return _done; }
        }

        public int Late
        {
            get { return _late; }
        }

        public int Progress
        {
            get { return _progress; }
        }

        private async void ApplyFilter()
        {
            string term = _searchText;
            string state = _stateFilter;

            // Anti-rebond maison : on laisse a l'utilisateur le temps de finir sa saisie.
            int wait = 400 - (term == null ? 0 : term.Length * 80);
            if (wait > 0)
            {
                await Task.Delay(wait);
            }

            List<TaskItem> filtered = new List<TaskItem>();

            for (int i = 0; i < _allTasks.Count; i++)
            {
                TaskItem task = _allTasks[i];

                bool matchText = true;
                if (!string.IsNullOrEmpty(term))
                {
                    matchText = task.Title.ToLower().Contains(term.ToLower())
                                || task.Description.ToLower().Contains(term.ToLower())
                                || task.Project.ToLower().Contains(term.ToLower())
                                || task.Assignee.ToLower().Contains(term.ToLower());
                }

                bool matchState = state == "Tous" || StatusToTextConverter.ToText(task.State) == state;

                if (matchText && matchState)
                {
                    filtered.Add(task);
                }
            }

            _tasks = filtered.OrderBy(t => t.DueDate).ThenBy(t => t.Title).ToList();

            OnPropertyChanged("Tasks");
            RecomputeStats();

            StatusMessage = _tasks.Count + " tache(s) affichee(s).";
        }

        private void RecomputeStats()
        {
            _total = _tasks.Count;
            _done = 0;
            _late = 0;

            foreach (TaskItem task in _tasks)
            {
                if (task.State == TaskState.Terminee)
                {
                    _done++;
                }

                if (task.IsLate)
                {
                    _late++;
                }
            }

            _progress = _done / _total * 100;

            OnPropertyChanged("Total");
            OnPropertyChanged("Done");
            OnPropertyChanged("Late");
            OnPropertyChanged("Progress");
        }

        private bool HasSelection(object parameter)
        {
            return _selectedTask != null;
        }

        private void ExecuteNew(object parameter)
        {
            TaskItem item = new TaskItem();
            item.Id = _allTasks.Count + 1;
            item.Title = "Nouvelle tache";
            item.Description = "";
            item.Project = "Non classe";
            item.Assignee = "Non assigne";
            item.Priority = Priority.Normale;
            item.State = TaskState.AFaire;
            item.DueDate = DateTime.Today.AddDays(7);
            item.EstimatedHours = 1;

            TaskEditWindow window = new TaskEditWindow(item);
            bool? result = window.ShowDialog();

            if (result == true)
            {
                _allTasks.Add(item);
                OnPropertyChanged("Tasks");
                RecomputeStats();
            }
        }

        private void ExecuteEdit(object parameter)
        {
            if (_selectedTask == null)
            {
                return;
            }

            TaskEditWindow window = new TaskEditWindow(_selectedTask);
            window.ShowDialog();

            OnPropertyChanged("Tasks");
            RecomputeStats();
        }

        private void ExecuteDelete(object parameter)
        {
            if (_selectedTask == null)
            {
                return;
            }

            MessageBoxResult answer = MessageBox.Show(
                "Supprimer la tache selectionnee ?",
                "TaskFlow",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (answer != MessageBoxResult.Yes)
            {
                return;
            }

            _allTasks.Remove(_selectedTask);
            _tasks.Remove(_selectedTask);

            OnPropertyChanged("Tasks");
            RecomputeStats();
        }

        private void ExecuteSave(object parameter)
        {
            TaskRepository.Save(_allTasks);
            StatusMessage = "Donnees enregistrees a " + DateTime.Now.ToString("HH:mm:ss");
        }
    }
}
