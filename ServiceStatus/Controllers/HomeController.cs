using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace WBPlatform.ServiceStatus
{
    public class HomeController : Controller
    {
        public static StatusReportObject ServerStatus { get; set; } = new StatusReportObject();
        public IActionResult Index()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            Type type = typeof(StatusReportObject);
            PropertyInfo[] ps = type.GetProperties();
            foreach (PropertyInfo info in ps)
            {
                var name = info.GetCustomAttribute<DisplayNameAttribute>();
                dict.Add(name == null ? info.Name : name.DisplayName, info.GetValue(ServerStatus, null) ?? new object());
            }
            return View(dict);
        }
        public IActionResult Error() => NoContent();
        [HttpPost] public IActionResult AvailabilityReport() => Json("OK");
    }
}
