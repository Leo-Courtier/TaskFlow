using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using TaskFlow.Models;

namespace TaskFlow.Services
{
    public static class TaskRepository
    {
        // Tout le monde peut s'abonner pour savoir quand les donnees changent.
        public static event EventHandler DataChanged;

        private const int SeedCount = 800;

        private static string _filePath;

        public static string FilePath
        {
            get
            {
                if (_filePath == null)
                {
                    string folder = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "TaskFlow");

                    Directory.CreateDirectory(folder);
                    _filePath = Path.Combine(folder, "tasks.json");
                }

                return _filePath;
            }
        }

        public static List<TaskItem> Load()
        {
            // Latence du "backend".
            Thread.Sleep(900);

            if (File.Exists(FilePath))
            {
                try
                {
                    string json = File.ReadAllText(FilePath);
                    List<TaskItem> loaded = JsonSerializer.Deserialize<List<TaskItem>>(json);
                    if (loaded != null && loaded.Count > 0)
                    {
                        return loaded;
                    }
                }
                catch (Exception)
                {
                }
            }

            List<TaskItem> seed = SeedData.Generate(SeedCount);
            Save(seed);
            return seed;
        }

        public static void Save(List<TaskItem> tasks)
        {
            // Latence du "backend".
            Thread.Sleep(900);

            try
            {
                JsonSerializerOptions options = new JsonSerializerOptions();
                options.WriteIndented = true;

                FileStream stream = File.Create(FilePath);
                JsonSerializer.Serialize(stream, tasks, options);
            }
            catch (Exception)
            {
            }

            if (DataChanged != null)
            {
                DataChanged(null, EventArgs.Empty);
            }
        }
    }
}
