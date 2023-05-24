#pragma warning disable CS8602, CS8604, CS8625, CS8603

using System;
using BattleIA;


namespace ShieldManagment
{
    public class Sheild
    {
        public byte shieldLevel = 50;
        public byte[] ActivateShield()
        {
            return BotHelper.ActionShield(shieldLevel); 
        }

        public byte[] DesactivateShield()
        {
            return BotHelper.ActionShield(0); 
        }


    }
}