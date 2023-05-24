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

        public ushort currentEnergy = 100;
        public bool hasAShield = false;
        public bool debug = true;
        public StatusEnergy energyStatus = StatusEnergy.Medium;



        // ****************************************************************************************************
        // Ne s'exécute qu'une seule fois au tout début
        // C'est ici qu'il faut initialiser le bot
        public void DoInit()
        {
            MainBot.nbEnergy = 0;
            MainBot.ChangeScanLevel(MainBot.baseScanLevel);
        }

        // ****************************************************************************************************
        /// Réception de la mise à jour des informations du bot
        public void StatusReport(UInt16 turn, UInt16 energy, UInt16 shieldLevel, UInt16 cloakLevel)
        {
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

            currentEnergy = energy;

            switch (energyStatus)
            {
                //We create energy range in wich you can be in two status depending on when you came from
                //to avoid switching between status to much
                case StatusEnergy.Low:
                    if (currentEnergy > 100)
                    {
                        energyStatus = StatusEnergy.Medium;
                    }
                    break;
                case StatusEnergy.Medium:
                    if (currentEnergy > 200)
                    {
                        energyStatus = StatusEnergy.High;
                    }
                    else if (currentEnergy < 50)
                    {
                        energyStatus = StatusEnergy.Low;
                    }
                    break;
                case StatusEnergy.High:
                    if (currentEnergy > 400)
                    {
                        energyStatus = StatusEnergy.VeryHigh;
                    }
                    else if (currentEnergy < 150)
                    {
                        energyStatus = StatusEnergy.Medium;
                    }
                    break;
                case StatusEnergy.VeryHigh:
                    if (currentEnergy < 350)
                    {
                        energyStatus = StatusEnergy.High;
                    }
                    break;
            }
        }

        // ****************************************************************************************************
        /// On nous demande la distance de scan que l'on veut effectuer
        public byte GetScanSurface()
        {
            if (!MainBot.hasAScan)
            {
                MainBot.xOfBot = MainBot.scanLevel;
                MainBot.yOfBot = MainBot.scanLevel;
                MainBot.listOfMoves.Clear();
                return MainBot.scanLevel;
            }
            return 0;
        }

        // ****************************************************************************************************
        /// Résultat du scan
        public void AreaInformation(byte distance, byte[] informations)
        {
            //TODO : Check if AreaInformation is called if you don't scan, we could then remove the following if
            if (!MainBot.hasAScan)
            {
                //energy memory reset
                for (int i = 0; i < MainBot.nbEnergy; i++)
                {
                    MainBot.energyPos[i] = null;
                }
                MainBot.nbEnergy = 0;
                //TODO : Removes the nbEnergy parameter by using MainBot.energyPos.Length


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

                //DEBUG PRINT
                Console.WriteLine("Scan:");
                for (int y = 0; y < MainBot.memScan.GetLength(0); y++)
                {
                    for (int x = 0; x < MainBot.memScan.GetLength(1); x++)
                    {
                        Console.Write(MainBot.memScan[y, x]);
                    }
                    Console.WriteLine();
                }
                MainBot.hasAScan = true;
            }
        }

        // ****************************************************************************************************
        /// Action à effectuer

        public byte[] GetAction()
        {
            //DEBUG PRINT OF ALL THE VARIABLES IF NEEDED
            if (debug)
            {
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("DEBUG PRINT OF ALL THE VARIABLES");
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("hasAScan:" + MainBot.hasAScan);
                Console.WriteLine("hasAPath:" + MainBot.hasAPath);
                Console.WriteLine("*****");
                //Energy status
                Console.WriteLine("energyStatus:" + energyStatus);
                //hasAShield
                Console.WriteLine("hasAShield:" + hasAShield);
                //scanLevel
                Console.WriteLine("scanLevel:" + MainBot.scanLevel);
                Console.WriteLine("*****");
                //nbEnergy + energyPos
                Console.WriteLine("nbEnergy:" + MainBot.nbEnergy);
                for (int i = 0; i < MainBot.nbEnergy; i++)
                {
                    Console.WriteLine("energyPos[" + i + "]:" + MainBot.energyPos[i]);
                }
                Console.WriteLine("*****");
                //listOfMoves
                Console.WriteLine("listOfMoves.Count:" + MainBot.listOfMoves.Count);
                for (int i = 0; i < MainBot.listOfMoves.Count; i++)
                {
                    Console.WriteLine("listOfMoves[" + i + "]:" + MainBot.listOfMoves[i]);
                }
                Console.WriteLine("*****");
                //xOfBot + yOfBot
                Console.WriteLine("POS OfBot:" + MainBot.xOfBot + "/" + MainBot.yOfBot);
                //xOfGoal + yOfGoal
                Console.WriteLine("POS OfGoal:" + MainBot.xOfGoal + "/" + MainBot.yOfGoal);
                Console.WriteLine("*****");
                //memScan
                Console.WriteLine("memScan:");
                for (int y = 0; y < MainBot.memScan.GetLength(0); y++)
                {
                    for (int x = 0; x < MainBot.memScan.GetLength(1); x++)
                    {
                        Console.Write(MainBot.memScan[y, x]);
                    }
                    Console.WriteLine();
                }
                //Print the retunr of the Is an ennemy function
                Console.WriteLine("IsAnEnnemy:" + MainBot.IsAnEnnemy());
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("END OF DEBUG PRINT OF ALL THE VARIABLES");
                Console.WriteLine("-------------------------------------------------------------------");
            }



            if (energyStatus == StatusEnergy.Low)
            {
                if (hasAShield)
                {
                    //DEBUG PRINT
                    Console.WriteLine("You have called the shield desactivation");

                    return Shield.DesactivateShield();
                }

                //DEBUG PRINT
                Console.WriteLine("You have called the energy mode");

                return EnergyMode.Execute();
            }

            if (energyStatus == StatusEnergy.Medium)
            {
                //Resets the scan levels if it was changed elsewhere
                if (MainBot.scanLevel != MainBot.baseScanLevel)
                {
                    MainBot.ChangeScanLevel(MainBot.baseScanLevel);
                }

                string temp = OppShoot.ScanOpportunityEnnemy();
                if (temp != "None")
                {
                    //DEBUG PRINT
                    Console.WriteLine("You have called the oppshoot");

                    return OppShoot.Execute(temp);
                }

                //DEBUG PRINT
                Console.WriteLine("You have called the energy mode");

                return EnergyMode.Execute();
            }

            if (energyStatus == StatusEnergy.High)
            {
                MainBot.hasAScan = false;
                if (!hasAShield)
                {
                    //DEBUG PRINT
                    Console.WriteLine("You have called the shield activation");

                    return Shield.ActivateShield();
                }

                if (MainBot.IsAnEnnemy())
                {
                    //DEBUG PRINT
                    Console.WriteLine("You have called the massmurder");

                    return MassMurder.Execute();
                }

                //DEBUG PRINT
                Console.WriteLine("You have called the energy mode");

                return EnergyMode.Execute();
            }

            //VeryHigh status => you scan at evry turn
            if (!hasAShield)
            {
                //DEBUG PRINT
                Console.WriteLine("You have called the shield activation");

                return Shield.ActivateShield();
            }

            if (MainBot.IsAnEnnemy())
            {
                //DEBUG PRINT
                Console.WriteLine("You have called the massmurder");

                return MassMurder.Execute();
            }

            //DEBUG PRINT
            Console.WriteLine("You have called the energy mode");

            MainBot.hasAScan = false;

            return EnergyMode.Execute();



        }
    }
}
