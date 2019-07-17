using System;
using System.Collections;
using System.Collections.Generic;

namespace AdvancedGeneric
{
    [Serializable]
    public class AutoSortList<T> : 
        ICollection, ICollection<T>, IReadOnlyCollection<T>, 
        IList<T>, IReadOnlyList<T>, 
        IEnumerable, IEnumerable<T>, 
        IEnumerator, IEnumerator<T>
    {
        public int Count { get { return container.Count; } }

        public T Current { get { return ((IEnumerator<T>)container).Current; } }

        object IEnumerator.Current { get { return ((IEnumerator)container).Current; } }

        public bool IsReadOnly { get { return ((IList<T>)container).IsReadOnly; } }

        public bool IsSynchronized { get { return ((ICollection)container).IsSynchronized; } }

        public object SyncRoot { get { return ((ICollection)container).SyncRoot; } }

        T IList<T>.this[int index] { get { return this[index]; } set { this[index] = value; } }

        public T this[int index]
        {
            get
            {
                return container[index];
            }
            set
            {   
                int currentIndex = index;
                int result;
                while((result = comparison(value, container[currentIndex])) != 0)
                {
                    if (result < 0)
                    {
                        if (currentIndex - 1 >= 0)
                        {

                            if (comparison(value, container[currentIndex - 1]) < 0)
                            {
                                Swap(ref container, currentIndex, currentIndex - 1);
                                currentIndex--;
                            }
                            else
                                break;
                        }
                        else break;
                    }
                    else if (result > 0)
                    {
                        if (currentIndex + 1 < container.Count)
                        {
                            if (comparison(value, container[currentIndex + 1]) > 0)
                            {
                                Swap(ref container, currentIndex, currentIndex + 1);
                                currentIndex++;
                            }
                            else break;
                        }
                        else break;
                    }
                    else
                        throw new InvalidOperationException("Same key item has been existed.");
                }
                container[currentIndex] = value;
            }
        }

        private static void Swap(ref List<T> list, int x, int y)
        {
            var tmp = list[x];
            list[x] = list[y];
            list[y] = tmp;
        }

        private List<T> container;
        public Comparison<T> comparison { get; private set; }

        #region Constructors
        public AutoSortList(Comparison<T> comparison)
        {
            container = new List<T>();
            this.comparison = comparison;
        }

        public AutoSortList(IEnumerable<T> items, Comparison<T> comparison)
        {
            container = new List<T>(items);
            this.comparison = comparison;
            container.Sort(comparison);
        }

        public AutoSortList(AutoSortList<T> list)
        {
            this.container = new List<T>(list.container);
            comparison = list.comparison;
        }
        #endregion

        public void SetComparison(Comparison<T> comparison)
        {
            this.comparison = comparison;
            container.Sort(comparison);
        }

        public void Add(T item)
        {
            if (!TryAdd(item))
                throw new InvalidOperationException("Same key item has been added.");
        }

        public bool TryAdd(T item)
        {  
            int min = 0;
            int max = container.Count - 1;
            int middle = -1;
            int result = 0;
            while (min <= max)
            {
                middle = (min + max) / 2;
                result = comparison(item, container[middle]);
                if (result < 0)
                    max = middle - 1;
                else if (result > 0)
                    min = middle + 1;
                else
                    return false;
            }
            if (middle < 0)
                container.Add(item);
            else
            {
                if (result > 0)
                    container.Insert(middle + 1, item);
                else if (result < 0)
                    container.Insert(middle, item);
            }
            return true;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if(index >= 0)
            {
                container.RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            container.RemoveAt(index);
        }

        public int RemoveAll(Predicate<T> predicate)
        {
            return container.RemoveAll(predicate);
        }

        public void RemoveRange(int index, int count)
        {
            container.RemoveRange(index, count);
        }

        public bool Remove<TKey>(TKey key, Func<TKey, T, int> comparison)
        {
            int min = 0;
            int max = container.Count - 1;
            int middle = -1;
            int result = 0;
            while (min <= max)
            {
                middle = (min + max) / 2;
                result = comparison(key, container[middle]);
                if (result < 0)
                    max = middle - 1;
                else if (result > 0)
                    min = middle + 1;
                else
                {
                    container.RemoveAt(middle);
                    return true;
                }
            }
            return false;
        }

        public List<T> GetRange(int index, int count)
        {
            return container.GetRange(index, count);
        }

        public int IndexOf(T item)
        {
            int min = 0;
            int max = container.Count - 1;
            int middle = -1;
            int result = 0;
            while (min <= max)
            {
                middle = (min + max) / 2;
                result = comparison(item, container[middle]);
                if (result < 0)
                    max = middle - 1;
                else if (result > 0)
                    min = middle + 1;
                else
                    return middle;
            }
            return -1;
        }

        public T Find(Predicate<T> predicate)
        {
            return container.Find(predicate);
        }

        public T Find<TKey>(TKey key, Func<TKey, T, int> comparison)
        {
            int min = 0;
            int max = container.Count - 1;
            int middle = -1;
            int result = 0;
            while (min <= max)
            {
                middle = (min + max) / 2;
                result = comparison(key, container[middle]);
                if (result < 0)
                    max = middle - 1;
                else if (result > 0)
                    min = middle + 1;
                else
                {   
                    return container[middle];
                }
            }
            return default;
        }

        public List<T> FindAll(Predicate<T> predicate)
        {
            return container.FindAll(predicate);
        }

        public List<T> FindAll<TKey>(TKey[] sortedKeys, Func<TKey, T, int> comparison)
        {
            int i = 0, j = 0;
            int result = 0;
            List<T> found = new List<T>();
            while(i < container.Count && j < sortedKeys.Length)
            {
                result = comparison(sortedKeys[j], container[i]);
                if (result > 0)
                    i++;
                else if (result < 0)
                    j++;
                else
                {
                    found.Add(container[i]);
                    i++;
                    j++;
                }
            }
            return found;
        }

        public List<T> ToList()
        {
            return new List<T>(container);
        }

        public T[] ToArray()
        {
            return container.ToArray();
        }

        #region Set logic methods
        public List<T> UnionWith(IEnumerable<T> items)
        {
            List<T> tmp_items = new List<T>(items);
            tmp_items.Sort(comparison);
            List<T> added = new List<T>();
            List<T> newList = new List<T>();
            int i = 0, j = 0;
            int result = 0;
            while (i < container.Count && j < tmp_items.Count)
            {
                result = comparison(container[i], tmp_items[j]);
                if (result < 0)
                    newList.Add(container[i++]);
                else if (result > 0)
                {
                    var newItem = tmp_items[j++];
                    added.Add(newItem);
                    newList.Add(newItem);
                }
                else
                {
                    newList.Add(tmp_items[i]);
                    i++; j++;
                }
            }
            if (i < container.Count)
                newList.AddRange(container.GetRange(i, container.Count - i));
            if (j < tmp_items.Count)
            {
                var remainAdded = tmp_items.GetRange(j, tmp_items.Count - j);
                added.AddRange(remainAdded);
                newList.AddRange(remainAdded);
            }
            container = newList;
            return added;
        }

        public void UnionWith(AutoSortList<T> sortList)
        {
            List<T> newList = new List<T>();
            int i = 0, j = 0;
            int result = 0;
            while (i < container.Count && j < sortList.Count)
            {
                result = comparison(container[i], sortList[j]);
                if (result < 0)
                {
                    newList.Add(container[i++]);
                }
                else if (result > 0)
                {
                    newList.Add(sortList[j++]);
                }
                else
                {
                    newList.Add(sortList[i]);
                    i++; j++;
                }
            }
            if (i < container.Count)
                newList.AddRange(container.GetRange(i, container.Count - i));
            if (j < sortList.Count)
                newList.AddRange(sortList.GetRange(j, sortList.Count - j));
            container = newList;
        }

        public void IntersectWith(IEnumerable<T> items)
        {
            List<T> tmp_items = new List<T>(items);
            tmp_items.Sort(comparison);
            List<T> newList = new List<T>();
            int i = 0, j = 0;
            int result = 0;
            while (i < container.Count && j < tmp_items.Count)
            {
                result = comparison(container[i], tmp_items[j]);
                if (result < 0)
                    i++;
                else if (result > 0)
                    j++;
                else
                {
                    newList.Add(container[i]);
                    i++; j++;
                }
            }
            container = newList;
        }

        public void IntersectWith(AutoSortList<T> sortList)
        {
            List<T> newList = new List<T>();
            int i = 0, j = 0;
            int result = 0;
            while(i < container.Count && j < sortList.Count)
            {
                result = comparison(container[i], sortList[j]);
                if (result < 0)
                    i++;
                else if (result > 0)
                    j++;
                else
                {
                    newList.Add(container[i]);
                    i++; j++;
                }
            }
            container = newList;
        }

        public void ExceptWith(IEnumerable<T> items)
        {
            List<T> tmp_items = new List<T>(items);
            tmp_items.Sort(comparison);
            List<T> newList = new List<T>();
            int i = 0, j = 0;
            int result = 0;
            while (i < container.Count && j < tmp_items.Count)
            {
                result = comparison(container[i], tmp_items[j]);
                if (result < 0)
                    newList.Add(container[i++]);
                else if (result > 0)
                    j++;
                else
                {
                    i++; j++;
                }
            }
            if (i < container.Count)
                newList.AddRange(container.GetRange(i, container.Count - i));
            container = newList;
        }

        public void ExceptWith(AutoSortList<T> sortList)
        {
            List<T> newList = new List<T>();
            int i = 0, j = 0;
            int result = 0;
            while(i < container.Count && j < sortList.Count)
            {
                result = comparison(container[i], sortList[j]);
                if (result < 0)
                    newList.Add(container[i++]);
                else if (result > 0)
                    j++;
                else
                {
                    i++; j++;
                }
            }
            container = newList;
        }

        public void Diff<TOther>(
            IEnumerable<TOther> others, 
            out List<TOther> added, 
            out List<T> removed, 
            out List<T> existed, 
            out List<TOther> updated,
            Func<T, TOther, int> comparison)
        {
            List<TOther> tmp_others = new List<TOther>(others);
            added = new List<TOther>();
            removed = new List<T>();
            existed = new List<T>();
            updated = new List<TOther>();
            int i = 0, j = 0;
            int result = 0;
            while(i < container.Count && j < tmp_others.Count)
            {
                result = comparison(container[i], tmp_others[j]);
                if (result < 0)
                    removed.Add(container[i++]);
                else if (result > 0)
                    added.Add(tmp_others[j++]);
                else
                {
                    existed.Add(container[i++]);
                    updated.Add(tmp_others[j++]);
                }
            }
            if (i < container.Count)
                removed.AddRange(container.GetRange(i, container.Count - i));
            if (j < tmp_others.Count)
                added.AddRange(tmp_others.GetRange(j, tmp_others.Count - j));
        }
        #endregion

        #region IEnumerable, IEnumerator methods
        public IEnumerator<T> GetEnumerator()
        {
            return container.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)container;
        }

        public bool MoveNext()
        {
            return ((IEnumerator)container).MoveNext();
        }

        public void Reset()
        {
            ((IEnumerator)container).Reset();
        }

        public void Dispose()
        {
            container.GetEnumerator().Dispose();
        }
        #endregion

        #region ICollection, IList methods
        public void Insert(int index, T item)
        {
            Add(item);
        }

        public void Clear()
        {
            container.Clear();
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            container.CopyTo(array, arrayIndex);
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)container).CopyTo(array, index);
        }
        #endregion

        public static explicit operator List<T>(AutoSortList<T> list)
        {
            return new List<T>(list.container);
        }
    }

}
