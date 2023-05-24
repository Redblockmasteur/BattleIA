#pragma warning disable CS8602, CS8604, CS8625, CS8603

using System;
using BattleIA;
using System.Collections.Generic; // pour utiliser List<T>
using AStarPathfinding;
using EnergyModeObj;
using EnergyWhoreV3;
using Main;

namespace OpportunityShoot
{
    public class OppShoot
    {

        public byte[] Execute(string direction)
        {
            if (direction == "North")
            {
                return BotHelper.ActionShoot(MoveDirection.North);
            }
            if (direction == "South")
            {
                return BotHelper.ActionShoot(MoveDirection.South);
            }
            if (direction == "East")
            {
                return BotHelper.ActionShoot(MoveDirection.West);
            }
            if (direction == "West")
            {
                return BotHelper.ActionShoot(MoveDirection.East);
            }
            else
            {
                return BotHelper.ActionNone();
            }
        }

        public string ScanOpportunityEnnemy()
        {
            for (int i = 0; i < MainBot.scanLevel; i++)
            {
                if (MainBot.memScan[MainBot.scanLevel, i] == 2)
                {
                    //We change the value of the cell to 0 to avoid shooting again
                    //because we don't rescan after an opportunity shoot
                    MainBot.memScan[MainBot.scanLevel, i] = 0;
                    return "West";
                }
                if (MainBot.memScan[i, MainBot.scanLevel] == 2)
                {
                    MainBot.memScan[i, MainBot.scanLevel] = 0;
                    return "North";
                }
            }
            for (int i = MainBot.scanLevel + 1; i < (MainBot.scanLevel * 2 + 1); i++)
            {
                if (MainBot.memScan[MainBot.scanLevel, i] == 2)
                {
                    MainBot.memScan[MainBot.scanLevel, i] = 0;
                    return "East";
                }
                if (MainBot.memScan[i, MainBot.scanLevel] == 2)
                {
                    MainBot.memScan[i, MainBot.scanLevel] = 0;
                    return "South";
                }
            }
            return "None";
        }

    }
}