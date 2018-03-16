/*
 * Copyright (C) 2013 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    [IAbilityType(new ushort[] { 9566 }, "DealDamagesInFront", "Deal Damages In Front Zone Handler")] // Essence Lash
    public class DealDamagesInFront : IAbilityTypeHandler
    {
        public Ability Ab;
        public DealDamagesHandler DealHandler;

        public override void InitAbility(Ability Ab)
        {
            this.Ab = Ab;
            this.DealHandler = new DealDamagesHandler();
            DealHandler.InitAbility(Ab);
        }

        public override void Start(Ability Ab)
        {
            base.Start(Ab);
            DealHandler.Start(Ab);
        }

        public override GameData.AbilityResult CanCast(bool IsStart)
        {
            GameData.AbilityResult Result = GameData.AbilityResult.ABILITYRESULT_OK;

            return Result;
        }

        public override IAbilityTypeHandler Cast()
        {
            int Count = 0;

            GetTargets((Unit Target) =>
            {

                //All Classes
                if (Ab.Info.Entry == 607 & Count >= 9) // (Raze', All Classes)
                    return;

                //Slayer
                if (Ab.Info.Entry == 1747 & Count >= 3) // (Slayer, Flurry) , hit 3 targets
                    return;
                if (Ab.Info.Entry == 1490 & Count >= 9) // (Accuracy, Slayer)
                    return;
                if (Ab.Info.Entry == 1436 & Count >= 9) // (Onslaught, Slayer)
                    return;

                //Choppa
                if (Ab.Info.Entry == 1435 & Count >= 3) // (Lotsa, Choppa) , hit 3 targets
                    return;
                if (Ab.Info.Entry == 1748 & Count >= 9) // (Yer All Bleedin' Now, Choppa)
                    return;

                //Shaman
                if (Ab.Info.Entry == 1360 & Count >= 9) // (Scuse Me!, Shaman)
                    return;

                //Black Orc
                if (Ab.Info.Entry == 1678 & Count >= 9) // (Da Big Un', Black Orc)
                    return;

                // BlackGuard
                if (Ab.Info.Entry == 9323 & Count >= 9) // (Monstrous Rending, Blackguard)
                    return;
                if (Ab.Info.Entry == 9344 & Count >= 9) // (Crimson Death, Blackguard)
                    return;
                if (Ab.Info.Entry == 9348 & Count >= 9) // (Wave of Scorn, Blackguard)
                    return;
                if (Ab.Info.Entry == 9387 & Count >= 9) // (Blast of Hatred, Blackguard)
                    return;

                //Disciple of Khaine
                if (Ab.Info.Entry == 9566 & Count >= 9) // (Essence Lash, Dok)
                    return;

                //Bright Wizard
                if (Ab.Info.Entry == 8171 & Count >= 9) // (Flame Breath, BW)
                    return;
                if (Ab.Info.Entry == 8221 & Count >= 9) // (Wall of Fire, BW)
                    return;

                //Chosen
                if (Ab.Info.Entry == 8344 & Count >= 9) // (Rending Blade, Chosen)
                    return;
                if (Ab.Info.Entry == 8377 & Count >= 9) // (Warping Embrace, Chosen)
                    return;

                //Iron Breaker
                if (Ab.Info.Entry == 1366 & Count >= 9) // (Rune-Etched Axe, IB)
                    return;
                if (Ab.Info.Entry == 1425 & Count >= 9) // (Axe Slam, IB)
                    return;

                //Knight of the Blazing Sun
                if (Ab.Info.Entry == 8026 & Count >= 9) // (Arcing Swing, Kotbs)
                    return;
                if (Ab.Info.Entry == 8074 & Count >= 9) // (Nova Strike, Kotbs)
                    return;

                //Magus
                if (Ab.Info.Entry == 368 & Count >= 9) // (Warping Energy, Magus)
                    return;
                if (Ab.Info.Entry == 8487 & Count >= 9) // (Infernal Blast, Magus)
                    return;
                if (Ab.Info.Entry == 8534 & Count >= 9) // (Deamonic Scream, Magus)
                    return;

                //Marauder
                if (Ab.Info.Entry == 1793 & Count >= 9) // (Demolition, Marauder)
                    return;
                if (Ab.Info.Entry == 8422 & Count >= 9) // (Wave of Terror, Marauder)
                    return;
                if (Ab.Info.Entry == 8423 & Count >= 9) // (Concussive Jolt , Marauder)Wrecking Ball 
                    return;
                if (Ab.Info.Entry == 8425 & Count >= 9) // (Wrecking Ball, Marauder)
                    return;
                if (Ab.Info.Entry == 8450 & Count >= 9) // (Great Fang, Marauder)
                    return;

                //Runepriest
                if (Ab.Info.Entry == 1597 & Count >= 9) // (Rune of Cleaving, RP)
                    return;
                if (Ab.Info.Entry == 1650 & Count >= 9) // (Rune of Skewering, RP)
                    return;

                //Shadow Warrior
                if (Ab.Info.Entry == 9100 & Count >= 9) // (Lileath's Arrow, SW)
                    return;
                if (Ab.Info.Entry == 9107 & Count >= 9) // (Sweeping Slash, SW)
                    return;
                if (Ab.Info.Entry == 2256 & Count >= 9) // (Barrage, SW)
                    return;
                if (Ab.Info.Entry == 2287 & Count >= 9) // (Penetrating Arrow, SW)
                    return;

                //Sorcerer
                if (Ab.Info.Entry == 9486 & Count >= 9) // (Ice Spikes, Sorc)
                    return;
                if (Ab.Info.Entry == 9488 & Count >= 9) // (Infernal Wave, Sorc)
                    return;

                //Squig Herder
                if (Ab.Info.Entry == 1840 & Count >= 9) // (Shoot Thru Ya, SH)
                    return;
                if (Ab.Info.Entry == 1849 & Count >= 9) // (Indigestion, SH)
                    return;
                if (Ab.Info.Entry == 1885 & Count >= 9) // (Arrer O' Mork, SH)
                    return;

                //Warrior Priest
                if (Ab.Info.Entry == 8250 & Count >= 9) // (Smite, WP)
                    return;
                if (Ab.Info.Entry == 8254 & Count >= 9) // (Divine Shock, WP)
                    return;

                //White Lion
                if (Ab.Info.Entry == 9176 & Count >= 9) // (Slashing Blade, WL)
                    return;

                //Witch Elf
                if (Ab.Info.Entry == 9444 & Count >= 3) // (Broad Severing, WE)
                    return;
                if (Ab.Info.Entry == 9460 & Count >= 9) // (Web of Shadows, WE)
                    return;
                if (Ab.Info.Entry == 9460 & Count >= 9) // (Fling Poison, WE)
                    return;

                //Witch Hunter
                if (Ab.Info.Entry == 8147 & Count >= 9) // (Reversal of Fortune, WH)
                    return;
                if (Ab.Info.Entry == 8152 & Count >= 9) // (Divine Blast, WH)
                    return;

                //Zealot
                if (Ab.Info.Entry == 8563 & Count >= 9) // (Demon Spittle, Zealot)
                    return;

                DealHandler.InitTargets(Target);
                if (DealHandler.CanCast(false) == GameData.AbilityResult.ABILITYRESULT_OK)
                {
                    //Marauder
                    if (Ab.Info.Entry == 8397) // Mouth of Tzeentch        //Interrupting Spells in front and deal damages
                        Target.AbtInterface.Cancel(false);

                    DealHandler.Cast();

                    //Knight of the Blazing Sun
                    if (Ab.Info.Entry == 8038) // Heaven's Fury , movement stuck 9 seconds // Should be stagger but atm its allright.
                        Target.DisableMovements(9000);

                    //Chosen
                    if (Ab.Info.Entry == 8349) // Quake , movement stuck 9 seconds // Should be stagger but atm its allright.
                        Target.DisableMovements(9000);

                    ++Count;
                }
            });

            return null;
        }

        public override void GetTargets(OnTargetFind OnFind)
        {
            Unit Target;
            foreach (Object Obj in Ab.Caster._ObjectRanged)
            {
                if (!Obj.IsUnit())
                    continue;

                Target = Obj.GetUnit();
                if (!CombatInterface.CanAttack(Ab.Caster, Target))
                    continue;

                if (Obj.GetDistanceTo(Ab.Px, Ab.Py, Ab.Pz) < Ab.Info.GetRadius(0) && Ab.Caster.IsObjectInFront(Target,90))
                {
                    OnFind(Target);
                }
            }
        }
    }
}
