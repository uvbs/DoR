/*leo228
 * 
 * Dawn of Reckoning 2008-2019
 */ 
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    public enum Objective_Type
    {
        QUEST_UNKNOWN = 0,
        QUEST_SPEACK_TO = 1,
        QUEST_KILL_MOB = 2,
        QUEST_USE_GO = 3,
        QUEST_GET_ITEM = 4,
        QUEST_KILL_PLAYERS = 5,
        QUEST_PROTECT_UNIT = 6,

       // missing enums
        QUEST_USE_ITEM = 7,
        QUEST_WIN_SCENARIO = 8,
        QUEST_CAPTURE_BO = 9,
        QUEST_CAPTURE_KEEP = 10,
        QUEST_KILL_GO = 11





    };

    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "quests_objectives", DatabaseName = "World")]
    [Serializable]
    public class Quest_Objectives : DataObject
    {
        [PrimaryKey(AutoIncrement=true)]
        public int Guid;

        [DataElement()]
        public UInt16 Entry;

        [DataElement()]
        public uint ObjType;

        [DataElement()]
        public uint ObjCount;

        [DataElement()]
        public string Description;

        [DataElement()]
        public string ObjID;

        public Quest Quest;
        public Item_Info Item = null;
        public Creature_proto Creature = null;
        public GameObject_proto GameObject = null;
    }
}
