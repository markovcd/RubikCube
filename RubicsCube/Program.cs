using System;


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
            var cube = Cube.Create();
            string s;
            do
            {
                Console.WriteLine(cube);
                Console.WriteLine("Is finished : {0}", cube.IsFinished());

                var k = Console.ReadKey();
                if (k.Modifiers == ConsoleModifiers.Control)
                {
                    if (k.Key == ConsoleKey.RightArrow) cube.Transform(Axis.Y, Axis.X, false);
                    if (k.Key == ConsoleKey.LeftArrow) cube.Transform(Axis.X, Axis.Y, false);
                    if (k.Key == ConsoleKey.UpArrow) cube.Transform(Axis.Z, Axis.X);
                    if (k.Key == ConsoleKey.DownArrow) cube.Transform(Axis.X, Axis.Z);
                }
                else
                {
                    if (k.Key == ConsoleKey.RightArrow) cube.Transform(Axis.Y, Axis.Z, false);
                    if (k.Key == ConsoleKey.LeftArrow) cube.Transform(Axis.Z, Axis.Y, false);
                    if (k.Key == ConsoleKey.UpArrow) cube.Transform(Axis.Z, Axis.X, false);
                    if (k.Key == ConsoleKey.DownArrow) cube.Transform(Axis.X, Axis.Z, false);
                }
                //s = Console.ReadLine();
                Console.Clear();

                /*if (s == "") continue;

                var cmd = s.Split();

                var a1 = ToAxis(cmd[0][0]);
                var a2 = ToAxis(cmd[0][1]);
                if (cmd.Length == 1)
                {
                    cube.Transform(a1, a2);
                }
                else
                {
                    bool side = Convert.ToBoolean(int.Parse(cmd[1]));
                    cube.Transform(a1, a2, side);
                }*/


            } while (/*s != ""*/true);

        }
    }
}
