/*// LEO
 * Copyright (C) 2019 Dawn of Reckoning
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "gameobject_spawns", DatabaseName = "World")]
    [Serializable]
    public class GameObject_spawn : DataObject
    {
        public GameObject_proto Proto;

        [PrimaryKey(AutoIncrement=true)]
        public uint Guid;

        [DataElement()]
        public uint Entry;

        [DataElement()]
        public ushort ZoneId;

        [DataElement()]
        public int WorldX;

        [DataElement()]
        public int WorldY;

        [DataElement()]
        public int WorldZ;

        [DataElement()]
        public int WorldO;

        [DataElement()]
        public uint DisplayID;

        [DataElement(AllowDbNull = true)]
        public UInt16[] Unks = new UInt16[6];

        public UInt16 GetUnk(int Id)
        {
            if (Id >= Unks.Length)
                return 0;

            return Unks[Id];
        }

        [DataElement()]
        public byte Unk1;

        [DataElement()]
        public byte Unk2;

        [DataElement()]
        public UInt32 Unk3;

        [DataElement()]
        public UInt32 Unk4;


        [DataElement()]
        public uint DoorId;


        [DataElement()]
        public uint DoorOpen;



    
        [DataElement()]
        public UInt16 GameObjTimer;



        public void BuildFromProto(GameObject_proto Proto)
        {
            this.Proto = Proto;
            Entry = Proto.Entry;
            Unks = Proto.Unks;
            DisplayID = Proto.DisplayID;
        }
    }
}
