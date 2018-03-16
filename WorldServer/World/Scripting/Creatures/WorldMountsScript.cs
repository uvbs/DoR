/*leo228
 * Copyright (C) Dawn of Reckoning 2008-2019
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    [GeneralScript(false, "", CreatureEntry = 116, GameObjectEntry = 0)]
    public class WorldOrderMountScript : AGeneralScript
    {
        public override void OnInteract(Object Obj, Player Target, InteractMenu Menu)
        {
            Mount(Target);
        }

        static public void Mount(Player Target)
        {
            if (Target.MvtInterface.IsMount())
                return;

            if(Target._Info.Race == (byte)GameData.Races.RACES_DWARF)
                Target.MvtInterface.CurrentMount.SetMount(8);
            else if (RandomMgr.Next(4) == 1)
                Target.MvtInterface.CurrentMount.SetMount(180);
            else
                Target.MvtInterface.CurrentMount.SetMount(1);
        }
    }

    [GeneralScript(false, "", CreatureEntry = 155, GameObjectEntry = 0)]
    public class WorldDestructionMountScript : AGeneralScript
    {
        public override void OnInteract(Object Obj, Player Target, InteractMenu Menu)
        {
            Mount(Target);
        }

        static public void Mount(Player Target)
        {
            if (Target.MvtInterface.IsMount())
                return;

            if (RandomMgr.Next(4) == 1)
                Target.MvtInterface.CurrentMount.SetMount(3);
            else
                Target.MvtInterface.CurrentMount.SetMount(12);
        }
    }

    [GeneralScript(true, "Horse Mount Scrypt")]
    public class WorldFleeAbilityMount : AGeneralScript
    {
        public override void OnCastAbility(Ability Ab)
        {
            if (Ab.Caster.IsPlayer() && Ab.Info.Entry == 15071)//245) // Flee
            {
                if (Ab.Caster.GetPlayer().MvtInterface.IsMount())
                {
                    Ab.Caster.GetPlayer().MvtInterface.UnMount();
                    return;
                }

                if (Ab.Caster.GetPlayer().Realm == GameData.Realms.REALMS_REALM_ORDER)
                    WorldOrderMountScript.Mount(Ab.Caster.GetPlayer());
                else
                    WorldDestructionMountScript.Mount(Ab.Caster.GetPlayer());
            }
        }
    }
}
