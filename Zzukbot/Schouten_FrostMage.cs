using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZzukBot.Engines.CustomClass;

namespace something
{
    public class Schouten_FrostMage : CustomClass
    {
        private string[] WaterName = {"", "Conjured Water", "Conjured Fresh Water",
                                         "Conjured Purified Water", "Conjured Spring Water",
                                         "Conjured Mineral Water", "Conjured Sparkling Water",
                                         "Conjured Crystal Water"};
        private string[] FoodName = {"", "Conjured Muffin", "Conjured Bread",
                                         "Conjured Rye", "Conjured Pumpernickel",
                                         "Conjured Sourdough", "Conjured Sweet Roll",
                                         "Conjured Cinnamon Roll"};
        public override byte DesignedForClass
        {
            get
            {
                return PlayerClass.Mage;
            }
        }

        public override string CustomClassName
        {
            get
            {
                return "Schouten_FrostMage";
            }
        }

        public override void PreFight()
        {
            //attack and set the distance
            this.Player.Attack();
            this.SetCombatDistance(28);

            //set ice barrier
            if (this.Player.GetSpellRank("Ice Barrier") != 0)
            {
                if (!this.Player.GotBuff("Ice Barrier"))
                {
                    if (this.Player.CanUse("Ice Barrier"))
                    {
                        this.Player.Cast("Ice Barrier");
                        return;
                    }
                }
            }
            //lead in with the frostbolt at distance
            if (this.Target.DistanceToPlayer >= 25 && this.Player.CanUse("Frostbolt"))
            {
                this.Player.Cast("Frostbolt");
                return;
            }
            //follow up with arcane missles mid range (doesnt channel right)
            /**
            if (this.Target.DistanceToPlayer >= 10 && this.Player.CanUse("Arcane Missiles"))
            {
                this.Player.CastWait("Arcane Missiles", 2250);
                return;
            }
            */
            //then cast another frostbolt close range
            if (this.Player.CanUse("Frostbolt"))
            {
                this.Player.Cast("Frostbolt");
                return;
            }
            //wand
            if (this.Player.IsWandEquipped())
            {
                this.Player.StartWand();
                return;
            }
        }


        public override void Fight()
        {
            //backup for frosted targets
            if (this.Target.GotDebuff("Frost Nova") && this.Target.DistanceToPlayer < 6)
            {
                this.Player.ForceBackup(6);
            }
            else
            {
                this.Player.StopForceBackup();
            }

            //make sure to kill the low health one
            ZzukBot.Engines.CustomClass.Objects._Unit lowHealthMob = lowestHealthAttackingMob();
            if (this.Player.TargetGuid != lowHealthMob.Guid)
            {
                this.Player.SetTargetTo(lowHealthMob);
            }
            //handle multimob
            if (this.Attackers.Count >= 2)
            {
                //if one is about to die kill it!
                if (attackersAboutToDie() || this.Attackers.Count >= 5 && this.Target.DistanceToPlayer < 10 )
                {
                    if(this.Player.GetSpellRank("Arcane Explosion") != 0)
                    {
                        if(this.Player.CanUse("Arcane Explosion"))
                        {
                            this.Player.Cast("Arcane Explosion");
                            return;
                        }
                    }
                }
                //cast frost nova
                if (this.Player.ManaPercent >= 20 && this.Player.GetSpellRank("Frost Nova") != 0 && this.Target.DistanceToPlayer <= 8)
                {
                    if (this.Player.CanUse("Frost Nova"))
                    {
                        this.Player.Cast("Frost Nova");
                        return;
                    }
                }
                //mana shield
                if (this.Player.ManaPercent >= 65 && !this.Player.GotBuff("Mana Shield"))
                {
                    if (this.Player.GetSpellRank("Mana Shield") != 0)
                    {
                        if (this.Player.CanUse("Mana Shield"))
                        {
                            this.Player.Cast("Mana Shield");
                            return;
                        }
                    }
                }
                //cast blizzard
                if (this.Player.ManaPercent >= 20 && this.Player.GetSpellRank("Blizzard") != 0)
                {
                    if (this.Player.CanUse("Blizzard"))
                    {
                        this.Player.CastAoe("Blizzard", this.Target);
                        return;
                    }
                }
            }
            //OOM wand that bitch
            if (this.Player.ManaPercent < 10)
            {
                if (this.Player.IsWandEquipped())
                {
                    this.Player.StartWand();
                    return;
                }
                
            }
            //mana shield
            if (this.Player.ManaPercent >= 75 && !this.Player.GotBuff("Mana Shield"))
            {
                if (this.Player.GetSpellRank("Mana Shield") != 0)
                {
                    if (this.Player.CanUse("Mana Shield"))
                    {
                        this.Player.Cast("Mana Shield");
                        return;
                    }
                }
            }
            //ice barrier
            if (this.Player.ManaPercent >= 35 && this.Player.GetSpellRank("Ice Barrier") != 0 && (this.Attackers.Count > 1 || this.Target.HealthPercent > 10))
            {
                if (!this.Player.GotBuff("Ice Barrier"))
                {
                    if (this.Player.CanUse("Ice Barrier"))
                    {
                        this.Player.Cast("Ice Barrier");
                        return;
                    }
                }
            }
            //counterspell
            if (this.Player.ManaPercent >= 5 && this.Target.IsCasting != "" || this.Target.IsChanneling != "")
            {
                if (this.Player.GetSpellRank("Counterspell") != 0)
                {
                    if (this.Player.CanUse("Counterspell"))
                    {
                        this.Player.Cast("Counterspell");
                        return;
                    }
                }
            }
            
            //wand almost dead mobs
            if (this.Player.IsWandEquipped() && this.Target.HealthPercent < 5)
            {
                this.Player.StartWand();
                return;
            }
            //cast frost nova
            if (this.Player.ManaPercent >= 10 && this.Player.GetSpellRank("Frost Nova") != 0 && this.Target.DistanceToPlayer <= 10 && this.Target.HealthPercent >= 20)
            {
                if (this.Player.CanUse("Frost Nova"))
                {
                    this.Player.Cast("Frost Nova");
                    return;
                }
            }

            //always fireblast
            if (this.Player.ManaPercent >= 10 && this.Player.GetSpellRank("Fire Blast") != 0 && this.Target.DistanceToPlayer <= 20)
            {
                if (this.Player.CanUse("Fire Blast"))
                {
                    this.Player.Cast("Fire Blast");
                    return;
                }
            }
            //frostbolt
            if(this.Player.ManaPercent >= 10 && this.Player.GetSpellRank("Frostbolt") != 0)
            {
                if (this.Player.CanUse("Frostbolt"))
                {
                    this.Player.Cast("Frostbolt");
                    return;
                }
            }

            //wand
            if (this.Player.IsWandEquipped())
            {
                this.Player.StartWand();
                return;
            }
        }

