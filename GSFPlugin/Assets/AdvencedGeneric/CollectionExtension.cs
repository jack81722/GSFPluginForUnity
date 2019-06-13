using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedGeneric
{
    public static class CollectionExtension
    {
        public static T[] Sort<T>(this T[] array)
        {
            T[] temp = new T[array.Length];
            Array.Copy(array, temp, array.Length);
            Array.Sort(temp);
            return temp;
        }

        public static T[] Sort<T>(this T[] array, Comparison<T> comparison)
        {
            T[] temp = new T[array.Length];
            Array.Copy(array, temp, array.Length);
            Array.Sort(temp, comparison);
            return temp;
        }

        public static void Shuffle<T>(this T[] array)
        {
            Random random = new Random();
            int rIndex;
            for (int i = 0; i < array.Length - 1; i++)
            {
                rIndex = random.Next(i, array.Length);
                Swap(ref array[i], ref array[rIndex]);
            }
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            Random random = new Random();
            int rIndex;
            for (int i = 0; i < list.Count - 1; i++)
            {
                rIndex = random.Next(i, list.Count);
                list.Swap(i, rIndex);
            }
        }

        public static void Swap<T>(this IList<T> list, int x, int y)
        {
            var tmp = list[x];
            list[x] = list[y];
            list[y] = tmp;
        }

        public static void Swap<T>(ref T x, ref T y)
        {
            var tmp = x;
            x = y;
            y = tmp;
        }

        public static string GetElementsString<T>(this IList<T> list)
        {
            string str = "";
            if (list.Count > 0)
                str = list[0].ToString();
            for (int i = 1; i < list.Count; i++)
                str += ", " + list[i].ToString();
            return str;
        }

        public static string GetElementsString<T>(this T[] array)
        {
            string str = "";
            if (array.Length > 0)
                str = array[0].ToString();
            for (int i = 1; i < array.Length; i++)
                str += ", " + array[i].ToString();
            return str;
        }

        public static string GetString<T>(this IList<T> list, Func<T, string> stringFunc)
        {
            string str = "";
            if (list.Count > 0)
                str = stringFunc(list[0]);
            for(int i = 1; i < list.Count; i++)
            {
                str += ", " + stringFunc(list[i]);
            }
            return str;
        }

        public static string GetString<T>(this T[] array, Func<T, string> stringFunc)
        {
            string str = "";
            if (array.Length > 0)
                str = stringFunc(array[0]);
            for (int i = 1; i < array.Length; i++)
            {
                str += ", " + stringFunc(array[i]);
            }
            return str;
        }

        public static T[] SubArray<T>(this T[] array, int index, int count)
        {
            T[] sub = new T[count];
            Array.Copy(array, index, sub, 0, count);
            return sub;
        }
    }
}
