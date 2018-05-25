using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace virtualMemory
{
    public class SpecialQueue<T>
    {
        LinkedList<T> list = new LinkedList<T>();

        public void Enqueue(T t)
        {
            list.AddLast(t);
        }

        public T Dequeue()
        {
            var result = list.First.Value;
            list.RemoveFirst();
            return result;
        }

        public T Peek()
        {
            return list.First.Value;
        }

        public void Remove(int proceso)
        {
            for(int i=0; i<list.Count; i++)
            {
                int[] arr = (int[])(object)list.ElementAt(i);
               
                if(arr[0]== proceso)
                {
                    list.Remove(list.ElementAt(i));
                    i--;
              
                }
            }
            
        }

        public int Count { get { return list.Count; } }
    }
}
