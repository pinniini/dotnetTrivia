using System;
using Newtonsoft.Json;

namespace dotnetTrivia
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set encoding;
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            GameEngine engine = new GameEngine();
            engine.RunGame(args);
        }
    }
}
