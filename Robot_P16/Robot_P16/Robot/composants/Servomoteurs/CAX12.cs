using System;
using System.IO;
using System.Threading;
using System.IO.Ports;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
//using GHI.Premium.Hardware;
//using GHI.Premium.Hardware.LowLevel;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using GHI.Processor;  // assembly GHI.Hardware

//https://old.ghielectronics.com/community/codeshare/entry/350
// https://old.ghielectronics.com/community/forum/topic?id=8035&page=2
// http://support.robotis.com/en/product/actuator/dynamixel/communication/dxl_instruction.htm
// http://www.crustcrawler.com/products/bioloid/docs/AX-12.pdf
// http://forums.trossenrobotics.com/tutorials/how-to-diy-128/controlling-ax-12-servos-3275/


namespace Robot_P16.Robot.composants.Servomoteurs
{
    public enum AX12Mode { joint, wheel };

    public enum speed { stop = 0, reverse = 1023, forward = 2047 }

    public enum Instruction : byte
    {
        AX_PING = 0x01,
        AX_READ_DATA = 0x02,
        AX_WRITE_DATA = 0x03,
        AX_REG_WRITE = 0x04,
        AX_ACTION = 0x05,
        AX_RESET = 0x06,
        AX_SYNC_WRITE = 0x83,
    }

    public enum Address : byte
    {
        AX_MODEL_NUMBER = 0x00,
        AX_VERSION_FIRMWARE = 0x02,
        AX_ID = 0x03,
        AX_BAUD_RATE = 0x04,
        AX_RETURN_DELAY = 0x05,
        AX_CW_LIMIT = 0x06,
        AX_CCW_LIMIT = 0x08,
        AX_HIGH_LIMIT_TEMPERATURE = 0x0B,
        AX_LIMIT_VOLTAGE = 0x0C,
        AX_MAX_TORQUE = 0x0E,
        AX_STATUS_RETURN_LEVEL = 0x10,
        AX_ALARM_LED = 0x11,
        AX_ALARM_SHUTDOWN = 0x12,
        // 0x13 : R�serv�e
        AX_DOWN_CALIBRATION = 0x14,
        AX_UP_CALIBRATION = 0x16,
        AX_TORQUE_ENABLE = 0x18,
        AX_LED = 0x19,
        AX_CW_COMPLIANCE_MARGIN = 0x1A,
        AX_CCW_COMPLIANCE_MARGIN = 0x1B,
        AX_CW_COMPLIANCE_SLOPE = 0x1C,
        AX_CCW_COMPLIANCE_SLOPE = 0x1D,
        AX_GOAL_POSITION = 0x1E,

        AX_MOVING_SPEED = 020,
        AX_TORQUE_LIMIT = 0x22,
        AX_PRESENT_POSITION = 0x24,
        AX_PRESENT_SPEED = 0x26,
        AX_PRESENT_LOAD = 0x28,
        AX_PRESENT_VOLTAGE = 0x2A,
        AX_PRESENT_TEMPERATURE = 0x2B,
        AX_REGISTERED_INSTRUCTION = 0x2C,
        // 0x2D = R�serv�e
        AX_MOVING = 0x2E,
        AX_LOCK = 0x2F,
        AX_PUNCH = 0x30,


    }

    public class CAX_12
    {
        OutputPort m_Direction;
        SerialPort m_serial;
        private byte[] m_commande = new byte[20];
        public byte m_ID = 0;
        AX12Mode m_mode = 0;
        uint m_posCourrante = 0, m_posPrecedente = 0;
        int m_nbTour = 0;
        speed m_speed = speed.stop;

        int TEMPORAIRE_position = 0;

        public CAX_12(byte ID, SerialPort portserie, OutputPort direction)
        {
            m_serial = portserie;
            m_ID = ID;
            m_Direction = direction;

            Register U0FDR = new Register(0xE000C028, (8 << 4) | 1);

            Register PINSEL0 = new Register(0xE002C000);

            m_Direction.Write(true); // Servo en mode reception uniquement

        }


        public int setMode(AX12Mode modeAX)
        {
            m_mode = modeAX;
            if (m_mode == AX12Mode.joint)
            {
                int CW = 0;
                int CCW = 1023;
                byte[] limitsCW = { 0x06, (byte)CW, (byte)(CW >> 8) };
                byte[] limitsCCW = { 0x08, (byte)CCW, (byte)(CCW >> 8) };
                sendCommand(m_ID, Instruction.AX_WRITE_DATA, limitsCW);
                sendCommand(m_ID, Instruction.AX_WRITE_DATA, limitsCCW);
            }

            else if (m_mode == AX12Mode.wheel)
            {
                int CW = 0;
                int CCW = 0;
                byte[] limitsCW = { 0x06, (byte)CW, (byte)(CW >> 8) };
                byte[] limitsCCW = { 0x08, (byte)CCW, (byte)(CCW >> 8) };
                sendCommand(m_ID, Instruction.AX_WRITE_DATA, limitsCW);
                sendCommand(m_ID, Instruction.AX_WRITE_DATA, limitsCCW);

            }
            return -1;

        }

        private byte calculeCRC()
        {
            int taille = m_commande[3] + 2;
            byte crc = 0;
            for (int i = 2; i < taille + 1; i++)
            {
                crc += m_commande[i];
            }
            return (byte)(0xFF - crc);
        }



        public bool sendCommand(byte ID, Instruction instruction, byte[] parametres)
        {
            bool error = false;
            int length = 0;
            if (parametres != null)
            {
                length = parametres.Length;
            }

            m_commande[0] = 0xFF;
            m_commande[1] = 0XFF;
            m_commande[2] = ID;
            m_commande[3] = (byte)(length + 2);//len
            m_commande[4] = (byte)instruction;

            for (int i = 5; i < length + 5; i++)
            {
                m_commande[i] = parametres[i - 5];
            }
            m_commande[length + 5] = calculeCRC();


            // send data
            if (m_serial.IsOpen)
            {
                // UART en transmission TX activ�
                m_Direction.Write(true);
                //   m_serial.DiscardInBuffer();
                Thread.Sleep(10);
                //m_serial.DiscardOutBuffer();
                m_serial.Write(m_commande, 0, length + 6);
                //wait till all is sent

                while (m_serial.BytesToWrite > 0) ;

                m_serial.DiscardInBuffer();
                // UART en transmission TX desactiv�
                m_Direction.Write(false);
                // Thread.Sleep(100);
                error = true;
            }
            return error;
            //the response is now coming back so you must read it
        }


        public bool move(int value)
        {
            bool erreur = false;

            if (m_mode == AX12Mode.joint)
            {
                Informations.printInformations(Priority.LOW, "Servo ID "+m_ID+" en mode joint, moving...");
                byte[] buf = { 0x1E, (byte)(value), (byte)(value >> 8) };

                erreur = sendCommand(m_ID, Instruction.AX_WRITE_DATA, buf);
                Informations.printInformations(Priority.LOW, "Servo move joint : status : "+erreur);
                TEMPORAIRE_position = value;
            }

            Informations.printInformations(Priority.LOW, "Servo move : status : " + erreur);
            return erreur;

        }

        public int setMovingSpeed(speed vitesse)
        {
            m_speed = vitesse;
            int value = (int)vitesse;
            if (m_mode == AX12Mode.wheel)
            {
                byte[] buf = { 0x20, (byte)(value), (byte)(value >> 8) };
                sendCommand(m_ID, Instruction.AX_WRITE_DATA, buf);
            }

            return -1;
        }



    }
}

