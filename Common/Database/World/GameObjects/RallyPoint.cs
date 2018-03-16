using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;


namespace Common
{

    [DataTable(PreCache = false, TableName = "RallyPoints", DatabaseName = "World")]
    [Serializable]
    public class RallyPoint : DataObject
    {
        private ushort _Id;


        [DataElement(AllowDbNull = false)]
        public ushort Id
        {
            get { return _Id; }
            set { _Id = value; Dirty = true; }
        }
    }
}
