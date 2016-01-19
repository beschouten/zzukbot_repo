using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZzukBot.Engines.CustomClass;

namespace something
{
    public class Schouten_Rogue : CustomClass
    {

        private int[][] EviscerateDamage = new int[][]
            {
                //Element at 0 is estimated white swing dmg
                new int[] {0,0,0,0,0},
                new int[] {10, 10,15,20,25,30},
                new int[] {20, 22,33,44,55,66},
                new int[] {30, 39,58,77,96,115},
                new int[] {50, 61,92,123,154,185},
                new int[] {60, 60,135,180,225,270},
                new int[] {80, 143,220,297,374,451},
                new int[] {100, 212,322,432,542,652},
                new int[] {150, 295,446,597,748,899},
                new int[] {200, 332,502,672,842,1012},
            };

        /*Additional stuff for Rogues - As a general rule avoid LUA as much as possible for more responsive combat*/
        private bool ShouldWeEviscerate()
        {
            int rank = this.Player.GetSpellRank("Eviscerate");
            int damage = 0;
            int comboPoints = this.Player.ComboPoints;
            if (rank >= 1 && comboPoints >= 1)
            {               //Estimated combo point dmg + Estimated white swing dmg
                damage = (EviscerateDamage[rank][comboPoints] + EviscerateDamage[rank][0]);
                if (this.Target.Health <= damage)
                {
                    return true;
                }
            }
            return false;
        }

        //Execute LUA to get buff duration - NOT Recomended
        private double GetSliceAndDiceDuration()
        {
            //Register the function - should be re-done more effeciently
            try
            {
                this.Player.DoString("function getBuffDuration(name) GetSpellForBot = name " +
                "timeleft = -1 for i=0,31 do local id,cancel = GetPlayerBuff(i,'HELPFUL|HARMFUL|PASSIVE') if(name == GetPlayerBuffTexture(id)) then " +
                "timeleft = GetPlayerBuffTimeLeft(id) " +/*DEFAULT_CHAT_FRAME:AddMessage(timeleft) + */" return timeleft end end return timeleft end");
            }
            catch
            {
                return -3;
            }

            try
            {
                string tex = @"Interface\\Icons\\Ability_Rogue_SliceDice";
                this.Player.DoString("points = getBuffDuration('" + tex + "')");
                return Convert.ToDouble(this.Player.GetText("points"));
            }
            catch
            {
                return -3;
            }
        }

        public override byte DesignedForClass
        {
            get
            {
                return PlayerClass.Rogue;
            }
        }
        public override string CustomClassName
        {
            get
            {
                return "Schouten_Rogue";
            }
        }

        public override void PreFight()
        {
            this.SetCombatDistance(3);
            if (this.Target.DistanceToPlayer >= 20 && this.Player.GetSpellRank("Sprint") != 0 && this.Player.CanUse("Sprint"))
            {
                this.Player.Cast("Sprint");
                return;
            }
            if(this.Player.GetSpellRank("Cheap Shot") != 0)
            {
                if (!this.Player.GotBuff("Stealth") && this.Player.CanUse("Stealth") && this.Target.DistanceToPlayer < 30)
                {
                    this.Player.Cast("Stealth");
                    return;
                }
            }
            if (this.Player.GotBuff("Stealth") && this.Player.GetSpellRank("Cheap Shot") != 0)
            {
                if(this.Target.DistanceToPlayer < 8)
                {
                    this.Player.Cast("Cheap Shot");
                    return;          
                }
            }
            else if(this.Target.DistanceToPlayer < 8)
            {
                this.Player.Attack();
                return;
            }
        }

