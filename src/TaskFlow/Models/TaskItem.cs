using System;

namespace TaskFlow.Models
{
    // Modele de tache. TODO: voir avec l'equipe si on doit notifier la vue.
    public class TaskItem
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Project { get; set; }

        public string Assignee { get; set; }

        public Priority Priority { get; set; }

        public TaskState State { get; set; }

        public DateTime DueDate { get; set; }

        public int EstimatedHours { get; set; }

        public bool IsLate
        {
            get { return State != TaskState.Terminee && DueDate < DateTime.Now; }
        }

        public string DisplayLabel
        {
            get { return "#" + Id + " - " + Title; }
        }
    }
}
