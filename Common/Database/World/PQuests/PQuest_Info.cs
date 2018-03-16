/*
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "pquest_info", DatabaseName = "World")]
    [Serializable]
    public class PQuest_Info : DataObject
    {
        [PrimaryKey()]
        public UInt32 Entry;

        [DataElement(Varchar=255,AllowDbNull=false)]
        public string Name;

        [DataElement(AllowDbNull = false)]
        public byte Type;

        [DataElement(AllowDbNull = false)]
        public byte Level;

        [DataElement(AllowDbNull = false)]
        public UInt16 ZoneId;

        [DataElement(AllowDbNull = false)]
        public UInt32 PinX;

        [DataElement(AllowDbNull = false)]
        public UInt32 PinY;

        public UInt32 ChapterId;

        [DataElement(AllowDbNull = false)]
        public UInt32 TokDiscovered;

        [DataElement(AllowDbNull = false)]
        public UInt32 TokUnlocked;


        [DataElement(AllowDbNull = false)]
        public byte ObjectiveSize;


        [DataElement(AllowDbNull = false)]
        public byte AreaId2;


        [DataElement(AllowDbNull = false)]
        public byte RVR;



        public ushort OffX;
        public ushort OffY;
        public List<PQuest_Objective> Objectives;
    }
}
