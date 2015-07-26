using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RubiksCube
{
    public enum Axis { X, Y, Z }

    public struct Point : IEquatable<Point>
    {
        public int Value { get; }

        public bool this[Axis a]
        {
            get
            {
                var p = (int)Math.Pow(2, (int)a);
                return Convert.ToBoolean(Value / p % 2);
            }
        }

        private static void SetAxis(ref int value, Axis a, bool axisValue)
        {
            var p = (int)Math.Pow(2, (int)a);
            value -= (value / p) % 2 * p;
            value += axisValue ? p : 0;
        }

        public Point(int value)
        {
            Value = value;
        }

        public Point(bool x, bool y, bool z)
        {
            var value = 0;
            SetAxis(ref value, Axis.X, x);
            SetAxis(ref value, Axis.Y, y);
            SetAxis(ref value, Axis.Z, z);

            Value = value;
        }

        public Point Transform(Axis a1, Axis a2)
        {
            int value = Value;
            SetAxis(ref value, a1, !this[a2]);
            SetAxis(ref value, a2, this[a1]);
            return new Point(value);
        }

        public bool Equals(Point other) => Value.Equals(other.Value);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() =>
            $"{(this[Axis.X] ? 1 : 0)} {(this[Axis.Y] ? 1 : 0)} {(this[Axis.Z] ? 1 : 0)}";
    }

    public struct Cubelet : IEquatable<Cubelet>
    {
        public Cubelet(Point location, int xy, int yz, int xz)
        {
            int value = 0;

            SetFace(ref value, Axis.X, Axis.Y, xy);
            SetFace(ref value, Axis.X, Axis.Z, xz);
            SetFace(ref value, Axis.Y, Axis.Z, yz);

            Location = location;
            Value = value;
        }

        public Cubelet(Point location, int value)
        {
            Location = location;
            Value = value;
        }

        public Cubelet(int index)
        {
            Location = new Point(index);
            int value = 0;

            SetFace(ref value, Axis.X, Axis.Y, Location[Axis.Z] ? 2 : 1);
            SetFace(ref value, Axis.X, Axis.Z, Location[Axis.Y] ? 4 : 3);
            SetFace(ref value, Axis.Y, Axis.Z, Location[Axis.X] ? 6 : 5);

            Value = value;
        }

        public int Value { get; }

        public Point Location { get; }

        public int this[Axis a1, Axis a2]
        {
            get
            {
                var a = (int)a1 + (int)a2 - 1;
                return Value / (int)Math.Pow(10, a) % 10;
            }
        }

        private static void SetFace(ref int value, Axis a1, Axis a2, int faceValue)
        {
            int a = (int)a1 + (int)a2 - 1;
            var p = (int)Math.Pow(10, a);
            value -= (value / p) % 10 * p;
            value += faceValue * p;
        }

        public Cubelet Transform(Axis a1, Axis a2)
        {
            var a = (Axis)(3 - (int)a1 - (int)a2);

            int value = Value;
            SetFace(ref value, a, a1, this[a, a2]);
            SetFace(ref value, a, a2, this[a, a1]);

            return new Cubelet(Location.Transform(a1, a2), value);
        }

        public bool Equals(Cubelet other) =>
            Value.Equals(other.Value) && Location.Equals(other.Location);

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

        public override string ToString() =>
            $"L {Location} XY {this[Axis.X, Axis.Y]} XZ {this[Axis.X, Axis.Z]} YZ {this[Axis.Y, Axis.Z]}";
    }

    public class Cubelets : IEnumerable<Cubelet>
    {
        private readonly IList<Cubelet> cubelets;

        public IEnumerable<Cubelet> Transform(Axis a1, Axis a2, bool? side = null)
        {
            var a = (Axis)(3 - (int)a1 - (int)a2);
            return cubelets.Select(c => !side.HasValue || c.Location[a] == side ? c.Transform(a1, a2) : c);
        }

        public IEnumerable<Cubelet> GetSide(Axis a1, Axis a2, bool side)
        {
            var a = (Axis)(3 - (int)a1 - (int)a2);
            return cubelets.Where(c => c.Location[a] == side);
        }

        private static IEnumerable<Tuple<Axis, Axis, bool>> Moves()
        {
            for (int a1 = 0; a1 < 3; a1++)
                for (int a2 = 0; a2 < 3; a2++)
                {
                    if (a1 == a2) continue;
                    yield return Tuple.Create((Axis)a1, (Axis)a2, false);
                    yield return Tuple.Create((Axis)a1, (Axis)a2, true);
                }
        }

        public Cubelets(IEnumerable<Cubelet> cubelets)
        {
            this.cubelets = cubelets.ToArray();
        }

        public Cubelets(int randomize = 0)
        {
            cubelets = new Cubelet[8];

            for (int i = 0; i < 8; i++)
                cubelets[i] = new Cubelet(i);

            var random = new Random();
            var moves = Moves().ToList();

            while (randomize-- > 0)
            {
                var t = moves[random.Next(0, moves.Count)];
                int i = 0;
                foreach (var c in Transform(t.Item1, t.Item2, t.Item3))
                    cubelets[i++] = c;
            }
        }

        public IEnumerator<Cubelet> GetEnumerator() => cubelets.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();        
    }

    public class Cube : IEquatable<Cube>, INode<Cube>
    {
        public Cubelets Cubelets { get; }
        public int Steps { get; set; }

        public Cube(int randomize = 0)
        {
            Cubelets = new Cubelets(randomize);
        }

        private Cube(IEnumerable<Cubelet> cubelets)
        {
            Cubelets = new Cubelets(cubelets);
        }

        public IEnumerable<Cube> Next()
        {
            for (int a1 = 0; a1 < 3; a1++)
                for (int a2 = 0; a2 < 3; a2++)
                {
                    if (a1 == a2) continue;

                    yield return Transform((Axis)a1, (Axis)a2, false);
                    yield return Transform((Axis)a1, (Axis)a2, true);
                }
        }

        public int[,] GetFace(Axis a1, Axis a2, bool side, bool flip1 = false, bool flip2 = false)
        {
            var result = new int[2, 2];

            foreach (var c in Cubelets.GetSide(a1, a2, side))
            {
                int i = c.Location[a1] ^ flip1 ? 1 : 0;
                int j = c.Location[a2] ^ flip2 ? 1 : 0;

                result[i, j] = c[a1, a2];
            }

            return result;
        }

        private bool IsFinished(Axis a1, Axis a2, bool side)
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

        public Cube Transform(Axis a1, Axis a2, bool? side = null) =>
            new Cube(Cubelets.Transform(a1, a2, side));

        public bool Equals(Cube other) => other.GetHashCode() == GetHashCode();

        public override int GetHashCode()
        {
            unchecked
            {
                return Cubelets.Aggregate(17, (current, c) => current * 23 + c.GetHashCode());
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
                xy[b ? 1 : 0, 0] = $"{f[0, 0]}{f[0, 1]}";
                xy[b ? 1 : 0, 1] = $"{f[1, 0]}{f[1, 1]}";

                f = GetFace(Axis.X, Axis.Z, b, false, !b);
                xz[b ? 1 : 0, 0] = $"{f[0, 0]}{f[0, 1]}";
                xz[b ? 1 : 0, 1] = $"{f[1, 0]}{f[1, 1]}";

                f = GetFace(Axis.Z, Axis.Y, b, !b);
                zy[b ? 1 : 0, 0] = $"{f[0, 0]}{f[0, 1]}";
                zy[b ? 1 : 0, 1] = $"{f[1, 0]}{f[1, 1]}";

                b = !b;
            } while (b);

            var s = new StringBuilder();
            s.AppendLine("   " + zy[0, 0]);
            s.AppendLine("   " + zy[0, 1]);

            s.AppendLine($"{xz[0, 0]} {xy[0, 0]} {xz[1, 0]} {xy[1, 0]}");
            s.AppendLine($"{xz[0, 1]} {xy[0, 1]} {xz[1, 1]} {xy[1, 1]}");

            s.AppendLine("   " + zy[1, 0]);
            s.AppendLine("   " + zy[1, 1]);

            return s.ToString();
        }
    }
}
