/*
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "quests", DatabaseName = "World")]
    [Serializable]
    public class Quest : DataObject
    {
        [PrimaryKey()]
        public UInt16 Entry;

        [DataElement(Varchar=255,AllowDbNull=false)]
        public string Name;

        [DataElement(AllowDbNull = false)]
        public byte Type;

        [DataElement(AllowDbNull = false)]
        public byte Level;

        [DataElement(AllowDbNull = false)]
        public string Description;

        [DataElement(AllowDbNull = false)]
        public string OnCompletionQuest;

        [DataElement(AllowDbNull = false)]
        public string ProgressText;

        [DataElement(AllowDbNull = false)]
        public string Particular;

        [DataElement(AllowDbNull = false)]
        public uint Xp;

        [DataElement(AllowDbNull = false)]
        public uint Gold;

        [DataElement(AllowDbNull = false)]
        public string Given;

        [DataElement(AllowDbNull = false)]
        public string Choice;

        [DataElement(AllowDbNull = false)]
        public byte ChoiceCount;

        [DataElement(AllowDbNull = false)]
        public UInt16 PrevQuest;

        public List<Quest_Objectives> Objectives = new List<Quest_Objectives>();
        public List<Quest_Map> Maps = new List<Quest_Map>();
        public Dictionary<Item_Info, uint> Rewards = new Dictionary<Item_Info, uint>();
    }
}
