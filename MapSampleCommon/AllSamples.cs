using Mapsui.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MapSampleCommon
{
    public static class AllSamples
    {
        public static IEnumerable<ISample>? GetSamples()
        {
            var type = typeof(ISample);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => a.FullName.StartsWith("MapSampleCommon"));

            try
            {
                return (assemblies?
                        .SelectMany(s => s.GetTypes())
                        .Where(p => type.IsAssignableFrom(p) && !p.IsInterface)
                        .Select(Activator.CreateInstance)).Where(f => f is not null).OfType<ISample>()
                        .OrderBy(s => s?.Name)
                        .ThenBy(s => s?.Category)
                        .ToList();
            }
            catch (ReflectionTypeLoadException ex)
            {
                var sb = new StringBuilder();
                foreach (var exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    if (exSub is FileNotFoundException exFileNotFound)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                Logger.Log(LogLevel.Error, sb.ToString(), ex);
            }

            return null;
        }
    }
}
