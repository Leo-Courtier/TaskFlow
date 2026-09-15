using System;
using System.Collections.Generic;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public static class SeedData
    {
        private static readonly string[] Projects =
        {
            "Refonte intranet", "Migration ERP", "Application mobile",
            "Portail client", "Infrastructure reseau", "Mise en conformite RGPD"
        };

        private static readonly string[] Verbs =
        {
            "Corriger", "Documenter", "Refactorer", "Tester",
            "Deployer", "Analyser", "Migrer", "Optimiser", "Securiser"
        };

        private static readonly string[] Subjects =
        {
            "le formulaire de connexion", "l'export CSV", "la page d'accueil",
            "le service d'authentification", "la sauvegarde nocturne", "le module de facturation",
            "la file d'impression", "le cache des images", "la recherche full-text",
            "les notifications par mail", "le tableau de bord", "la synchronisation hors-ligne"
        };

        private static readonly string[] People =
        {
            "A. Martin", "C. Dubois", "L. Bernard", "M. Petit",
            "S. Moreau", "T. Laurent", "V. Girard", "Y. Rousseau"
        };

        public static List<TaskItem> Generate(int count)
        {
            // Graine fixe pour que tout le monde ait le meme jeu de donnees.
            Random random = new Random(20240915);

            List<TaskItem> result = new List<TaskItem>();

            for (int i = 1; i <= count; i++)
            {
                string description = "";
                for (int j = 1; j <= 8; j++)
                {
                    description = description + "Etape " + j + " : verifier " + Subjects[random.Next(Subjects.Length)] + ". ";
                }

                TaskItem item = new TaskItem();
                item.Id = i;
                item.Title = Verbs[random.Next(Verbs.Length)] + " " + Subjects[random.Next(Subjects.Length)];
                item.Description = description;
                item.Project = Projects[random.Next(Projects.Length)];
                item.Assignee = People[random.Next(People.Length)];
                item.Priority = (Priority)random.Next(0, 4);
                item.State = (TaskState)random.Next(0, 4);
                item.DueDate = DateTime.Today.AddDays(random.Next(-90, 120));
                item.EstimatedHours = random.Next(1, 40);

                result.Add(item);
            }

            return result;
        }
    }
}
