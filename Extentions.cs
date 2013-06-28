using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot
{
    public static class Extentions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var rand = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rand.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        /// <summary>
        /// Compares this array to another array. Returns true if contents are equal.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="array">The byte array to compare.</param>
        /// <returns></returns>
        public static bool DataEquals(this byte[] arr, byte[] array)
        {
            if (arr.Length != array.Length) return false;
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != array[i]) return false;
            }
            return true;
        }
        /// <summary>
        /// Generates a string containing n number of whitespaces.
        /// </summary>
        /// <param name="number">How many whitespace characters to generate.</param>
        /// <returns></returns>
        public static string GenerateIndentation(this int number)
        {
            string s = string.Empty;
            for (int i = 0; i < number; i++) s += " ";
            return s;
        }
    }
}
