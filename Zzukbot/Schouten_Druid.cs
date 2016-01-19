using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZzukBot.Engines.CustomClass;

namespace something
{
    public class Schouten_Druid : CustomClass
    {

        private bool Shifted()
        {
            return (this.Player.GotBuff("Cat Form") || this.Player.GotBuff("Bear Form")
                || this.Player.GotBuff("Dire Bear Form") || this.Player.GotBuff("Aquatic Form")
                || this.Player.GotBuff("Travel Form"));
        }

        private bool TargetCanBleed()
        {
            return (this.Target.CreatureType != CreatureType.Elemental && this.Target.CreatureType != CreatureType.Mechanical);

        }

        private bool CanShift()
        {
            return (this.Player.GetSpellRank("Cat Form") != 0 || this.Player.GetSpellRank("Bear Form") != 0);
        }

        private void Shift()
        {
            if (this.Player.GetSpellRank("Cat Form") != 0 && this.Attackers.Count < 2 && this.Player.CanUse("Cat Form"))
            {
                this.Player.CastWait("Cat Form", 500);
            }
            else if (this.Player.GetSpellRank("Dire Bear Form") != 0 && this.Player.CanUse("Dire Bear Form"))
            {
                this.Player.CastWait("Dire Bear Form", 500);
            }
            else if (this.Player.GetSpellRank("Bear Form") != 0 && this.Player.CanUse("Bear Form"))
            {
                this.Player.CastWait("Bear Form", 500);
            }
        }

        public override byte DesignedForClass
        {
            get
            {
                return PlayerClass.Druid;
            }
        }

        public override string CustomClassName
        {
            get
            {
                return "Schouten_Druid";
            }
        }

        public override void PreFight()
        {
            this.SetCombatDistance(25);
            if (CanShift())
            {
                if (Shifted())
                {
                    if (this.Target.DistanceToPlayer <= 25 && !this.Target.GotDebuff("Faerie Fire (Feral)") && TargetCanBleed() && this.Player.CanUse("Faerie Fire (Feral)"))
                    {
                        this.Player.DoString("CastSpellByName('Faerie Fire (Feral)()');");
                        //this.Player.TryCast("Faerie Fire (Feral)()"); //<-- Sort of untested but i dont think it works
                    }
                }
                else
                {
                    if (this.Target.DistanceToPlayer <= 25 && !this.Target.GotDebuff("Moonfire") && this.Player.CanUse("Moonfire"))
                    {
                        this.Player.TryCast("Moonfire");
                        Shift();
                    }
                }
                this.SetCombatDistance(3);
                this.Player.Attack();
            }
            else
            {
                if (this.Target.DistanceToPlayer <= 25 && !this.Target.GotDebuff("Moonfire") && this.Player.CanUse("Moonfire"))
                {
                    this.Player.TryCast("Moonfire");
                }
                if (this.Target.DistanceToPlayer <= 25 && this.Player.CanUse("Wrath"))
                {
                    this.Player.CastWait("Wrath", 1000);
                }
            }
        }

