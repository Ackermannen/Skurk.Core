using Skurk.Core.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skurk.Core.Shared.Orders
{
    /// <summary>
    /// Contains HTTP and Command names for different routes.
    /// The system scans the assembly for constant names towards specific commands
    /// For the system to work properly, please name each constant the same as a command inside the assembly.
    /// </summary>
    public static class OrderRoutes
    {
        public const string CreateOrderCommand = "/Orders/";
        public const string DeleteOrderCommand = "/Orders/";
        public const string GetOrdersPaginatedQuery = "/Orders/";
    }
}
