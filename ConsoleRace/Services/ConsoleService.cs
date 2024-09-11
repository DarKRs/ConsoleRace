using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleRace.Services
{
    internal static class ConsoleService
    {
        const int BufferWidth = 120;
        const int BufferHeight = 40;
        public const int WindowWidth = 120;
        public const int WindowHeight = 40;

        static int trackWidth = 80;
        static int trackHeight = WindowHeight - 1;
        static List<string> track = new List<string>();
        static int currentLeft = trackWidth / 2 - 10;
        static int currentRight = trackWidth / 2 + 10;
        static int turnLength = 0; 
        static int turnDirection = 0; 
        static int minRoadWidth = 8; // Минимальная ширина дороги
        static List<Tuple<int, int>> obstacles = new List<Tuple<int, int>>(); // Список препятствий (позиция в строке, оставшееся время жизни)

        static char[,] buffer;
        static HashSet<Rectangle> dirtyRectangles = new HashSet<Rectangle>();
        static Random random = new Random();

        public static void Init()
        {
            SetConsoleSize();
            for (int i = 0; i < trackHeight; i++)
            {
                track.Add(GenerateTrackSegment());
            }
        }

        public static void MoveDrawTrack()
        {
            MoveTrack();
            DrawTrack();
        }

        static string GenerateTrackSegment()
        {
            if (turnLength <= 0)
            {
                turnDirection = random.Next(-1, 2); // -1 = влево, 0 = прямо, 1 = вправо
                turnLength = random.Next(3, 10); // Длина поворота
            }
            else
            {
                turnLength--; 
            }

            if (turnDirection == -1) // Поворот влево
            {
                if (currentLeft > 1) 
                {
                    currentLeft--;
                    currentRight = Math.Max(currentLeft + minRoadWidth, currentRight - 1);
                }
            }
            else if (turnDirection == 1) // Поворот вправо
            {
                if (currentRight < trackWidth - 2) 
                {
                    currentRight++;
                    currentLeft = Math.Min(currentRight - minRoadWidth, currentLeft + 1); 
                }
            }

            char[] segment = new char[trackWidth];
            for (int i = 0; i < trackWidth; i++)
            {
                if (i >= currentLeft && i <= currentRight)
                {
                    segment[i] = ' '; // Дорога
                }
                else
                {
                    segment[i] = '|'; // Стены
                }
            }

            // Добавляем препятствия с небольшой вероятностью
            if (random.Next(0, 10) < 2) 
            {
                int obstaclePosition = random.Next(currentLeft + 1, currentRight - 1);
                segment[obstaclePosition] = '#'; 
                obstacles.Add(new Tuple<int, int>(obstaclePosition, trackHeight)); 
            }

            return new string(segment);
        }



        static void MoveTrack()
        {
            track.RemoveAt(track.Count - 1); 
            track.Insert(0, GenerateTrackSegment());

            
            for (int i = obstacles.Count - 1; i >= 0; i--)
            {
                var obstacle = obstacles[i];
                if (obstacle.Item2 - 1 <= 0)
                {
                    obstacles.RemoveAt(i); 
                }
                else
                {
                    obstacles[i] = new Tuple<int, int>(obstacle.Item1, obstacle.Item2 - 1); 
                }
            }
        }


        static void DrawTrack()
        {
            int padding = (WindowWidth - trackWidth) / 2;

            Console.SetCursorPosition(0, 0); 
            foreach (string segment in track)
            {
                Console.Write(new string(' ', padding));
                Console.WriteLine(segment);
            }

            // Очищаем последнюю строку
            Console.SetCursorPosition(0, WindowHeight - 1);
            Console.Write(new string(' ', WindowHeight));
        }



        static void SetConsoleSize()
        {
            var currentBufferSize = Console.BufferWidth;
            var currentWindowSize = Console.WindowWidth;

            const int maxBufferWidth = 8191;
            const int maxBufferHeight = 30000;
            const int maxWindowWidth = 8191;
            const int maxWindowHeight = 30000;



            try
            {
                Console.SetBufferSize(
                    Math.Min(BufferWidth, maxBufferWidth),
                    Math.Min(BufferHeight, maxBufferHeight)
                );

                Console.SetWindowSize(
                    Math.Min(WindowWidth, maxWindowWidth),
                    Math.Min(WindowHeight, maxWindowHeight)
                );
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Error setting console size: {ex.Message}");
            }
        }

    }
}
