using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;


namespace WS2300
{
    class WEATHERSTATION : System.IO.Ports.SerialPort
    {
        const int BAUDRATE = 2400;
        const string DEFAULT_SERIAL_DEVICE = "COM1";

        public WEATHERSTATION(String port)
        {
            if (port.Trim().Length == 0) port = DEFAULT_SERIAL_DEVICE;
            base.PortName = port;
            base.BaudRate = BAUDRATE;
            base.DtrEnable = false;
            base.RtsEnable = true;
            base.DataBits = 8;
            base.Parity = Parity.None;
            base.Handshake = Handshake.None;
            base.ReadTimeout = 175;
            base.WriteTimeout = 175;
            base.Open();
        }

        public string status()
        {
            string str = "CLOSED";
            if (base.IsOpen)
            {
                str = base.PortName.ToString();
                str += ": " + base.BaudRate.ToString();
                str += "," + base.Parity.ToString().Substring(0, 1);
                str += "," + base.DataBits.ToString();
                str += "," + base.StopBits.ToString();
                str += " HndShk:" + base.Handshake.ToString();
            }
            return str;
        }
    }
}
