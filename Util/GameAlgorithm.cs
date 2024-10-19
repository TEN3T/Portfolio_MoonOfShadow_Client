using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameAlgorithm
{
    private static System.Random random = new System.Random();

    public static void FisherYateShuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            --n;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static Dictionary<TKey, TValue> FisherVateShuffle<TKey, TValue>(this Dictionary<TKey, TValue> dict)
    {
        return dict.OrderBy(x => random.Next()).ToDictionary(item => item.Key, item => item.Value);
    }
}
