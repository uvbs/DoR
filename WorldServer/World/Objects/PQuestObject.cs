using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;
namespace WorldServer
{
    public class PQuestCreature : Creature
    {
        PQuestObjective Objective;

        public PQuestCreature(Creature_spawn Spawn, PQuestObjective Objective)
        {


            this.Spawn = Spawn;
            Name = Spawn.Proto.Name;
            this.Objective = Objective;

            if (Objective.Objective.Type == (byte)Objective_Type.QUEST_PROTECT_UNIT)
                EvtInterface.AddEvent(Protected, Objective.Objective.Count, 1);


        }

        public override void RezUnit()
        {


            Objective.ActiveCreatures.Remove(this);
            PQuestCreature NewCreatue = new PQuestCreature(Spawn, Objective);
            Objective.ActiveCreatures.Add(NewCreatue);
            Region.AddObject((Object)NewCreatue, Spawn.ZoneId);
            Dispose();
        }







        public void Protected()
        {
            Objective.Quest.HandleEvent(Objective_Type.QUEST_PROTECT_UNIT, Spawn.Entry, 10000);
        }


    }
    public class PQuestStage
    {
        public int Number;
        public string StageName;
        public string Description;
        public List<PQuestObjective> Objectives = new List<PQuestObjective>();




        public void AddObjective(PQuestObjective Objective)
        {
            Objectives.Add(Objective);


        }

        public void Reset()
        {
            foreach (PQuestObjective Obj in Objectives)
            {
                Obj.Count = 0;
                Obj.Reset();

            }

        }

        public bool IsDone()
        {

            bool done = true;
            foreach (PQuestObjective Obj in Objectives)
            {
                if (!Obj.IsDone())
                    done = false;
            }

            return done;

        }

        public void Cleanup()
        {
            foreach (PQuestObjective Obj in Objectives)
            {
                Obj.Cleanup();
            }
        }

    }

    public class PQuestObjective
    {


        public PQuestObject Quest;
        public PQuest_Objective Objective;
        public List<PQuestCreature> ActiveCreatures = new List<PQuestCreature>();
        public uint ObjectiveID;
        public int _Count;



        public int Count
        {

            get
            {
                return _Count;

            }
            set
            {
                _Count = value;

            }






        }
        public bool IsDone()
        {


            if (Objective == null)


                return false;
            else
                return Count >= Objective.Count;



        }
        public void Reset()
        {


            foreach (PQuest_Spawn Spawn in Objective.Spawns)
            {

                //Log.Info("Creature", "npc = " + Name + "  Oid = " + Oid + "  X= " + Spawn.WorldX + "  Y= " + Spawn.WorldY + "  Z= " + Spawn.WorldZ);

                Log.Success("SENDING Spawn.Type", "STARTING Spawn.Type ");


                if (Spawn.Type == 1)
                {
                    Creature_proto Proto = WorldMgr.GetCreatureProto(Spawn.Entry);
                    if (Proto == null)
                    {


                        Log.Error("PQCreatue", "No Proto");
                        return;
                    }

                    Creature_spawn S = new Creature_spawn();

                    S.Guid = (uint)WorldMgr.GenerateCreatureSpawnGUID();
                    S.BuildFromProto(Proto);
                    S.WorldO = Spawn.WorldO;
                    S.WorldY = Spawn.WorldY;
                    S.WorldZ = Spawn.WorldZ;
                    S.WorldX = Spawn.WorldX;
                    S.ZoneId = Spawn.ZoneId;
                    S.Bytes = "";

                    PQuestCreature NewCreature = new PQuestCreature(S, this);
                    this.ActiveCreatures.Add(NewCreature);
                    Quest.Region.AddObject((Object)NewCreature, Spawn.ZoneId);


                }
            }

        }

        public void Cleanup()
        {

            foreach (PQuestCreature Creature in ActiveCreatures)
            {


                Creature.Dispose();
            }

            ActiveCreatures = new List<PQuestCreature>();
        }


    }

    public class PQuestObject : Object
    {
        public PQuest_Info Info;
        public PQuestStage Stage;
        public List<Player> Players;
        public List<PQuestStage> Stages;
        bool started = false;
        bool ended = false;
        private static ushort TIME_PQ_RESET = 160;//2 minutes between each reset
        private static ushort TIME_EACH_STAGE = 10000;//10 seconds interval between each stage

