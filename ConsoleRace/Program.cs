using ConsoleRace.Services;
using System;
using System.Drawing;
using System.Numerics;

namespace ConsoleRace
{
    internal class Program
    {


        static void Main(string[] args)
        {
            Random rnd = new Random();
            Console.Clear();
            ConsoleService.Init();
            do
            {
                while (!Console.KeyAvailable)
                {
                    ConsoleService.MoveDrawTrack();
                    Thread.Sleep(100);

                }
            }
            while (Console.ReadKey(true).Key != ConsoleKey.Q);
        }





    }
}