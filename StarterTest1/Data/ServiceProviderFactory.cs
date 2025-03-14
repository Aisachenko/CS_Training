using System;
using Microsoft.Extensions.DependencyInjection;
using WindowsFormsApp1.Repositories;

namespace WindowsFormsApp1.Data
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider ServiceProvider { get; set; }
        static ServiceProviderFactory()
        {
            var services = new ServiceCollection();
            services.AddScoped<DataContext>();
            services.AddScoped<IPeople, PeopleRepository>();
            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
