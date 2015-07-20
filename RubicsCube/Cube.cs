using System; using System.Collections.Generic; using System.Linq;  namespace RubiksCube {     public enum Axis { X, Y, Z }          public class Point     {         public int Value;          public bool this[Axis a]         {             get { return Convert.ToBoolean(Value / (int)Math.Pow(2, (int)a) % 2); }             set             {                 var m = (int)Math.Pow(2, (int)a);                 Value -= (Value / m) % 2 * m;                 Value += Convert.ToInt32(value) * m;             }         }          public Point(bool x, bool y, bool z)          {             this[Axis.X] = x;             this[Axis.Y] = y;             this[Axis.Z] = z;         }          public Point(int value)         {             Value = value;         }                  public void Transform(Axis a1, Axis a2)         {             var tmp = this[a1];             this[a1] = !this[a2];             this[a2] = tmp;         }          public override string ToString()         {             return String.Format("{0} {1} {2}", this[Axis.X]?1:0, this[Axis.Y]?1:0, this[Axis.Z]?1:0);         }     }      public class Cubelet     {         public Cubelet(Point p, int xy, int yz, int xz)         {             this[Axis.X, Axis.Y] = xy;             this[Axis.Y, Axis.Z] = yz;             this[Axis.X, Axis.Z] = xz;             Location = p;         }          public int Value;          public Point Location { get; set; }          public int this[Axis a1, Axis a2]         {             get             {                 int a = (int)a1 + (int)a2 - 1;                 return Value / (int)Math.Pow(10, a) % 10;             }             set             {                 int a = (int)a1 + (int)a2 - 1;                 var m = (int)Math.Pow(10, a);                 Value -= (Value / m) % 10 * m;                 Value += value * m;             }         }          public void Transform(Axis a1, Axis a2)         {
            var a = (Axis)(3 - (int)a1 - (int)a2);              var tmp = this[a, a1];             this[a, a1] = this[a, a2];             this[a, a2] = tmp;                          Location.Transform(a1, a2);         } 
        public override string ToString()
        {
            return String.Format("L {0} XY {1} XZ {2} YZ {3}", Location, 
                this[Axis.X, Axis.Y], this[Axis.X, Axis.Z], this[Axis.Y, Axis.Z]);
        }
    }      public class Cube     {         public IList<Cubelet> Cubelets { get; private set; }          public Cube()         {             Cubelets = new Cubelet[8];              for (int i = 0; i < 8; i++)             {                 var p = new Point(i);                 Cubelets[i] = new Cubelet(p, p[Axis.Z]?2:1, p[Axis.X]?4:3, p[Axis.Y]?6:5);             }         }                  public IEnumerable<Cubelet> GetSide(Axis a1, Axis a2, bool side)         {
            var a = (Axis)(3 - (int)a1 - (int)a2);             return Cubelets.Where(c => c.Location[a] == side);                }                  public int[,] GetFace(Axis a1, Axis a2, bool side)         {             var result = new int[2,2];                          foreach (var c in GetSide(a1, a2, side))             {                 int i = c.Location[a1] ? 1 : 0;                 int j = c.Location[a2] ? 1 : 0;                 result[i, j] = c[a1, a2];             }                                              return result;         }

        public bool IsFinished(Axis a1, Axis a2, bool side)
        {
            int? j = null;
            foreach (var i in GetFace(a1, a2, side))
            {
                if (j == null) j = i;
                if (j != i) return false;              
            }
            return true;
        }

        public bool IsFinished()
        {
            bool b = true;

            for (int a1 = 0; a1 < 2; a1++)
                for (int a2 = a1+1; a2 < 3; a2++)
                {
                    b = b && IsFinished((Axis)a1, (Axis)a2, false);
                    b = b && IsFinished((Axis)a1, (Axis)a2, true);
                }

            return b;
        }          public void Transform(Axis a1, Axis a2, bool side)         {             GetSide(a1, a2, side).ToList().ForEach(c => c.Transform(a1, a2));         }

        public void Transform(Axis a1, Axis a2)         {
            Transform(a1, a2, false);
            Transform(a1, a2, true);         }     } } 