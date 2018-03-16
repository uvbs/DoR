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
    public class Unit : Object
    {
        #region Static

        static public int HEALTH_REGEN_TIME = 4000; // 4seconds
        static public int ACTION_REGEN_TIME = 1000; // 1second
        static public int STATE_INTERVAL = 10000; // 10seconds

        static public GameData.InteractType GenerateInteractType(UInt16 Title)
        {
            GameData.InteractType Type = GameData.InteractType.INTERACTTYPE_IDLE_CHAT;

            switch (Title)
            {
                //////// TRAINERS//////////
                case 1: goto case 40; // 1 = Trainer
                case 2: goto case 40; // 2 = Career Trainer
                case 3: goto case 40;// 3 Renown Trainer
                case 40:// 40 = Apprentice Renown Trainer
                    Type = GameData.InteractType.INTERACTTYPE_TRAINER;
                    break;

                ///////// FLIGHT ZONE TAXIS////////
                case 18: goto case 131;// 18 = Flight Master
                case 131:// 131 = Expedition Quartermaster 
                    Type = GameData.InteractType.INTERACTTYPE_FLIGHT_MASTER;
                    break;

                ///////MERCHANTS //////////
                case 10: goto case 135;// 10 = Merchant
                case 11: goto case 135;// 11 = Armor Merchant
                case 12: goto case 135;// 12 = Weapon Merchant
                case 13: goto case 135;// 13 = Camp Merchant
                case 14: goto case 135;// 14 = Siege Weapon Merchant
                case 15: goto case 135;// 15 = Craft Supply Merchant
                case 16: goto case 135;// 16 =  Renown Gear Merchant
                // 135 = Talisman Merchant

                case 17:// 17 = Rally Master
                    //Type = GameData.InteractType.INTERACTTYPE_INFLUENCE;// missing buy selection on invertery boxs 6 gold /8/9/11/13
                    Type = GameData.InteractType.INTERACTTYPE_BINDER;//INTERACTTYPE_BINDER;? missing buy selection on invertery boxs 6 gold /8/9/11/13
                    break;


                case 22:// 22 = Banker
                    Type = GameData.InteractType.INTERACTTYPE_BANKER;
                    break;


                case 23:// 23 =	Auctioneer
                    Type = GameData.InteractType.INTERACTTYPE_AUCTIONEER;//need to fix add items to auctioneer and selling items
                    break;

                case 37:// 37 = Vault Keeper
                    Type = GameData.InteractType.INTERACTTYPE_GUILD_VAULT;
                    break;



                case 19:// 19 = Guild Registrar
                    Type = GameData.InteractType.INTERACTTYPE_GUILD_REGISTRAR;// need to add cost to code
                    break;



                case 400:// SIEGEWEAP  ADDED BUT PROBLY WRONG
                    Type = GameData.InteractType.INTERACTTYPE_SIEGEWEAP;
                    break;




                case 20:// 20 = Healer
                    Type = GameData.InteractType.INTERACTTYPE_HEALER;//need to check if it heals
                    break;

                case 160:// 160 = Barber Surgeon
                    Type = GameData.InteractType.INTERACTTYPE_BARBERSHOP;// need to fix the checkout 
                    break;


                case 165:// 165 = Fortress General
                    Type = GameData.InteractType.INTERACTTYPE_FORTRESS;// dont know which npc
                    break;

                // do we need to change title  135 to 150 ?

                case 31: goto case 135;// 31 =	Mount Vendor
                case 34: goto case 135;// 34 = Quartermaster
                case 115: goto case 135;// 115 = Tome Trophy Librarian
                case 116: goto case 135;// 116 = Basic Renown Gear Merchant
                case 117: goto case 135;// 117 = Veteran Renown Gear Merchant
                case 118: goto case 135;// 118 = Advanced Renown Gear Merchant
                case 119: goto case 135;// 119 = Elite Renown Gear Merchant
                case 120: goto case 135;// 120 = Upgrade Merchant
                case 121: goto case 135;// 121 = Door Repair Merchant
                case 122: goto case 135;// 122 = Standard Merchant
                case 125: goto case 135;// 125 = Siege Quartermaster
                case 126: goto case 135;// 126 = Recruit's Medallion Quartermaster
                case 127: goto case 135;// 127 = Scout's Medallion Quartermaster
                case 128: goto case 135;// 128 = Soldier's Medallion Quartermaster
                case 129: goto case 135;// 129 = Officer's Medallion Quartermaster
                case 130: goto case 135;// 130 = Royal Quartermaster
                case 139: goto case 135;// 139 = Recruit's Emblem Quartermaster
                case 148: goto case 135;// 148 = Greater Talisman Merchant
                case 149: goto case 135;// 149 = Superior Talisman Merchant
                case 150: goto case 135;// 150 = Potent Talisman Merchant

                case 152: goto case 135;// 152 = Light Mount Vendor
                case 153: goto case 135;// 153 = Heavy Mount Vendor
                case 154: goto case 135;// 154 = Specialty Mount Wrangler
                case 155: goto case 135;// 155 = Renown Armor Quartermaster

                case 135:// 135 = Talisman Merchant
                    Type = GameData.InteractType.INTERACTTYPE_DYEMERCHANT;
                    break;


                case 144: goto case 40;// 144 = Apprentice Career Trainer // 40 = Apprentice renown trainer
                case 157: goto case 135;// 157 = Companion Keeper// 135 = Talisman Merchant
                case 158: goto case 135;// 158 = Novelty Vendor
                case 168: goto case 135; //added claud specal vendor
                case 167: goto case 135;// added Emblem Exchange/Medallion Exchange vendors
                case 151: goto case 135;// 151 = Specialized Armorsmith
                case 156: goto case 135;// 156 = Renown Weapon Quartermaster
            };

            return Type;
        }

        #endregion

        public List<byte> States = new List<byte>();
        public GameData.InteractType InteractType = GameData.InteractType.INTERACTTYPE_IDLE_CHAT;

        public Point3D SpawnPoint = new Point3D(0, 0, 0);
        public UInt16 SpawnHeading = 0;
        public bool StateDirty = false;
        public bool IsInvinsible = false;

        public Unit()
            : base()
        {
            ItmInterface = AddInterface<ItemsInterface>();
            CbtInterface = AddInterface<CombatInterface>();
            StsInterface = AddInterface<StatsInterface>();
            QtsInterface = AddInterface<QuestsInterface>();
            MvtInterface = AddInterface<MovementInterface>();
            AbtInterface = AddInterface<AbilityInterface>();
            AiInterface = AddInterface<AIInterface>();
        }

        public override void OnLoad()
        {
            SpawnPoint.X = X;
            SpawnPoint.Y = Y;
            SpawnPoint.Z = Z;
            SpawnHeading = Heading;
            base.OnLoad();
            StateDirty = false;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
        public override void Update(long Tick)
        {
            if (!IsDead)
            {
                UpdateHealth(Tick);
                UpdateActionPoints(Tick);
                UpdateSpeed(Tick);
            }

            if (NextSend < Tick)
            {
                NextSend = Tick + STATE_INTERVAL;
                StateDirty = true;
            }

            base.Update(Tick);

            if (StateDirty)
            {
                NextSend = Tick + STATE_INTERVAL;
                StateDirty = false;
                SendState(null);
            }
        }

        #region Sender

        private long NextSend = 0;

        public override void SendMeTo(Player Plr)
        {
            ItmInterface.SendEquiped(Plr);
            SendState(Plr);
            MvtInterface.CurrentMount.SendMount(Plr);
            base.SendMeTo(Plr);
        }
        public virtual void SendState(Player Plr, ushort TargetX, ushort TargetY, ushort TargetZ, byte MovementType)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_OBJECT_STATE);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16((ushort)X);
            Out.WriteUInt16((ushort)Y);
            Out.WriteUInt16((ushort)Z);
            Out.WriteByte(PctHealth);
            Out.WriteByte(MovementType); // Movement Type 0 None, 1 Walk, 2 ?, 3 Run
            Out.WriteByte((byte)Zone.ZoneId);
            Out.Fill(0, 4);

            if (MovementType < 3)
                Out.WriteUInt16(0x55);
            else
                Out.WriteUInt16(0XEB); // ? 0XEB

            Out.WriteByte(0);
            Out.WriteUInt16(0x75); // ? 0xCE

            Out.WriteUInt16(TargetX);
            Out.WriteUInt16(TargetY);
            Out.WriteByte(0);
            Out.WriteByte((byte)Zone.ZoneId);
            Out.WriteUInt16(0x5300);

            //Log.Dump("Test", Out, true);

            //Out.WriteUInt16(7018);

            if (Plr == null)
                DispatchPacket(Out, false);
            else
                Plr.SendPacket(Out);
        }

        public virtual void SendState(Player Plr)
        {
            if (!IsInWorld())
                return;

            if (MvtInterface.CurrentSpeed != 0 && !IsPlayer())
            {
                SendState(Plr, (ushort)MvtInterface.TargetPosition.X, (ushort)MvtInterface.TargetPosition.Y, (ushort)MvtInterface.TargetPosition.Z, (byte)(MvtInterface.CurrentSpeed >= 50 ? 3 : 1));
            }
            else
            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_OBJECT_STATE);
                Out.WriteUInt16(Oid);
                Out.WriteUInt16((ushort)X);
                Out.WriteUInt16((ushort)Y);
                Out.WriteUInt16((ushort)Z);
                Out.WriteByte(PctHealth);
                Out.WriteByte(0); // Movement Type 0 None, 1 Walk, 2 ?, 3 Run
                Out.WriteByte((byte)Zone.ZoneId);
                Out.Fill(0, 5);
                Out.WriteUInt16R(Heading);

                if (Plr == null)
                    DispatchPacket(Out, false);
                else
                    Plr.SendPacket(Out);
            }

            if (MvtInterface.CurrentSpeed != 0)
                SendAnimation();


        }

        public virtual void SendAnimation()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_ANIMATION);
            Out.WriteUInt16(Oid);
            Out.WriteUInt32(0);
            DispatchPacket(Out, false);
        }









        #endregion

        #region Interfaces

        public MovementInterface MvtInterface;
        public ItemsInterface ItmInterface;
        public CombatInterface CbtInterface;
        public StatsInterface StsInterface;
        public QuestsInterface QtsInterface;
        public AbilityInterface AbtInterface;
        public AIInterface AiInterface;

        #endregion

        #region Health&&Damage

        public uint _Health = 0;
        public uint MaxHealth = 0;
        public uint BonusHealth = 0;

        public uint Health
        {
            get { return _Health; }
            set
            {
                _Health = value;
                if (IsPlayer())
                    GetPlayer().SendHealth();
                else
                    StateDirty = true;
            }
        }

        public UInt16 _ActionPoints = 0;
        public UInt16 ActionPoints
        {
            get
            {
                return _ActionPoints;
            }
            set
            {
                if (value > MaxActionPoints)
                    value = MaxActionPoints;

                if (_ActionPoints != value)
                {
                    _ActionPoints = value;

                    if (IsPlayer())
                        GetPlayer().SendHealth();
                }
            }
        }
        public UInt16 MaxActionPoints = 250;
        public long NextHpRegen = 0;
        public long NextApRegen = 0;

        public void ResetNextActionPoints(long MSTime)
        {
            NextApRegen = TCPManager.GetTimeStampMS() + MSTime;
        }

        public uint TotalHealth { get { return MaxHealth + BonusHealth; } }
        public byte PctHealth { get { return (byte)((Health * 100) / TotalHealth); } }
        public byte PctAp { get { return (byte)((ActionPoints * 100) / MaxActionPoints); } }
        public bool IsDead
        {
            get
            {
                if (Health <= 0)
                    return true;

                return false;
            }
        }

        public void UpdateHealth(long Tick)
        {
            if (Tick >= NextHpRegen)
            {
                NextHpRegen = Tick + HEALTH_REGEN_TIME;

                if (CbtInterface.IsFighting())
                    return;

                if (Health < TotalHealth)
                {
                    uint Regen = TotalHealth / 8;

                    if (Health + Regen > TotalHealth)
                        Health = TotalHealth;
                    else
                        Health += Regen;
                }
            }
        }

        public void UpdateActionPoints(long Tick)
        {
            if (Tick >= NextApRegen)
            {
                if (AbtInterface.IsOnGlobalCooldown())
                {
                    NextApRegen = Tick + 300;
                    return;
                }

                NextApRegen = Tick + ACTION_REGEN_TIME;

                if (ActionPoints < MaxActionPoints)
                {
                    ActionPoints += 25;

                    if (ActionPoints > MaxActionPoints)
                        ActionPoints = MaxActionPoints;
                }
            }
        }

        public virtual void Strike(Unit Target, EquipSlot Slot = EquipSlot.MAIN_DROITE)
        {
            if (Target == null || Target.IsDead)
                return;

            float Damage = 0;
            float DmgReduce = 0;
            float RealDamage = 0;

            Damage = StsInterface.CalculDamage(Slot, Target);
            DmgReduce = Target.StsInterface.CalculReduce();
            DmgReduce = Math.Min(75, DmgReduce);

            RealDamage = (Damage - ((DmgReduce / 100f * DmgReduce)));
            if (RealDamage <= 2)
                RealDamage = 2;

            SendAttackMovement(Target);
            DealDamages(Target, null, (uint)RealDamage);
        }
        public void SendCastEffect(Unit Target, ushort AbilityEntry, GameData.CombatEvent Event, uint Count)
        {
            if (IsPlayer())
            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_CAST_PLAYER_EFFECT);
                Out.WriteUInt16(Oid);
                Out.WriteUInt16(Target.Oid);
                Out.WriteUInt16(AbilityEntry);
                Out.WriteByte(0);
                Out.WriteByte((byte)Event);
                Out.WriteByte(0x13);
                Out.WriteByte((byte)((128 + (Count % 64) * 2) + 1));
                Out.WriteByte((byte)(Count / 64));
                Out.WriteByte(0);
                DispatchGroup(Out);
            }

            if (Target != this && Target.IsPlayer())
            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_CAST_PLAYER_EFFECT);
                Out.WriteUInt16(Oid);
                Out.WriteUInt16(Target.Oid);
                Out.WriteUInt16(AbilityEntry);
                Out.WriteByte(0);
                Out.WriteByte((byte)Event);
                Out.WriteByte(0x13);
                Out.WriteByte((byte)((128 + (Count % 64) * 2) + 1));
                Out.WriteByte((byte)(Count / 64));
                Out.WriteByte(0);
                Target.DispatchGroup(Out);
            }
        }

        public void SendAttackMovement(Unit Target)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_USE_ABILITY);
            Out.WriteUInt16(0);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16(0);
            Out.WriteUInt16(Target.Oid);
            Out.WriteByte(2);
            Out.Fill(0, 9);
            DispatchPacket(Out, true);
        }

        /// <summary>
        /// Deal Damages but check abilities buff reductions (absorb, invinsibility, etc)
        /// </summary>
        public virtual void DealDamages(Unit Target, Ability Ab, uint Damages)
        {
            AbtInterface.OnDealDamages(Target, Ab, ref Damages);
            Target.AbtInterface.OnReceiveDamages(this, Ab, ref Damages);

            if (Ab != null) // Ability Damage
            {
                Ab.SendSpellDamage(Target, Damages, false);
                Ab.SendSpellEffect(this, Target, (ushort)Damages, (ushort)Damages, Ab.Info);
            }
            else // Weapon
            {
                SendCastEffect(Target, 0, GameData.CombatEvent.COMBATEVENT_HIT, Damages);
            }

            DealDamages(Target, (uint)Damages);
        }

        /// <summary>
        /// Deal direct damage , no calculations
        /// </summary>
        public virtual void DealDamages(Unit Target, uint Damages)
        {
            if (Target == null || Target.IsDead)
                return;

            CbtInterface.OnDealDamage(Target, Damages);
            Target.CbtInterface.OnTakeDamage(this, Damages);

            if (Target.IsInvinsible)
                return;

            if (IsCreature() && Target.IsCreature())
                return;

            if (Target.Health <= Damages)
            {
                Target.SetDeath(this);
                CbtInterface.OnTargetDie(Target);
            }
            else
            {
                Target.Health -= Damages;
            }
        }

        /// <summary>
        /// Deal Heal, check abilities buff (heal bonus , etc )
        /// </summary>
        public virtual void DealHeal(Unit Target, Ability Ab, uint Heal)
        {
            AbtInterface.OnDealHeals(Target, Ab, ref Heal);
            Target.AbtInterface.OnReceiveHeal(this, Ab, ref Heal);

            if (Ab != null)
            {
                Ab.SendSpellDamage(Target, Heal, true);
                Ab.SendSpellEffect(this, Target, (ushort)Heal, (ushort)Heal, Ab.Info);
            }

            DealHeal(Target, (uint)Heal);
        }

        /// <summary>
        /// Direct Heal , no calculation
        /// </summary>
        public virtual void DealHeal(Unit Target, uint Value)
        {
            if (Target == null || Target.IsDead)
                return;

            CbtInterface.OnDealHeal(Target, Value);
            Target.CbtInterface.OnTakeHeal(this, Value);

            if (Value + Target.Health > Target.MaxHealth)
                Target.Health = Target.MaxHealth;
            else
                Target.Health += Value;
        }

        /// <summary>
        /// Kill this unit, Generate Xp and Loots
        /// </summary>
        /// <param name="Killer"></param>
        public virtual void SetDeath(Unit Killer)
        {
            Health = 0;

            States.Add(3); // Death State

            PacketOut Out = new PacketOut((byte)Opcodes.F_OBJECT_DEATH);
            Out.WriteUInt16(Oid);
            Out.WriteByte(1);
            Out.WriteByte(0);
            Out.WriteUInt16(Killer.Oid);
            Out.Fill(0, 6);
            DispatchPacket(Out, true);

            CbtInterface.Evade();

            WorldMgr.GenerateXP(Killer, this);
            GenerateLoot(Killer);

            ScrInterface.OnDie(this);
            EvtInterface.Notify(EventName.ON_DIE, this, null);
        }

        public virtual void RezUnit()
        {
            CbtInterface.Evade();
            States.Remove(3); // Death State
            Health = TotalHealth;
            Region.UpdateRange(this, true);

            EvtInterface.Notify(EventName.ON_REZURECT, this, null);
        }

        #endregion



        #region Loots

        public Loot Loots = null;

        public void GenerateLoot(Unit Killer)
        {
            if (Killer == null)
                return;

            Loots = LootsMgr.GenerateLoot(this, Killer);
            if (Loots != null && Killer.IsPlayer())
                SetLootable(true, Killer.GetPlayer());
        }

        public void SetLootable(bool Value, Player Looter)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
            Out.WriteUInt16(Oid);
            Out.WriteByte(9);
            Out.WriteByte((byte)(Value ? 1 : 0));
            Out.Fill(0, 6);
            if (Looter != null)
                Looter.SendPacket(Out);
            else
                DispatchPacket(Out, false);




            /*
                // put here but probly not work
                public virtual void SendStateUpdate()
                {
                    PacketOut  Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
                    Out.WriteUInt16(Oid);
                    Out.WriteByte(6);
                    Out.WriteUInt16(0);//(byte)(Value ? 1 : 0));
                    Out.WriteByte(8);//8
                    Out.WriteUInt16(2);//2
                    //Out.WriteUInt16(Stage.Objectives.First().Objective.TreeFall);//2
                    Out.WriteUInt32(0xDE3E7080);//(0xDE3E7080);
                    Out.WriteUInt32(0);
                    Out.WriteUInt16(0);
                    //Plr.SendPacket(Out);
                    DispatchPacket(Out, false);            

            }

                */




        }

        #endregion

        #region Interact

        public override void SendInteract(Player Plr, InteractMenu Menu)
        {
            if (IsDead && Loots != null)
            {
                Loots.SendInteract(Plr, Menu);
                if (!Loots.IsLootable())
                    SetLootable(false, Plr);
            }

            base.SendInteract(Plr, Menu);
        }

        #endregion

        #region Values

        public UInt16 Speed
        {
            get
            {
                return (UInt16)((float)StsInterface.Speed + (((float)StsInterface.Speed * (float)StsInterface.BonusSpeed) * 0.01f));
            }
            set
            {
                StsInterface.Speed = value;
                if (IsPlayer())
                    GetPlayer().SendSpeed(CanMove() ? Speed : (ushort)0);
            }
        }
        public long NextAllowedMovements = 0;
        private byte _Level = 1;
        public byte Level
        {
            get
            {
                if (IsPlayer())
                    return GetPlayer()._Value.Level;
                else
                    return _Level;
            }
            set
            {
                if (IsPlayer())
                    GetPlayer().SetLevel(value);
                else
                    _Level = value;
            }
        }

        private byte _Renown = 1;
        public byte Renown
        {
            get
            {
                if (IsPlayer())
                    return GetPlayer()._Value.RenownRank;
                else
                    return _Renown;
            }
            set
            {
                if (IsPlayer())
                    GetPlayer().SetRenownLevel(value);
                else
                    _Renown = value;
            }
        }

        private UInt16 _Model = 0;
        public UInt16 Model
        {
            get
            {
                if (IsPlayer())
                    return GetPlayer()._Info.ModelId;
                else
                    return _Model;
            }
            set
            {
                if (IsPlayer())
                    GetPlayer()._Info.ModelId = (byte)value;
                else
                    _Model = value;
            }
        }

        public byte Rank = 0; // Normal,Champion,Hero,Lord
        public byte Faction = 0; // Faction Flag
        public byte FactionId = 0; // FactionFlag/8
        public bool Agressive = false;
        public GameData.Realms Realm = GameData.Realms.REALMS_REALM_NEUTRAL;

        public void SetFaction(byte NewFaction)
        {
            Faction = NewFaction;

            FactionId = (byte)(NewFaction / 8);
            Faction = (byte)(NewFaction % 8);
            Agressive = Convert.ToBoolean(Faction % 2);
            Rank = (byte)(Faction / 2);

            if (FactionId >= 8 && FactionId <= 15)
                Realm = GameData.Realms.REALMS_REALM_ORDER;
            else if (FactionId >= 16 && FactionId <= 23)
                Realm = GameData.Realms.REALMS_REALM_DESTRUCTION;
            else
                Realm = GameData.Realms.REALMS_REALM_NEUTRAL;

            Faction = NewFaction;

            if (Agressive)
                AiInterface.SetBrain(new AgressiveBrain(AiInterface));
        }

        public void DisableMovements(long MSTime)
        {
            NextAllowedMovements = TCPManager.GetTimeStampMS() + MSTime;
            Log.Info("DisableMovements", "NextAllowedMovements: " + NextAllowedMovements + ",Time=" + MSTime);
            if (IsPlayer())
                GetPlayer().SendSpeed(0);
        }

        public bool CanMove()
        {
            return NextAllowedMovements == 0;
        }

        public void UpdateSpeed(long Tick)
        {
            if (NextAllowedMovements != 0 && NextAllowedMovements < Tick)
            {
                Log.Info("UpdateSpeed", "Can move : " + CanMove());
                NextAllowedMovements = 0;
                if (IsPlayer())
                    GetPlayer().SendSpeed(Speed);
                else
                    MvtInterface.CancelWalkTo();
            }
        }

        #endregion

        #region Range

        public override void AddInRange(Object Obj)
        {
            if (Obj.IsUnit())
                AiInterface.AddRange(Obj.GetUnit());

            base.AddInRange(Obj);
        }

        public override void RemoveInRange(Object Obj)
        {
            if (Obj.IsUnit())
                AiInterface.RemoveRange(Obj.GetUnit());

            base.RemoveInRange(Obj);
        }

        public override void ClearRange()
        {
            AiInterface.ClearRange();
            base.ClearRange();
        }

        #endregion


    }
}
