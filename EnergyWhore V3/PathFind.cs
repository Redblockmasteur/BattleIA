#pragma warning disable CS8602, CS8604, CS8625, CS8603

using System;
using Main;

namespace AStarPathfinding
{

    public class Location
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int F { get; set; }
        public int G { get; set; }
        public int H { get; set; }
        public Location? Parent { get; set; }

        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Program

    {

        public System.Collections.Generic.List<string> Execute(int[,] map)
        {
            Console.WriteLine("-----PATHFINDING ALGORITHM-----");
            var path = FindPath(map);
            Console.WriteLine("Path:");
            //Print the path
            foreach (var move in path)
            {
                Console.WriteLine(move);
            }
            //Print the coordonates of the goal and of the bot
            Console.WriteLine("Goal: " + MainBot.xOfGoal + " " + MainBot.yOfGoal);
            Console.WriteLine("Bot: " + MainBot.xOfBot + " " + MainBot.yOfBot);

            return path;
        }
        public List<string> FindPath(int[,] map)
        {
            int width = map.GetLength(1);
            int height = map.GetLength(0);

            var openList = new List<Location>();

            var closedList = new List<Location>();

            var start = new Location(MainBot.yOfBot, MainBot.xOfBot);

            var goal = new Location(MainBot.yOfGoal, MainBot.xOfGoal);

            // add start location to open list

            openList.Add(start);

            while (openList.Count > 0)
            {
                // find the location with the lowest F score
                var current = openList[0];
                for (int i = 1; i < openList.Count; i++)
                {
                    if (openList[i].F < current.F)
                    {
                        current = openList[i];
                    }
                }

                // if we've reached the goal, we're done
                if (current.X == goal.X && current.Y == goal.Y)
                {
                    return GetPath(current);
                }

                // move current location from open to closed list
                openList.Remove(current);
                closedList.Add(current);

                // find adjacent locations
                var adjacentLocations = new List<Location>()
                {
                    new Location(current.X - 1, current.Y), // left
                    new Location(current.X + 1, current.Y), // right
                    new Location(current.X, current.Y - 1), // up
                    new Location(current.X, current.Y + 1)  // down
                };

                foreach (var location in adjacentLocations)
                {
                    // ignore locations outside of the map
                    if (location.X < 0 || location.X >= width || location.Y < 0 || location.Y >= height)
                    {
                        continue;
                    }

                    // ignore obstacles and closed locations
                    if (map[location.Y, location.X] == 3 || map[location.Y, location.X] == 2 || closedList.Exists(l => l.X == location.X && l.Y == location.Y))
                    {
                        continue;
                    }

                    // calculate G, H and F scores
                    int gScore = current.G + 1;
                    int hScore = Math.Abs(location.X - goal.X) + Math.Abs(location.Y - goal.Y);
                    int fScore = gScore + hScore;

                    // if location
                    var openLocation = openList.Find(l => l.X == location.X && l.Y == location.Y);
                    if (openLocation == null)
                    {
                        openLocation = new Location(location.X, location.Y);
                        openLocation.G = gScore;
                        openLocation.H = hScore;
                        openLocation.F = fScore;
                        openLocation.Parent = current;
                        openList.Add(openLocation);
                    }

                    else if (gScore < openLocation.G)
                    {
                        // this path to the location is better, so update it
                        openLocation.G = gScore;
                        openLocation.F = gScore + openLocation.H;
                        openLocation.Parent = current;
                    }
                }
            }

            // no path found
            return new List<string>();
        }

        public List<string> GetPath(Location location)
        {
            var path = new List<string>();

            while (location.Parent != null)
            {
                int dx = location.X - location.Parent.X;
                int dy = location.Y - location.Parent.Y;

                if (dx == -1)
                {
                    path.Insert(0, "left");
                }

                else if (dx == 1)
                {
                    path.Insert(0, "right");
                }

                else if (dy == -1)
                {
                    path.Insert(0, "up");
                }

                else if (dy == 1)
                {
                    path.Insert(0, "down");
                }
                location = location.Parent;
            }
            return path;
        }
    }
}