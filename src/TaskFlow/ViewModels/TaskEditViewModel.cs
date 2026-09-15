using System;
using System.Windows;
using TaskFlow.Models;

namespace TaskFlow.ViewModels
{
    public class TaskEditViewModel : ViewModelBase
    {
        private readonly TaskItem _task;

        public TaskEditViewModel(TaskItem task)
        {
            _task = task;

            Title = task.Title;
            Description = task.Description;
            Project = task.Project;
            Assignee = task.Assignee;
            SelectedPriority = task.Priority;
            SelectedState = task.State;
            DueDateText = task.DueDate.ToString("dd/MM/yyyy");
            EstimatedHoursText = task.EstimatedHours.ToString();
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Project { get; set; }

        public string Assignee { get; set; }

        public string DueDateText { get; set; }

        public string EstimatedHoursText { get; set; }

        public Priority SelectedPriority { get; set; }

        public TaskState SelectedState { get; set; }

        public Array Priorities
        {
            get { return Enum.GetValues(typeof(Priority)); }
        }

        public Array States
        {
            get { return Enum.GetValues(typeof(TaskState)); }
        }

        public string WindowTitle
        {
            get { return "Tache #" + _task.Id; }
        }

        public bool Apply()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                MessageBox.Show("Le titre est obligatoire.", "TaskFlow");
                return false;
            }

            _task.Title = Title;
            _task.Description = Description;
            _task.Project = Project;
            _task.Assignee = Assignee;
            _task.Priority = SelectedPriority;
            _task.State = SelectedState;
            _task.DueDate = DateTime.Parse(DueDateText);
            _task.EstimatedHours = int.Parse(EstimatedHoursText);

            return true;
        }
    }
}
