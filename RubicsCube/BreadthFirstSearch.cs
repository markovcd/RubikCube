using System;
using System.Collections.Generic;
using System.Linq;

namespace RubiksCube
{
    public interface INode<T>
    {
        IEnumerable<T> Next();
        T Parent { get; set; }
    }

    public class BreadthFirstSearch<T> where T : INode<T>
    {
        public T Find(T start, Func<T, bool> predicate)
        {
            var q = new Queue<T>();
            var d = new HashSet<T>();

            q.Enqueue(start);
            d.Add(start);
            int i = 0;
            while (q.Any())
            {
                var n1 = q.Dequeue();
                if (predicate(n1)) return n1;

                foreach (var n2 in n1.Next())
                {
                    if (!d.Contains(n2))
                    {
                        q.Enqueue(n2);
                        d.Add(n2);
                        Console.WriteLine("Nodes discovered {0}", ++i);
                    }
                }
            }

            return default(T);
        }

        public int GetDepth(T node)
        {
            int i = 0;

            while (node.Parent != null)
            {
                node = node.Parent;
                i++;
            }

            return i;
        }
    }
}
