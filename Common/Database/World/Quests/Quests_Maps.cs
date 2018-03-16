using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "quests_maps", DatabaseName = "World")]
    [Serializable]
    public class Quest_Map : DataObject
    {
        [DataElement()]
        public ushort Entry;

        [DataElement()]
        public byte Id;

        [DataElement()]
        public string Name;

        [DataElement()]
        public string Description;

        [DataElement()]
        public ushort ZoneId;

        [DataElement()]
        public ushort Icon;

        [DataElement()]
        public ushort X;

        [DataElement()]
        public ushort Y;

        [DataElement()]
        public ushort Unk;

        [DataElement()]
        public byte When;
    }
}
