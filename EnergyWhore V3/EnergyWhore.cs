#pragma warning disable CS8602, CS8604, CS8625, CS8603

using System;
using BattleIA;
using Main;

namespace EnergyWhoreV3
{
    public enum StatusEnergy
    {
        Low,
        Medium,
        High,
        VeryHigh
    }

    public class EnergyIAV3
    {

        /* -----------------------------------------------------------------------------------------------------
                                            CALLING OF THE OTHER CLASSES
        --------------------------------------------------------------------------------------------------------*/

        static EnergyModeObj.NrjMode EnergyMode = new EnergyModeObj.NrjMode();

        static OpportunityShoot.OppShoot OppShoot = new OpportunityShoot.OppShoot();

        static ShieldManagment.Sheild Shield = new ShieldManagment.Sheild();

        static MassMurder.Murder MassMurder = new MassMurder.Murder();

        /* -----------------------------------------------------------------------------------------------------
                                            LOCAL VARIABLES
        --------------------------------------------------------------------------------------------------------*/

        public bool hasAShield = false;
        public StatusEnergy energyStatus = StatusEnergy.Medium;


        public void DoInit()
        {
            MainBot.ChangeScanLevel(MainBot.baseScanLevel);
        }


        public void StatusReport(UInt16 turn, UInt16 energy, UInt16 shieldLevel, UInt16 cloakLevel)
        {
            if (MainBot.hasANewScan)
            {
                MainBot.hasANewScan = false;
            }

            if (turn == 0)
            {
                MainBot.hasAScan = false;
            }

            if (shieldLevel > 0)
            {
                hasAShield = true;
            }
            else
            {
                hasAShield = false;
            }


            switch (energyStatus)
            {
                //We create energy range in wich you can be in two status depending on when you came from
                //to avoid switching between status to much
                case StatusEnergy.Low:
                    if (energy > 100)
                    {
                        energyStatus = StatusEnergy.Medium;
                    }
                    break;
                case StatusEnergy.Medium:
                    if (energy > 200)
                    {
                        energyStatus = StatusEnergy.High;
                    }
                    else if (energy < 50)
                    {
                        MainBot.hasAScan = false;
                        energyStatus = StatusEnergy.Low;
                    }
                    break;
                case StatusEnergy.High:
                    if (energy > 400)
                    {
                        energyStatus = StatusEnergy.VeryHigh;
                    }
                    else if (energy < 150)
                    {
                        MainBot.hasAScan = false;
                        energyStatus = StatusEnergy.Medium;
                    }
                    break;
                case StatusEnergy.VeryHigh:
                    if (energy < 350)
                    {
                        energyStatus = StatusEnergy.High;
                    }
                    break;
            }
        }


        public byte GetScanSurface()
        {
            if (!MainBot.hasAScan)
            {
                MainBot.xOfBot = MainBot.scanLevel;
                MainBot.yOfBot = MainBot.scanLevel;
                MainBot.listOfMoves.Clear(); //We make sure that we don't follow a path decided in a previous scan
                return MainBot.scanLevel;
            }
            return 0;
        }


        public void AreaInformation(byte distance, byte[] informations)
        {
            //TODO : Check if AreaInformation is called if you don't scan, we could then remove the following if
            //TODO : Update : I think it is called because I had errors when I removed it
            if (!MainBot.hasAScan)
            {
                //TODO : Revoves the nbennergy and remplace it by the MainBot.energyPos.Length
                //energy memory reset
                for (int i = 0; i < MainBot.nbEnergy; i++)
                {
                    MainBot.energyPos[i] = null;
                }
                MainBot.nbEnergy = 0;


                //Here is the loop that fills the MainBot.memScan tab
                int index = 0;
                for (int i = 0; i < distance; i++)
                {
                    for (int j = 0; j < distance; j++)
                    {
                        switch ((CaseState)informations[index++])
                        {
                            case CaseState.Empty: MainBot.memScan[i, j] = 0; break;
                            case CaseState.Energy: MainBot.memScan[i, j] = 1; break;
                            case CaseState.Ennemy: MainBot.memScan[i, j] = 2; break;
                            case CaseState.Wall: MainBot.memScan[i, j] = 3; break;
                        }

                        //If there is an energy, add it to the MainBot.energyPos tab
                        if (MainBot.memScan[i, j] == 1)
                        {
                            MainBot.energyPos[MainBot.nbEnergy] = new Tuple<int, int>(i, j);
                            MainBot.nbEnergy++;
                        }
                    }
                }

                MainBot.hasAScan = true;
                MainBot.hasANewScan = true;
            }
        }