        public override void Fight()
        {
            if (Shifted())
            {
                this.SetCombatDistance(3);
                this.Player.Attack();
                if (!this.Target.GotDebuff("Faerie Fire (Feral)") && TargetCanBleed() && this.Player.CanUse("Faerie Fire (Feral)"))
                {
                    this.Player.DoString("CastSpellByName('Faerie Fire (Feral)()');");
                    //this.Player.TryCast("Faerie Fire (Feral)()"); //<-- Sort of untested but i dont think it works
                }

                if (this.Player.HealthPercent < 35 && this.Target.HealthPercent > 15 && this.Player.ManaPercent >= 35)
                {
                    this.Player.CancelShapeShift();
                    return;
                }

                if (this.Player.GotBuff("Cat Form"))
                {
                    if (this.Player.Energy >= 30)
                    {
                        if (TargetCanBleed())
                        {
                            if (this.Target.HealthPercent > 50 && this.Player.Energy >= 30 && this.Player.ComboPoints >= 5 && !this.Target.GotDebuff("Rip") && this.Player.CanUse("Rip"))
                            {
                                this.Player.TryCast("Rip");
                            }
                            if (this.Target.HealthPercent >= 40 && this.Player.Energy >= 40 && this.Player.ComboPoints < 5 && !this.Target.GotDebuff("Rake") && this.Player.CanUse("Rake"))
                            {
                                this.Player.TryCast("Rake");
                            }
                        }

                        if (this.Player.Energy >= 45 && this.Player.ComboPoints >= 1)
                        {
                            if (this.Player.Energy >= 80)
                            {
                                if (this.Player.ComboPoints >= 5 || this.Target.HealthPercent <= 35)
                                {
                                    if (!this.Player.GotBuff("Tiger's Fury") && this.Player.CanUse("Tiger's Fury"))
                                    {
                                        this.Player.TryCast("Tiger's Fury");
                                    }
                                    if (this.Player.GetSpellRank("Ferocious Bite") != 0 && this.Player.CanUse("Ferocious Bite"))
                                    {
                                        this.Player.TryCast("Ferocious Bite");
                                    }
                                    else
                                    {
                                        if (this.Player.CanUse("Rip"))
                                        {
                                            this.Player.TryCast("Rip");
                                        }
                                    }
                                }
                            }
                            if (this.Player.ComboPoints < 5 && this.Target.HealthPercent >= 35 && this.Player.CanUse("Claw"))
                            {
                                this.Player.Cast("Claw");
                            }
                        }
                        else if (this.Player.Energy >= 45 && this.Player.CanUse("Claw"))
                        {
                            this.Player.Cast("Claw");
                        }
                    }
                    return;
                }

                else if (this.Player.GotBuff("Bear Form") || this.Player.GotBuff("Dire Bear Form"))
                {
                    if (this.Player.Rage >= 10)
                    {
                        if (this.Player.CanUse("Bash"))
                        {
                            this.Player.TryCast("Bash");
                        }
                        if (/* Player.IsAoeSafe(12) && */!this.Target.GotDebuff("Demoralizing Roar") && this.Player.CanUse("Demoralizing Roar"))
                        {
                            this.Player.TryCast("Demoralizing Roar");
                        }
                    }
                    if (this.Player.Rage >= 50 && (this.Player.HealthPercent >= 70 || !this.Player.CanUse("Frenzied Regeneration")) && this.Attackers.Count <= 1 && this.Player.CanUse("Maul"))
                    {
                        this.Player.CastWait("Maul", 2500);
                    }
                    if (this.Player.Rage >= 50 && (this.Player.HealthPercent >= 70 || !this.Player.CanUse("Frenzied Regeneration")) && this.Attackers.Count >= 2 && this.Player.CanUse("Swipe"))
                    {
                        this.Player.TryCast("Swipe");
                    }
                    if (this.Player.Rage >= 50 && this.Player.HealthPercent <= 50)
                    {
                        if (this.Player.CanUse("Frenzied Regeneration"))
                        {
                            this.Player.TryCast("Frenzied Regeneration");
                        }
                    }
                    return;
                }
            }
            else
            {
                this.SetCombatDistance(25);
                if (this.Player.CanUse("Innervate") && this.Player.ManaPercent <= 30)
                {
                    this.Player.TryCast("Innervate");
                }

                if (this.Player.HealthPercent <= 35)
                {
                    this.Player.TryCast("War Stomp");
                    if (!this.Player.GotBuff("Rejuvenation") && this.Player.ManaPercent >= 10 && this.Player.CanUse("Rejuvenation"))
                    {
                        this.Player.CastWait("Rejuvenation", 500);
                    }
                    if (this.Player.ManaPercent >= 25 && this.Player.CanUse("Healing Touch"))
                    {
                        this.Player.CastWait("Healing Touch", 5000);
                    }

                    if (this.Player.HealthPercent >= 60 && CanShift() && this.Player.ManaPercent >= 40)
                    {
                        Shift();
                    }
                }

                else
                {
                    //Melee Them
                    if (CanShift())
                    {
                        if (this.Player.HealthPercent >= 60 && this.Player.ManaPercent >= 40)
                        {
                            Shift();
                        }
                        this.SetCombatDistance(3);
                        this.Player.Attack();
                    }
                    if (!CanShift() && this.Player.ManaPercent >= 10)
                    {
                        this.SetCombatDistance(25);
                        if (this.Target.DistanceToPlayer <= 25 && !this.Target.GotDebuff("Moonfire") && this.Player.CanUse("Moonfire"))
                        {
                            this.Player.TryCast("Moonfire");
                        }
                        if (this.Target.DistanceToPlayer <= 25 && this.Player.CanUse("Wrath"))
                        {
                            this.Player.CastWait("Wrath", 1000);
                        }
                    }
                    else
                    {
                        this.SetCombatDistance(3);
                        this.Player.Attack();
                    }
                }
                return;
            }
        }

