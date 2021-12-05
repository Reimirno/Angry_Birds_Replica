/***
IEnumerableExt.cs

Description: Some utility methods for C# IEnumerables
Author: Yu Long
Created: Monday, November 22 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
***/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reimirno
{
    public static class IEnumerableExt
    {
        /// <summary>
        /// One simple method to print a C# ICollection type.
        /// 
        /// Use: 
        ///     var lst = new List<int>{1, 2, 3, 4, 5};
        ///     lst.Show();
        /// </summary>
        public static void Show<T>(this ICollection<T> list)
        {
            foreach (T ele in list)
                Console.Write("{0} ", ele);
            Console.WriteLine();
        }

        /// <summary>
        /// One simple method to print a C# IDictionary type.
        /// 
        /// Use:
        ///     var dic = new Dictionary<int, string>
        ///     {
        ///         {9, "nine}, {10, "ten"}, {11, "eleven"}
        ///     };
        ///     dic.Show();
        /// </summary>
        public static void Show<T1, T2>(this IDictionary<T1, T2> list)
        {
            foreach (KeyValuePair<T1, T2> ele in list)
                Console.Write("{0}:{1}", ele.Key, ele.Value);
            Console.WriteLine();
        }

        /// <summary>
        /// One simple foreach method that works for all IEnumerable types just like List<T> foreach and foreach loop.
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> f)
        {
            foreach (T ele in list)
                f(ele);
        }

        /// <summary>
        /// Returns a random element in a sequence by weight.
        /// This is from https://stackoverflow.com/a/11930875
        /// 
        /// Use:
        ///     var foo = new Dictionary<string, float>();
        ///     foo.Add("Item 25% 1", 0.5f);
        ///     foo.Add("Item 25% 2", 0.5f);
        ///     foo.Add("Item 50%", 1f);
        ///     
        ///     for(int i = 0; i< 10; i++)
        ///         Console.WriteLine(this, "Item Chosen {0}", foo.RandomElementByWeight(e => e.Value));
        /// </summary>
        /// <param name="sequence">The list of element from which selection happens.</param>
        /// <param name="weightSelector">A lambda that takes in an element and returns its corresponding weight</param>
        /// <returns></returns>
        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = (float)new Random().NextDouble() * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex >= itemWeightIndex)
                    return item.Value;

            }

            return default(T);

        }

    }
}
