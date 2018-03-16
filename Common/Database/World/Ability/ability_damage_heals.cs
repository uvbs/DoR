using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{

    [DataTable(PreCache = false, TableName = "ability_damage_heals", DatabaseName = "World")]
    [Serializable]
    public class ability_damage_heals : DataObject
    {

        [PrimaryKey()]
        public UInt16 Entry;

        [DataElement()]
        public UInt16 Index;


        [DataElement()]
        public UInt16 MinDamage;

        [DataElement()]
        public UInt16 MaxDamage;


        [DataElement()]
        public UInt16 DamageType;

        [DataElement()]
        public UInt16 ParentCommandID;


        [DataElement()]
        public UInt16 ParentCommandSequence;


    }
}

