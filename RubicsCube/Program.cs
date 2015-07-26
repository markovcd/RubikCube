using System;


namespace RubiksCube
{
    class Program
    {
        static void CubeInteface()
        {
            var cube = new Cube(100);
            
            do
            {
                Console.Clear();
                Console.WriteLine(cube);
                Console.WriteLine("Is finished : {0}", cube.IsFinished());
                Console.WriteLine("Hash code : {0}", cube.GetHashCode());

                var k = Console.ReadKey();
                if (k.Modifiers == ConsoleModifiers.Control)
                {
                    if (k.Key == ConsoleKey.RightArrow) cube = cube.Transform(Axis.Y, Axis.X, false);
                    if (k.Key == ConsoleKey.LeftArrow) cube = cube.Transform(Axis.X, Axis.Y, false);
                    if (k.Key == ConsoleKey.UpArrow) cube = cube.Transform(Axis.Z, Axis.X);
                    if (k.Key == ConsoleKey.DownArrow) cube = cube.Transform(Axis.X, Axis.Z);
                }
                else
                {
                    if (k.Key == ConsoleKey.RightArrow) cube = cube.Transform(Axis.Y, Axis.Z, false);
                    if (k.Key == ConsoleKey.LeftArrow) cube = cube.Transform(Axis.Z, Axis.Y, false);
                    if (k.Key == ConsoleKey.UpArrow) cube = cube.Transform(Axis.Z, Axis.X, false);
                    if (k.Key == ConsoleKey.DownArrow) cube = cube.Transform(Axis.X, Axis.Z, false);
                } 
            } while (true);
        }

        public static void BfsInterface()
        {
            while (true)
            {
                var cube = new Cube(100);

                var bfs = new BreadthFirstSearch<Cube>();
                var found = bfs.Find(cube, c => c.IsFinished());

                Console.WriteLine(cube);
                Console.WriteLine();
                Console.WriteLine("Can be solved after {0} moves.", found.Steps);
                Console.ReadLine();
            }
        }

        static void Main(string[] args)
        {
            Console.Write("Launch BFS? y/n: ");

            if (Console.ReadLine() == "y")
                BfsInterface();
            else
                CubeInteface();
        }
    }
}
