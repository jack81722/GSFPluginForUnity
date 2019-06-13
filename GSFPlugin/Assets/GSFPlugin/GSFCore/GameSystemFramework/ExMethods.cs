using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem
{
    public static class ExMethods
    {
        public static TResult[] SelectToArray<TSource, TResult>(this List<TSource> sources, Func<TSource, TResult> selector)
        {
            TResult[] results = new TResult[sources.Count];
            for(int i = 0; i < sources.Count; i++)
            {
                results[i] = selector(sources[i]);
            }
            return results;
        }

        public static T[] ToType<T>(object[] array)
        {
            T[] ts = new T[array.Length];
            for(int i = 0; i < ts.Length; i++)
            {
                ts[i] = (T)array[i];
            }
            return ts;
        }

        public static List<T> ToType<T>(List<object> list)
        {
            List<T> ts = new List<T>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                ts.Add((T)list[i]);
            }
            return ts;
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;
            a = b;
            b = tmp;
        }
    }
}