        public override void Rest()
        {
            if (Shifted())
            {
                this.Player.CancelShapeShift();
            }
            if (this.Player.HealthPercent <= 70 && this.Player.ManaPercent >= 25 && !this.Player.GotBuff("Drink") && this.Player.CanUse("Healing Touch"))
            {
                this.Player.CastWait("Healing Touch", 5000);
            }
            else if (this.Player.HealthPercent <= 80 && this.Player.ManaPercent >= 10 && !this.Player.GotBuff("Rejuvenation") && !this.Player.GotBuff("Drink") && this.Player.CanUse("Rejuvenation"))
            {
                this.Player.CastWait("Rejuvenation", 500);
            }
            if (!this.Player.GotBuff("Drink") && this.Player.ManaPercent <= 40)
            {
                this.Player.Drink();
            }
            if (!this.Player.GotBuff("Shadowmeld") && this.Player.GotBuff("Drink"))
            {
                this.Player.TryCast("Shadowmeld");
            }
            Player.DoString("DoEmote('Sit')");
        }

        public override bool Buff()
        {
            if (this.Player.GetSpellRank("Thorns") != 0 && !this.Player.GotBuff("Thorns"))
            {
                if (Shifted())
                {
                    this.Player.CancelShapeShift();
                }
                if (this.Player.CanUse("Thorns"))
                {
                    this.Player.Cast("Thorns");
                }

                if (this.Player.HealthPercent <= 45 && this.Player.ManaPercent >= 25 && !this.Player.GotBuff("Drink") && this.Player.CanUse("Healing Touch"))
                {
                    this.Player.CastWait("Healing Touch", 5000);
                }
                else if (this.Player.HealthPercent >= 35 && this.Player.HealthPercent <= 80 && this.Player.ManaPercent >= 10 && !this.Player.GotBuff("Rejuvenation") && !this.Player.GotBuff("Drink") && this.Player.CanUse("Rejuvenation"))
                {
                    this.Player.CastWait("Rejuvenation", 500);
                }
            }
            if (this.Player.GetSpellRank("Mark of the Wild") != 0 && !this.Player.GotBuff("Mark of the Wild"))
            {
                if (Shifted())
                {
                    this.Player.CancelShapeShift();
                }
                if (this.Player.CanUse("Mark of the Wild"))
                {
                    this.Player.Cast("Mark of the Wild");
                }

                if (this.Player.HealthPercent <= 45 && this.Player.ManaPercent >= 25 && !this.Player.GotBuff("Drink") && this.Player.CanUse("Healing Touch"))
                {
                    this.Player.CastWait("Healing Touch", 5000);
                }
                else if (this.Player.HealthPercent >= 35 && this.Player.HealthPercent <= 80 && this.Player.ManaPercent >= 10 && !this.Player.GotBuff("Rejuvenation") && !this.Player.GotBuff("Drink") && this.Player.CanUse("Rejuvenation"))
                {
                    this.Player.CastWait("Rejuvenation", 500);
                }
            }
            if (this.Player.GetSpellRank("Omen of Clarity") != 0 && !this.Player.GotBuff("Omen of Clarity"))
            {
                if (Shifted())
                {
                    this.Player.CancelShapeShift();
                }
                if (this.Player.CanUse("Omen of Clarity"))
                {
                    this.Player.Cast("Omen of Clarity");
                }

                if (this.Player.HealthPercent <= 45 && this.Player.ManaPercent >= 25 && !this.Player.GotBuff("Drink") && this.Player.CanUse("Healing Touch"))
                {
                    this.Player.CastWait("Healing Touch", 5000);
                }
                else if (this.Player.HealthPercent >= 35 && this.Player.HealthPercent <= 80 && this.Player.ManaPercent >= 10 && !this.Player.GotBuff("Rejuvenation") && !this.Player.GotBuff("Drink") && this.Player.CanUse("Rejuvenation"))
                {
                    this.Player.CastWait("Rejuvenation", 500);
                }
                scrollBuff();
            }
            return true;
        }

        //function library should be seperated but fuck it

