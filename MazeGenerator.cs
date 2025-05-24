using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabyrinthGame
{
    public static class MazeGenerator
    {
        public static void Generate(int width, int height, out bool[,] walls)
        {
            walls = new bool[width, height];
            var random = new Random();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    walls[x, y] = true;
                }
            }

            int startX = 1;
            int startY = 1;
            walls[startX, startY] = false;

            var wallList = new List<Wall>();
            AddWalls(startX, startY, walls, wallList);

            while (wallList.Count > 0)
            {
                int randomIndex = random.Next(0, wallList.Count);
                Wall wall = wallList[randomIndex];
                wallList.RemoveAt(randomIndex);

                int x = wall.X;
                int y = wall.Y;
                int oppositeX = wall.OppositeX;
                int oppositeY = wall.OppositeY;

                if (oppositeX > 0 && oppositeX < width - 1 &&
                    oppositeY > 0 && oppositeY < height - 1 &&
                    walls[oppositeX, oppositeY])
                {
                    walls[x, y] = false;
                    walls[oppositeX, oppositeY] = false;
                    AddWalls(oppositeX, oppositeY, walls, wallList);
                }
            }

            walls[1, 0] = false;
            walls[width - 2, height - 1] = false;
        }

        private static void AddWalls(int x, int y, bool[,] walls, List<Wall> wallList)
        {
            int width = walls.GetLength(0);
            int height = walls.GetLength(1);

            if (x > 1 && walls[x - 1, y]) wallList.Add(new Wall(x - 1, y, x - 2, y));
            if (x < width - 2 && walls[x + 1, y]) wallList.Add(new Wall(x + 1, y, x + 2, y));
            if (y > 1 && walls[x, y - 1]) wallList.Add(new Wall(x, y - 1, x, y - 2));
            if (y < height - 2 && walls[x, y + 1]) wallList.Add(new Wall(x, y + 1, x, y + 2));
        }

        private struct Wall
        {
            public int X { get; }
            public int Y { get; }
            public int OppositeX { get; }
            public int OppositeY { get; }

            public Wall(int x, int y, int oppositeX, int oppositeY)
            {
                X = x;
                Y = y;
                OppositeX = oppositeX;
                OppositeY = oppositeY;
            }
        }
    }
}