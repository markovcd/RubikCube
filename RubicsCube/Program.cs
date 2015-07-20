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
       
        static void Main(string[] args)
        {
            var cube = new Cube();
            var a = cube.GetFace(Axis.X, Axis.Y, false);
            cube.RotateFace(Axis.X, Axis.Z, false);
            var f = cube.GetFace(Axis.X, Axis.Y, false);
            cube.RotateFace(Axis.X, Axis.Y, false);
            var g = cube.GetFace(Axis.X, Axis.Y, false);
        }
    }
}
