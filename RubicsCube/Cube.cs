using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubiksCube
{
    public enum Axis { X, Y, Z }

    public class Point : IEquatable<Point>
    {
        public int Value;

        public bool this[Axis a]
        {
            get
            {
                int p = (int)Math.Pow(2, (int)a);
                return Convert.ToBoolean(Value / p % 2);
            }
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

        public void Transform(Axis a1, Axis a2)
        {
            var tmp = this[a1];
            this[a1] = !this[a2];
            this[a2] = tmp;
        }

        public Point Clone()
        {
            return new Point(Value);
        }

        public bool Equals(Point other)
        {
            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", this[Axis.X] ? 1 : 0, this[Axis.Y] ? 1 : 0, this[Axis.Z] ? 1 : 0);
        }
    } 

    public class Cubelet : IEquatable<Cubelet>
    {
        public Cubelet(Point location, int xy, int yz, int xz)
        {
            this[Axis.X, Axis.Y] = xy;
            this[Axis.Y, Axis.Z] = yz;
            this[Axis.X, Axis.Z] = xz;
            Location = location;
        }

        public Cubelet(Point location, int value)
        {
            Location = location;
            Value = value;
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
            var a = (Axis)(3 - (int)a1 - (int)a2);

            var tmp = this[a, a1];
            this[a, a1] = this[a, a2];
            this[a, a2] = tmp;

            Location.Transform(a1, a2);
        }

        public Cubelet Clone()
        {
            return new Cubelet(Location.Clone(), Value);
        }

        public bool Equals(Cubelet other)
        {
            return Value.Equals(other.Value) && Location.Equals(other.Location);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + Location.GetHashCode();
                hash = hash * 23 + Value.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return String.Format("L {0} XY {1} XZ {2} YZ {3}", Location,
                this[Axis.X, Axis.Y], this[Axis.X, Axis.Z], this[Axis.Y, Axis.Z]);
        }
    }

    public class Cube : IEquatable<Cube>, INode<Cube>
    {
        public IList<Cubelet> Cubelets { get; private set; }
        public Cube Parent { get; set; }

        public static Cube Create()
        {
            var cube = new Cube();

            for (int i = 0; i < 8; i++)
            {
                var p = new Point(i);
                cube.Cubelets.Add(new Cubelet(p, p[Axis.Z] ? 2 : 1, p[Axis.X] ? 4 : 3, p[Axis.Y] ? 6 : 5));
            }

            return cube;
        }

        public Cube()
        {
            Cubelets = new List<Cubelet>();
        }

        public IEnumerable<Cube> Next()
        {
            for (int a1 = 0; a1 < 2; a1++)
                for (int a2 = a1 + 1; a2 < 3; a2++)
                {
                    bool b = false;
                    do
                    {
                        var cube = Clone();
                        cube.Transform((Axis)a1, (Axis)a2, b);
                        cube.Parent = this;
                        yield return cube;

                        cube = Clone();
                        cube.Transform((Axis)a2, (Axis)a1, b);
                        cube.Parent = this;
                        yield return cube;
                        b = !b;
                    } while (b);
                }
        }

        public IEnumerable<Cubelet> GetSide(Axis a1, Axis a2, bool side)
        {
            var a = (Axis)(3 - (int)a1 - (int)a2);
            return Cubelets.Where(c => c.Location[a] == side);
        }

        public int[,] GetFace(Axis a1, Axis a2, bool side, bool flip1 = false, bool flip2 = false)
        {
            var result = new int[2, 2];

            foreach (var c in GetSide(a1, a2, side))
            {
                int i = c.Location[a1]^flip1 ? 1 : 0;
                int j = c.Location[a2]^flip2 ? 1 : 0;

                result[i, j] = c[a1, a2];
            }

            return result;
        }

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
                for (int a2 = a1 + 1; a2 < 3; a2++)
                {
                    b = b && IsFinished((Axis)a1, (Axis)a2, false);
                    b = b && IsFinished((Axis)a1, (Axis)a2, true);
                }

            return b;
        }

        public void Transform(Axis a1, Axis a2, bool side)
        {
            GetSide(a1, a2, side).ToList().ForEach(c => c.Transform(a1, a2));
        }

        public void Transform(Axis a1, Axis a2)
        {
            Cubelets.ToList().ForEach(c => c.Transform(a1, a2));
        }

        public Cube Clone()
        {
            var cube = new Cube();

            foreach (var cubelet in Cubelets)
                cube.Cubelets.Add(cubelet.Clone());

            return cube;
        }

        private IEnumerable<Tuple<Axis, Axis, bool>> Moves()
        {
            for (int a1 = 0; a1 < 3; a1++)
                for (int a2 = 0; a2 < 3; a2++)
                {
                    if (a1 == a2) continue;
                    yield return Tuple.Create((Axis)a1, (Axis)a2, false);
                    yield return Tuple.Create((Axis)a1, (Axis)a2, true);
                }
        }

        public void Scramble(int steps = 10)
        {
            var random = new Random();

            var moves = Moves().ToList();

            while (steps-- > 0)
            {
                var t = moves[random.Next(0, moves.Count)];
                Transform(t.Item1, t.Item2, t.Item3);
            }

        }

        public bool Equals(Cube other)
        {
            return other.GetHashCode() == GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach (var c in Cubelets)
                    hash = hash * 23 + c.GetHashCode();

                return hash;
            }
        }

        public override string ToString()
        {
            var xy = new string[2, 2];
            var xz = new string[2, 2];
            var zy = new string[2, 2];

            bool b = false;
            do
            {
                var f = GetFace(Axis.X, Axis.Y, b, false, b);
                xy[b ? 1 : 0, 0] = f[0, 0].ToString() + f[0, 1];
                xy[b ? 1 : 0, 1] = f[1, 0].ToString() + f[1, 1];

                f = GetFace(Axis.X, Axis.Z, b, false, !b);
                xz[b ? 1 : 0, 0] = f[0, 0].ToString() + f[0, 1];
                xz[b ? 1 : 0, 1] = f[1, 0].ToString() + f[1, 1];

                f = GetFace(Axis.Z, Axis.Y, b, !b, false);
                zy[b ? 1 : 0, 0] = f[0, 0].ToString() + f[0, 1];
                zy[b ? 1 : 0, 1] = f[1, 0].ToString() + f[1, 1];

                b = !b;
            } while (b);

            var s = new StringBuilder();
            s.AppendLine("   " + zy[0, 0]);
            s.AppendLine("   " + zy[0, 1]);

            s.AppendLine(xz[0, 0] + " " + xy[0, 0] + " " + xz[1, 0] + " " + xy[1, 0]);
            s.AppendLine(xz[0, 1] + " " + xy[0, 1] + " " + xz[1, 1] + " " + xy[1, 1]);

            s.AppendLine("   " + zy[1, 0]);
            s.AppendLine("   " + zy[1, 1]);

            return s.ToString();
        }
    }
}
