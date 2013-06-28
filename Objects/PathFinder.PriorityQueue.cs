using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KarelazisBot.Objects
{
    public partial class PathFinder
    {
        public class PriorityQueue<T>
        {
            #region contructors
            public PriorityQueue()
            {
                this.Comparer = Comparer<T>.Default;
            }
            public PriorityQueue(IComparer<T> comparer)
            {
                this.Comparer = comparer;
            }
            public PriorityQueue(IComparer<T> comparer, int capacity)
            {
                this.Comparer = comparer;
                this.InnerList.Capacity = capacity;
            }
            #endregion

            #region properties
            private List<T> InnerList = new List<T>();
            private IComparer<T> Comparer;
            #endregion

            #region methods
            private void SwitchElements(int i, int j)
            {
                T h = this.InnerList[i];
                this.InnerList[i] = this.InnerList[j];
                this.InnerList[j] = h;
            }
            private int OnCompare(int i, int j)
            {
                return this.Comparer.Compare(this.InnerList[i], this.InnerList[j]);
            }
            /// <summary>
            /// Push an object onto the PQ
            /// </summary>
            /// <param name="O">The new object</param>
            /// <returns>The index in the list where the object is _now_. This will change when objects are taken from or put onto the PQ.</returns>
            public int Push(T item)
            {
                int p = this.InnerList.Count, p2;
                this.InnerList.Add(item);
                while (true)
                {
                    if (p == 0) break;
                    p2 = (p - 1) / 2;
                    if (this.OnCompare(p, p2) < 0)
                    {
                        this.SwitchElements(p, p2);
                        p = p2;
                    }
                    else break;
                }
                return p;
            }
            /// <summary>
            /// Get the smallest object and remove it.
            /// </summary>
            /// <returns>The smallest object</returns>
            public T Pop()
            {
                T result = this.InnerList[0];
                int p = 0, p1, p2, pn;
                this.InnerList[0] = this.InnerList[this.InnerList.Count - 1];
                this.InnerList.RemoveAt(this.InnerList.Count - 1);
                while (true)
                {
                    pn = p;
                    p1 = 2 * p + 1;
                    p2 = 2 * p + 2;
                    if (this.InnerList.Count > p1 && this.OnCompare(p, p1) > 0) p = p1;
                    if (this.InnerList.Count > p2 && this.OnCompare(p, p2) > 0) p = p2;

                    if (p == pn) break;
                    this.SwitchElements(p, pn);
                }

                return result;
            }
            /// <summary>
            /// Notify the PQ that the object at position i has changed
            /// and the PQ needs to restore order.
            /// Since you dont have access to any indexes (except by using the
            /// explicit IList.this) you should not call this function without knowing exactly
            /// what you do.
            /// </summary>
            /// <param name="i">The index of the changed object.</param>
            public void Update(int i)
            {
                int p = i, pn;
                int p1, p2;
                while (true)
                {
                    if (p == 0) break;
                    p2 = (p - 1) / 2;
                    if (this.OnCompare(p, p2) < 0)
                    {
                        this.SwitchElements(p, p2);
                        p = p2;
                    }
                    else break;
                }
                if (p < i) return;
                while (true)
                {
                    pn = p;
                    p1 = 2 * p + 1;
                    p2 = 2 * p + 2;

                    if (this.InnerList.Count > p1 && this.OnCompare(p, p1) > 0) p = p1;
                    if (this.InnerList.Count > p2 && this.OnCompare(p, p2) > 0) p = p2;

                    if (p == pn) break;
                    this.SwitchElements(p, pn);
                }
            }
            /// <summary>
            /// Get the smallest object without removing it.
            /// </summary>
            /// <returns>The smallest object</returns>
            public T Peek()
            {
                if (this.InnerList.Count > 0) return this.InnerList[0];
                return default(T);
            }
            public void Clear()
            {
                this.InnerList.Clear();
            }
            public int Count
            {
                get { return this.InnerList.Count; }
            }
            public void RemoveLocation(T item)
            {
                int index = -1;
                for (int i = 0; i < this.InnerList.Count; i++)
                {
                    if (this.Comparer.Compare(this.InnerList[i], item) == 0) index = i;
                }

                if (index != -1) this.InnerList.RemoveAt(index);
            }
            public T this[int index]
            {
                get { return this.InnerList[index]; }
                set
                {
                    this.InnerList[index] = value;
                    this.Update(index);
                }
            }
            #endregion
        }
    }
}
