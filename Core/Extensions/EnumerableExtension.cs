using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>Indicates whether the specified enumerable is null or has a length of zero.</summary>
        /// <param name="data">The data to test.</param>
        /// <returns>true if the array parameter is null or has a length of zero; otherwise, false.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> data)
        {
            return data == null || !data.Any();
        }

        public static int GetOrderIndependentHashCode<T>(this IEnumerable<T> source)
        {
            int hash = 0;
            //Need to force order to get  order independent hash code
            foreach (T element in source.OrderBy(x => x, Comparer<T>.Default))
            {
                hash = hash ^ EqualityComparer<T>.Default.GetHashCode(element);
            }
            return hash;
        }

        public static string GenerateCSV<T>(this IEnumerable<T> data)
        {
            StringBuilder output = new StringBuilder();
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            List<string> line = new List<string>();
            foreach (PropertyDescriptor prop in props)
            {
                line.Add(prop.DisplayName);
            }
            output.AppendLine(String.Join(',', line));


            foreach (T item in data)
            {
                line.Clear();
                foreach (PropertyDescriptor prop in props)
                {
                    line.Add(SanitiseCSVField(prop.Converter.ConvertToString(
                         prop.GetValue(item))));
                }
                output.AppendLine(String.Join(',', line));
            }
            return output.ToString();
        }

        public static string SanitiseCSVField(string data)
        {
            return data.Replace(',', '/'); ;
        }

    }
}
