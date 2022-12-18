using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Skurk.Core.Client.State.Services;
using Skurk.Core.Client.State.Store.Common;
using Skurk.Core.Client.State.Store.HandleTime;
using Skurk.Core.Shared.Common;
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
        public static IServiceCollection AddStatefulDependencies(this IServiceCollection services, IWebAssemblyHostEnvironment hostEnvironment)
        {
            services.AddScoped<HandleTimeState>();
            services.AddScoped<CommonState>();
            services.AddScoped(sp => new SameSiteClient(
                new HttpClient { BaseAddress = new Uri(hostEnvironment.BaseAddress + GenericConstants.ApiRoutePrefix) },
                sp.GetRequiredService<RouteFinder>(),
                sp.GetRequiredService<ISnackbar>(),
                sp.GetRequiredService<CommonState>()
                ));
            return services;
        }
    }
}
