using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmdMapMaid.Core.Helpers;
public static class Extensions
{
    public static IEnumerable<T> TakeCyclic<T>(this IEnumerable<T> source)
    {
        while (true)
        {
            foreach (var item in source) {
                yield return item;
            }
        }
    }
}
