using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZzukBot.Engines.CustomClass;

namespace something
{
    public class Schouten_Warrior : CustomClass
    {
        public override byte DesignedForClass
        {
            get
            {
                return PlayerClass.Warrior;
            }
        }
        public override string CustomClassName
        {
            get
            {
                return "Schouten_Warrior";
            }
        }

        public override void PreFight()
        {
            if (this.Player.GetSpellRank("Charge") != 0)
            {
                if (this.Player.CanUse("Charge"))
                {
                    this.Player.Cast("Charge");
                    return;
                }
            }
            if (this.Player.GetSpellRank("Intercept") != 0)
            {
                if (this.Player.CanUse("Intercept"))
                {
                    this.Player.Cast("Intercept");
                    return;
                }
            }
            this.Player.Attack();
            this.SetCombatDistance(3);
            return;
        }

        public override void Rest()
        {
            if (this.Player.HealthPercent < 60)
                this.Player.Eat();
            Player.DoString("DoEmote('Sit')");
        }

        public override void Fight()
        {
            this.Player.Attack();
            //handle multi-mob
            if (this.Attackers.Count >= 2)
            {
                //keep the clap up
                if (this.Player.GetSpellRank("Thunder Clap") != 0 && !this.Target.GotDebuff("Thunder Clap"))
                {
                    if (this.Player.CanUse("Thunder Clap"))
                    {
                        this.Player.Cast("Thunder Clap");
                        return;
                    }
                }
                //Get the damage down with Demoralizing Shout
                if (this.Player.GetSpellRank("Demoralizing Shout") != 0 && !this.Target.GotDebuff("Demoralizing Shout"))
                {
                    if (this.Player.CanUse("Demoralizing Shout"))
                    {
                        this.Player.Cast("Demoralizing Shout");
                        return;
                    }
                }
                //how about a little retaliation?
                if (this.Player.GetSpellRank("Retaliation") != 0)
                {
                    if (this.Player.CanUse("Retaliation"))
                    {
                        this.Player.Cast("Retaliation");
                        return;
                    }
                }
                //cleave them down if there is lots of rage
                if (this.Player.GetSpellRank("Cleave") != 0)
                {
                    if (this.Player.CanUse("Cleave"))
                    {
                        this.Player.Cast("Cleave");
                        return;
                    }
                }
            }

            //keep battle shout up
            if (this.Player.GetSpellRank("Battle Shout") != 0 && !this.Player.GotBuff("Battle Shout"))
            {
                if (this.Player.CanUse("Battle Shout"))
                {
                    this.Player.Cast("Battle Shout");
                    return;
                }
            }
            
            //keep rend up if target over 50%
            if (this.Player.GetSpellRank("Rend") != 0 && !this.Target.GotDebuff("Rend") && this.Target.HealthPercent > 50)
            {
                if (this.Player.CanUse("Rend"))
                {
                    this.Player.Cast("Rend");
                    return;
                }
            }

            //top of the list, always execute
            if (this.Player.GetSpellRank("Execute") != 0 && this.Target.HealthPercent <= 20)
            {
                if (this.Player.CanUse("Execute"))
                {
                    this.Player.Cast("Execute");
                    return;
                }
            }

            // i like overpower
            if(this.Player.GetSpellRank("Overpower") != 0)
            {
                if(this.Player.CanUse("Overpower"))
                {
                    this.Player.Cast("Overpower");
                    return;
                }
            }
            //interrupt casting
            /**
            if(this.Player.GetSpellRank("Pummel") != 0 && this.Target.IsCasting != "" || this.Target.IsChanneling != "")
            {
                if (this.Player.CanUse("Pummel"))
                {
                    this.Player.Cast("Pummel");
                }
            }
            **/
            /*
            if (this.Player.GetSpellRank("Berserker Rage") != 0)
            {
                if (this.Player.CanUse("Berserker Rage"))
                {
                    this.Player.Cast("Berserker Rage");
                }
            }
            
            //bloodthirst for grind-a-lot
            if (this.Player.GetSpellRank("Bloodthirst") != 0)
            {
                if (this.Player.CanUse("Bloodthirst"))
                {
                    this.Player.Cast("Bloodthirst");
                    return;
                }
            }
            */

            //slam is the jam
            if (this.Player.GetSpellRank("Slam") != 0)
            {
                if (this.Player.CanUse("Slam"))
                {
                    this.Player.Cast("Slam");
                    return;
                }
            }

            //mortal strike is the jam
            if (this.Player.GetSpellRank("Mortal Strike") != 0)
            {
                if (this.Player.CanUse("Mortal Strike"))
                {
                    this.Player.Cast("Mortal Strike");
                    return;
                }
            }

            if (this.Player.GetSpellRank("Cleave") != 0 && this.Player.Rage >= 80)
            {
                if (this.Player.CanUse("Cleave"))
                {
                    this.Player.Cast("Cleave");
                    return;
                }
            }

            //heroic strike is low level horse shit spell so set the rage a little higher
            if (this.Player.GetSpellRank("Heroic Strike") != 0)
            {
                if (this.Player.CanUse("Heroic Strike"))
                {
                    this.Player.Cast("Heroic Strike");
                    return;
                }
            }
            //fuck it not doing anything else!
            if (this.Player.GetSpellRank("Blood Rage") != 0)
            {
                if (this.Player.CanUse("Blood Rage"))
                {
                    this.Player.Cast("Blood Rage");
                    return;
                }
            }
            this.Player.Attack();
            return;
        }

        public override bool Buff()
        {
            return true;
        }
    }
}
