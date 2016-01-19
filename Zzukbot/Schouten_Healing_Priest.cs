using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZzukBot.Engines.CustomClass;

namespace something
{
    public class Schouten_Healing_Priest : CustomClass
    { 
        public override byte DesignedForClass
        {
            get
            {
                return PlayerClass.Priest;
            }
        }
        public override string CustomClassName
        {
            get
            {
                return "Schouten_Healing_Priest";
            }
        }
        public override void PreFight()
        {
            //set the attack distance
            this.SetCombatDistance(25);
            
            if (this.Player.IsWandEquipped())
            {
                this.Player.StartWand();
                return;
            }
            return;
        }

        public override void Fight()
        {
            //freak out because the shit is hitting the fan
            if (this.Player.HealthPercent < 10 && this.Player.CanUse("Psychic Scream"))
            {
                this.Player.StopWand();
                this.Player.Cast("Psychic Scream");
            }

            //Requires Quickheal addon
            this.Player.DoString("QuickHeal()");



            if (this.Player.GetSpellRank("Inner Fire") != 0)
            {
                if (!Player.GotBuff("Inner Fire"))
                {
                    this.Player.StopWand();
                    this.Player.Cast("Inner Fire");
                    return;
                }
            }

            if (this.Player.IsWandEquipped())
            {
                this.Player.StartWand();
            }
            else
            {
                this.Player.Attack();
            }
            return;
        }

        public override void Rest()
        {
            if(this.Player.Debuffs.Count > 0)
            {
                if(this.Player.CanUse("Purify") && this.Player.GetSpellRank("Purify") != 0)
                    this.Player.Cast("Purify");
            }
            if (this.Player.HealthPercent <= 80 && this.Player.GetSpellRank("Flash of Light") != 0)
            {
                this.Player.Cast("Flash  Heal");
            }
            else if (this.Player.HealthPercent <= 50)
            {
                if (this.Player.GetSpellRank("Heal") != 0)
                {
                    this.Player.Cast("Heal");
                }
                else
                {
                    this.Player.Cast("Lesser Heal");
                }
            }
            if (this.Player.ManaPercent < 60)
            {
                this.Player.Drink();
            }
            Player.DoString("DoEmote('Sit')");
        }

        public override bool Buff()
        {
            //this should prevent more than one spell from trying to be called at once
            if (this.Player.IsCasting != "")
                return false;

            if (this.Player.GetSpellRank("Touch of Weakness") != 0)
            {
                if (!this.Player.GotBuff("Touch of Weakness"))
                {
                    this.Player.Cast("Touch of Weakness");
                    return false;
                }
            }
            if (this.Player.GetSpellRank("Inner Fire") != 0)
            {
                if (!this.Player.GotBuff("Inner Fire"))
                {
                    this.Player.Cast("Inner Fire");
                    return false;
                }
            }
            if (this.Player.GetSpellRank("Power Word: Fortitude") != 0)
            {
                if (!this.Player.GotBuff("Power Word: Fortitude"))
                {
                    this.Player.Cast("Power Word: Fortitude");
                    return false;
                }
            }
            if (this.Player.GetSpellRank("Divine Spirit") != 0)
            {
                if (!this.Player.GotBuff("Divine Spirit"))
                {
                    this.Player.Cast("Divine Spirit");
                    return false;
                }
            }
            //True means we are done buffing, or cannot buff
            return true;
        }
        //function library

            
        /// <summary>
        /// tests if the unit has a debuff
        /// </summary>
        /// <param name="unit">the unit to look for debuff</param>
        /// <param name="debuff">string name of the debuff</param>
        /// <returns>true if it has the debuff, false if it doesnt</returns>
        public bool hasDebuff(ZzukBot.Engines.CustomClass.Objects._Unit unit, String debuff)
        {
            foreach (string listDebuff in unit.Debuffs)
            {
                if (listDebuff == debuff)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// tests if the unit has the buff
        /// </summary>
        /// <param name="unit">unit to look for the buff on</param>
        /// <param name="buff">string value of the buff</param>
        /// <returns>true if the unit has the buff, false if it does not</returns>
        public bool hasBuff(ZzukBot.Engines.CustomClass.Objects._Unit unit, String buff)
        {
            foreach (string listBuff in unit.Buffs)
            {
                if (listBuff == buff)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// test to see if any number of attackers are about to die
        /// </summary>
        /// <returns>true if one of the attackers health is less than 10%, otherwise false</returns>
        public bool attackersAboutToDie()
        {
            foreach (ZzukBot.Engines.CustomClass.Objects._Unit unit in this.Attackers)
            {
                if (unit.HealthPercent < 10)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// checks each attacking mob to see if it has the specified debuff
        /// </summary>
        /// <param name="debuff">the name of the debuff to check for</param>
        /// <returns>the first unit without the debuff</returns>
        public ZzukBot.Engines.CustomClass.Objects._Unit mobWithoutDebuff(String debuff)
        {
            foreach(ZzukBot.Engines.CustomClass.Objects._Unit unit in this.Attackers)
            {
                if (!unit.GotDebuff(debuff))
                {
                    return unit;
                }
            }
            return null;
        }
        /// <summary>
        /// finds from all the attacking mobs the one with the lowest health percentage 
        /// </summary>
        /// <returns>Unit with the lowest health percentage</returns>
        public ZzukBot.Engines.CustomClass.Objects._Unit lowestHealthAttackingMob()
        {
            ZzukBot.Engines.CustomClass.Objects._Unit lowestHealthMob = this.Attackers[0];
            foreach (ZzukBot.Engines.CustomClass.Objects._Unit unit in this.Attackers)
            {
                if(unit.HealthPercent < lowestHealthMob.HealthPercent)
                {
                    lowestHealthMob = unit;
                }
            }
            return lowestHealthMob;
        }
    }
}
