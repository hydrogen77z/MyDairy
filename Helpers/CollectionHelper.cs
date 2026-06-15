using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDairy.Common;

namespace MyDairy.Helpers;
public static class CollectionHelper
{
    public static void UpdateCollectionNoClear<T>(IList<T> targetList, IReadOnlyList<T> newItems, Action<T> oldItemsChanging = null)
    {
        var i = 0;
        while (i < targetList.Count)
        {
            if (!newItems.Contains(targetList[i]))
            {
                targetList.RemoveAt(i);
                continue;
            }
            i++;
        }

        if (oldItemsChanging is not null)
        {
            foreach (var item in targetList)
            {
                oldItemsChanging(item);
            }
        }

        for (i = 0; i < newItems.Count; i++)
        {
            if (!targetList.Contains(newItems[i]))
            {
                if (i > targetList.Count)
                {
                    targetList.Add(newItems[i]);
                }
                else
                {
                    targetList.Insert(i, newItems[i]);
                }
            }
        }
    }

    public static IEnumerable<T> MergeArgument<T>(params object[] args)
    {
        foreach (var item in args)
        {
            if (item is T value)
            {
                yield return value;
            }
            else if (item is IList<T> subList)
            {
                foreach (var itemOfSubList in subList)
                {
                    yield return itemOfSubList;
                }
            }
            else if (item is IEnumerable<T> subEnumerable)
            {
                foreach (var itemOfSubEnumerable in subEnumerable)
                {
                    yield return itemOfSubEnumerable;
                }
            }
            else if (item is IList<object> subObjectList)
            {
                foreach (var itemOfSubObjectList in subObjectList)
                {
                    if (itemOfSubObjectList is T valueOfSubObjectList)
                    {
                        yield return valueOfSubObjectList;
                    }
                }
            }
            else if (item is IEnumerable<object> subObjectEnumerable)
            {
                foreach (var itemOfSubObjectEnumerable in subObjectEnumerable)
                {
                    if (itemOfSubObjectEnumerable is T valueOfSubObjectEnumerable)
                    {
                        yield return valueOfSubObjectEnumerable;
                    }
                }
            }
        }
    }
}
