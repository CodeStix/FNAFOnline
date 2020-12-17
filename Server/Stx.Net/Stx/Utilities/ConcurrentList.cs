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
        public List<T> Underlaying { get; set; }

        public int Count
        {
            get
            {
                lock (Locker)
                {
                    return Underlaying.Count;
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
                    return Underlaying[index];
                }
            }
            set
            {
                lock (Locker)
                {
                    Underlaying[index] = value;
                }
            }
        }

        public object Locker
        {
            get
            {
                return ((ICollection)Underlaying).SyncRoot;
            }
        }

        public ConcurrentList()
        {
            Underlaying = new List<T>();
        }

        public void Add(T item)
        {
            lock (Locker)
            {
                Underlaying.Add(item);
            }
        }

        public void AddRange(params T[] items)
        {
            lock (Locker)
            {
                Underlaying.AddRange(items);
            }
        }

        public void Clear()
        {
            lock (Locker)
            {
                Underlaying.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock(Locker)
            {
                return Underlaying.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (Locker)
            {
                Underlaying.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (Locker)
            {
                return Underlaying.Remove(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock(Locker)
            {
                return Underlaying.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock(Locker)
            {
                return Underlaying.GetEnumerator();
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
                return Underlaying.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock(Locker)
            {
                Underlaying.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (Locker)
            {
                Underlaying.RemoveAt(index);
            }
        }

        public T[] ToArray()
        {
            lock(Locker)
            {
                return Underlaying.ToArray();
            }
        }

        public void ForEach(Action<T> action)
        {
            lock(Locker)
            {
                Underlaying.ForEach(action);
            }
        }

        public List<T> SafeCopy()
        {
            lock(Locker)
            {
                return new List<T>(Underlaying);
            }
        }
        /*public T First(Func<T, bool> check, T defaultValue = default(T))
        {
            lock(Locker)
            {
                foreach(T item in Underlaying)
                {
                    if (check.Invoke(item))
                        return item;
                }
            }

            return defaultValue;
        }*/
    }
}
