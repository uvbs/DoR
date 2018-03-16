using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;



namespace WorldServer
{


    public class Scenarios
    {

        public void SendScenarios(Player Plr)
        {


            /// <summary>
            /// This will light up icon with the lobby maps of the scenarios
            /// </summary>


            PacketOut Out1 = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);//Size = 9
            Out1.WriteByte(9);
            Out1.WriteUInt16(2);//WAS 2//How many scenarios available (0 = Your scenarios are currently unavailable at this time) 
            Out1.WriteUInt16(0);
            Out1.WriteUInt16(0x07D0);//Gates of Ekrund SCENARO//ScenarioID
            Out1.WriteUInt16(0);
            Out1.WriteUInt16(0x0834);//Nordenwatch SCENARO//ScenarioID
            Plr.SendPacket(Out1);



/*
            //
            // shows scenaro lobby with maps of scenaros
            PacketOut Out1 = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);//Size = 9
            Out1.WriteByte(9);
            //Out1.WriteByte(6);//next byte 0=scenaro list 1=in queue 2=removed from qeue 6=scenaro ready
            Out1.WriteUInt16(36);//WAS 2//How many scenarios available (0 = Your scenarios are currently unavailable at this time) 
            //Out1.WriteUInt16(0);
            //Out1.WriteUInt16(0x07D0);//Gates of Ekrund SCENARO//ScenarioID
            Out1.WriteUInt16(0);
            Out1.WriteUInt16(0x0834);//Nordenwatch SCENARO//ScenarioID
            //Out1.WriteUInt16(0);
            //Out1.WriteUInt16(0x0898);//Khaine's Embrace //ScenarioID
            //Out1.WriteUInt16(0);
            //Out1.WriteUInt16(0x089C);//Serpent's Passage//ScenarioID
            Plr.SendPacket(Out1);
            */


        }
    }
}