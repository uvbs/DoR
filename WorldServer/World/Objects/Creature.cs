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
    public class Creature : Unit
    {
        public Creature_spawn Spawn;
        public uint Entry
        {
            get
            {
                if (Spawn == null)
                    return 0;

                return Spawn.Entry;
            }
        }

        public Creature()
            : base()
        {
        }

        public Creature(Creature_spawn Spawn)
            : this()
        {
            this.Spawn = Spawn;
            Name = Spawn.Proto.Name;
        }

        public override void Update(long Tick)
        {
            base.Update(Tick);
        }

        static public ushort GenerateWounds(byte Level, byte Rank)
        {
            float Wounds = 0;
            Wounds += 70 * (Level + Level / 2);
            //Wounds += Level * 5.5f;

            if (Rank > 0)
                Wounds += Rank * Rank * Wounds;
            return (ushort)(Wounds / 10);
        }
        public override void OnLoad()
        {
            InteractType = GenerateInteractType(Spawn.Title != 0 ? Spawn.Title : Spawn.Proto.Title);

            SetFaction(Spawn.Faction != 0 ? Spawn.Faction : Spawn.Proto.Faction);

            ItmInterface.Load(WorldMgr.GetCreatureItems(Spawn.Entry));
            if (Spawn.Proto.MinLevel > Spawn.Proto.MaxLevel)
                Spawn.Proto.MinLevel = Spawn.Proto.MaxLevel;

            if (Spawn.Proto.MaxLevel <= Spawn.Proto.MinLevel)
                Spawn.Proto.MaxLevel = Spawn.Proto.MinLevel;

            if (Spawn.Proto.MaxLevel == 0) Spawn.Proto.MaxLevel = 1;
            if (Spawn.Proto.MinLevel == 0) Spawn.Proto.MinLevel = 1;


            if (Spawn.Level != 0)
            {
                if (Spawn.Level > 2)
                    Level = (byte)RandomMgr.Next((int)Spawn.Level - 1, Spawn.Level + 1);
                else
                    Level = (byte)RandomMgr.Next((int)Spawn.Level, Spawn.Level + 1);
            }
            else
            {
                Level = (byte)RandomMgr.Next((int)Spawn.Proto.MinLevel, (int)Spawn.Proto.MaxLevel + 1);
            }
            StsInterface.SetBaseStat((byte)GameData.Stats.STATS_WOUNDS, GenerateWounds(Level, Rank));
            StsInterface.ApplyStats();
            Health = TotalHealth;

            X = Zone.CalculPin((uint)(Spawn.WorldX), true);
            Y = Zone.CalculPin((uint)(Spawn.WorldY), false);
            Z = (ushort)(Spawn.WorldZ);
            /*
            if (Zone.ZoneId == 161)
            {
                Z += 16384;
                X += 16384;
                Y += 16384;
            }
            //*/
            // TODO : Bad Height Formula
            /*
               //int HeightMap = HeightMapMgr.GetHeight(Zone.ZoneId, X, Y);
            int HeightMap = ClientFileMgr.GetHeight(Zone.ZoneId, X, Y);
            
            if (Z < HeightMap)
            {
                Log.Error("Creature", "["+Spawn.Entry+"] Invalid Height : Min=" + HeightMap + ",Z=" + Z);
                return;
            }*/

            Heading = (ushort)Spawn.WorldO;
            WorldPosition.X = Spawn.WorldX;
            WorldPosition.Y = Spawn.WorldY;
            WorldPosition.Z = Spawn.WorldZ;

            SetOffset((ushort)(Spawn.WorldX >> 12), (ushort)(Spawn.WorldY >> 12));
            ScrInterface.AddScript(Spawn.Proto.ScriptName);
            base.OnLoad();

            /*AiInterface.Waypoints = WorldMgr.GetNpcWaypoints(Spawn.Guid);

            if (Spawn.Title == 0 && Spawn.Icone == 0 && Spawn.Proto.Title == 0 && Spawn.Icone == 0 && Spawn.Emote == 0 && Spawn.Proto.FinishingQuests == null && Spawn.Proto.StartingQuests == null)
            {
                if (Faction <= 1 || Faction == 128 || Faction == 129)
                {
                    if (AiInterface.Waypoints.Count <= 4)
                    {
                        int i = 0;
                        if (AiInterface.Waypoints.Count != 0)
                            i = AiInterface.Waypoints.Count - 1;
                        for (; i < 3; ++i)
                        {
                            AiInterface.AddWaypoint(new Waypoint());
                        }
                    }
                    foreach (Waypoint Wp in AiInterface.Waypoints)
                    {
                        AiInterface.RandomizeWaypoint(Wp);
                    }
                }
            }*/
            IsActive = true;

            if (InteractType == GameData.InteractType.INTERACTTYPE_TRAINER)
                States.Add(1);

            if (InteractType == GameData.InteractType.INTERACTTYPE_BANKER)
                States.Add(11);// 11

            if (InteractType == GameData.InteractType.INTERACTTYPE_GUILD_VAULT)// added 
                States.Add(0);// dont know what states are yet ?



            if (InteractType == GameData.InteractType.INTERACTTYPE_AUCTIONEER)
                States.Add(12);

            if (InteractType == GameData.InteractType.INTERACTTYPE_GUILD_REGISTRAR)
                States.Add(14);

            if (InteractType == GameData.InteractType.INTERACTTYPE_FLIGHT_MASTER)
                States.Add(15);

            if (InteractType == GameData.InteractType.INTERACTTYPE_DYEMERCHANT)
                States.Add(26);




        }
        public override void SendMeTo(Player Plr)
        {
            // Log.Success("Creature", "SendMe " + Name);


            //Log.Info("Creature", "npc = " + Name + "  Oid = " + Oid + "  X= " + Spawn.WorldX + "  Y= " + Spawn.WorldY + "  Z= " + Spawn.WorldZ);



            PacketOut Out = new PacketOut((byte)Opcodes.F_CREATE_MONSTER);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16(0);

            Out.WriteUInt16((UInt16)Heading);
            Out.WriteUInt16((UInt16)WorldPosition.Z);
            Out.WriteUInt32((UInt32)WorldPosition.X);
            Out.WriteUInt32((UInt32)WorldPosition.Y);
            Out.WriteUInt16(0); // Speed Z
            // 18


            if (Spawn.Proto.Model2 != 0)
                if (RandomMgr.Next(0, 100) < 50)
                    Out.WriteUInt16(Spawn.Proto.Model2);
                else
                    Out.WriteUInt16(Spawn.Proto.Model1);
            else
                Out.WriteUInt16(Spawn.Proto.Model1);

            Out.WriteByte((byte)RandomMgr.Next(Spawn.Proto.MinScale, Spawn.Proto.MaxScale));
            Out.WriteByte(Level);
            Out.WriteByte(Faction);

            Out.WriteByte(0);
            Out.WriteByte(Spawn.Val);
            Out.WriteUInt16(0);

            Out.WriteByte(Spawn.Emote);
            Out.WriteByte(0); // ?
            Out.WriteUInt16(Spawn.Proto._Unks[1]);
            Out.WriteByte(0);
            Out.WriteUInt16(Spawn.Proto._Unks[2]);
            Out.WriteUInt16(Spawn.Proto._Unks[3]);
            Out.WriteUInt16(Spawn.Proto._Unks[4]);
            Out.WriteUInt16(Spawn.Proto._Unks[5]);
            Out.WriteUInt16(Spawn.Proto._Unks[6]);
            Out.WriteUInt16(Spawn.Title);

            long TempPos = Out.Position;
            byte TempLen = (byte)(Spawn.bBytes.Length + States.Count);
            Out.WriteByte(TempLen);
            Out.Write(Spawn.bBytes, 0, Spawn.bBytes.Length);
            Out.Write(States.ToArray(), 0, States.Count);
            if (QtsInterface.CreatureHasStartQuest(Plr))
            {
                Out.WriteByte(5);
                Out.Position = TempPos;
                Out.WriteByte((byte)(TempLen + 1));
            }
            else if (QtsInterface.CreatureHasQuestToAchieve(Plr))
            {
                Out.WriteByte(4);
                Out.Position = TempPos;
                Out.WriteByte((byte)(TempLen + 1));
            }
            else if (QtsInterface.CreatureHasQuestToComplete(Plr))
            {
                Out.WriteByte(7);
                Out.Position = TempPos;
                Out.WriteByte((byte)(TempLen + 1));
            }

            Out.Position = Out.Length;

            Out.WriteByte(0);

            Out.WriteStringBytes(Name);



            
            Out.WriteByte(Spawn.D1);
            Out.WriteByte(Spawn.D2);

            Out.WriteUInt16(0); //00 00
            Out.WriteUInt16(0);// 00 00

            Out.WriteByte(Spawn.Unk1);

            Out.WriteByte(0);
            Out.WriteByte(48);
            Out.WriteByte(0x01);
            Out.WriteByte(0x0A);

            Out.WriteUInt16(0);
            Out.WriteByte(0);
        

            Out.WriteByte(Spawn.Icone);
            Out.WriteByte((byte)Spawn.Proto._Unks[0]);
            Out.WriteByte(0);


            Out.WriteUInt16(Oid);
            Out.WriteUInt32((UInt32)Spawn.Flag);
            Out.WriteUInt16((UInt16)Spawn.WorldZ);

            Out.WriteByte(100);

            Out.WriteUInt16(Spawn.ZoneId);

            Out.WriteUInt32(0);
            Out.WriteByte(Spawn.Head0);
            
            Out.WriteUInt16R((UInt16)Heading);


            Out.WriteUInt16(Oid);
            Out.WriteByte(0);
            Out.WriteByte(7);

            Plr.SendPacket(Out);
  





            
            Out = new PacketOut((byte)Opcodes.F_PLAY_SOUND);
            Out.WriteByte(0);
            Out.WriteUInt32((UInt32)Spawn.Sound1);
            Out.WriteUInt32((UInt32)Spawn.Flag);
            Out.WriteUInt16((UInt16)Spawn.WorldZ);
            Out.WriteUInt16(0);
            Plr.SendPacket(Out);

           
            Out = new PacketOut((byte)Opcodes.F_PLAY_SOUND);
            Out.WriteByte(0);
            Out.WriteUInt32((UInt32)Spawn.Sound2);
            Out.WriteUInt32((UInt32)Spawn.Flag);
            Out.WriteUInt16((UInt16)Spawn.WorldZ);
            Out.WriteUInt16(0);
            Plr.SendPacket(Out);


       

            base.SendMeTo(Plr);



        }
        public override void SendInteract(Player Plr, InteractMenu Menu)
        {
            Log.Success("SendInteract", "" + Name + " -> " + Plr.Name + ",Type=" + InteractType);



            Plr.QtsInterface.HandleEvent(Objective_Type.QUEST_SPEACK_TO, Spawn.Entry, 1);

            if (!IsDead)
            {
                // perhaps do some checks?
                if (Menu.Menu == 7) // Trainer Spells
                {
                    PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                    Out.WriteByte(5);
                    Out.WriteByte(0x0F);
                    Out.WriteByte(6);
                    Out.WriteUInt16(0);
                    Plr.SendPacket(Out);
                }
                else if (Menu.Menu == 9) // List items for sale
                    WorldMgr.SendVendor(Plr, Spawn.Entry);
                else if (Menu.Menu == 11) // Buy an item
                    WorldMgr.BuyItemVendor(Plr, Menu, Spawn.Entry);
                else if (Menu.Menu == 14) // Sells an Item
                    Plr.ItmInterface.SellItem(Menu);
                else if (Menu.Menu == 25) // Set rally point
                {
                    RallyPoint Rally = WorldMgr.GetRallyPointFromNPC(Entry);
                    if (Rally != null)
                    {
                        Plr._Value.RallyPoint = Rally.Id;

                        PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                        Out.WriteByte(0x12);
                        Out.WriteUInt16(Menu.Oid);
                        Out.WriteUInt16(Plr._Value.RallyPoint);
                        Plr.SendPacket(Out);
                    }
                    else
                        Plr.SendLocalizeString("ERROR: Unknown Rally Point NPC (" + Entry + ").", GameData.Localized_text.CHAT_TAG_DEFAULT);
                }
                else if (Menu.Menu == 36) // Buy back item
                    Plr.ItmInterface.BuyBackItem(Menu);
                else if (Menu.Menu == 37) // Dye menu
                {
                    byte MAX_DYES = 30;

                    PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                    Out.WriteByte(0x1B);

                    List<Dye_Info> Dyes = WorldMgr.GetDyes();
                    byte count = (byte)Math.Min(Dyes.Count, MAX_DYES);

                    Out.WriteByte(count);
                    for (byte i = 0; i < count; i++)
                    {
                        Out.WriteByte(i);
                        Out.WriteUInt16(Dyes[i].Entry);
                        Out.WriteUInt32(Dyes[i].Price);
                    }

                    Plr.SendPacket(Out);
                }
                else if (Menu.Menu == 38) // Dye one item
                {
                    Item item = Plr.ItmInterface.GetItemInSlot(Menu.Num);

                    if (item == null)
                        return;

                    byte PrimaryDye = Menu.Packet.GetUint8();
                    byte SecondaryDye = Menu.Packet.GetUint8();
                    ushort PrimaryDyeId = 0;
                    ushort SecondaryDyeId = 0;

                    uint cost = 0;
                    // 255 = no dye selected
                    if (PrimaryDye != 255)
                        cost += WorldMgr.GetDyes()[PrimaryDye].Price;

                    // 255 = no dye selected
                    if (SecondaryDye != 255)
                        cost += WorldMgr.GetDyes()[SecondaryDye].Price;

                    if (!Plr.RemoveMoney(cost))
                    {
                        Plr.SendLocalizeString("", GameData.Localized_text.TEXT_AUCTION_NOT_ENOUGH_MONEY);
                        return;
                    }

                    // 255 = no dye selected
                    if (PrimaryDye != 255)
                        PrimaryDyeId = WorldMgr.GetDyes()[PrimaryDye].Entry;

                    // 255 = no dye selected
                    if (SecondaryDye != 255)
                        SecondaryDyeId = WorldMgr.GetDyes()[SecondaryDye].Entry;

                    Plr.ItmInterface.DyeItem(item, PrimaryDyeId, SecondaryDyeId);

                    if (Plr._IsActive && Plr.IsInWorld() && Plr._Loaded)
                    {
                        foreach (Player P in _PlayerRanged)
                        {
                            if (P.HasInRange(Plr))
                                Plr.ItmInterface.SendEquiped(P);
                        }
                    }
                }
                else if (Menu.Menu == 39) // Dye all items
                {
                    byte Count = 0;
                    for (UInt16 i = 0; i < ItemsInterface.MAX_EQUIPED_SLOT; ++i)
                        if (Plr.ItmInterface.Items[i] != null) // && is dyable
                            ++Count;

                    byte PrimaryDye = Menu.Packet.GetUint8();
                    byte SecondaryDye = Menu.Packet.GetUint8();
                    ushort PrimaryDyeId = 0;
                    ushort SecondaryDyeId = 0;

                    uint cost = 0;
                    // 255 = no dye selected
                    if (PrimaryDye != 255)
                        cost += WorldMgr.GetDyes()[PrimaryDye].Price * Count;

                    // 255 = no dye selected
                    if (SecondaryDye != 255)
                        cost += WorldMgr.GetDyes()[SecondaryDye].Price * Count;

                    if (!Plr.RemoveMoney(cost))
                    {
                        Plr.SendLocalizeString("", GameData.Localized_text.TEXT_AUCTION_NOT_ENOUGH_MONEY);
                        return;
                    }

                    // 255 = no dye selected
                    if (PrimaryDye != 255)
                        PrimaryDyeId = WorldMgr.GetDyes()[PrimaryDye].Entry;

                    // 255 = no dye selected
                    if (SecondaryDye != 255)
                        SecondaryDyeId = WorldMgr.GetDyes()[SecondaryDye].Entry;

                    for (UInt16 i = 0; i < ItemsInterface.MAX_EQUIPED_SLOT; ++i)
                        if (Plr.ItmInterface.Items[i] != null) // && is dyable
                            Plr.ItmInterface.DyeItem(Plr.ItmInterface.Items[i], PrimaryDyeId, SecondaryDyeId);

                    if (Plr._IsActive && Plr.IsInWorld() && Plr._Loaded)
                    {
                        foreach (Player P in _PlayerRanged)
                        {
                            if (P.HasInRange(Plr))
                                Plr.ItmInterface.SendEquiped(P);
                        }
                    }
                }
                else
                {
                    switch (InteractType)
                    {

                        case GameData.InteractType.INTERACTTYPE_FLIGHT_MASTER:
                            {
                                byte[] data = new byte[62]
		                    {
			                    0x01,0xF4,0x00,0x00,0x00,0x00,0x00,0x00,0x64,0x42,0x39,0x00,0x00,0x00,0xC0,0xE3,
			                    0x03,0x39,0xA0,0xD1,0x6F,0x00,0xC8,0xA8,0x1D,0x37,0x28,0x94,0x79,0x33,0xB2,0x24,
			                    0x32,0x44,0xDB,0xD7,0x1C,0x5D,0x18,0x5D,0xDD,0x1C,0xA4,0x0D,0x00,0x00,0xA8,0x6B,
			                    0x21,0x36,0x11,0x00,0x00,0x00,0xC8,0xD0,0xAF,0x3A,0x78,0xD1,0x6F,0x00
		                        };

                                UInt16 Counts = 1;

                                PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                                Out.WriteByte(0x0A);
                                List<Zone_Taxi> Taxis = WorldMgr.GetTaxis(Plr);
                                Out.WriteByte((byte)Taxis.Count);
                                foreach (Zone_Taxi Taxi in Taxis)
                                {
                                    Out.WriteUInt16(Counts);
                                    Out.WriteByte((byte)Taxi.Info.Pairing);
                                    Out.WriteUInt16(Taxi.Info.Price);
                                    Out.WriteUInt16(Taxi.Info.ZoneId);
                                    Out.WriteByte(1);
                                    ++Counts;
                                }
                                Out.Write(data);
                                Plr.SendPacket(Out);
                            } break;
                        case GameData.InteractType.INTERACTTYPE_BANKER:
                            {
                                PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                                Out.WriteByte(0x1D);
                                Out.WriteByte(0);// need to find bank buy box



                                /*
                                Out = new PacketOut((byte)Opcodes.F_BAG_INFO);
                                Out.WriteByte(0x0F);
                                Out.WriteByte(ItmInterface.GetTotalSlot()); // Number of available slots  // GetTotalSlot
                                Out.WriteUInt16((UInt16)ItemsInterface.INVENTORY_SLOT_COUNT);
                                Out.WriteByte(0);
                                Out.WriteUInt32R(ItmInterface.GetBagPrice());//GetBagPrice

                                Out.WriteUInt16(2);// was 2
                                Out.WriteByte(0x50);// was 0x50
                                Out.WriteUInt16(0x08);// 0x08
                                Out.WriteUInt16(0x60);// 0x60
                                Out.WriteByte(0xEA);// 0xEA
                                Out.WriteUInt16(0);
                                */




                                Plr.SendPacket(Out);








                            } break;


                        case GameData.InteractType.INTERACTTYPE_SIEGEWEAP:
                            {
                                //     /*
                                PacketOut Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);

                                Out.WriteUInt16(Oid);
                                Out.WriteByte(0x1D);
                                Out.WriteByte(1);
                                Out.WriteByte(1);
                                Out.WriteByte(2);
                                Out.WriteUInt16(0);
                                Out.WriteByte(0x02);
                                Out.WriteByte(0x4A);
                                Out.WriteUInt16(0);
                                Plr.SendPacket(Out);
                                //21 D9 1D 01 01 02 00 00 02 4A 00 00



                                Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                                Out.WriteByte(0x18);
                                Out.WriteByte(1);
                                Out.WriteUInt16(0x005C);
                                Out.WritePascalString(Name);
                                Out.WriteByte(1);
                                Out.WriteByte(0x3F);
                                Out.WriteUInt16(0);
                                Out.WriteByte(0x0E);
                                //Out.WriteByte(0xA6);//(byte)Zone.ZoneId);//zone
                                Out.WriteHexStringBytes("A6103C000400011E0000000002");
                                Out.WriteByte(0);// reload?
                                Out.WriteUInt16(0x0B);
                                Out.WriteByte(0);


                                //  Out.WriteHexStringBytes("1801005C0D456D706972652043616E6E6F6E013F00000EA6103C000400011E000000000200000B00");

                                Plr.SendPacket(Out);



                                Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
                                Out.WriteUInt16(Oid);
                                Out.WriteByte(0x21);
                                Out.WriteByte(0x3C);// 3c=60 seconds 6000 14=20 seconds=2000
                                Out.Fill(0, 6);
                                Plr.SendPacket(Out);



                                Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
                                Out.WriteUInt16(Oid);
                                Out.WriteByte(0x1C);
                                Out.WriteByte(0x3);//14=20 seconds=2000
                                //Out.Fill(0, 6);
                                Plr.SendPacket(Out);


                                // 1D 01 01 02 00 00 8A DF 00 00

                                Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
                                Out.WriteUInt16(Oid);
                                Out.WriteByte(0x1D);
                                Out.WriteByte(1);
                                Out.WriteByte(1);
                                Out.WriteByte(2);
                                Out.WriteUInt16(0);
                                Out.WriteByte(0x8A);
                                Out.WriteByte(0xDF);
                                Out.WriteUInt16(0);
                                Plr.SendPacket(Out);


                                //                          */




                                /*

                                    // controll timer // size 13
                                     Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
                                     Out.WriteUInt16(Oid);
                                     Out.WriteByte(0x21);
                                     Out.WriteByte(0x3C);// 3c=60 seconds 6000 14=20 seconds=2000
                                     Out.Fill(0, 6);
                                    Plr.SendPacket(Out);

                                    // size 7
                                    Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
                                    Out.WriteUInt16(Oid);
                                    Out.WriteByte(0x1C);
                                    Out.WriteByte(0x3);//14=20 seconds=2000
                                    //Out.Fill(0, 6);
                                    Plr.SendPacket(Out);



    */






                                /*
                                //size 15
                                Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
                                Out.WriteUInt16(Oid);
                                Out.WriteByte(0x1D);
                                Out.WriteByte(1);
                                Out.WriteByte(1);
                                Out.WriteByte(2);
                                Out.WriteUInt16(0);
                                Out.WriteByte(2);
                                Out.WriteByte(0x4A);
                                Out.WriteUInt16(0);

                                //  Out.WriteByte(0x3);//14=20 seconds=2000
                                //Out.Fill(0, 6);
                                Plr.SendPacket(Out);
                                */




                                /*
                                // target icon
                                Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
                                Out.WriteUInt16(Oid);
                                Out.WriteByte(0x1C);
                                Out.WriteByte(0x3);//14=20 seconds=2000
                                Out.Fill(0, 6);
                                Plr.SendPacket(Out);
                           // */




                            } break;




                        case GameData.InteractType.INTERACTTYPE_GUILD_VAULT:
                            {


                                //Size = 43
                                // opens guild vault 5 slots
                                PacketOut Out = new PacketOut((byte)Opcodes.F_GUILD_DATA);
                                Out.WriteByte(0x18); // ??
                                //Out.WriteByte(0x1);// this shows a buy box
                                Out.WriteByte(5);// number of vault boxs  0 to 5
                                Out.Fill(0, 8);

                                // VAULT BOX 1
                                Out.WriteUInt16(0x003C);
                                //Out.WriteByte(0x3C);
                                Out.Fill(0, 4);

                                // VAULT BOX 2
                                Out.WriteUInt16(0x003C);
                                //Out.WriteByte(0x3C);
                                Out.Fill(0, 4);

                                // VAULT BOX 3
                                Out.WriteUInt16(0x003C);
                                //Out.WriteByte(0x3C);
                                Out.Fill(0, 4);

                                // VAULT BOX 4
                                Out.WriteUInt16(0x003C);
                                //Out.WriteByte(0x3C);
                                Out.Fill(0, 4);

                                // VAULT BOX 5
                                Out.WriteUInt16(0x003C);
                                //Out.WriteByte(0x3C);
                                Out.Fill(0, 4);

                                Plr.SendPacket(Out);
                            } break;



                        //Heal Penalties structure i added
                        case GameData.InteractType.INTERACTTYPE_HEALER:
                            {  // packet size 9  -3=6
                                PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                                Out.WriteByte(0x13);
                                Out.WriteByte(1);// this the penalty points u have
                                Out.WriteUInt16(0);
                                // Out.WriteByte(0);// gold ?
                                //Out.WriteByte(0);// gold
                                Out.WriteByte(1);//1 silver
                                Out.WriteByte(0);// 90??// brass
                                Plr.SendPacket(Out);

                                // this packet heals size 6 -3 =3 size
                                Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                                Out.WriteByte(0x14);
                                Out.WriteByte(1);// this the penalty points u have
                                Out.WriteByte(90);// 90??// brass
                                Plr.SendPacket(Out);



                            } break;
                        //10 00 03 2E 49 01 00 00 00 00
                        case GameData.InteractType.INTERACTTYPE_BARBERSHOP:
                            {  // packet size 13 -3=10
                                PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                                Out.WriteByte(0x10);
                                Out.WriteByte(0);
                                Out.WriteByte(3);// 3
                                Out.WriteByte(0x2E);//2E
                                Out.WriteByte(0x49);//49
                                Out.WriteByte(1);// 1 // this is tokens that is required if set to 5 it says=(you dont have enough tokens in your inventory 5 token are required
                                Out.WriteUInt32(0);
                                Plr.SendPacket(Out);




                            } break;

                        //0E 01 01 00
                        case GameData.InteractType.INTERACTTYPE_GUILD_REGISTRAR:
                            {


                                // packet size 13 -3=10
                                PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                                Out.WriteByte(0x0E);
                                Out.WriteByte(1);//0= sucess full /1=
                                Out.WriteByte(1);//0= sucess full /1=
                                Out.WriteByte(0);//
                                Plr.SendPacket(Out);


                            } break;


                        case GameData.InteractType.INTERACTTYPE_AUCTIONEER://INTERACTTYPE_AUCTIONEER = 13,
                            {
                                PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                                Out.WriteByte(0x1A);
                                Out.WriteByte(0);
                                Plr.SendPacket(Out);


                                Out = new PacketOut((byte)Opcodes.F_AUCTION_SEARCH_RESULT);
                                Out.WritePacketString(@"|00 00 9E 22 00 01 01 00 00 00 00 00 71 |..............q|
|67 F1 00 01 82 52 00 00 00 00 00 00 27 10 03 43 |g....R......'..C|
|68 6F 73 73 65 74 74 65 5E 4D 00 00 00 00 00 00 |hossette^M......|
|00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 |................|
|00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 FF |................|
|5F 39 00 00 01 4B B6 0F 30 00 00 00 00 00 00 00 |_9...K..0.......|
|00 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 |................|
|00 00 00 00 00 00 06 00 14 00 14 00 00 00 00 00 |................|
|00 00 00 00 00 00 00 10 57 69 6C 74 65 64 20 57 |........Wilted W|
|69 6C 64 20 57 65 65 64 00 00 00 06 08 00 05 07 |ild Weed........|
|00 04 01 00 00 02 00 00 04 00 00 0F 00 01 00 00 |................|
|00 00 01 00 00 03 02 00 08 00 00 00 00 00 00 00 |................|
|00 00 00 00 00 00 00 00 00 00 00 00 00          |.............   |");
                                Plr.DispatchPacket(Out, true);


                                //  Out = new PacketOut((byte)Opcodes.F_AUCTION_BID_STATUS);
                            } break;

                        /////////added////////////////////////////////////////////////////////////////////////////////////////
                        // case GameData.InteractType.INTERACTTYPE_SIEGEWEAP:
                        //  {
                        //   PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                        //  Out.WriteByte(0x1A);
                        //  Out.WriteByte(0);
                        //  Plr.SendPacket(Out);
                        //  } break;
                        default:
                            {
                                ushort MenuItems = 0;
                                string Text = WorldMgr.GetCreatureText(Spawn.Entry);

                                if (InteractType == GameData.InteractType.INTERACTTYPE_DYEMERCHANT)
                                {

                                    MenuItems += 2; // Shop // 2
                                    MenuItems += 16384; // Dyes

                                    // You need Text to see the 'dyes' option
                                    if (Text == String.Empty)
                                        Text = "Selling a bit of everything here, come on have a look see!";


                                    //Text = "Forget about the drut he's selling; take a look at these beauties!"; ORIGNAL IN DB


                                    //Text = "Come and see what I have.";
                                }

                                bool HasQuests = QtsInterface.HasQuestInteract(Plr, this);
                                if (HasQuests)
                                    MenuItems += 64; // Quests




                                if (InteractType == GameData.InteractType.INTERACTTYPE_GUILD_REGISTRAR)
                                {
                                    MenuItems += 128; // Guild Register =128
                                    // Guild Regiser needs text
                                    if (Text == String.Empty)
                                        Text = "Let's get started. To form a guild, you'll need to have a full group of six adventurers with you. None of you can belong to another guild. For a modest fee of only fifty silver I can create your guild.";
                                }


                                if (InteractType == GameData.InteractType.INTERACTTYPE_TRAINER)
                                {
                                    MenuItems += 1; // Trainer =1
                                    // Theese were previously in there, nice to keep them unless theres info in creature_texts
                                    if (Text == String.Empty)
                                        if (Plr.Realm == GameData.Realms.REALMS_REALM_ORDER)
                                            Text = "Hail defender of the Empire!  Your performance in battle is the only thing that keeps the hordes of Chaos at bay. Let's begin your training at once!";


                                        else
                                            Text = "Learn these lessons well, for gaining the favor of the Raven god should be of utmost importance to you. Otherwise... There is always room for more Spawn within our ranks.";

                                }

                                if (InteractType == GameData.InteractType.INTERACTTYPE_BINDER)
                                    MenuItems += 228; // Rally Point =256/ 228 is rally quest+influnce

                                if (Text.Length > 0)
                                    MenuItems += 32; // Text
                                // VENDORS INVENTERY BOX PACKET
                                PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);

                                Log.Info("Creature", "npc = " + Name + "  Oid = " + Oid + " MenuItems = " + MenuItems + "  X= " + Spawn.WorldX + "  Y= " + Spawn.WorldY + "  Z= " + Spawn.WorldZ);
                                Out.WriteByte(0);
                                Out.WriteUInt16(Menu.Oid);
                                Out.WriteUInt16(0);
                                Out.WriteUInt16(MenuItems);
                                if (HasQuests)
                                    QtsInterface.BuildInteract(Plr, this, Out);
                                if (Text.Length > 0)

                                    Out.WriteByte(0);// added and fixed the dye screen error, was missing this byte now working whoohooo
                                Out.WriteShortString(Text);// need to fix this ??
                                //Out.WritePascalString(Text);// added this because above gives error
                                Out.WriteByte(0);// added this is on live server mite not need this byte
                                Plr.SendPacket(Out);
                            } break;
                    }
                }
            }
            base.SendInteract(Plr, Menu);
        }


        /*
             // Group LOOT : Pass , accept, cancel
              Out.WritePacketString(@"|07 19 0A 00 00 00 00 03 2E 56 22 B9 00 |............V..|
    |00 00 00 00 00 00 00 00 24 00 00 00 00 00 01 00 |........$.......|
    |00 00 00 00 00 00 00 00 00 00 00 00 00 09 C4 00 |................|
    |01 00 00 00 00 00 00 00 00 00 00 00 00 09 57 61 |..............Wa|
    |72 20 43 72 65 73 74 00 00 00 00 00 00 71 50 72 |r Crest......qPr|
    |6F 6F 66 20 6F 66 20 79 6F 75 72 20 76 61 6C 6F |oof of your valo|
    |72 20 6F 6E 20 74 68 65 20 66 69 65 6C 64 20 6F |r on the field o|
    |66 20 62 61 74 74 6C 65 2E 20 54 68 65 73 65 20 |f battle. These |
    |6D 61 79 20 62 65 20 75 73 65 64 20 74 6F 20 74 |may be used to t|
    |72 61 64 65 20 66 6F 72 20 65 71 75 69 70 6D 65 |rade for equipme|
    |6E 74 20 66 72 6F 6D 20 76 61 72 69 6F 75 73 20 |nt from various |
    |51 75 61 72 74 65 72 6D 61 73 74 65 72 73 2E 01 |Quartermasters..|
    |00 00 00 03 06 00 08 00 00 00 00 00 00 00 00 00 |................|
    |00 00 00 00 00 00 00 00 00 00                   |..........      |");
        */


   


        public override void SetDeath(Unit Killer)
        {
            if (Killer.IsPlayer())
            {
                Player Plr = Killer.GetPlayer();

                PQuestObject pq = Plr.QtsInterface.GetPublicQuest();
                if (pq != null)
                {
                    pq.HandleEvent(Objective_Type.QUEST_KILL_MOB, Spawn.Entry, 1);
                }

            }

            Killer.QtsInterface.HandleEvent(Objective_Type.QUEST_KILL_MOB, Spawn.Entry, 1);
            base.SetDeath(Killer);

            if (Spawn.Entry == 28432 || Spawn.Entry == 34357 || Spawn.Entry == 5780)     // Lair Bosses 2 hour spawn time
                EvtInterface.AddEvent(RezUnit, (7200000 + RandomMgr.Next(0, 3600000)), 1); // 2 hours seconds Rez
            else
                EvtInterface.AddEvent(RezUnit, 50000 + Level * 1000, 1); // 30 seconds Rez


        }

        public override void RezUnit()
        {
            Region.CreateCreature(Spawn);
            Dispose();
        }

        public override string ToString()
        {
            return "SpawnId=" + Spawn.Guid + ",Entry=" + Spawn.Entry + ",Name=" + Name + ",Level=" + Level + ",Rank=" + Rank + ",Max Health=" + MaxHealth + ",Faction=" + Faction + ",Emote=" + Spawn.Emote + "AI:" + AiInterface.State + ",Position :" + base.ToString();
        }
    }
}
