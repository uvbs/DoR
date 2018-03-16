/*
 * Copyright (C) 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class ClientDatas : IPacketHandler
    {
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_CLIENT_DATA, 0, "F_CLIENT_DATA")]
        static public void F_CLIENT_DATA(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;
            //Log.Dump("FCLIENT", packet, true);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_UI_MOD, 0, "F_UI_MOD")]
        static public void F_UI_MOD(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;
            //Log.Dump("F_UI_MOD", packet, true);
        }
    }
}
