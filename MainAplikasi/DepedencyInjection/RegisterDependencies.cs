using MainAplikasi.Presentations.MainWindow;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainAplikasi.DepedencyInjection
{
    public static class RegisterDependencies
    {
        public static IServiceCollection InitializeServices(this IServiceCollection services)
        {
            // add business logic layer
            //services.AddSingleton<IDataService, DataService>();

            // add domain access layer
            //services.AddSingleton<IUserRepository, UserRepository>();

            // add viewmodel layer
            services.AddTransient<MainWindowViewModel>();

            // add view layer, tidak dianjurkan tapi oke ga apa-apa

            return services;
        }
    }
}