        public byte[] GetAction()
        {
            //DEBUG PRINT OF ALL THE VARIABLES IF NEEDED
            if (MainBot.debug)
            {
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("DEBUG PRINT OF ALL THE VARIABLES");
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("hasAScan:" + MainBot.hasAScan);
                Console.WriteLine("hasAPath:" + MainBot.hasAPath);
                Console.WriteLine("*****");
                Console.WriteLine("energyStatus:" + energyStatus);
                Console.WriteLine("hasAShield:" + hasAShield);
                Console.WriteLine("scanLevel:" + MainBot.scanLevel);
                Console.WriteLine("*****");
                Console.WriteLine("nbEnergy:" + MainBot.nbEnergy);
                for (int i = 0; i < MainBot.nbEnergy; i++)
                {
                    Console.WriteLine("energyPos[" + i + "]:" + MainBot.energyPos[i]);
                }
                Console.WriteLine("*****");
                Console.WriteLine("listOfMoves.Count:" + MainBot.listOfMoves.Count);
                for (int i = 0; i < MainBot.listOfMoves.Count; i++)
                {
                    Console.WriteLine("listOfMoves[" + i + "]:" + MainBot.listOfMoves[i]);
                }
                Console.WriteLine("*****");
                Console.WriteLine("POS OfBot:" + MainBot.xOfBot + "/" + MainBot.yOfBot);
                Console.WriteLine("POS OfGoal:" + MainBot.xOfGoal + "/" + MainBot.yOfGoal);
                Console.WriteLine("*****");
                Console.WriteLine("memScan:");
                for (int y = 0; y < MainBot.memScan.GetLength(0); y++)
                {
                    for (int x = 0; x < MainBot.memScan.GetLength(1); x++)
                    {
                        int cellValue = MainBot.memScan[y, x];
                        string displayValue;
                        switch (cellValue)
                        {
                            case 0:
                                displayValue = ".";
                                break;
                            case 1:
                                displayValue = "1";
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case 2:
                                displayValue = "2";
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case 3:
                                displayValue = "█";
                                break;
                            default:
                                displayValue = "?";
                                break;
                        }
                        Console.Write(displayValue);
                        Console.ResetColor();
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("IsAnEnnemy:" + MainBot.IsAnEnnemy());
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("END OF DEBUG PRINT OF ALL THE VARIABLES");
                Console.WriteLine("-------------------------------------------------------------------");
            }



            if (energyStatus == StatusEnergy.Low)
            {
                if (hasAShield)
                {
                    if (MainBot.debug)
                    {
                        Console.WriteLine("You have called the shield desactivation");
                    }

                    return Shield.DesactivateShield();
                }

                if (MainBot.debug)
                {
                    Console.WriteLine("You have called the energy mode");
                }

                return EnergyMode.Execute();
            }

            if (energyStatus == StatusEnergy.Medium)
            {
                //Resets the scan levels if it was changed elsewhere
                if (MainBot.scanLevel != MainBot.baseScanLevel)
                {
                    MainBot.ChangeScanLevel(MainBot.baseScanLevel);
                }

                string temp = OppShoot.ScanOpportunityEnnemy(); //We store the result first to avoid calling the method twice
                if (temp != "None" & MainBot.hasANewScan) //the second condition is to avoid shooting a bot recorded in a previous scan
                {
                    if (MainBot.debug)
                    {
                        Console.WriteLine("You have called the oppshoot");
                    }

                    return OppShoot.Execute(temp);
                }

                if (MainBot.debug)
                {
                    Console.WriteLine("You have called the energy mode");
                }

                return EnergyMode.Execute();
            }

            if (energyStatus == StatusEnergy.High)
            {
                if (!hasAShield)
                {
                    if (MainBot.debug)
                    {
                        Console.WriteLine("You have called the shield activation");
                    }

                    return Shield.ActivateShield();
                }

                if (MainBot.IsAnEnnemy() & MainBot.hasANewScan)
                {
                    if (MainBot.debug)
                    {
                        Console.WriteLine("You have called the massmurder");
                    }

                    return MassMurder.Execute();
                }

                if (MainBot.debug)
                {
                    Console.WriteLine("You have called the energy mode");
                }

                return EnergyMode.Execute();
            }

            //VeryHigh status => you scan at evry turn
            MainBot.hasAScan = false;

            if (!hasAShield)
            {
                if (MainBot.debug)
                {
                    Console.WriteLine("You have called the shield activation");
                }

                return Shield.ActivateShield();
            }

            if (MainBot.IsAnEnnemy() & MainBot.hasANewScan)
            {
                if (MainBot.debug)
                {
                    Console.WriteLine("You have called the massmurder");
                }

                return MassMurder.Execute();
            }

            if (MainBot.debug)
            {
                Console.WriteLine("You have called the energy mode");
            }

            return EnergyMode.Execute();
        }
    }
}
