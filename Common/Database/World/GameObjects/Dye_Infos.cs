using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using FrameWork;


namespace Common
{



    [DataTable(PreCache = false, TableName = "Dye_Infos", DatabaseName = "World")]
    [Serializable]
    public class Dye_Info : DataObject
    {
 
        public ushort _Entry;
        public UInt32 _Price;
        public byte _Count;




        [DataElement(AllowDbNull = false)]
        public ushort Entry
        {
            get { return _Entry; }
            set { _Entry = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt32 Price
        {
            get { return _Price; }
            set { _Price = value; Dirty = true; }
        }

    }
}