        public PQuestObject()
            : base()
        {

        }

        public PQuestObject(PQuest_Info Info)
            : this()
        {
            this.Info = Info;
            Name = Info.Name;
            Players = new List<Player>();
            Stages = new List<PQuestStage>();

            foreach (PQuest_Objective Obj in Info.Objectives)
            {

                Boolean exists = false;
                foreach (PQuestStage Stage in Stages)
                {
                    if (Stage.StageName == Obj.StageName)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    PQuestStage Stage = new PQuestStage();
                    Stage.StageName = Obj.StageName;
                    Stage.Number = Stages.Count;

                    Stage.Description = Obj.Description;
                    Stages.Add(Stage);
                }

                foreach (PQuestStage Stage in Stages)
                {

                    if (Stage.StageName == Obj.StageName)
                    {
                        PQuestObjective Objective = new PQuestObjective();
                        Objective.Quest = this;
                        Objective.Objective = Obj;
                        Objective.ObjectiveID = Obj.Guid;


                        Objective.Count = 0;

                        Stage.AddObjective(Objective);


                    }
                }

            }

        }
        public void SendCurrentStage(Player Plr)
        {

            Plr.QtsInterface.updateObjects();
            PacketOut Out = new PacketOut((byte)Opcodes.F_OBJECTIVE_INFO);
            Out.WriteUInt32(Info.Entry);
            Out.WriteByte(0);
            Out.WriteByte((byte)Plr.Realm);
            Out.WriteByte(Info.Type);
            Out.WriteUInt16(0);
            Out.WritePascalString(Info.Name);
            Out.WriteByte((byte)Plr.Realm);
            Out.WriteUInt16(0); 


           
            Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Guid);
            Out.WriteByte((byte)Stage.Objectives.First().Objective.G0);
            Out.WriteByte((byte)Stage.Objectives.First().Objective.G1);
            Out.WriteByte(Stage.Objectives.First().Objective.RedDot);




            // SEND GREEN DOT 1
            Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);
            Out.WriteUInt16((ushort)0);
            Out.WriteByte(0);
            Out.WritePascalString(Stage.Objectives.First().Objective.Objective);



