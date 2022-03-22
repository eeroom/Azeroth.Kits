using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExcelUserFunctionXll
{
    public class LockedQueue<T>
    {
        Queue<T> queue{ get; set; }
        object mylock=new object();
        public LockedQueue()
        {
            this.queue = new Queue<T>();
        }
        public List<T> ToList()
        {
            lock (this.mylock)
            {
                var lst = new List<T>(queue.Count);
                while (queue.Count>0)
                {
                    lst.Add(queue.Dequeue());
                }
                return lst;
            }
        }

        public int Count()
        {
            lock (this.mylock)
            {
                return queue.Count;
            }
        }

        public void Enqueue(T entity)
        {
            lock (this.mylock)
            {
                queue.Enqueue(entity);
            }
        }
    }
}
