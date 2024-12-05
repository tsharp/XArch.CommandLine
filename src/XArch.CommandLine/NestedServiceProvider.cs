using System;

namespace XArch.CommandLine
{
    internal sealed class NestedServiceProvider : IServiceProvider
    {
        private readonly IServiceProvider parentServiceProvider;
        private readonly IServiceProvider serviceProvider;

        public NestedServiceProvider(IServiceProvider parentServiceProvider, IServiceProvider serviceProvider)
        {
            this.parentServiceProvider = parentServiceProvider;
            this.serviceProvider = serviceProvider;
        }

        public object GetService(Type serviceType)
        {
            return serviceProvider.GetService(serviceType) ??
                parentServiceProvider.GetService(serviceType);
        }
    }
}