            //RUINS OF SCHLOS EASY (2 dots) ST 1
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x0385)
            {

                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count2);
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
                
            }
            //RUINS OF SCHLOS EASY (2 dots) ST 2
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x0386)
            {

                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count2);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0);
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }


            //THE WEBWORKS EASY
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x0439)
            {

                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }




            // PLAGUE ON THE WIND EASY
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x0472)
            {

                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }



            // THE BLACK MIRE  EASY (OSTLAND)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x049D)
            {

                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }



            // HOCHNAR  EASY (OSTLAND) //
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x0488)
            {

                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }


            // THE GRISLY HEARD  EASY (HIGH PASS) 
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x057D)
            {

                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }


            // CULT OF THE MAGUS  EASY (HIGH PASS) 
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x0559)
            {

                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }

            // CULT OF THE MAGUS  EASY (HIGH PASS) 
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x0559)
            {

                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective3);
            }




            // TOMB OF THE TRAITOR  EASY (HIGH PASS) 
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x0565)
            {

                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }


            // THE FALL OF NIGHT EASY (CHAOS WASTE) (3 COUNTERS)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x06E9)
            {

                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }

            // THE FALL OF NIGHT EASY (CHAOS WASTE) (3 COUNTERS)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x06E9)
            {

                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective3);
            }


            // THE siren sea EASY (CHAOS WASTE) (2 COUNTERS)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x06A1)
            {

                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }

            // burning windmill NORMAL(nordland st3)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x036C)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count2);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }






            // lost artifacts normal (CHAOS WASTE)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x06B8)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);

            }





            // PLAGUE TROLLS NORMAL(TROLL COUNTRY)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x049A)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);

            }


            // siege of bohsenfels NORMAL (OSTLAND)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x04AF)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);

            }



            // TEMPLE OF CHANGE NORMAL (HIGH PASS)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x055C)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);

            }

            // TEMPLE OF CHANGE NORMAL (HIGH PASS)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x055C)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective3);

            }


            // THE FOETID PLAINS NORMAL(HIGH PASS)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x0568)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);

            }

            // ALTAR OF MADNESS NORMAL(chaos waste)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x06E5)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);

            }



            // ALTAR OF MADNESS NORMAL(chaos waste)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x06E5)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective3);

            }
            // MADNESS NORMAL(chaos wastes)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x06AB)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }


            // GREEN DOT 2
            ///////////////

            if (Stage.Objectives.First().Objective.Guid == (ushort)0x034F)
            {
                ///////////////SECOND STUFF counter///////////////
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }



            // DEATHSTONE QUARRY HARD (sent 4 times)  2
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x0469)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }

            // DEATHSTONE QUARRY HARD   3
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x0469)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective3);
            }

            // DEATHSTONE QUARRY HARD   4
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x0469)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective4);

            }

            //Gore Wood HARD (ostland)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x04C3)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }


            //LAKE OF THE DAMNED HARD (HIGH PASS)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x055F)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }


            //The Reaping Field HARD (chaos wastes)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x06A8)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }


            //The Reaping Field HARD (chaos wastes)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x06A8)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective3);
            }



            //Ebon keep HARD (chaos wastes)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x06E0)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }


            //Ebon keep HARD (chaos wastes)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x06E0)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective3);
            }


            //Ebon keep HARD (chaos wastes)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x06E0)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective4);
            }


            //DANCE OF BONES HARD (chaos wastes)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x06AE)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective);
            }



            //Broken ground HARD (pragg)
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x062F)
            {
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//second dot counter 0/4 
                Out.WriteUInt16(0);// count killed ?
                Out.WriteByte(0); //
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);
            }



            // DISABLING THIS MAKES NORSE ARE COMING EASY GREEN

            // GREEN DOT 3  // some bug on guidbyte1 set at  3dots
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x034F)
            {

                ///////////////THIRD STUFF counter///////////////
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);// 0= green 1= red //turns THIRD  dot red
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count);//
                Out.WriteUInt16(0);// THIRD HOW MANY BEEN KILLED COUNT  0028= 40
                Out.WriteByte(0);
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective3);

                /////////////////////////////////////////////////////////

            }
            // ENABLING this makes PILLAGERS APPROCH RED HARD
            // GREEN DOT 4 // 0445= 1093= pillagers approch
            if (Stage.Objectives.First().Objective.Guid == (ushort)0x0445)
            {
                //////////////////////////////
                Out.WriteByte(Stage.Objectives.First().Objective.RedDot);// 0= green 1= red //turns THIRD  dot red
                Out.WriteUInt16((ushort)Stage.Objectives.First().Objective.Count2);//
                Out.WriteUInt16(0);// THIRD HOW MANY BEEN KILLED COUNT  0028= 40
                Out.WriteByte(0);
                Out.WritePascalString(Stage.Objectives.First().Objective.Objective2);

                /////////////////////////////////////////////////////////
            }
            foreach (PQuest_Spawn Spawn in Stage.Objectives.First().Objective.Spawns)
            {

                Log.Info("Creature", "npc = " + Name + "  Oid = " + Oid + "  X= " + Spawn.WorldX + "  Y= " + Spawn.WorldY + "  Z= " + Spawn.WorldZ);

                // Log.Success("SENDING Spawn.Type", "STARTING Spawn.Type ");

                //if (Stage.Objectives.First().Objective.Guid == (UInt16)0x052E)

                if (Spawn.Type == 1 & Stage.Objectives.First().Objective.Guid == (UInt16)0x052E)
                {
                    Creature_proto Proto = WorldMgr.GetCreatureProto(Spawn.Entry);
                    if (Proto == null)
                    {


                        Log.Error("PQCreatue", "No Proto");
                        return;
                    }


                    Creature_spawn S = new Creature_spawn();


                    S.Guid = (uint)WorldMgr.GenerateCreatureSpawnGUID();
                    S.BuildFromProto(Proto);
                    S.WorldO = Spawn.WorldO;
                    S.WorldY = Spawn.WorldY;
                    S.WorldZ = Spawn.WorldZ;
                    S.WorldX = Spawn.WorldX;
                    S.ZoneId = Spawn.ZoneId;
                    S.Bytes = "";

                }

            }

            /////////////////////////////////////////////////
            Out.WriteByte(Stage.Objectives.First().Objective.PQDifficulty); // THIS SHOULD BE 0   // dificulty // 0= easy 2-3 players 1= normal but shows no players //  2= hard 9 or more players //  4,5= normal 6-9 players
            Out.WriteByte(0);
            Out.WritePascalString(Stage.StageName);
            Out.WriteByte(0);// added this which shows the description in white writing kill the raven host marauders terr
            Out.WritePascalString(Stage.Objectives.First().Objective.Description);//52=size of string= 4B 69 6C 6C 20 74 68 65 20 52 61 76 65 6E 20 48 6F 73
            



            Out.WriteUInt32((uint)Stage.Objectives.First().Objective.Timer); //00 00 00 00// clock////////////
            Out.WriteUInt32((uint)Stage.Objectives.First().Objective.Timer);// 00 00 00 00 // clock////////////

            //Out.WriteUInt32(0);//00 00 00 00
            Out.WriteUInt16(0);/////////////////


            Out.WriteByte(0);// is this in here ??? 

            // MAY BE ADD THE IF TO THIS FOR BATTLEFEALD OBJECTIVE ?
            //         Out.WritePascalString(Info.Name);// weard this shows battle feald objective if anabled

            //Out.WriteUInt16(0x0048);//  GetInfluenceInfo()->influenceid);//////////////////////
            Out.WriteUInt16(0x0044);// only for pillger pq//////////////
            Out.WriteUInt32(0);
            Plr.SendPacket(Out);


            // GOLD CHEST ON MAP TURNS GOLD WHEN MORE THAN ONE PLAYER IS IN THE ZONE
            Out = new PacketOut((byte)Opcodes.F_OBJECTIVE_UPDATE);
            Out.WriteUInt32(0);
            Out.WriteByte(6);
            Out.WriteUInt32(Info.ZoneId);//zoneid
            Out.WriteUInt16(2);// zone public quests size 00 02 zone.public quest counts
            Out.WriteUInt32(Info.Entry);//(entry);//02 26// entry=550
            Out.WriteUInt16((ushort)Zone._Players.Count);//(0x0004);// get player counts
            //Out.WriteUInt16((UInt16)1);
            Out.WriteUInt16(0);
            Out.WriteUInt16(0x0238);// ???
            Out.WriteUInt16(0);
            DispatchPacket(Out, true);



            // SENDS STATE OF BO FLAGS
        }
        public void SendObjState(Player Plr)
        {
            //.
            PacketOut Out = new PacketOut((byte)Opcodes.F_OBJECTIVE_STATE);// size =19

            Out.WriteUInt32((UInt32)Stage.Objectives.First().Objective.Entry);//Info.Entry);//
            Out.WriteUInt32(0xFFFFFFFF);// lock timer 00 00 00 30=48 seconds timer on flag 
            Out.WriteUInt16(0xFFFF);// was 2
            Out.WriteByte(0);
            Out.WriteByte(0);//50 ?
            Out.WriteByte((byte)Plr.Realm);//Realm// 0= green flag /1= order /2=destruction
            Out.WriteByte(1);//1
            Out.WriteByte(0);//10?
            Out.WriteByte(0);//00
            Plr.SendPacket(Out);


            // SENT 2ND 
        }
        public void SendStageInfo(Player Plr)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_OBJECTIVE_UPDATE);
            Out.WriteUInt32(Info.Entry);
            Out.WriteByte((byte)9);
            Out.Fill(0, 3);
            Plr.SendPacket(Out);


        }
        public void SendCoutUpdate(Player Plr)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_ACTION_COUNTER_UPDATE);
            Out.WriteByte(1);
            Out.WriteByte(0xE7);//
            Out.Fill(0, 6);
            Plr.SendPacket(Out);



        }// PQ RESTART TIMER
        public void SendReinitTime(Player Plr, ushort Time)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_OBJECTIVE_INFO);
            Out.WriteUInt32(Info.Entry);
            Out.WriteByte(1);
            Out.WriteByte(1);// is 1 // why set as 2 ?
            Out.WriteByte(1);
            Out.WriteUInt16(0);
            Out.WritePascalString(Info.Name);// 19
            Out.WriteUInt16(0);
            Out.WriteUInt16(Time); // Time in seconds
            Out.WriteUInt16(0);
            Out.WriteUInt16(0);
            Plr.SendPacket(Out);


            {
                if (Stage.Objectives.First().Objective.Guid == (ushort)0x052E)//0x032B)// =811

                    Out = new PacketOut((byte)Opcodes.F_ANIMATION);
                Out.WriteUInt16((ushort)Oid);//Oid);// 0280// ObjectID
                Out.WriteByte(0);// 0= on 1 = off
                Out.WriteByte((byte)0x44);
                Out.WriteUInt16((ushort)0);
                Plr.SendPacket(Out);

            }
        }
        public override void OnLoad()
        {

            Log.Success("PQ", "Loading :" + Info.Name);

            X = (int)Info.PinX;
            Y = (int)Info.PinY;
           


            Z = 16384;
            SetOffset(Info.OffX, Info.OffY);
            Region.UpdateRange(this);
            // if (distance >200.0f)//TODO: check ke each quest has not a range different

            base.OnLoad();
            IsActive = true;





        }
        public override void SendMeTo(Player Plr)
        {
            // prints out big white onscreen txt of zone
            PacketOut Out = new PacketOut((byte)Opcodes.F_LOCALIZED_STRING);
            Out.WriteByte(1);
            Out.WriteByte(0);
            Out.WriteUInt16(0);
            Out.WriteUInt16((ushort)0x0070);
            Out.WriteUInt16(0);
            Out.WriteByte(0);
            Out.WriteByte(1);
            Out.WriteByte(1);
            Out.WriteByte(0);
            Out.WritePascalString(Info.Name);
            Plr.SendPacket(Out);



            Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
            Out.WriteByte(0);
            Out.WriteByte((byte)Info.AreaId2);
            Out.WriteByte(0x11);
            Out.WriteByte(2);
            Out.WriteByte(1);
            Out.WriteByte(1);
            Out.WriteUInt32(0);
            Plr.SendPacket(Out);




            if ((byte)Plr.Realm != Info.Type)
                return;

            SendCurrentStage(Plr);
            SendStageInfo(Plr);// ??
            SendObjState(Plr);
            SendCoutUpdate(Plr);
            // TODO
            // Send Quest Info && Current Stage && Current Players


        }
        public void HandleEvent(Objective_Type Type, uint Entry, int Count)
        {

            if (Stage == null)
                return;

            foreach (PQuestObjective Objective in Stage.Objectives)
            {






                if (Objective.IsDone())
                    continue;

                if (Objective.Objective.Type != (int)Type)
                    continue;

                bool CanAdd = false;
                int NewCount = Objective.Count;






                if (Type == Objective_Type.QUEST_SPEACK_TO || Type == Objective_Type.QUEST_KILL_MOB || Type == Objective_Type.QUEST_PROTECT_UNIT)
                {
                    if (Objective.Objective.Creature != null && Entry == Objective.Objective.Creature.Entry)
                    {
                        CanAdd = true;
                        NewCount += Count;

                    }
                }
                else if (Type == Objective_Type.QUEST_USE_GO)
                {
                    if (Objective.Objective.GameObject != null && Entry == Objective.Objective.GameObject.Entry)
                    {
                        CanAdd = true;
                        NewCount += Count;
                    }
                }
                else if (Type == Objective_Type.QUEST_UNKNOWN)
                {
                    if (Objective.Objective.Guid == Entry)
                    {
                        CanAdd = true;
                        NewCount += Count;
                    }
                }

                if (CanAdd)
                {



                    Objective.Count = NewCount;
                    foreach (Player Plr in Players)
                    {




                        //PQ UPDATE KILLS
                        PacketOut Out = new PacketOut((byte)Opcodes.F_OBJECTIVE_UPDATE);
                        Out.WriteUInt32(Info.Entry);
                        Out.WriteByte((byte)Plr.Realm);
                        Out.WriteByte(Info.Type);
                        Out.WriteUInt32(Objective.Objective.Guid);//(0x000002F3);
                        Out.WriteByte((byte)Objective.Objective.Dotupdater);// some kind of reset ?//    1= update 2nd counter dot
                        Out.WriteUInt16((ushort)Objective.Count);// kill counter
                        DispatchPacket(Out, true);



                        // SENDS THE 100 INFLUNCE ?
                        Out = new PacketOut((byte)Opcodes.F_INFLUENCE_UPDATE);
                        Out.WriteUInt16((ushort)48);//AreaInfluence);// 48 info.infuluce id table player influnce id
                        Out.WriteUInt16(0);
                        Out.WriteUInt32(0x00000064);//(Info.infuluce);// 64 =100 c8 =200 influnce
                        Out.WriteByte(1);// 1
                        Out.Fill(0, 3);
                        DispatchPacket(Out, true);



                        Plr.SendLocalizeString(Objective.Objective.Objective + " " + Objective.Count + "/" + Objective.Objective.Count, GameData.Localized_text.CHAT_TAG_MONSTER_EMOTE);
                    }
                }
            }

            if (Stage.IsDone())
            {
                NextStage();
            }



        }

        public override void AddInRange(Object Obj)
        {




            if (Obj.IsPlayer())
            {
                if ((byte)Obj.GetPlayer().Realm != Info.Type)
                    return;

                Players.Add(Obj.GetPlayer());

                //Log.Success("PQuest", "Adding " + Obj.GetPlayer().Name);
                Log.Success("PQuest", "AddingPlayer To PublicQuest " + Obj.GetPlayer().Name);


                Obj.GetPlayer().QtsInterface.SetPublicQuest(this);

                if (!started && !ended)
                    Start();






            }

            base.AddInRange(Obj);



        }
        public override void RemoveInRange(Object Obj)
        {


            if (Obj.IsPlayer())
            {

                
                PacketOut Out = new PacketOut((byte)Opcodes.F_OBJECTIVE_UPDATE);
                Out.WriteUInt32(Info.Entry);
                Out.WriteUInt32(0);
                DispatchPacket(Out, true);

                Players.Remove(Obj.GetPlayer());



                //Log.Success("PQuest", "Removing " + Obj.GetPlayer().Name);
                Log.Success("PQuest", "RemovePlayer From PublicQuest " + Obj.GetPlayer().Name);


                Obj.GetPlayer().QtsInterface.SetPublicQuest(null);


            }

            base.RemoveInRange(Obj);





        }

        public void Start()
        {
            if (started || ended)
                return;

            foreach (PQuestStage sStage in Stages)
            {
                if (sStage.Number == 0)
                {
                    Stage = sStage;
                    Stage.Reset();
                    foreach (Player Plr in Players)
                    {

                        SendCurrentStage(Plr);

                    }
                    break;
                }
            }

            //Log.Success("PQuest", "Starting Quest " + Info.Name);
            Log.Success("PQuest", "Starting PublicQuest " + Info.Name);

            started = true;
        }

        public void NextStage()
        {



            Stage.Cleanup();
            int nextStageId = Stage.Number + 1;
            EvtInterface.RemoveEvent(Failed);

            foreach (PQuestStage sStage in Stages)
            {
                if (sStage.Number == nextStageId)
                {

                    Stage = sStage;
                    Stage.Reset();
                    foreach (Player Plr in Players)
                    {
                        SendCurrentStage(Plr);
                    }
                    EvtInterface.AddEvent(Failed, TIME_EACH_STAGE * 1000, 1);
                    return;
                }
            }

            End();
        }

        public void End()
        {
            //Log.Success("PQuest", "End");// added mite not work

            if (ended)
                return;

            EvtInterface.AddEvent(Reset, TIME_PQ_RESET * 1000, 1);
            started = false;
            ended = true;

            foreach (Player Plr in Players)
            {


                Plr.SendLocalizeString(Info.Name + " Complete", GameData.Localized_text.CHAT_TAG_MONSTER_EMOTE);
                SendReinitTime(Plr, TIME_PQ_RESET);
            }

        }

        public void Reset()
        {
            started = false;
            ended = false;

            EvtInterface.RemoveEvent(Failed);
            Start();
        }

        public void Failed()
        {



            



            EvtInterface.AddEvent(Reset, TIME_PQ_RESET * 1000, 1);
            started = false;
            ended = true;
            Stage.Cleanup();

            foreach (Player Plr in Players)
            {


                Plr.SendLocalizeString(Info.Name + " Failed", GameData.Localized_text.CHAT_TAG_MONSTER_EMOTE);
                SendReinitTime(Plr, TIME_PQ_RESET);


            }
        }
    }
}
