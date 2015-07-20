using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RubiksCube
{
    class Program
    {
        static Axis ToAxis(char c)
        {
            if (c == 'x') return Axis.X;
            if (c == 'y') return Axis.Y;
            if (c == 'z') return Axis.Z;

            throw new Exception();
        }

        static void Main(string[] args)
        {
            var cube = new Cube();
            var s = "";
            do
            {
                Console.WriteLine(cube);
                s = Console.ReadLine();
                Console.Clear();

                if (s != "")
                {
                    var cmd = s.Split();
                    bool side = Convert.ToBoolean(int.Parse(cmd[1]));
                    var a1 = ToAxis(cmd[0][0]);
                    var a2 = ToAxis(cmd[0][1]);

                    cube.Transform(a1, a2, side);

                }
            } while (s != "");
            
        }
    }
}
