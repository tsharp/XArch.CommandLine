using System;
using System.IO;

namespace XArch.CommandLine
{
    internal sealed class CommandContextDataProvider : ICommandContextDataProvider
    {
        private object? contextData = null;
        private string module;
        private string name;

        private static string GetContextDataPath(string module)
        {
            // Get AppData path
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var defaultContextDataPath = Path.Combine(appDataPath, "XArch", "module", module, ".env");

            // Create Full Path if it doesn't exist
            if (!Directory.Exists(defaultContextDataPath))
            {
                Directory.CreateDirectory(defaultContextDataPath);
            }

            return defaultContextDataPath;
        }

        public T? LoadCurrentContext<T>(string module)
            where T : ContextData, new()
        {
            var dataFile = Path.Combine(GetContextDataPath(module), $"_current_.yaml");

            if (!File.Exists(dataFile))
            {
                throw new InvalidOperationException("No context found. Please set the context first.");
            }

            var yaml = File.ReadAllText(dataFile);
            var namespaceContextData = yaml.DeserializeFromYaml<NamespaceContextData>();

            return Init<T>(module, namespaceContextData!.Name);
        }

        public void SetCurrentContext(string module, string name)
        {
            var dataFile = Path.Combine(GetContextDataPath(module), $"_current_.yaml");
            var contextDataFile = Path.Combine(GetContextDataPath(module), $"{name}.yaml");

            if (!File.Exists(contextDataFile))
            {
                throw new InvalidOperationException("Context does not exist.");
            }

            var namespaceContextData = new NamespaceContextData
            {
                Name = name
            };

            var yaml = namespaceContextData.SerializeToYaml();

            File.WriteAllText(dataFile, yaml);
        }

        public T Init<T>(string module, string name)
            where T : ContextData, new()
        {
            this.module = module;
            this.name = name;

            var dataFile = Path.Combine(GetContextDataPath(module), $"{name}.yaml");

            if (!File.Exists(dataFile))
            {
                contextData = new T();

                return GetContext<T>();
            }

            // Deserialize the file
            var yaml = File.ReadAllText(dataFile);
            contextData = yaml.DeserializeFromYaml<T>();

            return GetContext<T>();
        }

        public T GetContext<T>()
            where T : ContextData, new()
        {
            if (contextData == null)
            {
                throw new InvalidOperationException("Context not initialized");
            }

            return (contextData as T)!;
        }

        public void SaveContext()
        {
            var dataFile = Path.Combine(GetContextDataPath(module), $"{name}.yaml");

            // Serialize the context
            var yaml = contextData.SerializeToYaml();

            // Write the file
            File.WriteAllText(dataFile, yaml);
        }
    }
}
