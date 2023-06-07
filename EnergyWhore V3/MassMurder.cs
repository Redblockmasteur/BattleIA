#pragma warning disable CS8602, CS8604, CS8625, CS8603

using System;
using BattleIA;
using Main;

namespace MassMurder
{
    //Class that provide all parameters and methods for when the bot is trying to kill the ennemy
    public class Murder
    {
        AStarPathfinding.Program PathFinding = new AStarPathfinding.Program();
        public int nbEnnemy = 0;
        public Tuple<int, int>[] ennemyPos = new Tuple<int, int>[100];

        public byte[] Execute()
        {
            GetEnnemyPos();

            if (GetShortestDistance(ClosestEnnemy()) == 0)
            {
                //Here we adapt the scan to the distance with the ennemy +1 to take in consideretion
                //the fact that the ennemy could move out of our scan
                string temp = GetDirectionOfLineEnnemy(ClosestEnnemy());
                ChangeScanToEnnemyPlusOne();
                return Shoot(temp);
            }

            else
            {
                //The DefineGoal function sets the global variables for the pathfinding and return
                //true or false to make sure that we are not trying to go somewhere we can't (a wall)
                if (DefineGoal())
                {
                    MainBot.listOfMoves = PathFinding.Execute(MainBot.memScan);
                    ChangeScanToEnnemyPlusOne();
                    return MainBot.FollowPath();
                }
                return BotHelper.ActionNone();
            }
        }

        bool DefineGoal()
        {
            int xOfEnnemy = ennemyPos[ClosestEnnemy()].Item1;
            int yOfEnnemy = ennemyPos[ClosestEnnemy()].Item2;
            if (Math.Abs(xOfEnnemy - MainBot.xOfBot) == GetShortestDistance(ClosestEnnemy()))
            {
                //We check three position to see if we can go there, if not we abort the pathfinding
                if (MainBot.memScan[xOfEnnemy, MainBot.yOfBot] != 3)
                {
                    MainBot.xOfGoal = xOfEnnemy;
                    MainBot.yOfGoal = MainBot.yOfBot;
                }
                else if (MainBot.memScan[xOfEnnemy, MainBot.yOfBot + 1] != 3)
                {
                    MainBot.xOfGoal = xOfEnnemy;
                    MainBot.yOfGoal = MainBot.yOfBot + 1;
                }
                else if (MainBot.memScan[xOfEnnemy, MainBot.yOfBot - 1] != 3)
                {
                    MainBot.xOfGoal = xOfEnnemy;
                    MainBot.yOfGoal = MainBot.yOfBot - 1;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (MainBot.memScan[MainBot.xOfBot, yOfEnnemy] != 3)
                {
                    MainBot.xOfGoal = MainBot.xOfBot;
                    MainBot.yOfGoal = yOfEnnemy;
                }
                else if (MainBot.memScan[MainBot.xOfBot + 1, yOfEnnemy] != 3)
                {
                    MainBot.xOfGoal = MainBot.xOfBot + 1;
                    MainBot.yOfGoal = yOfEnnemy;
                }
                else if (MainBot.memScan[MainBot.xOfBot - 1, yOfEnnemy] != 3)
                {
                    MainBot.xOfGoal = MainBot.xOfBot - 1;
                    MainBot.yOfGoal = yOfEnnemy;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        byte[] Shoot(string dir)
        {
            if (dir == "North")
            {
                return BotHelper.ActionShoot(MoveDirection.North);
            }
            if (dir == "South")
            {
                return BotHelper.ActionShoot(MoveDirection.South);
            }
            if (dir == "West")
            {
                return BotHelper.ActionShoot(MoveDirection.East);
            }
            else
            {
                return BotHelper.ActionShoot(MoveDirection.West);
            }
        }


        string GetDirectionOfLineEnnemy(int index)
        {
            //TODO : Check the recording of x and y pos, i think there are not the same in all the function
            //TODO : It works like that but doesn't make sense

            if (MainBot.debug)
            {
                Console.WriteLine("Ennemy pos : " + ennemyPos[index].Item1 + " " + ennemyPos[index].Item2);
                Console.WriteLine("Bot pos : " + MainBot.scanLevel + " " + MainBot.scanLevel);
            }



            if (MainBot.scanLevel == ennemyPos[index].Item1)
            {
                if (MainBot.scanLevel > ennemyPos[index].Item2)
                {
                    if (MainBot.debug)
                    {
                        Console.WriteLine("Shoot West");
                    }
                    return "West";
                }
                else
                {
                    if (MainBot.debug)
                    {
                        Console.WriteLine("Shoot East");
                    }
                    return "East";
                }
            }
            else
            {
                if (MainBot.scanLevel > ennemyPos[index].Item1)
                {
                    if (MainBot.debug)
                    {
                        Console.WriteLine("Shoot North");
                    }
                    return "North";
                }
                else
                {
                    if (MainBot.debug)
                    {
                        Console.WriteLine("Shoot South");
                    }
                    return "South";
                }
            }

        }

        int GetShortestDistance(int index)
        {
            //Calculate the distance between the bot and the ennemyPos[index]
            int distX = Math.Abs(MainBot.scanLevel - ennemyPos[index].Item1);
            int distY = Math.Abs(MainBot.scanLevel - ennemyPos[index].Item2);
            int distMin = Math.Min(distX, distY);
            return distMin;
        }


        void GetEnnemyPos()
        {
            //Empty ennemyPos List
            if (ennemyPos != null)
            {
                Array.Clear(ennemyPos, 0, ennemyPos.Length);
            }
            nbEnnemy = 0;

            if (MainBot.debug)
            {
                Console.WriteLine("I try to get the ennemy pos");
            }

            //Go through the memScan
            for (int i = 0; i < MainBot.scanLevel * 2 + 1; i++)
            {
                for (int j = 0; j < MainBot.scanLevel * 2 + 1; j++)
                {
                    if (MainBot.memScan[i, j] == 2 && !MainBot.IsCaseOurself(i, j))
                    {
                        if (MainBot.debug)
                        {
                            Console.WriteLine("I found an ennemy at : " + i + " " + j);
                        }
                        ennemyPos[nbEnnemy] = new Tuple<int, int>(i, j);
                        nbEnnemy++;
                    }
                }
            }
            if (MainBot.debug)
            {
                Console.WriteLine("I found " + nbEnnemy + " ennemy");
                for (int i = 0; i < nbEnnemy; i++)
                {
                    Console.WriteLine("Ennemy " + i + " : " + ennemyPos[i].Item1 + " " + ennemyPos[i].Item2);
                }
            }
        }

        int ClosestEnnemy()
        {
            int index = 0;
            int minDistance = 100;
            for (int i = 0; i < nbEnnemy; i++)
            {
                int distance = Math.Min(Math.Abs(ennemyPos[i].Item1 - MainBot.scanLevel), Math.Abs(ennemyPos[i].Item2 - MainBot.scanLevel));
                if (distance < minDistance)
                {
                    minDistance = distance;
                    index = i;
                }
            }
            return index;
        }

        void ChangeScanToEnnemyPlusOne()
        {
            byte newScanLevel = (byte)(Math.Max(Math.Abs(MainBot.xOfBot - ennemyPos[ClosestEnnemy()].Item1), Math.Abs(MainBot.yOfBot - ennemyPos[ClosestEnnemy()].Item2)) + 1);
            MainBot.ChangeScanLevel(newScanLevel);
            MainBot.hasAScan = false;
        }
    }
}
