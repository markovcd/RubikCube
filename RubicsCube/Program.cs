using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RubicsCube
{
    class Program
    {
        static void Print(Face f)
        {
            Console.WriteLine("{0} {1}", f[new Point(false, false)], f[new Point(false, true)]);
            Console.WriteLine("{0} {1}", f[new Point(true, false)], f[new Point(true, true)]);
            Console.WriteLine();
        }
        
        static void Main(string[] args)
        {
            var f = new Face();
            f.Value = 4321;


            f[Direction.Up] = new Face{Value = 4444};
            f[Direction.Down] = new Face{Value = 3333};
            f[Direction.Left] = new Face { Value = 2222 };
            f[Direction.Right] = new Face { Value = 1111 };

       
            Print(f[Direction.Up]);
            Print(f[Direction.Down]);
            Print(f[Direction.Left]);
            Print(f[Direction.Right]);

            Print(f);

            f.Move(Direction.Left);



            Print(f[Direction.Up]);
            Print(f[Direction.Down]);
            Print(f[Direction.Left]);
            Print(f[Direction.Right]);

            Print(f);



            Console.ReadLine();
        }
    }
}
