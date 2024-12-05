namespace XArch.CommandLine
{
    public interface ICommandContextDataProvider
    {
        public T? LoadCurrentContext<T>(string @namespace)
            where T : ContextData, new();

        public void SetCurrentContext(string @namespace, string name);

        public T Init<T>(string @namespace, string name)
            where T : ContextData, new();

        public void SaveContext();
    }
}
