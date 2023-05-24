#pragma warning disable CS8602, CS8604, CS8625, CS8603

using System;
using Main;

namespace EnergyModeObj
{

    // Class that provide all parameters and methods for when the bot is activly searching for energy
    public class NrjMode
    {
        static AStarPathfinding.Program PathFinding = new AStarPathfinding.Program();

        public int index = 0;


        public byte[] Execute()
        {
            //TODO : When the energy is on the 0/0 pos the bot goes there and i think that the energy is not removed so it crashes
            //TODO : Update : same for each corner
            //TODO : Update : sometimes the bot tries to go to a corner that has an ennemy on it

            //Resets the scan levels if it was changed elsewhere
            if (MainBot.scanLevel != MainBot.baseScanLevel)
            {
                MainBot.ChangeScanLevel(MainBot.baseScanLevel);
            }

            //Detects if you have reached an energy and resets all the parameters
            if (MainBot.listOfMoves.Count == 0)
            {
                MainBot.hasAPath = false;
                if (MainBot.nbEnergy > 0)
                {
                    for (int i = 0; i < MainBot.nbEnergy; i++)
                    {
                        if (MainBot.energyPos[i].Item1 == MainBot.xOfGoal && MainBot.energyPos[i].Item2 == MainBot.yOfGoal)
                        {
                            RemoveLastEnergy();
                        }
                    }
                }

                //We check with a if and not an else because the RemoveLastEnergy() function 
                // can empty the list of energy
                if (MainBot.nbEnergy == 0)
                {

                    MainBot.hasAScan = false;
                }
            }

            if (!MainBot.hasAPath)
            {
                if (MainBot.nbEnergy > 0)
                {
                    FindClosestEnergy();
                }
                else
                {
                    RandomCorner();
                }

                MainBot.listOfMoves = PathFinding.Execute(MainBot.memScan);

                if (MainBot.listOfMoves.Count > 0)
                {
                    MainBot.hasAPath = true;
                }
                else
                {
                    //TODO : insert here what we do if the pathfinding doesn't find a path
                }
            }
            return (MainBot.FollowPath());
        }


