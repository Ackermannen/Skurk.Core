using Microsoft.Extensions.DependencyInjection;
using Skurk.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Client.State
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedClientDependencies(this IServiceCollection services)
        {
            services.AddSingleton<RouteFinder>();
            return services;
        }
    }
}
