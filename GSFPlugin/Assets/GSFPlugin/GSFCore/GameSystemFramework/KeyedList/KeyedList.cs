using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ExCollection
{
    [Serializable]
    public class KeyedList<TKey, TItem> : IEnumerable<TItem>, IComparer<TItem>
        where TItem : IKeyable<TKey> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// List of items
        /// </summary>
        private List<TItem> list;

        /// <summary>
        /// Item count
        /// </summary>
        public int Count { get { return list.Count; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public KeyedList()
        {
            list = new List<TItem>();
        }

        public KeyedList(IEnumerable<TItem> items)
        {
            list = new List<TItem>();
            // if type of items is quick list, then add element fastly
            if (items.GetType() == typeof(KeyedList<TKey, TItem>))
            {
                var source = (KeyedList<TKey, TItem>)items;
                for (int i = 0; i < source.list.Count; i++)
                {
                    list.Add(source.list[i]);
                }
            }
            else
            {
                AddRange(items);
            }
        }

        public TItem this[int index]
        {
            get { return list[index]; }
            set { list[index] = value; }
        }

        #region Search method
        /// <summary>
        /// Find item by key
        /// </summary>
        /// <param name="key">key of item</param>
        /// <returns>item</returns>
        public TItem FindByKey(TKey key)
        {
            int min = 0;
            int max = list.Count - 1;
            int middle, result;
            while (min <= max)
            {
                middle = (min + max) / 2;
                result = key.CompareTo(list[middle].Key);
                if (result == 0)
                    return list[middle];
                else if (result > 0)
                    min = middle + 1;
                else if (result < 0)
                    max = middle - 1;
            }
            return default(TItem);
        }

        /// <summary>
        /// Get index of item by key
        /// </summary>
        /// <param name="key">key of item</param>
        /// <returns>index of item</returns>
        public int IndexOf(TKey key)
        {
            int min = 0;
            int max = list.Count - 1;
            int middle = -1, result;
            while (min <= max)
            {
                middle = (min + max) / 2;
                result = key.CompareTo(list[middle].Key);
                if (result == 0)
                    return middle;
                else if (result > 0)
                    min = middle + 1;
                else if (result < 0)
                    max = middle - 1;
            }
            return -1;
        }

        /// <summary>
        /// Get index of item
        /// </summary>
        /// <param name="item">item</param>
        /// <returns>index of item</returns>
        public int IndexOf(TItem item)
        {
            int min = 0;
            int max = list.Count - 1;
            int middle = -1, result;
            while (min <= max)
            {
                middle = (min + max) / 2;
                result = item.Key.CompareTo(list[middle].Key);
                if (result == 0)
                    return middle;
                else if (result > 0)
                    min = middle + 1;
                else if (result < 0)
                    max = middle - 1;
            }
            return -1;
        }

        public bool ContainsKey(TKey key)
        {
            return IndexOf(key) >= 0;
        }

        public bool ContainsValue(TItem item)
        {
            return IndexOf(item) >= 0;
        }

        public TItem Find(Predicate<TItem> predicate)
        {
            return list.Find(predicate);
        }

        public IEnumerable<TItem> FindAll(Predicate<TItem> predicate)
        {
            return list.FindAll(predicate);
        }
        #endregion

        #region Add/Remove methods
        /// <summary>
        /// Add item
        /// </summary>
        /// <param name="item">item</param>
        public void Add(TItem item)
        {
            int min = 0;
            int max = list.Count - 1;
            int middle = -1, result = 0;
            while (min <= max)
            {
                middle = (min + max) / 2;
                result = item.Key.CompareTo(list[middle].Key);
                if (result == 0)
                    throw new InvalidOperationException("Same key item has been existed.");
                else if (result > 0)
                    min = middle + 1;
                else if (result < 0)
                    max = middle - 1;
            }
            if (middle < 0)
                list.Add(item);
            else
            {
                if (result > 0)
                    list.Insert(middle + 1, item);
                else if (result < 0)
                    list.Insert(middle, item);
            }
        }

        public bool TryAdd(TItem item)
        {
            int min = 0;
            int max = list.Count - 1;
            int middle = -1, result = 0;
            while (min <= max)
            {
                middle = (min + max) / 2;
                result = item.Key.CompareTo(list[middle].Key);
                if (result == 0)
                    return false;
                else if (result > 0)
                    min = middle + 1;
                else if (result < 0)
                    max = middle - 1;
            }
            if (middle < 0)
                list.Add(item);
            else
            {
                if (result > 0)
                    list.Insert(middle + 1, item);
                else if (result < 0)
                    list.Insert(middle, item);
            }
            return true;
        }

        /// <summary>
        /// Add items
        /// </summary>
        /// <param name="items">items</param>
        public void AddRange(IEnumerable<TItem> items)
        {
            foreach (var item in items)
            {
                if (!TryAdd(item))
                    Console.WriteLine("add same key gs.");
            }
        }

        /// <summary>
        /// Merge with other list
        /// </summary>
        /// <param name="items">merged list</param>
        /// <exception cref="InvalidOperationException">Thrown when items with same key.</exception>
        public void MergeWith(KeyedList<TKey, TItem> items)
        {
            List<TItem> resultList = new List<TItem>();
            int indexA = 0, indexB = 0;
            while (indexA < list.Count && indexB < items.Count)
            {
                int result = list[indexA].Key.CompareTo(items[indexB].Key);
                if (result == 0)
                    throw new InvalidOperationException("Same key item has been existed.");
                else if (result < 0)
                    resultList.Add(list[indexA++]);
                else if (result > 0)
                    resultList.Add(items[indexB++]);
            }
            while (indexA < list.Count)
                resultList.Add(list[indexA++]);
            while (indexB < items.Count)
                resultList.Add(items[indexB++]);
            list = resultList;
        }

        /// <summary>
        /// Remove specific item
        /// </summary>
        /// <param name="item">item</param>
        /// <returns>remove success result</returns>
        public bool Remove(TItem item)
        {
            int min = 0;
            int max = list.Count - 1;
            int middle = -1, result = 0;
            while (min <= max)
            {
                middle = (min + max) / 2;
                result = item.Key.CompareTo(list[middle].Key);
                if (result == 0)
                {
                    list.RemoveAt(middle);
                    return true;
                }
                else if (result > 0)
                    min = middle + 1;
                else if (result < 0)
                    max = middle - 1;
            }
            return false;
        }

        /// <summary>
        /// Remove specific item by key
        /// </summary>
        /// <param name="key">key of item</param>
        /// <returns>remove success result</returns>
        public bool RemoveByKey(TKey key)
        {
            int index = IndexOf(key);
            if (index >= 0)
            {
                list.RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove range in list
        /// </summary>
        /// <param name="index">start index</param>
        /// <param name="count">amount for removing</param>
        public void RemoveRange(int index, int count)
        {
            list.RemoveRange(index, count);
        }

        /// <summary>
        /// Remove range in list by key range
        /// </summary>
        /// <param name="min">minimum key</param>
        /// <param name="max">maximum key</param>
        public void RemoveRangeByKey(TKey min, TKey max)
        {
            if (min.CompareTo(max) > 0)
            {
                TKey tmp = min;
                min = max;
                max = tmp;
            }
            int start = FindCloseGreater(min);
            int end = FindCloseLess(max);
            if (start >= 0 && end >= 0)
                list.RemoveRange(start, end - start + 1);
        }

        private int FindCloseGreater(TKey key)
        {
            int min = 0;
            int max = list.Count - 1;
            int middle = -1, result = 0;
            while (min <= max)
            {
                middle = (min + max) / 2;
                result = key.CompareTo(list[middle].Key);
                if (result == 0)
                    return middle;
                else if (result > 0)
                    min = middle + 1;
                else if (result < 0)
                    max = middle - 1;
            }
            if (middle < 0)
                return -1;
            if (result > 0)
                return middle + 1;
            else
                return middle;
        }

        private int FindCloseLess(TKey key)
        {
            int min = 0;
            int max = list.Count - 1;
            int middle = -1, result = 0;
            while (min <= max)
            {
                middle = (min + max) / 2;
                result = key.CompareTo(list[middle].Key);
                if (result == 0)
                    return middle;
                else if (result > 0)
                    min = middle + 1;
                else if (result < 0)
                    max = middle - 1;
            }
            if (middle < 0)
                return -1;
            if (result > 0)
                return middle;
            else
                return middle - 1;
        }

        /// <summary>
        /// Remove all item in list if satisfy predicate
        /// </summary>
        /// <param name="predicate">remove predicate</param>
        /// <returns></returns>
        public int RemoveAll(Predicate<TItem> predicate)
        {
            return list.RemoveAll(predicate);
        }

        public void Clear()
        {
            list.Clear();
        }
        #endregion

        #region Set methods
        /// <summary>
        /// Union with other list
        /// </summary>
        /// <param name="items">unioned list</param>
        public void UnionWith(KeyedList<TKey, TItem> items)
        {
            List<TItem> resultList = new List<TItem>();
            int indexA = 0, indexB = 0;
            while (indexA < list.Count && indexB < items.Count)
            {
                int result = list[indexA].Key.CompareTo(items[indexB].Key);
                if (result == 0)
                {
                    resultList.Add(list[indexA++]);
                    indexB++;
                }
                if (result < 0)
                    resultList.Add(list[indexA++]);
                else if (result > 0)
                    resultList.Add(items[indexB++]);
            }
            while (indexA < list.Count)
                resultList.Add(list[indexA++]);
            while (indexB < items.Count)
                resultList.Add(items[indexB++]);
            list = resultList;
        }

        public void ExceptWith(KeyedList<TKey, TItem> items)
        {
            int indexA = 0, indexB = 0;
            int result;
            while (indexA < Count && indexB < items.Count)
            {
                result = list[indexA].Key.CompareTo(items[indexB].Key);
                if (result == 0)
                {
                    list.RemoveAt(indexA);
                    indexB++;
                }
                else if (result > 0)
                    indexB++;
                else if (result < 0)
                    indexA++;
            }
        }

        public void IntersectWith(KeyedList<TKey, TItem> items)
        {
            List<TItem> resultlist = new List<TItem>();
            int indexA = 0, indexB = 0, result;
            while (indexA < list.Count && indexB < list.Count)
            {
                result = list[indexA].Key.CompareTo(items[indexB].Key);
                if (result == 0)
                {
                    resultlist.Add(list[indexA]);
                    indexA++;
                    indexB++;
                }
                else if (result > 0)
                    indexB++;
                else if (result < 0)
                    indexA++;
            }
            list = resultlist;
        }
        #endregion

        #region IEnumerable<T>, IEnumerator<T>, IComparer<T> methods
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TItem>)list).GetEnumerator();
        }

        IEnumerator<TItem> IEnumerable<TItem>.GetEnumerator()
        {
            return ((IEnumerable<TItem>)list).GetEnumerator();
        }

        public int Compare(TItem x, TItem y)
        {
            return x.Key.CompareTo(y.Key);
        }
        #endregion

        #region Static set methods
        public static KeyedList<TKey, TItem> Union(KeyedList<TKey, TItem> a, KeyedList<TKey, TItem> b)
        {
            KeyedList<TKey, TItem> resultList = new KeyedList<TKey, TItem>();
            int indexA = 0, indexB = 0;
            while (indexA < a.Count && indexB < b.Count)
            {
                int result = a[indexA].Key.CompareTo(b[indexB].Key);
                if (result == 0)
                {
                    resultList.list.Add(a[indexA++]);
                    indexB++;
                }
                else if (result < 0)
                    resultList.list.Add(a[indexA++]);
                else if (result > 0)
                    resultList.list.Add(b[indexB++]);
            }
            while (indexA < a.Count)
                resultList.list.Add(a[indexA++]);
            while (indexB < b.Count)
                resultList.list.Add(b[indexB++]);
            return resultList;
        }

        public static KeyedList<TKey, TItem> Except(KeyedList<TKey, TItem> a, KeyedList<TKey, TItem> b)
        {
            KeyedList<TKey, TItem> resultList = new KeyedList<TKey, TItem>(a);
            int indexA = 0, indexB = 0;
            int result;
            while (indexA < resultList.Count && indexB < b.Count)
            {
                result = resultList[indexA].Key.CompareTo(b[indexB].Key);
                if (result == 0)
                {
                    resultList.list.RemoveAt(indexA);
                    indexB++;
                }
                else if (result > 0)
                    indexB++;
                else if (result < 0)
                    indexA++;
            }
            return resultList;
        }

        public static KeyedList<TKey, TItem> Intersect(KeyedList<TKey, TItem> a, KeyedList<TKey, TItem> b)
        {
            KeyedList<TKey, TItem> resultList = new KeyedList<TKey, TItem>();
            int indexA = 0, indexB = 0, result;
            while (indexA < a.Count && indexB < b.Count)
            {
                result = a[indexA].Key.CompareTo(b[indexB].Key);
                if (result == 0)
                {
                    resultList.list.Add(a[indexA]);
                    indexA++;
                    indexB++;
                }
                else if (result > 0)
                    indexB++;
                else if (result < 0)
                    indexA++;
            }
            return resultList;
        }
        #endregion
    }


}
