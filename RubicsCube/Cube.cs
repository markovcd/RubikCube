using System;
using System.Collections.Generic;
using System.Linq;

namespace RubiksCube
{
    public enum Axis { X, Y, Z }
    public enum Direction { Left, Right }
    
    public class Point
    {
        public int Value;

        public bool this[Axis a]
        {
            get { return Convert.ToBoolean(Value / (int)Math.Pow(2, (int)a) % 2); }
            set
            {
                var m = (int)Math.Pow(2, (int)a);
                Value -= (Value / m) % 2 * m;
                Value += Convert.ToInt32(value) * m;
            }
        }

        public Point(bool x, bool y, bool z) 
        {
            this[Axis.X] = x;
            this[Axis.Y] = y;
            this[Axis.Z] = z;
        }

        public Point(int value)
        {
            Value = value;
        }
    }

    public class Cubelet
    {
        public Cubelet(Point p, int xy, int yz, int xz)
        {
            this[Axis.X, Axis.Y] = xy;
            this[Axis.Y, Axis.Z] = yz;
            this[Axis.X, Axis.Z] = xz;
            Location = p;
        }

        public int Value;

        public Point Location { get; set; }

        public int this[Axis a1, Axis a2]
        {
            get
            {
                int a = (int)a1 + (int)a2 - 1;
                return Value / (int)Math.Pow(10, a) % 10;
            }
            set
            {
                int a = (int)a1 + (int)a2 - 1;
                var m = (int)Math.Pow(10, a);
                Value -= (Value / m) % 10 * m;
                Value += value * m;
            }
        }

        public void Transform(Axis a1, Axis a2)
        {
            var pivot = GetPivot(a1, a2);

            var tmp = this[pivot, a1];
            this[pivot, a1] = this[pivot, a2];
            this[pivot, a2] = tmp;
        }

        public static Axis GetPivot(Axis a1, Axis a2)
        {
            int i = 3 - (int)a1 - (int)a2;
            return (Axis)i;
        }

    }

    public class Cube
    {
        public IList<Cubelet> Cubelets { get; private set; }

        public Cube()
        {
            Cubelets = new Cubelet[8];

            for (int i = 0; i < 8; i++)
            {
                var p = new Point(i);
                Cubelets[i] = new Cubelet(p, p[Axis.Z]?2:1, p[Axis.X]?4:3, p[Axis.Y]?6:5);
            }
        }

        public void RotateFace(Axis a1, Axis a2, bool side)
        {
            var pivot = Cubelet.GetPivot(a1, a2);
            var cubelets = Cubelets.Where(c => c.Location[pivot] == side).ToList();

            foreach (var c in cubelets)
            {
                c.Transform(a1, a2); 
                c.Location[]
            }
        }
    }
}