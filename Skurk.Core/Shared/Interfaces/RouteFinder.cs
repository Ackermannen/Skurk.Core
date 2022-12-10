using Skurk.Core.Shared.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Shared.Interfaces
{
    /// <summary>
    /// Singleton service which scans the assembly for MediatorRequest/Httproute key value pairs
    /// </summary>
    public class RouteFinder
    {
        public Dictionary<string, string> RequestRoutes { get; private set; }

        public RouteFinder()
        {
            RequestRoutes = typeof(OrderRoutes).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
            .ToDictionary(x => x.Name, y => (string)y.GetRawConstantValue()!);
        }
    }
}
