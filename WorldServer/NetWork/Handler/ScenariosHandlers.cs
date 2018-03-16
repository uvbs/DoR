using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;



namespace WorldServer
{
    public class ScenariosHandlers
    {
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_INTERACT_QUEUE, "onInteractQueue")]
        static public void F_INTERACT_QUEUE(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;
            if (!cclient.IsPlaying() || !cclient.Plr.IsInWorld())
                return;

            Player Plr = cclient.Plr;

            packet.Skip(5);
            byte State = packet.GetUint8();
            UInt16 Scenario = packet.GetUint16();

            switch (State)
            {

                /// <summary>
                /// This will put you in the queue for a scenario
                /// STATE 1 YOUR IN SCENARIO QUEUE
                /// THIS MAKES IT FLASH THE ICON
                /// </summary>
                case 1:// signed up
                     Log.Success("ScenariosHandlers", Plr.Name + "  has joined scenario queue " + Scenario);
                    PacketOut Out1 = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                    Out1.WriteByte(9);// scenario
                    Out1.WriteByte(6);//STATE//0=scenaro list)  1=in queue) 2=removed from qeue)6=scenaro ready
                    Out1.WriteUInt16(0);//unknown
                    Out1.WriteUInt16(0x0834);//Serpent's Passage SCENARO//ScenarioID  List in Scenarios//0x089C
                    Out1.WriteUInt16(0);//unknown
                    Out1.WriteUInt16(0x07D0);//Gates of Ekrund SCENARO//ScenarioID
                    Plr.SendPacket(Out1);



/*

                    Log.Success("ScenariosHandlers", Plr.Name + "  has joined scenario queue " + Scenario);
                    //WorldMgr.ScenarioMgr.ScenarioJoin(Plr, Scenario);
                    PacketOut Out1 = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                    Out1.WriteByte(9);// scenario
                    Out1.WriteByte(1);//STATE//0=scenaro list)  1=in queue) 2=removed from qeue)6=scenaro ready
                    Out1.WriteUInt16(0);//unknown
                    Out1.WriteUInt16(0x089C);//Serpent's Passage SCENARO//ScenarioID  List in Scenarios//0x089C
                    Plr.SendPacket(Out1);

*/


                    break;

                default:

                    Log.Error("ScenariosHandlers", "Unhandled state: " + State);

                    break;


                /// <summary>
                /// This leaves queue for a scenario
                /// STATE 2 You are no longer in the queue
                /// THIS REMOVES U FROM THE QUEUE NEED TO ADD STRING// THAT SHOWS JOIN AND LEAVE IN CHAT BOX
                /// </summary>
                case 2:
                    Log.Success("ScenariosHandlers", Plr.Name + " leaves scenario queue " + Scenario);

                    PacketOut Out2 = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                    Out2.WriteByte(9);
                    Out2.WriteByte(2);
                    Out2.WriteUInt16(0);
              //      Out2.WriteUInt16(0x0834);
                    Plr.SendPacket(Out2);

                    break;

                /// <summary>
                /// This makes the scenario ready for you to join
                /// STATE 6 The scenario is ready for you to join
                /// </summary>
                case 3:
                    Log.Success("ScenariosHandlers", Plr.Name + " join scenario " + Scenario);

                    PacketOut Out3 = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                    Out3.WriteByte(9);
                    Out3.WriteByte(6);
                    Out3.WriteUInt16(0);
                    Out3.WriteUInt16(0x07D0);
                    //Out3.WriteUInt16(0x089C);//Serpent's Passage SCENARO//ScenarioID
                    Out3.WriteUInt16(0);
                    Out3.WriteUInt16(0x0834);//Nordenwatch SCENARO//ScenarioID
                    Plr.SendPacket(Out3);

                    //Plr.Teleport(234, 631034, 359179, (UInt16)12176, (UInt16)154);//Serpent's Passage

                    Plr.Teleport(130, 364585, 360055, (UInt16)7180, (UInt16)89);// i added all this// ports to nordenwatch scenaro

                    // Plr.Teleport(30, 194972, 198481, (UInt16)18024, (UInt16)47);//gates of ekrund
                    break;



            }
        }
    }
}

