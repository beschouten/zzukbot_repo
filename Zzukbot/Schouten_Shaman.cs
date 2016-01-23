using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZzukBot.Engines.CustomClass;

namespace something
{
    public class Schouten_Shaman : CustomClass
    {
        public override byte DesignedForClass
        {
            get
            {
                return PlayerClass.Shaman;
            }
        }
        public override string CustomClassName
        {
            get
            {
                return "Schouten_Shaman";
            }
        }
        //commented for easy modification from Jake....  enjoy

        //as I understnd pre-fight runs 10 time a second while the mob is approaching
        public override void PreFight()
        {
            this.SetCombatDistance(29);
            //get out of ghost wolf
            if (this.Player.IsShapeShifted)
            {
                this.Player.CancelShapeShift();
            }

            //pulling starts here
            //frost shock tries first for close range
            if (this.Target.DistanceToPlayer <= 15 && this.Player.GetSpellRank("Frost Shock") != 0)
            {
                if(this.Player.CanUse("Frost Shock"))
                {
                    this.Player.Cast("Frost Shock");
                }
            }

            //then flame shock for close range
            else if (this.Target.DistanceToPlayer <= 15 && this.Player.GetSpellRank("Flame Shock") != 0)
            {
                if (this.Player.CanUse("Flame Shock"))
                {
                    this.Player.Cast("Flame Shock");
                }
                
            }
            //if the close range is not availible then pull from range.  I think this is preferred but we should checck to make sure that the mob is not closing in					
            else if (this.Target.DistanceToPlayer <= 30 && this.Player.GetSpellRank("Lightning Bolt") != 0)
            {
                if (this.Player.CanUse("Lightning Bolt"))
                {
                    this.Player.Cast("Lightning Bolt");
                }
            }
            else
            {
                this.SetCombatDistance(3);
            }
        }

        //I think buff runs after combat and before moving.  since i didnt read the API i don't know what the return value does, but it seems to return false with a successful buff
        public override bool Buff()
        {
            //make sure lightning shield is on
            if (this.Player.GetSpellRank("Lightning Shield") != 0)
            {
                if (!this.Player.GotBuff("Lightning Shield"))
                {
                    this.Player.Cast("Lightning Shield");
                    return false;
                }
            }

            //make sure that a weapon buff is applied, windfury first
            if (this.Player.GetSpellRank("Windfury Weapon") != 0)
            {
                if (!this.Player.IsMainhandEnchanted())
                {
                    this.Player.Cast("Windfury Weapon");
                    return false;
                }
            }
            //else rockbiter
            else
            {
                if (!this.Player.IsMainhandEnchanted())
                {
                    this.Player.Cast("Rockbiter Weapon");
                    return false;
                }
            }
            //fuck it call scroll buffs in a shitty loop.  This is a bad programming construct so dont judge...
            while (!scrollBuff())
            {
                //keep calling until true...
            }

            //wolf time to travel between mobs
            /*
            if (!this.Player.IsResting() && this.Attackers.Count == 0 && this.Player.GetSpellRank("Ghost Wolf") != 0)
            {
                if (!hasBuff(this.Player, "Ghost Wolf"))
                {
                    this.Player.Cast("Ghost Wolf");
                    return false;
                }
            }
            */
            return true;
        }

        public override void Rest()
        {
            //get out of ghost wolf if issue
            if (this.Player.IsShapeShifted)
            {
                this.Player.CancelShapeShift();
            }
            //heal if needed
            if (this.Player.HealthPercent <= 75 && this.Player.CanUse("Healing Wave") && !this.Player.GotBuff("Drink"))
            {
                //Requires Quickheal addon
                this.Player.DoString("QuickHeal()");
            }
            //mana totem!
            if (this.Player.ManaPercent <= 35)
            {
                if (this.Player.GetSpellRank("Mana Tide Totem") != 0)
                {
                    float rangeToTotem = this.Player.IsTotemSpawned("Mana Tide Totem");
                    if ((rangeToTotem == -1 || rangeToTotem > 25) && this.Player.CanUse("Mana Tide Totem"))
                    {
                        this.Player.Cast("Mana Tide Totem");
                        return;
                    }
                }
            }
            if (this.Player.ManaPercent <= 65)
            {
                this.Player.Drink();
            }
            Player.DoString("DoEmote('Sit')");
        }