        public override void Fight()
        {
            int Energy = this.Player.Energy;
            int ComboPoint = this.Player.ComboPoints;

            this.Player.Attack();

            /*If we are under attack from multiple mobs*/
            if (this.Attackers.Count >= 2)
            {
                if (this.Player.GetSpellRank("Adrenaline Rush") != 0)
                {
                    if (this.Player.CanUse("Adrenaline Rush"))
                    {
                        this.Player.Cast("Adrenaline Rush");
                        return;
                    }
                }

                if (this.Player.GetSpellRank("Blood Fury") != 0)
                {
                    if (this.Player.CanUse("Blood Fury"))
                    {
                        this.Player.Cast("Blood Fury");
                        return;
                    }
                }

                if (this.Player.GetSpellRank("Blade Flurry") != 0 && Energy >= 25)
                {
                    if (this.Player.CanUse("Blade Flurry"))
                    {
                        this.Player.Cast("Blade Flurry");
                        return;
                    }
                }

                if (this.Player.GetSpellRank("Evasion") != 0)
                {
                    if (this.Player.CanUse("Evasion"))
                    {
                        this.Player.Cast("Evasion");
                        return;
                    }
                }
            }

            /*Rotation Below*/

            //We can't do anything!
            if (Energy <= 20)
            {
                return;
            }

            //Kick Logic
            if (Energy >= 25 && (this.Target.IsCasting != "" || this.Target.IsChanneling != ""))
            {
                if (this.Player.GetSpellRank("Kick") != 0)
                {
                    this.Player.Cast("Kick");
                }
            }
            //get em!
            if (Energy >= 35)
            {
                if (this.Player.GetSpellRank("Eviscerate") != 0)
                {
                    if (this.ShouldWeEviscerate() || ComboPoint == 5)
                    {
                        this.Player.Cast("Eviscerate");
                        return;
                    }
                }
            }

            if (Energy >= 25)
            {
                //keep the slice and dice up
                if (this.Player.GetSpellRank("Slice and Dice") != 0)
                {
                    //If we don't have slice and dice up
                    //or if slice and dice has less than 2.0 secs left
                    //BUT! If next eviscerate will kill the target wait for energy instead
                    if ((!this.Player.GotBuff("Slice and Dice") || this.GetSliceAndDiceDuration() <= 0.5) && !this.ShouldWeEviscerate() && ComboPoint > 0)
                    {
                        this.Player.Cast("Slice and Dice");
                        return;
                    }
                }
                //might as well rupture at top health with low combo
                if (this.Player.GetSpellRank("Rupture") != 0 && this.Target.HealthPercent > 60)
                {
                    if (ComboPoint >= 1 && ComboPoint <= 2 && !this.Target.GotDebuff("Rupture"))
                    {
                        this.Player.Cast("Rupture");
                        return;
                    }
                }
                //fuckin Riposte these assholes if you can
                /**
                if (this.Player.GetSpellRank("Riposte") != 0)
                {
                    if (this.Player.CanUse("Riposte"))
                    {
                        this.Player.Cast("Riposte");
                        return;
                    }
                }
                **/
                //If nothing else, cast SS
                if (Energy >= 40  && this.Player.CanUse("Sinister Strike"))
                {
                    this.Player.Cast("Sinister Strike");
                }

            }

        }
        public override void Rest()
        {
            if(this.Player.HealthPercent < 60)
                this.Player.Eat();
            Player.DoString("DoEmote('Sit')");
        }


        public override bool Buff()
        {
            if (this.Player.IsMainhandEnchanted() && this.Player.IsOffhandEnchanted())
                return true;

            if (this.Player.IsCasting != "")
                return false;

            string[] Posions = { "Instant Poison", "Instant Poison I", "Instant Poison II", "Instant Poison III", "Instant Poison IV", "Instant Poison V", "Instant Poison VI" };
            //Retrieve the most powerful posion in our inventory
            if (this.Player.GetLastItem(Posions) != String.Empty)
            {

                if (!this.Player.IsMainhandEnchanted())
                {
                    this.Player.EnchantMainhandItem(this.Player.GetLastItem(Posions));
                    return false;
                }

                if (!this.Player.IsOffhandEnchanted())
                {
                    this.Player.EnchantOffhandItem(this.Player.GetLastItem(Posions));
                    return false;
                }

            }
            //True means we are done buffing, or cannot buff
            return true;
        }
    }
}