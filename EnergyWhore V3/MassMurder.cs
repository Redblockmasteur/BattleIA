#pragma warning disable CS8602, CS8604, CS8625, CS8603

using System;
using BattleIA;
using Main;

namespace MassMurder
{
    public class Murder
    {
        AStarPathfinding.Program PathFinding = new AStarPathfinding.Program();
        public int nbEnnemy = 0;
        public Tuple<int, int>[] ennemyPos = new Tuple<int, int>[100];
        bool debug = true;


        public byte[] Execute()
        {
            GetEnnemyPos();

            if (GetShortestDistance(ClosestEnnemy()) == 0)
            {
                //Here we adapt the scan to the distance with the ennemy +1 to take in consideretion
                //the fact taht the ennemy could move out of our scan
                ChangeScanToEnnemyPlusOne();
                return Shoot(GetDirectionOfLineEnnemy(ClosestEnnemy()));
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
            if (MainBot.scanLevel == ennemyPos[index].Item1)
            {
                if (MainBot.scanLevel > ennemyPos[index].Item2)
                {
                    return "West";
                }
                else
                {
                    return "East";
                }
            }
            else
            {
                if (MainBot.scanLevel > ennemyPos[index].Item1)
                {
                    return "North";
                }
                else
                {
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

            int scanSize = MainBot.memScan.GetLength(0);

            //Go through the memScan
            for (int i = 0; i < scanSize; i++)
            {
                for (int j = 0; j < scanSize; j++)
                {
                    if (MainBot.memScan[i, j] == 2 && !MainBot.IsCaseOurself(i, j))
                    {
                        ennemyPos[nbEnnemy] = new Tuple<int, int>(i, j);
                        nbEnnemy++;
                    }
                }
            }
        }

        int ClosestEnnemy()
        {
            //Now go through the tuple and find the closest ennemy
            int index = 0;
            int minDistance = 100;
            for (int i = 0; i < nbEnnemy; i++)
            {
                int mainDistance;
                int distanceX = Math.Abs(ennemyPos[i].Item1 - MainBot.scanLevel);
                int distanceY = Math.Abs(ennemyPos[i].Item2 - MainBot.scanLevel);
                mainDistance = Math.Min(distanceX, distanceY);
                if (mainDistance < minDistance)
                {
                    minDistance = mainDistance;
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