        void RandomCorner()
        {
            //make a random switch with 4 option to direct the bot to a random corner if there is no energy in the scan
            Random rnd = new Random();
            int random = rnd.Next(1, 5);
            switch (random)
            {
                case 1:
                    //We first check that the corner is free
                    //A probleme may occur if all 4 corner are not free
                    //TODO : Create the function that automatically takes us to the closest free case of the wanted corner
                    if (MainBot.IsCaseFree(0, 0))
                    {
                        MainBot.xOfGoal = 0;
                        MainBot.yOfGoal = 0;
                    }
                    else if (MainBot.IsCaseFree(0, MainBot.memScan.GetLength(1) - 1))
                    {
                        MainBot.xOfGoal = 0;
                        MainBot.yOfGoal = MainBot.memScan.GetLength(1) - 1;
                    }
                    else if (MainBot.IsCaseFree(MainBot.memScan.GetLength(0) - 1, 0))
                    {
                        MainBot.xOfGoal = MainBot.memScan.GetLength(0) - 1;
                        MainBot.yOfGoal = 0;
                    }
                    else if (MainBot.IsCaseFree(MainBot.memScan.GetLength(0) - 1, MainBot.memScan.GetLength(1) - 1))
                    {
                        MainBot.xOfGoal = MainBot.memScan.GetLength(0) - 1;
                        MainBot.yOfGoal = MainBot.memScan.GetLength(1) - 1;
                    }
                    break;
                case 2:
                    if (MainBot.IsCaseFree(0, MainBot.memScan.GetLength(1) - 1))
                    {
                        MainBot.xOfGoal = 0;
                        MainBot.yOfGoal = MainBot.memScan.GetLength(1) - 1;
                    }
                    else if (MainBot.IsCaseFree(MainBot.memScan.GetLength(0) - 1, 0))
                    {
                        MainBot.xOfGoal = MainBot.memScan.GetLength(0) - 1;
                        MainBot.yOfGoal = 0;
                    }
                    else if (MainBot.IsCaseFree(MainBot.memScan.GetLength(0) - 1, MainBot.memScan.GetLength(1) - 1))
                    {
                        MainBot.xOfGoal = MainBot.memScan.GetLength(0) - 1;
                        MainBot.yOfGoal = MainBot.memScan.GetLength(1) - 1;
                    }
                    else if (MainBot.IsCaseFree(0, 0))
                    {
                        MainBot.xOfGoal = 0;
                        MainBot.yOfGoal = 0;
                    }
                    break;
                case 3:
                    if (MainBot.IsCaseFree(MainBot.memScan.GetLength(0) - 1, 0))
                    {
                        MainBot.xOfGoal = MainBot.memScan.GetLength(0) - 1;
                        MainBot.yOfGoal = 0;
                    }
                    else if (MainBot.IsCaseFree(MainBot.memScan.GetLength(0) - 1, MainBot.memScan.GetLength(1) - 1))
                    {
                        MainBot.xOfGoal = MainBot.memScan.GetLength(0) - 1;
                        MainBot.yOfGoal = MainBot.memScan.GetLength(1) - 1;
                    }
                    else if (MainBot.IsCaseFree(0, 0))
                    {
                        MainBot.xOfGoal = 0;
                        MainBot.yOfGoal = 0;
                    }
                    else if (MainBot.IsCaseFree(0, MainBot.memScan.GetLength(1) - 1))
                    {
                        MainBot.xOfGoal = 0;
                        MainBot.yOfGoal = MainBot.memScan.GetLength(1) - 1;
                    }
                    break;
                case 4:
                    if (MainBot.IsCaseFree(MainBot.memScan.GetLength(0) - 1, MainBot.memScan.GetLength(1) - 1))
                    {
                        MainBot.xOfGoal = MainBot.memScan.GetLength(0) - 1;
                        MainBot.yOfGoal = MainBot.memScan.GetLength(1) - 1;
                    }
                    else if (MainBot.IsCaseFree(0, 0))
                    {
                        MainBot.xOfGoal = 0;
                        MainBot.yOfGoal = 0;
                    }
                    else if (MainBot.IsCaseFree(0, MainBot.memScan.GetLength(1) - 1))
                    {
                        MainBot.xOfGoal = 0;
                        MainBot.yOfGoal = MainBot.memScan.GetLength(1) - 1;
                    }
                    else if (MainBot.IsCaseFree(MainBot.memScan.GetLength(0) - 1, 0))
                    {
                        MainBot.xOfGoal = MainBot.memScan.GetLength(0) - 1;
                        MainBot.yOfGoal = 0;
                    }
                    break;
            }
        }

        void FindClosestEnergy()
        {
            int min = 100;
            for (int i = 0; i < MainBot.nbEnergy; i++)
            {

                //Calculate the distance
                int xDistance = Math.Abs(MainBot.energyPos[i].Item1 - MainBot.xOfBot);
                int yDistance = Math.Abs(MainBot.energyPos[i].Item2 - MainBot.yOfBot);
                int distance = xDistance + yDistance;

                //Find the closest energy
                if (distance < min)
                {
                    min = distance;
                    MainBot.xOfGoal = MainBot.energyPos[i].Item1;
                    MainBot.yOfGoal = MainBot.energyPos[i].Item2;
                    index = i;
                }
            }
        }
        void RemoveLastEnergy()
        {
            MainBot.nbEnergy--;
            for (int i = 0; i < MainBot.nbEnergy; i++)
            {
                if (MainBot.energyPos[i].Item1 == MainBot.xOfGoal && MainBot.energyPos[i].Item2 == MainBot.yOfGoal)
                {
                    MainBot.energyPos[i] = MainBot.energyPos[MainBot.nbEnergy];
                    MainBot.energyPos[MainBot.nbEnergy] = null;
                }
            }
        }
    }
}