using System;
using System.Collections.Generic;
using System.Linq;

namespace TestApi.Common.Configuration.Helpers
{
    public interface ILoggingDataExtractor
    {
        Dictionary<string, object> ConvertToDictionary(object input, string path = null, int depth = 0);
    }

    public class LoggingDataExtractor : ILoggingDataExtractor
    {
        public Dictionary<string, object> ConvertToDictionary(object input, string path = null, int depth = 0)
        {
            var result = new Dictionary<string, object>();
            if (depth > 3)
            {
                // Protection from recursive properties
                return result;
            }

            if (input == null)
            {
                return result;
            }

            var type = input.GetType();
            if (!IsCustomType(type))
            {
                result.Add(path, input);
                return result;
            }

            foreach (var property in type.GetProperties())
            {
                var value = property.GetValue(input);
                if (IsCustomType(property.PropertyType))
                {
                    var propertyValues = ConvertToDictionary(value, GetPath(path, property.Name), depth++);
                    foreach (var (key, o) in propertyValues)
                    {
                        result.Add(key, o);
                    }
                }
                else if (property.PropertyType != typeof(string) && property.PropertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                {
                    // Could handle IEnumerable here
                }
                else
                {
                    result.Add(GetPath(path, property.Name), value);
                }
            }

            return result;
        }

        private static string GetPath(string path, string property) => $"{path}{(string.IsNullOrEmpty(path) ? string.Empty : ".")}{property}";

        /// <summary>
        /// Pass in type to see if we should reuse deeper
        /// Not generic due to use case.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsCustomType(Type type)
        {
            var assemblyQualifiedName = GetType().AssemblyQualifiedName;
            return assemblyQualifiedName != null &&
                   (type.AssemblyQualifiedName != null &&
                    (!type.IsEnum &&
                     type.AssemblyQualifiedName.StartsWith(assemblyQualifiedName.Split('.')[0])));
        }
    }
}
