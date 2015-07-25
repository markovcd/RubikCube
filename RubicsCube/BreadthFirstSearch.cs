using System;
using System.Collections.Generic;
using System.Linq;

namespace RubiksCube
{
    public interface INode<out T>
    {
        IEnumerable<T> Next();
        int Steps { get; set; }
    }

    public interface INodeSearch<T> where T : class, INode<T>
    {
         T Find(T start, Func<T, bool> predicate);
    }  

    public class BreadthFirstSearch<T> : INodeSearch<T> where T : class, INode<T>
    {
        public T Find(T start, Func<T, bool> predicate)
        {
            var q = new Queue<T>();
            var d = new HashSet<int>();

            q.Enqueue(start);
            d.Add(start.GetHashCode());

            while (q.Any())
            {
                var n1 = q.Dequeue();
                if (predicate(n1)) return n1;

                foreach (var n2 in n1.Next().Where(n2 => !d.Contains(n2.GetHashCode())))
                {
                    n2.Steps = n1.Steps + 1;
                    q.Enqueue(n2);
                    d.Add(n2.GetHashCode());
                }
            }

            throw new Exception();
        }
    }

    
}
