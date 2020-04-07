using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{
    public static class CollectionsExtension
    {
        /// <summary>
        /// 从List中移除并返回满足条件的数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static List<T> PopAll<T>(this List<T> source, Func<T, bool> predicate)
        {
            if (source == null || predicate == null)
            {
                throw new NullReferenceException();
            }

            List<T> res = new List<T>();
            if (source.Count == 0)
            {
                return res;
            }

            for (int freeIndex = 0, i = 0; i < source.Count; i++)
            {
                if (predicate(source[i]))
                {
                    res.Add(source[i]);
                }
                else
                {
                    if (freeIndex != i)
                    {
                        source[freeIndex] = source[i];
                    }
                    freeIndex++;
                }
            }

            source.RemoveRange(source.Count - res.Count, res.Count);
            return res;
        }
    }
}
