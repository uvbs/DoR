using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace Common
{
    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "ClientData", DatabaseName = "World")]
    [Serializable]
    public class ClientData : DataObject
    {

        [DataElement()]
        public ushort PlayerID;


        [DataElement()]
        public ushort SlotIndex;


        [DataElement()]
        public ushort AbilityID;
    }
}
