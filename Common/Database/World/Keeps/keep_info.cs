using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    [DataTable(PreCache = false, TableName = "keep_infos", DatabaseName = "World")]
    [Serializable]
    public class Keep_Info : DataObject
    {
        private UInt32 _KeepId;
        //private string _Name;

       // private byte _KeepId;
        private string _KeepName;

        [DataElement(Unique = false)]
        public UInt32 KeepId
        {
            get { return _KeepId; }
            set { _KeepId = value; Dirty = true; }
        }

        [DataElement(Varchar = 255)]
        public string KeepName
        {
            get { return _KeepName; }
            set { _KeepName = value; Dirty = true; }
        }
    }
}