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
    [DataTable(PreCache = false, TableName = "creature_spawns", DatabaseName = "World")]
    [Serializable]
    public class Creature_spawn : DataObject
    {
        public Creature_proto Proto;

        private uint _Guid;
        private uint _Entry;
        private ushort _ZoneId;
        public int _WorldX;
        public int _WorldY;
        public int _WorldZ;
        public int _WorldO;
        private string _Bytes;
        private byte _Icone;
        private byte _Emote;
        public byte _Unk1;

        public byte _IconByte;
        private byte _Val;
        private ushort _Title;
        private byte _Faction;
        public int _Level;
        private byte _D1;
        private byte _D2;
        public int _Flag;
        private byte _Head0;
        public byte _Head1;
        private ushort _Heading;
        private byte _EffectOn;
        private ushort _Effect;
        private int _Sound1;
        private int _Sound2;
        private byte _EffectColour;
        private byte _Animation;



        [PrimaryKey(AutoIncrement = true)]
        public uint Guid
        {
            get { return _Guid; }
            set { _Guid = value; Dirty = true; }
        }

        [DataElement(AllowDbNull=false)]
        public uint Entry
        {
            get { return _Entry; }
            set { _Entry = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort ZoneId
        {
            get { return _ZoneId; }
            set { _ZoneId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int WorldX
        {
            get { return _WorldX; }
            set { _WorldX = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int WorldY
        {
            get { return _WorldY; }
            set { _WorldY = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int WorldZ
        {
            get { return _WorldZ; }
            set { _WorldZ = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int WorldO
        {
            get { return _WorldO; }
            set { _WorldO = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public string Bytes
        {
            get { return _Bytes; }
            set { _Bytes = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Icone
        {
            get { return _Icone; }
            set { _Icone = value; Dirty = true; }
        }




        [DataElement(AllowDbNull = false)]
        public byte Val
        {
            get { return _Val; }
            set { _Val = value; Dirty = true; }
        }





        [DataElement(AllowDbNull = false)]
        public byte Emote
        {
            get { return _Emote; }
            set { _Emote = value; Dirty = true; }
        }


        [DataElement(AllowDbNull = false)]
        public byte Unk1
        {
            get { return _Unk1; }
            set { _Unk1 = value; Dirty = true; }
        }



        [DataElement(AllowDbNull = false)]
        public byte IconByte
        {
            get { return _IconByte; }
            set { _IconByte = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort Title
        {
            get { return _Title; }
            set { _Title = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Faction
        {
            get { return _Faction; }
            set { _Faction = value; Dirty = true; }
        }




        
        [DataElement(AllowDbNull = false)]
        public byte D1
        {
            get { return _D1; }
            set { _D1 = value; Dirty = true; }

        }


        [DataElement(AllowDbNull = false)]
        public byte D2
        {
            get { return _D2; }
            set { _D2 = value; Dirty = true; }


        }

        [DataElement(AllowDbNull = false)]
        public byte Head0
        {
            get { return _Head0; }
            set { _Head0 = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Head1
        {
            get { return _Head1; }
            set { _Head1 = value; Dirty = true; }

        }

        [DataElement(AllowDbNull = false)]
        public ushort Heading
        {
            get { return _Heading; }
            set { _Heading = value; Dirty = true; }
        }


        [DataElement(AllowDbNull = false)]
        public byte EffectOn
        {
            get { return _EffectOn; }
            set { _EffectOn = value; Dirty = true; }

        }


        /////////////////////added
        [DataElement(AllowDbNull = false)]
        public ushort Effect
        {
            get { return _Effect; }
            set { _Effect = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte EffectColour
        {
            get { return _EffectColour; }
            set { _EffectColour = value; Dirty = true; }

        }

        [DataElement(AllowDbNull = false)]
        public byte Animation
        {
            get { return _Animation; }
            set { _Animation = value; Dirty = true; }
        }

        ///////added
        [DataElement(AllowDbNull = false)]
        public int Sound1
        {
            get { return _Sound1; }
            set { _Sound1 = value; Dirty = true; }
        }


        ///////added
        [DataElement(AllowDbNull = false)]
        public int Sound2
        {
            get { return _Sound2; }
            set { _Sound2 = value; Dirty = true; }
        }


        //added
        [DataElement(AllowDbNull = false)]
        public int Flag
        {
            get { return _Flag; }
            set { _Flag = value; Dirty = true; }
        }

        //added
        [DataElement(AllowDbNull = false)]
        public int Level
        {
            get { return _Level; }
            set { _Level = value; Dirty = true; }


        }



        [DataElement()]
        public byte WaypointType = 0; // 0 = Loop Start->End->Start, 1 = Start->End, 2 = Random

        public byte[] bBytes
        {
            get
            {
                List<byte> Btes = new List<byte>();
                string[] Strs = _Bytes.Split(';');
                foreach (string Str in Strs)
                    if (Str.Length > 0)
                        Btes.Add(byte.Parse(Str));

                Btes.Remove(4);
                Btes.Remove(5);
                Btes.Remove(7);

                return Btes.ToArray();
            }
        }

        public void BuildFromProto(Creature_proto Proto)
        {
            this.Proto = Proto;
            Entry = Proto.Entry;
            Title = Proto.Title;
            Emote = Proto.Emote;
            Bytes = Proto.Bytes;
            Icone = Proto.Icone;
        }
    }
}
