using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stx.Collections.Concurrent
{
    public class ConcurrentList<T> : IList<T>, ICollection<T>
    {
        private List<T> inner;

        public int Count
        {
            get
            {
                lock (Locker)
                {
                    return inner.Count;
                }
            }
        }

        public bool IsReadOnly { get; } = false;

        public T this[int index]
        {
            get
            {
                lock(Locker)
                {
                    return inner[index];
                }
            }
            set
            {
                lock (Locker)
                {
                    inner[index] = value;
                }
            }
        }

        public object Locker
        {
            get
            {
                return ((ICollection)inner).SyncRoot;
            }
        }

        public ConcurrentList()
        {
            inner = new List<T>();
        }

        public void Add(T item)
        {
            lock (Locker)
            {
                inner.Add(item);
            }
        }

        public void AddRange(params T[] items)
        {
            lock (Locker)
            {
                inner.AddRange(items);
            }
        }

        public void Clear()
        {
            lock (Locker)
            {
                inner.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock(Locker)
            {
                return inner.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (Locker)
            {
                inner.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (Locker)
            {
                return inner.Remove(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock(Locker)
            {
                return inner.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock(Locker)
            {
                return inner.GetEnumerator();
            }
        }

        private static Random random = new Random();

        /// <summary>
        /// Safely returns a random item from the list.
        /// </summary>
        /// <returns>A random item int the list.</returns>
        public T GetRandom()
        {
            lock(Locker)
            {
                if (Count == 0)
                    return default(T);

                return this[random.Next(0, Count)];
            }
        }

        public int IndexOf(T item)
        {
            lock(Locker)
            {
                return inner.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock(Locker)
            {
                inner.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (Locker)
            {
                inner.RemoveAt(index);
            }
        }

        public T[] ToArray()
        {
            lock(Locker)
            {
                return inner.ToArray();
            }
        }

        public void ForEach(Action<T> action)
        {
            lock(Locker)
            {
                inner.ForEach(action);
            }
        }

        public List<T> SafeCopy()
        {
            lock(Locker)
            {
                return new List<T>(inner);
            }
        }

        public void RemoveAll(Predicate<T> where)
        {
            lock(Locker)
            {
                inner.RemoveAll(where);
            }
        }
    }
}