        public override void Fight()
        {
            //get out of ghost wolf if issue
            if (this.Player.IsShapeShifted)
            {
                this.Player.CancelShapeShift();
                return;
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

            //counterspell					
            if (this.Target.IsCasting != "" || this.Target.IsChanneling != "" && this.Player.ManaPercent >= 10)
            {
                if(this.Player.GetSpellRank("Earth Shock") != 0)
                {
                    if (Player.CanUse("Earth Shock"))
                    {
                        this.Player.Cast("Earth Shock");
                        return;
                    }
                }
            }
            
            //mana totem always!
            if (this.Player.GetSpellRank("Mana Tide Totem") != 0)
            {
                float rangeToTotem = this.Player.IsTotemSpawned("Mana Tide Totem");
                if (rangeToTotem == -1 && this.Player.CanUse("Mana Tide Totem"))
                {
                    this.Player.Cast("Mana Tide Totem");
                    return;
                }
            }

            //drop the searing totem
            if (this.Player.GetSpellRank("Searing Totem") != 0 && this.Player.ManaPercent >= 40 && this.Target.HealthPercent >= 70)
            {
                float rangeToTotem = this.Player.IsTotemSpawned("Searing Totem");
                if ((rangeToTotem == -1 || rangeToTotem > 25) && this.Player.CanUse("Searing Totem"))
                {
                    this.Player.Cast("Searing Totem");
                    return;
                }
            }

            //always cast stormstrike
            if (this.Player.GetSpellRank("Stormstrike") != 0 && this.Player.ManaPercent >= 20)
            {
                if (Player.CanUse("Stormstrike"))
                {
                    this.Player.Cast("Stormstrike");
                    return;
                }
            }

            //lightning shield = DPS
            if (this.Player.GetSpellRank("Lightning Shield") != 0 && this.Player.ManaPercent >= 40)
            {
                if (!this.Player.GotBuff("Lightning Shield"))
                {
                    this.Player.Cast("Lightning Shield");
                    return;
                }
            }
            //shocks
            if (this.Target.HealthPercent >= 40 && this.Player.ManaPercent >= 30 && this.Player.GetSpellRank("Flame Shock") != 0)
            {
                if (this.Player.CanUse("Flame Shock") && !this.Target.GotDebuff("Flame Shock"))
                {
                    this.Player.Cast("Flame Shock");
                    return;
                }
            }
            else if (this.Player.GetSpellRank("Earth Shock") != 0 && this.Player.ManaPercent >= 30)
            {
                if (this.Player.CanUse("Earth Shock"))
                {
                    this.Player.Cast("Earth Shock");
                    return;
                }
            }
            //standard attacks
            if (this.Target.DistanceToPlayer < 5)
            {
                this.Player.Attack();
                return;
            }
            else if(this.Player.ManaPercent >= 20 && this.Player.GetSpellRank("Lightning Bolt") != 0)
            {
                if(this.Player.CanUse("Lightning Bolt"))
                {
                    this.Player.Cast("Lightning Bolt");
                    return;
                }
            }

            //all else fails close the distance
            this.SetCombatDistance(3);
            return;
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
        /// finds from all the attacking mobs the one with the lowest health percentage 
        /// </summary>
        /// <returns>Unit with the lowest health percentage</returns>
        public ZzukBot.Engines.CustomClass.Objects._Unit lowestHealthAttackingMob()
        {
            ZzukBot.Engines.CustomClass.Objects._Unit lowestHealthMob = this.Attackers[0];
            foreach (ZzukBot.Engines.CustomClass.Objects._Unit unit in this.Attackers)
            {
                if (unit.HealthPercent < lowestHealthMob.HealthPercent)
                {
                    lowestHealthMob = unit;
                }
            }
            return lowestHealthMob;
        }
    }
}




