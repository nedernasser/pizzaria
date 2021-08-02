using Pizzaria.Core.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.Util
{
    public static class EnumUtil
    {
        public static Dictionary<int, string> GetValues<T>()
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .OrderBy(e => e.ToString())
                .ToDictionary(e =>
                    Convert.ToInt32(e),
                    e => e
                        .ToString()
                        .ToUpper()
                        .Replace("_", " "));
        }

        public static IEnumerable<KeyValueVO> GetValuesToList<T>()
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .OrderBy(e => e.ToString())
                .Select(e => new KeyValueVO
                {
                    Key = Convert.ToInt32(e),
                    Value = e
                        .ToString()
                        .ToUpper()
                        .Replace("_", " ")
                })
                .ToList();
        }

        public static string GetValue<T>(int keyOption)
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .FirstOrDefault(e => Convert.ToInt32(e) == keyOption)
                .ToString().Replace("_", " ");
        }
    }
}
