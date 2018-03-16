using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "new_ability_volatiles", DatabaseName = "World")]
    [Serializable]
    public class new_ability_volatiles : DataObject
    {
        [DataElement()]
        public byte CareerLine;

        [DataElement()]
        public uint CastTime;

        [DataElement()]
        public UInt16 Cooldown;

        [DataElement()]
        public uint SpecialCost;

        [DataElement()]
        public byte CanCastWhileMoving;
    }
}