        //buff with any scrolls that you have
        public void scrollBuff()
        {
            if (this.Player.CanUse("Scroll of Strength I") && this.Player.ItemCount("Scroll of Strength I") > 0 && (!this.Player.GotBuff("Scroll of Strength I") || !this.Player.GotBuff("Scroll of Strength II") || !this.Player.GotBuff("Scroll of Strength III") || !this.Player.GotBuff("Scroll of Strength IV")))
            {
                this.Player.UseItem("Scroll of Strength I");
            }
            if (this.Player.CanUse("Scroll of Strength II") && this.Player.ItemCount("Scroll of Strength II") > 0 && (!this.Player.GotBuff("Scroll of Strength I") || !this.Player.GotBuff("Scroll of Strength II") || !this.Player.GotBuff("Scroll of Strength III") || !this.Player.GotBuff("Scroll of Strength IV")))
            {
                this.Player.UseItem("Scroll of Strength II");
            }
            if (this.Player.CanUse("Scroll of Strength III") && this.Player.ItemCount("Scroll of Strength III") > 0 && (!this.Player.GotBuff("Scroll of Strength I") || !this.Player.GotBuff("Scroll of Strength II") || !this.Player.GotBuff("Scroll of Strength III") || !this.Player.GotBuff("Scroll of Strength IV")))
            {
                this.Player.UseItem("Scroll of Strength III");
            }
            if (this.Player.CanUse("Scroll of Strength IV") && this.Player.ItemCount("Scroll of Strength IV") > 0 && (!this.Player.GotBuff("Scroll of Strength I") || !this.Player.GotBuff("Scroll of Strength II") || !this.Player.GotBuff("Scroll of Strength III") || !this.Player.GotBuff("Scroll of Strength IV")))
            {
                this.Player.UseItem("Scroll of Strength IV");
            }
            if (this.Player.CanUse("Scroll of Agility I") && this.Player.ItemCount("Scroll of Agility I") > 0 && (!this.Player.GotBuff("Scroll of Agility I") || !this.Player.GotBuff("Scroll of Agility II") || !this.Player.GotBuff("Scroll of Agility III") || !this.Player.GotBuff("Scroll of Agility IV")))
            {
                this.Player.UseItem("Scroll of Agility I");
            }
            if (this.Player.CanUse("Scroll of Agility II") && this.Player.ItemCount("Scroll of Agility II") > 0 && (!this.Player.GotBuff("Scroll of Agility I") || !this.Player.GotBuff("Scroll of Agility II") || !this.Player.GotBuff("Scroll of Agility III") || !this.Player.GotBuff("Scroll of Agility IV")))
            {
                this.Player.UseItem("Scroll of Agility II");
            }
            if (this.Player.CanUse("Scroll of Agility III") && this.Player.ItemCount("Scroll of Agility III") > 0 && (!this.Player.GotBuff("Scroll of Agility I") || !this.Player.GotBuff("Scroll of Agility II") || !this.Player.GotBuff("Scroll of Agility III") || !this.Player.GotBuff("Scroll of Agility IV")))
            {
                this.Player.UseItem("Scroll of Agility III");
            }
            if (this.Player.CanUse("Scroll of Agility IV") && this.Player.ItemCount("Scroll of Agility IV") > 0 && (!this.Player.GotBuff("Scroll of Agility I") || !this.Player.GotBuff("Scroll of Agility II") || !this.Player.GotBuff("Scroll of Agility III") || !this.Player.GotBuff("Scroll of Agility IV")))
            {
                this.Player.UseItem("Scroll of Agility IV");
            }
            if (this.Player.CanUse("Scroll of Stamina I") && this.Player.ItemCount("Scroll of Stamina I") > 0 && (!this.Player.GotBuff("Scroll of Stamina I") || !this.Player.GotBuff("Scroll of Stamina II") || !this.Player.GotBuff("Scroll of Stamina III") || !this.Player.GotBuff("Scroll of Stamina IV")))
            {
                this.Player.UseItem("Scroll of Stamina I");
            }
            if (this.Player.CanUse("Scroll of Stamina II") && this.Player.ItemCount("Scroll of Stamina II") > 0 && (!this.Player.GotBuff("Scroll of Stamina I") || !this.Player.GotBuff("Scroll of Stamina II") || !this.Player.GotBuff("Scroll of Stamina III") || !this.Player.GotBuff("Scroll of Stamina IV")))
            {
                this.Player.UseItem("Scroll of Stamina II");
            }
            if (this.Player.CanUse("Scroll of Stamina III") && this.Player.ItemCount("Scroll of Stamina III") > 0 && (!this.Player.GotBuff("Scroll of Stamina I") || !this.Player.GotBuff("Scroll of Stamina II") || !this.Player.GotBuff("Scroll of Stamina III") || !this.Player.GotBuff("Scroll of Stamina IV")))
            {
                this.Player.UseItem("Scroll of Stamina III");
            }
            if (this.Player.CanUse("Scroll of Stamina IV") && this.Player.ItemCount("Scroll of Stamina IV") > 0 && (!this.Player.GotBuff("Scroll of Stamina I") || !this.Player.GotBuff("Scroll of Stamina II") || !this.Player.GotBuff("Scroll of Stamina III") || !this.Player.GotBuff("Scroll of Stamina IV")))
            {
                this.Player.UseItem("Scroll of Stamina IV");
            }
            if (this.Player.CanUse("Scroll of Spirit I") && this.Player.ItemCount("Scroll of Spirit I") > 0 && (!this.Player.GotBuff("Scroll of Spirit I") || !this.Player.GotBuff("Scroll of Spirit II") || !this.Player.GotBuff("Scroll of Spirit III") || !this.Player.GotBuff("Scroll of Spirit IV")))
            {
                this.Player.UseItem("Scroll of Spirit I");
            }
            if (this.Player.CanUse("Scroll of Spirit II") && this.Player.ItemCount("Scroll of Spirit II") > 0 && (!this.Player.GotBuff("Scroll of Spirit I") || !this.Player.GotBuff("Scroll of Spirit II") || !this.Player.GotBuff("Scroll of Spirit III") || !this.Player.GotBuff("Scroll of Spirit IV")))
            {
                this.Player.UseItem("Scroll of Spirit II");
            }
            if (this.Player.CanUse("Scroll of Spirit III") && this.Player.ItemCount("Scroll of Spirit III") > 0 && (!this.Player.GotBuff("Scroll of Spirit I") || !this.Player.GotBuff("Scroll of Spirit II") || !this.Player.GotBuff("Scroll of Spirit III") || !this.Player.GotBuff("Scroll of Spirit IV")))
            {
                this.Player.UseItem("Scroll of Spirit III");
            }
            if (this.Player.CanUse("Scroll of Spirit IV") && this.Player.ItemCount("Scroll of Spirit IV") > 0 && (!this.Player.GotBuff("Scroll of Spirit I") || !this.Player.GotBuff("Scroll of Spirit II") || !this.Player.GotBuff("Scroll of Spirit III") || !this.Player.GotBuff("Scroll of Spirit IV")))
            {
                this.Player.UseItem("Scroll of Spirit IV");
            }
            if (this.Player.CanUse("Scroll of Intellect I") && this.Player.ItemCount("Scroll of Intellect I") > 0 && (!this.Player.GotBuff("Scroll of Intellect I") || !this.Player.GotBuff("Scroll of Intellect II") || !this.Player.GotBuff("Scroll of Intellect III") || !this.Player.GotBuff("Scroll of Intellect IV")))
            {
                this.Player.UseItem("Scroll of Intellect I");
            }
            if (this.Player.CanUse("Scroll of Intellect II") && this.Player.ItemCount("Scroll of Intellect II") > 0 && (!this.Player.GotBuff("Scroll of Intellect I") || !this.Player.GotBuff("Scroll of Intellect II") || !this.Player.GotBuff("Scroll of Intellect III") || !this.Player.GotBuff("Scroll of Intellect IV")))
            {
                this.Player.UseItem("Scroll of Intellect II");
            }
            if (this.Player.CanUse("Scroll of Intellect III") && this.Player.ItemCount("Scroll of Intellect III") > 0 && (!this.Player.GotBuff("Scroll of Intellect I") || !this.Player.GotBuff("Scroll of Intellect II") || !this.Player.GotBuff("Scroll of Intellect III") || !this.Player.GotBuff("Scroll of Intellect IV")))
            {
                this.Player.UseItem("Scroll of Intellect III");
            }
            if (this.Player.CanUse("Scroll of Intellect IV") && this.Player.ItemCount("Scroll of Intellect IV") > 0 && (!this.Player.GotBuff("Scroll of Intellect I") || !this.Player.GotBuff("Scroll of Intellect II") || !this.Player.GotBuff("Scroll of Intellect III") || !this.Player.GotBuff("Scroll of Intellect IV")))
            {
                this.Player.UseItem("Scroll of Intellect IV");
            }
        }
    }
}