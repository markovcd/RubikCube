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

    public class FragmentedList<T>
    {
        private readonly IList<HashSet<T>> hashSets;

        public int Capacity { get; }

        public FragmentedList(int capacity = 1000000)
        {
            hashSets = new List<HashSet<T>>(new[] { new HashSet<T>() });
            Capacity = capacity;
        }

        public bool Contains(T item) => hashSets.Any(h => h.Contains(item));

        public bool Add(T item)
        {
            if (Contains(item)) return false;

            if (hashSets.Last().Count == Capacity) hashSets.Add(new HashSet<T>());

            return hashSets.Last().Add(item);
        }
    }

    public class FragmentedQueue<T>
    {
        private readonly IList<Queue<T>> queues;

        public int Capacity { get; }

        public FragmentedQueue(int capacity = 1000000)
        {
            queues = new List<Queue<T>>(new[] { new Queue<T>() });
            Capacity = capacity;
        }

        public void Enqueue(T item)
        {
            if (queues.Last().Count == Capacity) queues.Add(new Queue<T>());
            queues.Last().Enqueue(item);
        }

        public T Dequeue()
        {
            if (!queues.First().Any()) queues.RemoveAt(0);
            return queues.First().Dequeue();
        }

        public bool Any()
        {
            return queues.Any(q => q.Any());
        }
    }

    public class BreadthFirstSearch<T> : INodeSearch<T> where T : class, INode<T>
    {
        public T Find(T start, Func<T, bool> predicate)
        {
            var q = new FragmentedQueue<T>();
            var d = new FragmentedList<int>();

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
