/*leo228
 * Copyright (C) Dawn of Reckoning 2008-2019
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using FrameWork;

namespace Common
{
    public class PieceInformation
    {
        public byte PieceId = 0;
        public UInt16 OffsetX = 0;
        public UInt16 OffsetY = 0;
        public UInt16 Width = 0;
        public UInt16 Height = 0;
        public Bitmap File = null;
        public bool Loaded = false;
    }

    [DataTable(PreCache = false, TableName = "zone_areas", DatabaseName = "World")]
    [Serializable]
    public class Zone_Area : DataObject
    {
        [DataElement()]
        public UInt16 ZoneId;

        [DataElement()]
        public UInt16 AreaId;

        [DataElement()]
        public byte Realm;

        [DataElement()]
        public byte PieceId;

        [DataElement()]
        public uint InfluenceId;

        [DataElement()]
        public uint OrderInfluenceId;

        [DataElement()]
        public uint DestroInfluenceId;

        [DataElement()]
        public ushort TokExploreEntry;

        public PieceInformation Information;

        public bool IsOnArea(ushort PinX, ushort PinY)
        {
            if (Information == null)
                return false;

            PinX = (ushort)(PinX / 64);
            PinY = (ushort)(PinY / 64);

            Log.Info(InfluenceId + ",IsOnArea," + PieceId, "PinX=" + PinX + ",PinY=" + PinY + ",OX=" + Information.OffsetX + ",OY=" + Information.OffsetY + ",Size=" + Information.Width + "," + Information.Height);

            if (Information.OffsetX > PinX)
                return false;

            if (Information.OffsetY > PinY)
                return false;

            if (Information.OffsetX + Information.Width < PinX)
                return false;

            if (Information.OffsetY + Information.Height < PinY)
                return false;

            return true;
        }

        public bool IsLoaded()
        {
            if (Information == null)
                return true;

            return Information.Loaded;
        }

        public override string ToString()
        {
            return "[Area] " + AreaId + " PieceId=" + PieceId;
        }
    }
}
