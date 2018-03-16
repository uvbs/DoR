using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{

    [DataTable(PreCache = false, TableName = "buff_infos", DatabaseName = "World")]
    [Serializable]
    public class buff_infos : DataObject
    {
        [PrimaryKey()]
        public UInt16 Entry;


        [DataElement()]
        public UInt16 BuffClass;


        [DataElement()]
        public UInt16 Type;




        [DataElement()]
        public UInt16 Group;

        [DataElement()]
        public UInt16 AuraPropagation;


        [DataElement()]
        public UInt16 MaxCopies;

        [DataElement()]
        public UInt16 MaxStack;


        [DataElement()]
        public UInt16 UseMaxStackAsInitial;

        [DataElement()]
        public UInt16 StackLine;


        [DataElement()]
        public UInt16 Duration;


        [DataElement()]
        public UInt16 Interval;


        [DataElement()]
        public UInt16 PersistsOnDeath;


        [DataElement()]
        public UInt16 CanRefresh;


        [DataElement()]
        public UInt16 FriendlyEffectID;


        [DataElement()]
        public UInt16 EnemyEffectID;




        [DataElement()]
        public UInt16 Silent;



    }
}