        public override void Rest()
        {
            //evocation is quicker
            if (this.Player.GetSpellRank("Evocation") != 0)
            {
                if (this.Player.CanUse("Evocation") && Player.ManaPercent <= 20)
                {
                    this.Player.CastWait("Evocation", 1000);
                }
            }
            //otherwise drink
            else if(this.Player.ManaPercent <= 90)
            {
                this.Player.Drink(WaterName[this.Player.GetSpellRank("Conjure Water")]);
            }
            //if lower health eat
            if (this.Player.HealthPercent <= 90)
            {
                this.Player.Eat(FoodName[this.Player.GetSpellRank("Conjure Food")]);
            }
        }

        public override bool Buff()
        {
            if (this.Player.IsCasting != "")
                return false;

            if (this.Player.GetSpellRank("Conjure Water") != 0)
            {
                if (this.Player.ItemCount(WaterName[this.Player.GetSpellRank("Conjure Water")]) <= 5)
                {
                    this.Player.Cast("Conjure Water");
                    return false;
                }
            }

            if (this.Player.GetSpellRank("Conjure Food") != 0)
            {
                if (this.Player.ItemCount(FoodName[this.Player.GetSpellRank("Conjure Food")]) <= 5)
                {
                    this.Player.Cast("Conjure Food");
                    return false;
                }
            }

            //ice armor or frost armor
            if (this.Player.GetSpellRank("Ice Armor") != 0)
            {
                if (!this.Player.GotBuff("Ice Armor"))
                {
                    this.Player.Cast("Ice Armor");
                    return false;
                }
            }
            else if (this.Player.GetSpellRank("Frost Armor") != 0)
            {
                if (!this.Player.GotBuff("Frost Armor"))
                {
                    this.Player.Cast("Frost Armor");
                    return false;
                }
            }
            //arcane intellect
            if (this.Player.GetSpellRank("Arcane Intellect") != 0)
            {
                if (!this.Player.GotBuff("Arcane Intellect"))
                {
                    this.Player.Cast("Arcane Intellect");
                    return false;
                }
            }
            //dampen magic
            if (this.Player.GetSpellRank("Dampen Magic") != 0)
            {
                if (!this.Player.GotBuff("Dampen Magic"))
                {
                    this.Player.Cast("Dampen Magic");
                    return false;
                }
            }
            //scroll buffs
            if (scrollBuff())
            {
                return true;
            }
            else
            {
                return false;
            }
            
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
                if(listDebuff == debuff)
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
