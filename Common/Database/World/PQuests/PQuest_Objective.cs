/*

 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "pquest_objectives", DatabaseName = "World")]
    [Serializable]
    public class PQuest_Objective : DataObject
    {
        [PrimaryKey(AutoIncrement=true)]
        public UInt32 Guid;


        [DataElement(AllowDbNull = false)]
        public byte G0;

        // updates the dot kill counts
        [DataElement(AllowDbNull = false)]
        public byte Dotupdater;



        [DataElement(AllowDbNull = false)]
        public byte G1;




        [DataElement(AllowDbNull=false)]
        public UInt32 Entry;

        [DataElement(Varchar=255, AllowDbNull = false)]
        public string StageName;

        [DataElement(AllowDbNull = false)]
        public byte Type;

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string Objective;

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string Objective2;

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string Objective3;

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string Objective4;



        [DataElement(AllowDbNull = false)]
        public byte Val;

        [DataElement(AllowDbNull = false)]
        public UInt16 Count;

        [DataElement(AllowDbNull = false)]
        public UInt16 Count2;


        [DataElement(AllowDbNull = false)]
        public string Description;

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string ObjectId;

        [DataElement(AllowDbNull = false)]
        public UInt32 TokCompleted;



        [DataElement(AllowDbNull = false)]
        public byte RedDot;


        [DataElement(AllowDbNull = false)]
        public byte PQDifficulty;


        [DataElement(AllowDbNull = false)]
        public byte Race;

        [DataElement(AllowDbNull = false)]
        public byte TreeFall;



        [DataElement(AllowDbNull = false)]
        public UInt32 Timer;


        public PQuest_Info Quest;

        public Item_Info Item;
        public Creature_proto Creature;
        public GameObject_proto GameObject = null;

        public List<PQuest_Spawn> Spawns = new List<PQuest_Spawn>();
    }
}
