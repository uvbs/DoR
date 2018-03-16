using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{

    [DataTable(PreCache = false, TableName = "new_ability_infos", DatabaseName = "World")]
    [Serializable]
    public class new_ability_infos : DataObject
    {
        [PrimaryKey()]
        public UInt16 Entry;

        [DataElement()]
        public byte CareerLine;

        [DataElement()]
        public uint InvokeDelay;

        [DataElement()]
        public uint EffectDelay;

        [DataElement(AllowDbNull = false)]
        public UInt16 EffectID;

        [DataElement()]
        public Int16 ChannelID;

        [DataElement()]
        public UInt16 CooldownEntry;

        [DataElement()]
        public UInt16 ToggleEntry;

        [DataElement()]
        public byte CastAngle;

        [DataElement()]
        public byte AbilityType;

        [DataElement()]
        public ushort IgnoreGlobalCooldown;

        [DataElement()]
        public byte Fragile;







        public List<Ability_Stats> Stats;





    }
}
