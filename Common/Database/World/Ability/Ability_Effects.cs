using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "Ability_Effects", DatabaseName = "World")]
    [Serializable]
    public class Ability_Effects : DataObject
    {
        [PrimaryKey()]
        public UInt16 id;

        [DataElement()]
        public byte CareerLine;

        [DataElement(Varchar = 255)]
        public string Class;

        [DataElement(Varchar = 255)]
        public string Name;

        [DataElement()]
        public string Prerequirements;

        [DataElement()]
        public uint[] CastOn;

        [DataElement()]
        public string Trigger;

        [DataElement()]
        public uint[] TriggerdBy;

        [DataElement()]
        public uint[] Chance;

        [DataElement()]
        public string Affecting;

        [DataElement()]
        public string Effect;

        [DataElement()]
        public string Behaviour;

        [DataElement()]
        public uint Mult;

        [DataElement()]
        public uint Value;

        [DataElement()]
        public uint OverTime;

        [DataElement()]
        public uint TicksEvery;

        [DataElement()]
        public uint Duration;

        [DataElement()]
        public uint Charges;

        [DataElement()]
        public uint Stacks;

        [DataElement()]
        public string Ability;

        [DataElement()]
        public uint InternalCD;

        [DataElement()]
        public string lvl40;

        [DataElement()]
        public string lvl39;

        [DataElement()]
        public string lvl38;

        [DataElement()]
        public string lvl37;

        [DataElement()]
        public string lvl36;

        [DataElement()]
        public string lvl35;

        [DataElement()]
        public string lvl34;

        [DataElement()]
        public string lvl33;

        [DataElement()]
        public string lvl32;

        [DataElement()]
        public string lvl31;

        [DataElement()]
        public string lvl30;

        [DataElement()]
        public string lvl29;

        [DataElement()]
        public string lvl28;

        [DataElement()]
        public string lvl27;

        [DataElement()]
        public string lvl26;

        [DataElement()]
        public string lvl25;

        [DataElement()]
        public string lvl24;

        [DataElement()]
        public string lvl23;

        [DataElement()]
        public string lvl22;

        [DataElement()]
        public string lvl21;

        [DataElement()]
        public string lvl20;

        [DataElement()]
        public string lvl19;

        [DataElement()]
        public string lvl18;

        [DataElement()]
        public string lvl17;

        [DataElement()]
        public string lvl16;

        [DataElement()]
        public string lvl15;

        [DataElement()]
        public string lvl14;

        [DataElement()]
        public string lvl13;

        [DataElement()]
        public string lvl12;

        [DataElement()]
        public string lvl11;

        [DataElement()]
        public string lvl10;

        [DataElement()]
        public string lvl9;

        [DataElement()]
        public string lvl8;

        [DataElement()]
        public string lvl7;

        [DataElement()]
        public string lvl6;

        [DataElement()]
        public string lvl5;

        [DataElement()]
        public string lvl4;

        [DataElement()]
        public string lvl3;

        [DataElement()]
        public string lvl2;

        [DataElement()]
        public string lvl1;

        public Ability_Info Info;

        public uint GetCastOn(int Id)
        {
            if (CastOn == null || Id >= CastOn.Length)
                return 0;

            return CastOn[Id];
        }

        public uint GetTriggerdBy(int Id)
        {
            if (TriggerdBy == null || Id >= TriggerdBy.Length)
                return 0;

            return TriggerdBy[Id];
        }

        public uint GetChance(int Id)
        {
            if (Chance == null || Id >= Chance.Length)
                return 0;

            return Chance[Id];
        }
    }
}
