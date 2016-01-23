using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZzukBot.Engines.CustomClass;

namespace something
{
    public class Schouten_Priest : CustomClass
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
                return "Schouten_Priest";
            }
        }
        public override void PreFight()
        {
            //set the attack distance
            this.SetCombatDistance(29);

            //pull with mind blast
            if (this.Player.GetSpellRank("Mind Blast") != 0)
            {
                if(this.Player.CanUse("Mind Blast"))
                {
                    this.Player.Cast("Mind Blast");
                    return;
                }
            }
            //get a swp on the mob
            if (this.Player.GetSpellRank("Shadow Word: Pain") != 0 && !Target.GotDebuff("Shadow Word: Pain"))
            {
                this.Player.Cast("Shadow Word: Pain");
                return;
            }

            //shields up!
            if (this.Player.GetSpellRank("Power Word: Shield") != 0)
            {
                if (!this.Player.GotBuff("Power Word: Shield") && !this.Player.GotDebuff("Weakened Soul") && this.Player.ManaPercent > 50)
                {
                    this.Player.Cast("Power Word: Shield");
                    return;
                }
            }

            if (this.Player.IsWandEquipped())
            {
                this.Player.StartWand();
                return;
            }
            else if(this.Player.GetSpellRank("Smite") != 0)
            {
                if (this.Player.CanUse("Smite"))
                {
                    this.Player.Cast("Smite");
                    return;
                }
            }
            this.Player.Attack();
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

            //healing is priority
            if (this.Player.HealthPercent <= 40)
            {
                //Requires Quickheal addon
                this.Player.DoString("QuickHeal()");
            }

            //make sure to kill the low health one
            ZzukBot.Engines.CustomClass.Objects._Unit lowHealthMob = lowestHealthAttackingMob();
            if (this.Player.TargetGuid != lowHealthMob.Guid)
            {
                this.Player.SetTargetTo(lowHealthMob);
            }

            //multi mob SWP all the time
            ZzukBot.Engines.CustomClass.Objects._Unit mobwithoutSWP = mobWithoutDebuff("Shadow Word: Pain");
            if (mobwithoutSWP != null && this.Player.GetSpellRank("Shadow Word: Pain") != 0)
            {
                if (!mobwithoutSWP.GotDebuff("Shadow Word: Pain") && mobwithoutSWP.HealthPercent > 10)
                {
                    this.Player.StopWand();
                    this.Player.SetTargetTo(mobwithoutSWP);
                    this.Player.Cast("Shadow Word: Pain");
                    return;
                }

            }

            if (this.Player.GetSpellRank("Power Word: Shield") != 0)
            {
                if (!this.Player.GotBuff("Power Word: Shield") && !this.Player.GotDebuff("Weakened Soul") && this.Target.HealthPercent > 5)
                {
                    this.Player.StopWand();
                    this.Player.Cast("Power Word: Shield");
                }
            }

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
            if (this.Player.HealthPercent <= 50)
            {
                //Requires Quickheal addon
                this.Player.DoString("QuickHeal()");
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
        /// buffs with any scrolls that you find (besides protection??)
        /// </summary>
        /// <returns>false until all scrolls are used, true if all scrolls are used</returns>           
        public bool scrollBuff()
        {
            if (this.Player.CanUse("Scroll of Strength I") && this.Player.ItemCount("Scroll of Strength I") > 0 && (!this.Player.GotBuff("Scroll of Strength I") || !this.Player.GotBuff("Scroll of Strength II") || !this.Player.GotBuff("Scroll of Strength III") || !this.Player.GotBuff("Scroll of Strength IV")))
            {
                this.Player.UseItem("Scroll of Strength I");
                return false;
            }
            if (this.Player.CanUse("Scroll of Strength II") && this.Player.ItemCount("Scroll of Strength II") > 0 && (!this.Player.GotBuff("Scroll of Strength I") || !this.Player.GotBuff("Scroll of Strength II") || !this.Player.GotBuff("Scroll of Strength III") || !this.Player.GotBuff("Scroll of Strength IV")))
            {
                this.Player.UseItem("Scroll of Strength II");
                return false;
            }
            if (this.Player.CanUse("Scroll of Strength III") && this.Player.ItemCount("Scroll of Strength III") > 0 && (!this.Player.GotBuff("Scroll of Strength I") || !this.Player.GotBuff("Scroll of Strength II") || !this.Player.GotBuff("Scroll of Strength III") || !this.Player.GotBuff("Scroll of Strength IV")))
            {
                this.Player.UseItem("Scroll of Strength III");
                return false;
            }
            if (this.Player.CanUse("Scroll of Strength IV") && this.Player.ItemCount("Scroll of Strength IV") > 0 && (!this.Player.GotBuff("Scroll of Strength I") || !this.Player.GotBuff("Scroll of Strength II") || !this.Player.GotBuff("Scroll of Strength III") || !this.Player.GotBuff("Scroll of Strength IV")))
            {
                this.Player.UseItem("Scroll of Strength IV");
                return false;
            }
            if (this.Player.CanUse("Scroll of Agility I") && this.Player.ItemCount("Scroll of Agility I") > 0 && (!this.Player.GotBuff("Scroll of Agility I") || !this.Player.GotBuff("Scroll of Agility II") || !this.Player.GotBuff("Scroll of Agility III") || !this.Player.GotBuff("Scroll of Agility IV")))
            {
                this.Player.UseItem("Scroll of Agility I");
                return false;
            }
            if (this.Player.CanUse("Scroll of Agility II") && this.Player.ItemCount("Scroll of Agility II") > 0 && (!this.Player.GotBuff("Scroll of Agility I") || !this.Player.GotBuff("Scroll of Agility II") || !this.Player.GotBuff("Scroll of Agility III") || !this.Player.GotBuff("Scroll of Agility IV")))
            {
                this.Player.UseItem("Scroll of Agility II");
                return false;
            }
            if (this.Player.CanUse("Scroll of Agility III") && this.Player.ItemCount("Scroll of Agility III") > 0 && (!this.Player.GotBuff("Scroll of Agility I") || !this.Player.GotBuff("Scroll of Agility II") || !this.Player.GotBuff("Scroll of Agility III") || !this.Player.GotBuff("Scroll of Agility IV")))
            {
                this.Player.UseItem("Scroll of Agility III");
                return false;
            }
            if (this.Player.CanUse("Scroll of Agility IV") && this.Player.ItemCount("Scroll of Agility IV") > 0 && (!this.Player.GotBuff("Scroll of Agility I") || !this.Player.GotBuff("Scroll of Agility II") || !this.Player.GotBuff("Scroll of Agility III") || !this.Player.GotBuff("Scroll of Agility IV")))
            {
                this.Player.UseItem("Scroll of Agility IV");
                return false;
            }
            if (this.Player.CanUse("Scroll of Stamina I") && this.Player.ItemCount("Scroll of Stamina I") > 0 && (!this.Player.GotBuff("Scroll of Stamina I") || !this.Player.GotBuff("Scroll of Stamina II") || !this.Player.GotBuff("Scroll of Stamina III") || !this.Player.GotBuff("Scroll of Stamina IV")))
            {
                this.Player.UseItem("Scroll of Stamina I");
                return false;
            }
            if (this.Player.CanUse("Scroll of Stamina II") && this.Player.ItemCount("Scroll of Stamina II") > 0 && (!this.Player.GotBuff("Scroll of Stamina I") || !this.Player.GotBuff("Scroll of Stamina II") || !this.Player.GotBuff("Scroll of Stamina III") || !this.Player.GotBuff("Scroll of Stamina IV")))
            {
                this.Player.UseItem("Scroll of Stamina II");
                return false;
            }
            if (this.Player.CanUse("Scroll of Stamina III") && this.Player.ItemCount("Scroll of Stamina III") > 0 && (!this.Player.GotBuff("Scroll of Stamina I") || !this.Player.GotBuff("Scroll of Stamina II") || !this.Player.GotBuff("Scroll of Stamina III") || !this.Player.GotBuff("Scroll of Stamina IV")))
            {
                this.Player.UseItem("Scroll of Stamina III");
                return false;
            }
            if (this.Player.CanUse("Scroll of Stamina IV") && this.Player.ItemCount("Scroll of Stamina IV") > 0 && (!this.Player.GotBuff("Scroll of Stamina I") || !this.Player.GotBuff("Scroll of Stamina II") || !this.Player.GotBuff("Scroll of Stamina III") || !this.Player.GotBuff("Scroll of Stamina IV")))
            {
                this.Player.UseItem("Scroll of Stamina IV");
                return false;
            }
            if (this.Player.CanUse("Scroll of Spirit I") && this.Player.ItemCount("Scroll of Spirit I") > 0 && (!this.Player.GotBuff("Scroll of Spirit I") || !this.Player.GotBuff("Scroll of Spirit II") || !this.Player.GotBuff("Scroll of Spirit III") || !this.Player.GotBuff("Scroll of Spirit IV")))
            {
                this.Player.UseItem("Scroll of Spirit I");
                return false;
            }
            if (this.Player.CanUse("Scroll of Spirit II") && this.Player.ItemCount("Scroll of Spirit II") > 0 && (!this.Player.GotBuff("Scroll of Spirit I") || !this.Player.GotBuff("Scroll of Spirit II") || !this.Player.GotBuff("Scroll of Spirit III") || !this.Player.GotBuff("Scroll of Spirit IV")))
            {
                this.Player.UseItem("Scroll of Spirit II");
                return false;
            }
            if (this.Player.CanUse("Scroll of Spirit III") && this.Player.ItemCount("Scroll of Spirit III") > 0 && (!this.Player.GotBuff("Scroll of Spirit I") || !this.Player.GotBuff("Scroll of Spirit II") || !this.Player.GotBuff("Scroll of Spirit III") || !this.Player.GotBuff("Scroll of Spirit IV")))
            {
                this.Player.UseItem("Scroll of Spirit III");
                return false;
            }
            if (this.Player.CanUse("Scroll of Spirit IV") && this.Player.ItemCount("Scroll of Spirit IV") > 0 && (!this.Player.GotBuff("Scroll of Spirit I") || !this.Player.GotBuff("Scroll of Spirit II") || !this.Player.GotBuff("Scroll of Spirit III") || !this.Player.GotBuff("Scroll of Spirit IV")))
            {
                this.Player.UseItem("Scroll of Spirit IV");
                return false;
            }
            if (this.Player.CanUse("Scroll of Intellect I") && this.Player.ItemCount("Scroll of Intellect I") > 0 && (!this.Player.GotBuff("Scroll of Intellect I") || !this.Player.GotBuff("Scroll of Intellect II") || !this.Player.GotBuff("Scroll of Intellect III") || !this.Player.GotBuff("Scroll of Intellect IV")))
            {
                this.Player.UseItem("Scroll of Intellect I");
                return false;
            }
            if (this.Player.CanUse("Scroll of Intellect II") && this.Player.ItemCount("Scroll of Intellect II") > 0 && (!this.Player.GotBuff("Scroll of Intellect I") || !this.Player.GotBuff("Scroll of Intellect II") || !this.Player.GotBuff("Scroll of Intellect III") || !this.Player.GotBuff("Scroll of Intellect IV")))
            {
                this.Player.UseItem("Scroll of Intellect II");
                return false;
            }
            if (this.Player.CanUse("Scroll of Intellect III") && this.Player.ItemCount("Scroll of Intellect III") > 0 && (!this.Player.GotBuff("Scroll of Intellect I") || !this.Player.GotBuff("Scroll of Intellect II") || !this.Player.GotBuff("Scroll of Intellect III") || !this.Player.GotBuff("Scroll of Intellect IV")))
            {
                this.Player.UseItem("Scroll of Intellect III");
                return false;
            }
            if (this.Player.CanUse("Scroll of Intellect IV") && this.Player.ItemCount("Scroll of Intellect IV") > 0 && (!this.Player.GotBuff("Scroll of Intellect I") || !this.Player.GotBuff("Scroll of Intellect II") || !this.Player.GotBuff("Scroll of Intellect III") || !this.Player.GotBuff("Scroll of Intellect IV")))
            {
                this.Player.UseItem("Scroll of Intellect IV");
                return false;
            }
            return true;
        }
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
