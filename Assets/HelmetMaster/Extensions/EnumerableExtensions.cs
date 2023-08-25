using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace HelmetMaster.Extensions
{
    public static class EnumerableExtensions
    {
        #region Array

        public static T RandomItem<T>(this T[] array)
        {
            if (array.Length == 0)
            {
                throw new IndexOutOfRangeException("Cannot select a random item from an empty array");
            }

            var rnd = new global::System.Random(Random.Range(0, 1000));
            var index = rnd.Next(0, array.Length);
            return array[index];
        }

        public static void Shuffle<T>(this T[] array)
        {
            var rng = new global::System.Random(Random.Range(0, 1000));
            var n = array.Length;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = array[k];
                array[k] = array[n];
                array[n] = value;
            }
        }

        #endregion

        #region List

        public static void Shuffle<T>(this IList<T> list)
        {
            var rng = new global::System.Random(Random.Range(0, 1000));
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T RandomItem<T>(this IList<T> list)
        {
            if (list.Count == 0)
            {
                throw new IndexOutOfRangeException("Cannot select a random item from an empty list");
            }

            var rnd = new global::System.Random(Random.Range(0, 1000));
            var index = rnd.Next(0, list.Count);
            return list[index];
        }

        #endregion
        
        #region String
        
        // Split a string into an array based on a Separator
        public static string[] SplitString(this string save, string separator) {
            return save.Split(new string[] { separator }, StringSplitOptions.None);
        }
        
        //Returns the default value if cannot parse
        public static float Parse_Float(this string txt, float _default = -1) {
            float f;
            if (!float.TryParse(txt, out f)) {
                f = _default;
            }
            return f;
        }
        
        // Parse a int, return default if failed
        public static int Parse_Int(this string txt, int _default = -1) {
            int i;
            if (!int.TryParse(txt, out i)) {
                i = _default;
            }
            return i;
        }
        
        #endregion
    }
}