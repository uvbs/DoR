
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class GameObject : Unit
    {
        public GameObject_spawn Spawn;
        public bool Looted;
        static public int RELOOTABLE_TIME = 120000; // 2 Mins
        public bool DoorOpen = false;
        public GameObject()
            : base()
        {

        }

        public GameObject(GameObject_spawn Spawn)
            : this()
        {
            this.Spawn = Spawn;
            Name = Spawn.Proto.Name;
        }

        public override void OnLoad()
        {
            Faction = Spawn.Proto.Faction;
            while (Faction >= 8) Faction -= 8;
            if (Faction < 2) Rank = 0;
            else if (Faction < 4) Rank = 1;
            else if (Faction < 6) Rank = 2;
            else if (Faction < 9) Rank = 3;
            Faction = Spawn.Proto.Faction;

            Level = Spawn.Proto.Level;
            MaxHealth = Math.Min(1,Spawn.Proto.HealthPoints);
            Health = TotalHealth;

            X = Zone.CalculPin((uint)(Spawn.WorldX), true);
            Y = Zone.CalculPin((uint)(Spawn.WorldY), false);
            Z = (ushort)(Spawn.WorldZ);

            Heading = (ushort)Spawn.WorldO;
            SetOffset((ushort)(Spawn.WorldX >> 12), (ushort)(Spawn.WorldY >> 12));
            ScrInterface.AddScript(Spawn.Proto.ScriptName);
            base.OnLoad();
            IsActive = true;
            Looted = false;
        }




        public override void SendMeTo(Player Plr)
        {

        //    Log.Info("GO", "go = " + Name + "  Oid = " + Oid + "  X= " + Spawn.WorldX + "  Y= " + Spawn.WorldY + "  Z= " + Spawn.WorldZ + " DoorId= " + Spawn.DoorId);


            PacketOut Out = new PacketOut((byte)Opcodes.F_CREATE_STATIC);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16((byte)(DoorOpen ? 1 : Spawn.DoorOpen));

            Out.WriteUInt16((UInt16)Spawn.WorldO);
            Out.WriteUInt16((UInt16)Spawn.WorldZ);
            Out.WriteUInt32((UInt32)Spawn.WorldX);
            Out.WriteUInt32((UInt32)Spawn.WorldY);
            Out.WriteUInt16((ushort)Spawn.DisplayID);

            Out.WriteUInt16(Spawn.GetUnk(0));
            Out.WriteUInt16(Spawn.GetUnk(1));
            Out.WriteUInt16(Spawn.GetUnk(2));
            Out.WriteByte(Spawn.Unk1);

            int flags = Spawn.GetUnk(3);
            Loot Loots = LootsMgr.GenerateLoot(this, Plr);
            if ((Loots != null && Loots.IsLootable()) || (Plr.QtsInterface.GetPublicQuest() != null) || Plr.QtsInterface.GameObjectNeeded(Spawn.Entry) || Spawn.DoorId != 0)
            
            {
                flags = flags | 4;
            }

            Out.WriteUInt16((ushort)flags);
            Out.WriteByte(Spawn.Unk2);
            Out.WriteUInt32(Spawn.Unk3);
            Out.WriteUInt16(Spawn.GetUnk(4));
            Out.WriteUInt16(Spawn.GetUnk(5));
            Out.WriteUInt32(Spawn.Unk4);

            Out.WritePascalString(Name);

            if (Spawn.DoorId != 0)
            {
                Out.WriteByte(0x04);
                Out.WriteUInt32((uint)(Spawn.DoorId));
            }
            else
                Out.WriteByte(0x00);


            Plr.SendPacket(Out);

            base.SendMeTo(Plr);
        }

        public override void SendInteract(Player Plr, InteractMenu Menu)

        {
          Log.Success("SendInteract", "" + Name + " -> " + Plr.Name + ",Type=" + InteractType);


            PacketOut Out = new PacketOut((byte)Opcodes.F_SET_ABILITY_TIMER);
            Out.WriteByte(0);// 00
            Out.WriteByte(1);// 01
            Out.WriteByte(1);// 01
            Out.WriteByte(5);// 05
            Out.WriteUInt16(0);//00 00
            Out.WriteUInt16(Spawn.GameObjTimer);// 13 88 =5000
            Out.WriteUInt16(Oid);
            Out.WriteUInt16(0);
            Plr.SendPacket(Out);


            if (Spawn.GameObjTimer == 0)
            
            {
                Out = new PacketOut((byte)Opcodes.F_SET_ABILITY_TIMER);
                Out.WriteByte(0);// 00
                Out.WriteByte(1);// 01
                Out.WriteByte(1);// 01
                Out.WriteUInt16(0);//00 00
                Out.WriteUInt16(0);// 13 88
                Out.WriteUInt16(0);
                Out.WriteUInt16(0);
                Out.WriteByte(0);

                Plr.SendPacket(Out);
            }


            Out = new PacketOut((byte)Opcodes.F_PLAY_EFFECT);
            Out.WriteUInt16((UInt16)0x0144);
            Out.WriteByte(0);// 00
            Out.WriteUInt16((UInt16)Spawn.WorldZ);
            Out.WriteUInt32((UInt32)Spawn.WorldX);
            Out.WriteUInt32((UInt32)Spawn.WorldY);
            Out.WriteUInt16(0x0017);// 00 17
            Out.Fill(0, 5);
            Plr.SendPacket(Out);



            Tok_Info Info = WorldMgr.GetTok(Spawn.Proto.TokUnlock);

            if (!IsDead)
            {
                Plr.QtsInterface.HandleEvent(Objective_Type.QUEST_USE_GO, Spawn.Entry, 1);
            }

            if (Spawn.Proto.TokUnlock != 0)
                Plr.TokInterface.AddTok(Info);

            Loot Loots = LootsMgr.GenerateLoot(this, Plr);

            if (Loots != null)
            {
                Loots.SendInteract(Plr, Menu);
                // If object has been looted, make it unlootable
                // and then Reset its lootable staus in XX seconds
                if (!Loots.IsLootable())
                {
                    Looted = true;
                    foreach (Object Obj in this._ObjectRanged)
                    {
                        if (Obj.IsPlayer())
                        {
                            this.SendMeTo(Obj.GetPlayer());
                        }
                    }
                    EvtInterface.AddEvent(ResetLoot, RELOOTABLE_TIME, 1);   
                }
            }



            if (Spawn.DoorId != 0)
            {
                if (DoorOpen)
                    CloseDoor();
                else
                    OpenDoor();
            }








            base.SendInteract(Plr, Menu);
        }

        // This will reset the GameObject loot after it has
        // been looted. Allowing it to be looted again.
        public void ResetLoot()
        {
            Looted = false;
            foreach (Object Obj in this._ObjectRanged)
            {
                if (Obj.IsPlayer())
                {
                    this.SendMeTo(Obj.GetPlayer());
                }
            }
        }

        public override string ToString()
        {
            return "SpawnId=" + Spawn.Guid + ",Entry=" + Spawn.Entry + ",Name=" + Name + ",Level=" + Level + ",Faction=" + Faction + ",Position :" + base.ToString();
        }


        #region Door

        public void OpenDoor()
        {
            //NEED SLOW CLOSEING DOOR CODE

            PacketOut Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
            Out.WriteUInt16(Oid);
            Out.WriteByte(6);
            Out.WriteUInt16(0);
            Out.WriteByte(8);
            Out.WriteUInt16(0x0001);  
            Out.WriteUInt32(0);
            Out.WriteUInt32(0);
            Out.WriteUInt16(0);
            DispatchPacket(Out, true);
            //*/

            DoorOpen = true;
            foreach (Object Obj in this._ObjectRanged)
                if (Obj.IsPlayer())
                    this.SendMeTo(Obj.GetPlayer());

            EvtInterface.AddEvent(CloseDoor, 8000, 1);
        }

        public void CloseDoor()
        {
            EvtInterface.RemoveEvent(CloseDoor);
            PacketOut Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
            Out.WriteUInt16(Oid);//
            Out.WriteByte(6);
            Out.WriteUInt16(0);
            Out.WriteByte(8);
            Out.WriteUInt16(0);
            Out.WriteUInt32(0);
            Out.WriteUInt32(0);
            Out.WriteUInt16(0);
            DispatchPacket(Out, true);
            

            DoorOpen = false;
            foreach (Object Obj in this._ObjectRanged)
                if (Obj.IsPlayer())
                    this.SendMeTo(Obj.GetPlayer());
        }

        #endregion


    }
}
