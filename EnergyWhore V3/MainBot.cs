#pragma warning disable CS8602, CS8604, CS8625, CS8603

using System;
using BattleIA;

namespace Main
{
    // Class that provide all parameters and methods taht are used in more tahn one class.
    public static class MainBot
    {
        public static int xOfGoal;
        public static int yOfGoal;
        public static int xOfBot;
        public static int yOfBot;
        public static bool hasAPath = false;
        public static byte scanLevel;
        public static byte baseScanLevel = 7;
        public static System.Collections.Generic.List<string> listOfMoves = new List<string>();
        public static int[,]? memScan;
        public static Tuple<int, int>[] energyPos = new Tuple<int, int>[1000];
        public static int nbEnergy = 0;
        public static bool hasAScan = false;
        public static bool hasANewScan = false;
        public static bool debug = true;


        public static byte[] FollowPath()
        {
            //TODO : Create an efficient way to handle the case when the pathfinding doesn't find a path

            string move = listOfMoves[0];
            listOfMoves.RemoveAt(0);

            if (move == "up")
            {
                if (MainBot.debug)
                {
                    Console.WriteLine("up");
                }
                MainBot.xOfBot = MainBot.xOfBot - 1;
                return BotHelper.ActionMove(MoveDirection.North);
            }
            else if (move == "right")
            {
                if (MainBot.debug)
                {
                    Console.WriteLine("right");
                }
                MainBot.yOfBot = MainBot.yOfBot + 1;
                return BotHelper.ActionMove(MoveDirection.West);
            }
            else if (move == "down")
            {
                if (MainBot.debug)
                {
                    Console.WriteLine("down");
                }
                MainBot.xOfBot = MainBot.xOfBot + 1;
                return BotHelper.ActionMove(MoveDirection.South);
            }
            else if (move == "left")
            {
                if (MainBot.debug)
                {
                    Console.WriteLine("left");
                }
                MainBot.yOfBot = MainBot.yOfBot - 1;
                return BotHelper.ActionMove(MoveDirection.East);
            }
            else
            {
                if (MainBot.debug)
                {
                    Console.WriteLine("No path found");
                }
                MainBot.xOfBot = MainBot.xOfBot - 1;
                return BotHelper.ActionMove(MoveDirection.North);
            }
        }

        public static bool IsCaseFree(int x, int y)
        {
            return (memScan[x, y] == 3 || memScan[x, y] == 2) ? false : true;
        }

        public static bool IsCaseOurself(int x, int y)
        {
            //Her we use scanLevel and not xOfBot or yOfBot because we use this method only just after a scan
            //TODO : Make sure that xOfBot and yOfBot are always correct and that we can use them here
            return (x == scanLevel & y == scanLevel) ? true : false;
        }

        public static void ChangeScanLevel(byte scanLevel)
        {
            if (scanLevel > 10)
            {
                scanLevel = 10;
            }
            MainBot.scanLevel = scanLevel;
            MainBot.memScan = new int[scanLevel * 2 + 1, scanLevel * 2 + 1];
            hasAScan = false; //We want to scan evry time we change the scan level to avoid having an empty scan
        }

        public static bool IsAnEnnemy()
        {
            for (int i = 0; i < MainBot.scanLevel * 2 + 1; i++)
            {
                for (int j = 0; j < MainBot.scanLevel * 2 + 1; j++)
                {
                    //We check for an ennemy but because our bot is stored as a 2 in the memory scan
                    //we need to make sure that we don't check for ourself
                    if (MainBot.memScan[i, j] == 2 && !IsCaseOurself(i, j))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}