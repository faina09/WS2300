using System;
using System.Collections.Generic;
using System.Text;

/*  C# code based on OpenSource Project open2300  - win2300 library functions
 *  
 *  Version 1.10
 *  
 *  Control WS2300 weather station
 *  
 *  Copyright 2003-2005, Kenneth Lavrsen
 *  This program is published under the GNU General Public license
 */

namespace WS2300
{
    class WS2300base
    {
        private WEATHERSTATION wsdevice;
        public struct config_type
        {
            //char timezone[6];                //not integer because of half hour time zones
            public double wind_speed_conv_factor;     //from m/s to km/h or miles/hour
            public int temperature_conv;              //0=Celsius, 1=Fahrenheit
            public double rain_conv_factor;           //from mm to inch
            public double pressure_conv_factor;       //from hPa (=millibar) to mmHg
            public string history_log_type;           //cvs or sql
            public string port;
        }

        public struct timestamp
        {
            public int minute;
            public int hour;
            public int day;
            public int month;
            public int year;
        }

        #region ws2300

        const int MAXRETRIES = 50;
        const int MAXWINDRETRIES = 20;
        const int WRITENIB = 0x42;
        const int SETBIT = 0x12;
        const int UNSETBIT = 0x32;
        const int WRITEACK = 0x10;
        const int SETACK = 0x04;
        const int UNSETACK = 0x0C;
        public const byte RESET_MIN = 0x01;
        public const byte RESET_MAX = 0x02;

        const double METERS_PER_SECOND = 1.0;
        const double KILOMETERS_PER_HOUR = 3.6;
        const double MILES_PER_HOUR = 2.23693629;
        const int CELSIUS = 0;
        const int FAHRENHEIT = 1;
        const int MILLIMETERS = 1;
        const double INCHES = 25.4;
        const double HECTOPASCAL = 1.0;
        const double MILLIBARS = 1.0;
        const double INCHES_HG = 33.8638864;

        public WS2300base(String port)
        {
            wsdevice = new WEATHERSTATION(port);
        }

        public void close()
        {
            wsdevice.Close();
        }

        /********************************************************************
         * reset_06 WS2300 by sending command 06 (windows version) 
         * 
         * Input:   device number of the already open serial port
         *           
         * Returns: nothing, exits progrsm if failing to reset
         *
         ********************************************************************/
        void reset_06()
        {
            byte[] command = { 0x06 };
            byte[] answer = { 0xFF };
            int i;

            for (i = 0; i < MAXRETRIES; i++)
            {
                //printf("Iteration = " + i.ToString(), true);
                //PurgeComm(serdevice, PURGE_RXCLEAR);
                wsdevice.DiscardInBuffer();
                write_device(command, 1);

                // Occasionally 0, then 2 is returned.  If zero comes back, continue
                // reading as this is more efficient than sending an out-of sync
                // reset and letting the data reads restore synchronization.
                // Occasionally, multiple 2's are returned.  Read with a fast timeout
                // until all data is exhausted, if we got a two back at all, we
                // consider it a success
                while (1 == read_device(answer, 1))
                {
                    if (answer[0] == 2)
                    {
                        // clear anything that might come after the response
                        //PurgeComm(serdevice, PURGE_RXCLEAR);
                        wsdevice.DiscardInBuffer();
                        return;
                    }
                }
                Sleep(5 * i);
            }
            printf("Could not reset");
            exit(0);
        }

        /********************************************************************
         * read_device WIN32 emulation of Linux read() 
         * Reads data from the handle
         *
         * Inputs:  serdevice - opened file handle
         *          buffer - pointer to the buffer to read into
         *          size - number of bytes to read
         *
         * Output:  *buffer - modified on success
         * 
         * Returns: number of bytes read
         *
         ********************************************************************/
        int read_device(byte[] buffer, int size)
        {
            int i = 0;
            try
            {
                //while (serdevice.InBufferBytes > 0 && i < size)
                //while (serdevice.BytesToRead > 0 && i < size)
                {
                    buffer[i++] = (byte)wsdevice.ReadByte();
                }
            }
            catch
            {
                return -1;
            }
            return i;
            /*
            DWORD dwRead = 0;

            if (!ReadFile(serdevice, buffer, size, &dwRead, NULL))
            {
                return -1;
            }

            return (int)dwRead;
            */
        }
        int read_device(ref byte buffer)
        {
            byte[] buff ={ buffer };
            int i = read_device(buff, 1);
            buffer = buff[0];
            return i;
        }

        /********************************************************************
         * write_device WIN32 emulation of Linux write() 
         * Writes data to the handle
         *
         * Inputs:  serdevice - opened file handle
         *          buffer - pointer to the buffer to write from
         *          size - number of bytes to write
         *
         * Returns: number of bytes written
         *
         ********************************************************************/
        int write_device(byte[] buffer, int size)
        {
            try
            {
                wsdevice.Write(buffer, 0, size);
            }
            catch
            {
                size = -1;
            }
            return size;
            /*
            DWORD dwWritten;

            if (!WriteFile(serdevice, buffer, size, &dwWritten, NULL))
            {
                return -1;
            }

            return (int)dwWritten;
             * */
        }
        int write_device(byte buffer)
        {
            byte[] buff ={ buffer };
            return write_device(buff, 1);
        }

        /********************************************************************
         * sleep_short - Windows version
         * 
         * Inputs: Time in milliseconds (integer)
         *
         * Returns: nothing
         *
         ********************************************************************/
        void sleep_short(int milliseconds)
        {
            Sleep(milliseconds);
        }

        /********************************************************************
         * sleep_long - Windows version
         * 
         * Inputs: Time in seconds (integer)
         *
         * Returns: nothing
         *
         ********************************************************************/
        void sleep_long(int seconds)
        {
            Sleep(seconds * 1000);
        }


        /********************************************************************
         * address_encoder converts an 16 bit address to the form needed
         * by the WS-2300 when sending commands.
         *
         * Input:   address_in (interger - 16 bit)
         * 
         * Output:  address_out - Pointer to an unsigned character array.
         *          3 bytes, not zero terminated.
         * 
         * Returns: Nothing.
         *
         ********************************************************************/
        private void address_encoder(int address_in, byte[] address_out)
        {
            int i = 0;
            int adrbytes = 4;
            int shift;
            byte nibble;

            for (i = 0; i < adrbytes; i++)
            {
                //nibble = (byte)((byte)(address_in >> (4 * (3 - i))) & 0x0F);
                shift = address_in >> (4 * (3 - i));
                nibble = (byte)((byte)shift & 0x0F);
                address_out[i] = (byte)(0x82 + (nibble * 4));
            }

            return;
        }

        /********************************************************************
        * data_encoder converts up to 15 data bytes to the form needed
        * by the WS-2300 when sending write commands.
        *
        * Input:   number - number of databytes (integer)
        *          encode_constant - unsigned char
        *                            0x12=set bit, 0x32=unset bit, 0x42=write nibble
        *          data_in - char array with up to 15 hex values
        * 
        * Output:  address_out - Pointer to an unsigned character array.
        * 
        * Returns: Nothing.
        *
        ********************************************************************/
        void data_encoder(int number, byte encode_constant,
                          byte[] data_in, byte[] data_out)
        {
            int i = 0;

            for (i = 0; i < number; i++)
            {
                data_out[i] = (byte)(encode_constant + (data_in[i] * 4));
            }

            return;
        }


        /********************************************************************
         * numberof_encoder converts the number of bytes we want to read
         * to the form needed by the WS-2300 when sending commands.
         *
         * Input:   number interger, max value 15
         * 
         * Returns: bytewhich is the coded number of bytes
         *
         ********************************************************************/
        byte numberof_encoder(int number)
        {
            int coded_number;

            coded_number = (byte)(0xC2 + number * 4);
            if (coded_number > 0xfe)
                coded_number = 0xfe;

            return (byte)coded_number;
        }


        /********************************************************************
         * command_check0123 calculates the checksum for the first 4
         * commands sent to WS2300.
         *
         * Input:   pointer to char to check
         *          sequence of command - i.e. 0, 1, 2 or 3.
         * 
         * Returns: calculated checksum as unsigned char
         *
         ********************************************************************/
        byte command_check0123(byte command, int sequence)
        {
            int response;

            response = sequence * 16 + ((command) - 0x82) / 4;

            return (byte)response;
        }


        /********************************************************************
         * command_check4 calculates the checksum for the last command
         * which is sent just before data is received from WS2300
         *
         * Input: number of bytes requested
         * 
         * Returns: expected response from requesting number of bytes
         *
         ********************************************************************/
        byte command_check4(int number)
        {
            int response;

            response = 0x30 + number;

            return (byte)response;
        }


        /********************************************************************
         * data_checksum calculates the checksum for the data bytes received
         * from the WS2300
         *
         * Input:   pointer to array of data to check
         *          number of bytes in array
         * 
         * Returns: calculated checksum as unsigned char
         *
         ********************************************************************/
        byte data_checksum(byte[] data, int number)
        {
            int checksum = 0;
            int i;

            for (i = 0; i < number; i++)
            {
                checksum += data[i];
            }

            checksum &= 0xFF;

            return (byte)checksum;
        }


        /********************************************************************
         * initialize resets WS2300 to cold start (rewind and start over)
         * 
         * Input:   device number of the already open serial port
         *           
         * Returns: 0 if fail, 1 if success
         *
         ********************************************************************/
        int initialize()
        {
            byte[] command = { 0x06 };
            byte[] answer = { 0xFF };

            write_device(command, 1);

            if (read_device(answer, 1) != 1)
                return 0;

            write_device(command, 1);
            write_device(command, 1);

            if (read_device(answer, 1) != 1)
                return 0;

            write_device(command, 1);

            if (read_device(answer, 1) != 1)
                return 0;

            write_device(command, 1);

            if (read_device(answer, 1) != 1)
                return 0;

            if (answer[0] != 2)
                return 0;

            return 1;
        }


        /********************************************************************
         * read_data reads data from the WS2300 based on a given address,
         * number of data read, and a an already open serial port
         *
         * Inputs:  serdevice - device number of the already open serial port
         *          address (interger - 16 bit)
         *          number - number of bytes to read, max value 15
         *
         * Output:  readdata - pointer to an array of chars containing
         *                     the just read data, not zero terminated
         *          commanddata - pointer to an array of chars containing
         *                     the commands that were sent to the station
         * 
         * Returns: number of bytes read, -1 if failed
         *
         ********************************************************************/
        int read_data(int address, int number,
                      byte[] readdata, byte[] commanddata)
        {
            byte[] answer = { 0xFF };
            int i;

            // First 4 bytes are populated with converted address range 0000-13B0
            address_encoder(address, commanddata);
            // Last populate the 5th byte with the converted number of bytes
            commanddata[4] = numberof_encoder(number);

            for (i = 0; i < 4; i++)
            {
                if (write_device(commanddata[i]) != 1)
                    return -1;
                if (read_device(answer, 1) != 1)
                    return -1;
                if (answer[0] != command_check0123(commanddata[i], i))
                    return -1;
            }

            //Send the final command that asks for 'number' of bytes, check answer
            if (write_device(commanddata[4]) != 1)
                return -1;
            if (read_device(answer, 1) != 1)
                return -1;
            if (answer[0] != command_check4(number))
                return -1;

            //Read the data bytes
            for (i = 0; i < number; i++)
            {
                if (read_device(ref readdata[i]) != 1)
                    return -1;
            }

            //Read and verify checksum
            if (read_device(answer, 1) != 1)
                return -1;
            if (answer[0] != data_checksum(readdata, number))
                return -1;

            return i;

        }


        /********************************************************************
         * write_data writes data to the WS2300.
         * It can both write nibbles and set/unset bits
         *
         * Inputs:      ws2300 - device number of the already open serial port
         *              address (interger - 16 bit)
         *              number - number of nibbles to be written/changed
         *                       must 1 for bit modes (SETBIT and UNSETBIT)
         *                       max 80 for nibble mode (WRITENIB)
         *              encode_constant - unsigned char
         *                                (SETBIT, UNSETBIT or WRITENIB)
         *              writedata - pointer to an array of chars containing
         *                          data to write, not zero terminated
         *                          data must be in hex - one digit per byte
         *                          If bit mode value must be 0-3 and only
         *                          the first byte can be used.
         * 
         * Output:      commanddata - pointer to an array of chars containing
         *                            the commands that were sent to the station
         *
         * Returns:     number of bytes written, -1 if failed
         *
         ********************************************************************/
        int write_data(int address, int number,
                       byte encode_constant, byte[] writedata,
                       byte[] commanddata)
        {
            byte[] answer ={ 0xFF };
            byte[] encoded_data = new byte[80];
            int i = 0;
            byte ack_constant = WRITEACK;

            if (encode_constant == SETBIT)
            {
                ack_constant = SETACK;
            }
            else if (encode_constant == UNSETBIT)
            {
                ack_constant = UNSETACK;
            }

            // First 4 bytes are populated with converted address range 0000-13XX
            address_encoder(address, commanddata);
            // populate the encoded_data array
            data_encoder(number, encode_constant, writedata, encoded_data);

            //Write the 4 address bytes
            for (i = 0; i < 4; i++)
            {
                if (write_device(commanddata[i]) != 1)
                    return -1;
                if (read_device(answer, 1) != 1)
                    return -1;
                if (answer[0] != command_check0123(commanddata[i], i))
                    return -1;
            }

            //Write the data nibbles or set/unset the bits
            for (i = 0; i < number; i++)
            {
                if (write_device(encoded_data[i]) != 1)
                    return -1;
                if (read_device(answer, 1) != 1)
                    return -1;
                if (answer[0] != (writedata[i] + ack_constant))
                    return -1;
                commanddata[i + 4] = encoded_data[i];
            }

            return i;
        }


        /********************************************************************
         * read_safe Read data, retry until success or maxretries
         * Reads data from the WS2300 based on a given address,
         * number of data read, and a an already open serial port
         * Uses the read_data function and has same interface
         *
         * Inputs:  ws2300 - device number of the already open serial port
         *          address (interger - 16 bit)
         *          number - number of bytes to read, max value 15
         *
         * Output:  readdata - pointer to an array of chars containing
         *                     the just read data, not zero terminated
         *          commanddata - pointer to an array of chars containing
         *                     the commands that were sent to the station
         * 
         * Returns: number of bytes read, -1 if failed
         *
         ********************************************************************/
        int read_safe(int address, int number,
                      byte[] readdata, byte[] commanddata)
        {
            int j;

            for (j = 0; j < MAXRETRIES; j++)
            {
                reset_06();

                // Read the data. If expected number of bytes read break out of loop.
                if (read_data(address, number, readdata, commanddata) == number)
                {
                    break;
                }
            }

            // If we have tried MAXRETRIES times to read we expect not to
            // have valid data
            if (j == MAXRETRIES)
            {
                return -1;
            }

            return number;
        }


        /********************************************************************
         * write_safe Write data, retry until success or maxretries
         * Writes data to the WS2300 based on a given address,
         * number of data to write, and a an already open serial port
         * Uses the write_data function and has same interface
         *
         * Inputs:      serdevice - device number of the already open serial port
         *              address (interger - 16 bit)
         *              number - number of nibbles to be written/changed
         *                       must 1 for bit modes (SETBIT and UNSETBIT)
         *                       unlimited for nibble mode (WRITENIB)
         *              encode_constant - unsigned char
         *                               (SETBIT, UNSETBIT or WRITENIB)
         *              writedata - pointer to an array of chars containing
         *                          data to write, not zero terminated
         *                          data must be in hex - one digit per byte
         *                          If bit mode value must be 0-3 and only
         *                          the first byte can be used.
         * 
         * Output:      commanddata - pointer to an array of chars containing
         *                            the commands that were sent to the station
         * 
         * Returns: number of bytes written, -1 if failed
         *
         ********************************************************************/
        int write_safe(int address, int number,
                       byte encode_constant, byte[] writedata,
                       byte[] commanddata)
        {
            int j;

            for (j = 0; j < MAXRETRIES; j++)
            {
                //printf("Iteration = " + j.ToString(), true);
                reset_06();

                // Read the data. If expected number of bytes read break out of loop.
                if (write_data(address, number, encode_constant, writedata,
                    commanddata) == number)
                {
                    break;
                }
            }

            // If we have tried MAXRETRIES times to read we expect not to
            // have valid data
            if (j == MAXRETRIES)
            {
                return -1;
            }

            return number;
        }

        #endregion

        #region rw2300
        /********************************************************************/
        /* temperature_indoor
         * Read indoor temperature, current temperature only
         * 
         * Input: Handle to weatherstation
         *        temperature_conv flag (integer) controlling
         *            convertion to deg F
         *
         * Returns: Temperature (deg C if temperature_conv is 0)
         *                      (deg F if temperature_conv is 1)
         *
         ********************************************************************/
        public double temperature_indoor(int temperature_conv)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x346;
            int bytes = 2;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            if (temperature_conv == 1)
                return ((((data[1] >> 4) * 10 + (data[1] & 0xF) +
                          (data[0] >> 4) / 10.0 + (data[0] & 0xF) / 100.0) -
                          30.0) * 9 / 5 + 32);
            else
                return ((((data[1] >> 4) * 10 + (data[1] & 0xF) +
                          (data[0] >> 4) / 10.0 + (data[0] & 0xF) / 100.0) - 30.0));
        }


        /********************************************************************/
        /* temperature_indoor_minmax
         * Read indoor min/max temperatures with timestamps
         * 
         * Input: Handle to weatherstation
         *        temperature_conv flag (integer) controlling
         *            convertion to deg F
         *
         * Output: Temperatures temp_min and temp_max
         *                (deg C if temperature_conv is 0)
         *                (deg F if temperature_conv is 1)
         *         Timestamps for temp_min and temp_max in pointers to
         *                timestamp structures for time_min and time_max
         *
         ********************************************************************/
        public void temperature_indoor_minmax(
                                       int temperature_conv,
                                       ref double temp_min,
                                       ref double temp_max,
                                       ref timestamp time_min,
                                       ref timestamp time_max)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x34B;
            int bytes = 15;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            temp_min = ((data[1] >> 4) * 10 + (data[1] & 0xF) + (data[0] >> 4) / 10.0 +
                         (data[0] & 0xF) / 100.0) - 30.0;

            temp_max = ((data[4] & 0xF) * 10 + (data[3] >> 4) + (data[3] & 0xF) / 10.0 +
                         (data[2] >> 4) / 100.0) - 30.0;

            if (temperature_conv == 1)
            {
                temp_min = temp_min * 9 / 5 + 32;
                temp_max = temp_max * 9 / 5 + 32;
            }

            time_min.minute = ((data[5] & 0xF) * 10) + (data[4] >> 4);
            time_min.hour = ((data[6] & 0xF) * 10) + (data[5] >> 4);
            time_min.day = ((data[7] & 0xF) * 10) + (data[6] >> 4);
            time_min.month = ((data[8] & 0xF) * 10) + (data[7] >> 4);
            time_min.year = 2000 + ((data[9] & 0xF) * 10) + (data[8] >> 4);

            time_max.minute = ((data[10] & 0xF) * 10) + (data[9] >> 4);
            time_max.hour = ((data[11] & 0xF) * 10) + (data[10] >> 4);
            time_max.day = ((data[12] & 0xF) * 10) + (data[11] >> 4);
            time_max.month = ((data[13] & 0xF) * 10) + (data[12] >> 4);
            time_max.year = 2000 + ((data[14] & 0xF) * 10) + (data[13] >> 4);

            return;
        }

        /********************************************************************/
        /* temperature_indoor_reset
         * Reset indoor min/max temperatures with timestamps
         * 
         * Input: Handle to weatherstation
         *        minmax - char (8 bit integer) that controls if minimum,
         *                 maximum or both are reset
         * Output: None
         *
         * Returns: 1 if success
         *
         ********************************************************************/
        public int temperature_indoor_reset(byte minmax)
        {
            byte[] data_read = new byte[20];
            byte[] data_value = new byte[20];
            byte[] data_time = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address;
            int number;

            // First read current temperature into data_value
            address = 0x346;
            number = 2;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_value[0] = (byte)(data_read[0] & 0xF);
            data_value[1] = (byte)(data_read[0] >> 4);
            data_value[2] = (byte)(data_read[1] & 0xF);
            data_value[3] = (byte)(data_read[1] >> 4);

            // Get current time from station
            address = 0x23B;
            number = 6;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_time[0] = (byte)(data_read[0] & 0xF);
            data_time[1] = (byte)(data_read[0] >> 4);
            data_time[2] = (byte)(data_read[1] & 0xF);
            data_time[3] = (byte)(data_read[1] >> 4);
            data_time[4] = (byte)(data_read[2] >> 4);
            data_time[5] = (byte)(data_read[3] & 0xF);
            data_time[6] = (byte)(data_read[3] >> 4);
            data_time[7] = (byte)(data_read[4] & 0xF);
            data_time[8] = (byte)(data_read[4] >> 4);
            data_time[9] = (byte)(data_read[5] & 0xF);

            if ((minmax & RESET_MIN) == RESET_MIN) // minimum
            {
                // Set min value to current value
                address = 0x34B;
                number = 4;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set min value timestamp to current time
                address = 0x354;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            if ((minmax & RESET_MAX) == RESET_MAX)// maximum
            {
                // Set max value to current value
                address = 0x350;
                number = 4;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set max value timestamp to current time
                address = 0x35E;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            return 1;
        }


        /********************************************************************/
        /* temperature_outdoor
         * Read indoor temperature, current temperature only
         * 
         * Input: Handle to weatherstation
         *        temperature_conv flag (integer) controlling
         *            convertion to deg F
         *
         * Returns: Temperature (deg C if temperature_conv is 0)
         *                      (deg F if temperature_conv is 1)
         *
         ********************************************************************/
        public double temperature_outdoor(int temperature_conv)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x373;
            int bytes = 2;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            if (temperature_conv == 1)
                return ((((data[1] >> 4) * 10 + (data[1] & 0xF) +
                          (data[0] >> 4) / 10.0 + (data[0] & 0xF) / 100.0) -
                          30.0) * 9 / 5 + 32);
            else
                return ((((data[1] >> 4) * 10 + (data[1] & 0xF) +
                          (data[0] >> 4) / 10.0 + (data[0] & 0xF) / 100.0) - 30.0));
        }


        /********************************************************************
         * temperature_outdoor_minmax
         * Read outdoor min/max temperatures with timestamps
         * 
         * Input: Handle to weatherstation
         *        temperature_conv flag (integer) controlling
         *            convertion to deg F
         *
         * Output: Temperatures temp_min and temp_max
         *                (deg C if temperature_conv is 0)
         *                (deg F if temperature_conv is 1)
         *         Timestamps for temp_min and temp_max in pointers to
         *                timestamp structures for time_min and time_max
         *
         ********************************************************************/
        public void temperature_outdoor_minmax(
                                int temperature_conv,
                                ref double temp_min,
                                ref double temp_max,
                                ref timestamp time_min,
                                ref timestamp time_max)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x378;
            int bytes = 15;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            temp_min = ((data[1] >> 4) * 10 + (data[1] & 0xF) + (data[0] >> 4) / 10.0 +
                         (data[0] & 0xF) / 100.0) - 30.0;

            temp_max = ((data[4] & 0xF) * 10 + (data[3] >> 4) + (data[3] & 0xF) / 10.0 +
                         (data[2] >> 4) / 100.0) - 30.0;

            if (temperature_conv == 1)
            {
                temp_min = temp_min * 9 / 5 + 32;
                temp_max = temp_max * 9 / 5 + 32;
            }

            time_min.minute = ((data[5] & 0xF) * 10) + (data[4] >> 4);
            time_min.hour = ((data[6] & 0xF) * 10) + (data[5] >> 4);
            time_min.day = ((data[7] & 0xF) * 10) + (data[6] >> 4);
            time_min.month = ((data[8] & 0xF) * 10) + (data[7] >> 4);
            time_min.year = 2000 + ((data[9] & 0xF) * 10) + (data[8] >> 4);

            time_max.minute = ((data[10] & 0xF) * 10) + (data[9] >> 4);
            time_max.hour = ((data[11] & 0xF) * 10) + (data[10] >> 4);
            time_max.day = ((data[12] & 0xF) * 10) + (data[11] >> 4);
            time_max.month = ((data[13] & 0xF) * 10) + (data[12] >> 4);
            time_max.year = 2000 + ((data[14] & 0xF) * 10) + (data[13] >> 4);

            return;
        }


        /********************************************************************/
        /* temperature_outdoor_reset
         * Reset outdoor min/max temperatures with timestamps
         * 
         * Input: Handle to weatherstation
         *        minmax - char (8 bit integer) that controls if minimum,
         *                 maximum or both are reset
         * Output: None
         *
         * Returns: 1 if success
         *
         ********************************************************************/
        public int temperature_outdoor_reset(byte minmax)
        {
            byte[] data_read = new byte[20];
            byte[] data_value = new byte[20];
            byte[] data_time = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address;
            int number;

            // First read current temperature into data_value
            address = 0x373;
            number = 2;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_value[0] = (byte)(data_read[0] & 0xF);
            data_value[1] = (byte)(data_read[0] >> 4);
            data_value[2] = (byte)(data_read[1] & 0xF);
            data_value[3] = (byte)(data_read[1] >> 4);

            // Get current time from station
            address = 0x23B;
            number = 6;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_time[0] = (byte)(data_read[0] & 0xF);
            data_time[1] = (byte)(data_read[0] >> 4);
            data_time[2] = (byte)(data_read[1] & 0xF);
            data_time[3] = (byte)(data_read[1] >> 4);
            data_time[4] = (byte)(data_read[2] >> 4);
            data_time[5] = (byte)(data_read[3] & 0xF);
            data_time[6] = (byte)(data_read[3] >> 4);
            data_time[7] = (byte)(data_read[4] & 0xF);
            data_time[8] = (byte)(data_read[4] >> 4);
            data_time[9] = (byte)(data_read[5] & 0xF);

            if ((minmax & RESET_MIN) == RESET_MIN)// minimum
            {
                // Set min value to current value
                address = 0x378;
                number = 4;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set min value timestamp to current time
                address = 0x381;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            if ((minmax & RESET_MAX) == RESET_MAX) // maximum
            {
                // Set max value to current value
                address = 0x37D;
                number = 4;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set max value timestamp to current time
                address = 0x38B;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            return 1;
        }


        /********************************************************************
         * dewpoint
         * Read dewpoint, current value only
         * 
         * Input: Handle to weatherstation
         *        temperature_conv flag (integer) controlling
         *            convertion to deg F
         *
         * Returns: Dewpoint    (deg C if temperature_conv is 0)
         *                      (deg F if temperature_conv is 1)
         *
         ********************************************************************/
        public double dewpoint(int temperature_conv)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x3CE;
            int bytes = 2;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            if (temperature_conv == 1)
                return ((((data[1] >> 4) * 10 + (data[1] & 0xF) +
                          (data[0] >> 4) / 10.0 + (data[0] & 0xF) / 100.0) -
                          30.0) * 9 / 5 + 32);
            else
                return ((((data[1] >> 4) * 10 + (data[1] & 0xF) +
                          (data[0] >> 4) / 10.0 + (data[0] & 0xF) / 100.0) - 30.0));
        }


        /********************************************************************
         * dewpoint_minmax
         * Read outdoor min/max dewpoint with timestamps
         * 
         * Input: Handle to weatherstation
         *        temperature_conv flag (integer) controlling
         *            convertion to deg F
         *
         * Output: Dewpoints dp_min and dp_max
         *                (deg C if temperature_conv is 0),
         *                (deg F if temperature_conv is 1)
         *         Timestamps for dp_min and dp_max in pointers to
         *                timestamp structures for time_min and time_max
         *
         ********************************************************************/
        public void dewpoint_minmax(
                             int temperature_conv,
                             ref double dp_min,
                             ref double dp_max,
                             ref timestamp time_min,
                             ref timestamp time_max)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x3D3;
            int bytes = 15;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            dp_min = ((data[1] >> 4) * 10 + (data[1] & 0xF) + (data[0] >> 4) / 10.0 +
                       (data[0] & 0xF) / 100.0) - 30.0;

            dp_max = ((data[4] & 0xF) * 10 + (data[3] >> 4) + (data[3] & 0xF) / 10.0 +
                       (data[2] >> 4) / 100.0) - 30.0;

            if (temperature_conv == 1)
            {
                dp_min = dp_min * 9 / 5 + 32;
                dp_max = dp_max * 9 / 5 + 32;
            }

            time_min.minute = ((data[5] & 0xF) * 10) + (data[4] >> 4);
            time_min.hour = ((data[6] & 0xF) * 10) + (data[5] >> 4);
            time_min.day = ((data[7] & 0xF) * 10) + (data[6] >> 4);
            time_min.month = ((data[8] & 0xF) * 10) + (data[7] >> 4);
            time_min.year = 2000 + ((data[9] & 0xF) * 10) + (data[8] >> 4);

            time_max.minute = ((data[10] & 0xF) * 10) + (data[9] >> 4);
            time_max.hour = ((data[11] & 0xF) * 10) + (data[10] >> 4);
            time_max.day = ((data[12] & 0xF) * 10) + (data[11] >> 4);
            time_max.month = ((data[13] & 0xF) * 10) + (data[12] >> 4);
            time_max.year = 2000 + ((data[14] & 0xF) * 10) + (data[13] >> 4);

            return;
        }


        /********************************************************************/
        /* dewpoint_reset
         * Reset min/max dewpoint with timestamps
         * 
         * Input: Handle to weatherstation
         *        minmax - char (8 bit integer) that controls if minimum,
         *                 maximum or both are reset
         * Output: None
         *
         * Returns: 1 if success
         *
         ********************************************************************/
        public int dewpoint_reset(byte minmax)
        {
            byte[] data_read = new byte[20];
            byte[] data_value = new byte[20];
            byte[] data_time = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address;
            int number;

            // First read current dewpoint into data_value
            address = 0x3CE;
            number = 2;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_value[0] = (byte)(data_read[0] & 0xF);
            data_value[1] = (byte)(data_read[0] >> 4);
            data_value[2] = (byte)(data_read[1] & 0xF);
            data_value[3] = (byte)(data_read[1] >> 4);

            // Get current time from station
            address = 0x23B;
            number = 6;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_time[0] = (byte)(data_read[0] & 0xF);
            data_time[1] = (byte)(data_read[0] >> 4);
            data_time[2] = (byte)(data_read[1] & 0xF);
            data_time[3] = (byte)(data_read[1] >> 4);
            data_time[4] = (byte)(data_read[2] >> 4);
            data_time[5] = (byte)(data_read[3] & 0xF);
            data_time[6] = (byte)(data_read[3] >> 4);
            data_time[7] = (byte)(data_read[4] & 0xF);
            data_time[8] = (byte)(data_read[4] >> 4);
            data_time[9] = (byte)(data_read[5] & 0xF);

            if ((minmax & RESET_MIN) == RESET_MIN) // minimum
            {
                // Set min value to current value
                address = 0x3D3;
                number = 4;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set min value timestamp to current time
                address = 0x3DC;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            if ((minmax & RESET_MAX) == RESET_MAX) // maximum
            {
                // Set max value to current value
                address = 0x3D8;
                number = 4;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set max value timestamp to current time
                address = 0x3E6;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            return 1;
        }


        /********************************************************************
         * humidity_indoor
         * Read indoor relative humidity, current value only
         * 
         * Input: Handle to weatherstation
         * Returns: relative humidity in percent (integer)
         * 
         ********************************************************************/
        public int humidity_indoor()
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x3FB;
            int bytes = 1;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            return ((data[0] >> 4) * 10 + (data[0] & 0xF));
        }


        /********************************************************************
         * humidity_indoor_all
         * Read both current indoor humidity and min/max values with timestamps
         * 
         * Input: Handle to weatherstation
         * Output: Relative humidity in % hum_min and hum_max (integers)
         *         Timestamps for hum_min and hum_max in pointers to
         *                timestamp structures for time_min and time_max
         * Returns: releative humidity current value in % (integer)
         *
         ********************************************************************/
        public int humidity_indoor_all(
                                ref int hum_min,
                                ref int hum_max,
                                ref timestamp time_min,
                                ref timestamp time_max)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x3FB;
            int bytes = 13;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            hum_min = (data[1] >> 4) * 10 + (data[1] & 0xF);
            hum_max = (data[2] >> 4) * 10 + (data[2] & 0xF);

            time_min.minute = ((data[3] >> 4) * 10) + (data[3] & 0xF);
            time_min.hour = ((data[4] >> 4) * 10) + (data[4] & 0xF);
            time_min.day = ((data[5] >> 4) * 10) + (data[5] & 0xF);
            time_min.month = ((data[6] >> 4) * 10) + (data[6] & 0xF);
            time_min.year = 2000 + ((data[7] >> 4) * 10) + (data[7] & 0xF);

            time_max.minute = ((data[8] >> 4) * 10) + (data[8] & 0xF);
            time_max.hour = ((data[9] >> 4) * 10) + (data[9] & 0xF);
            time_max.day = ((data[10] >> 4) * 10) + (data[10] & 0xF);
            time_max.month = ((data[11] >> 4) * 10) + (data[11] & 0xF);
            time_max.year = 2000 + ((data[12] >> 4) * 10) + (data[12] & 0xF);

            return ((data[0] >> 4) * 10 + (data[0] & 0xF));
        }


        /********************************************************************/
        /* humidity_indoor_reset
         * Reset min/max indoor humidity with timestamps
         * 
         * Input: Handle to weatherstation
         *        minmax - char (8 bit integer) that controls if minimum,
         *                 maximum or both are reset
         * Output: None
         *
         * Returns: 1 if success
         *
         ********************************************************************/
        public int humidity_indoor_reset(byte minmax)
        {
            byte[] data_read = new byte[20];
            byte[] data_value = new byte[20];
            byte[] data_time = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address;
            int number;

            // First read current humidity into data_value
            address = 0x3FB;
            number = 1;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_value[0] = (byte)(data_read[0] & 0xF);
            data_value[1] = (byte)(data_read[0] >> 4);

            // Get current time from station
            address = 0x23B;
            number = 6;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_time[0] = (byte)(data_read[0] & 0xF);
            data_time[1] = (byte)(data_read[0] >> 4);
            data_time[2] = (byte)(data_read[1] & 0xF);
            data_time[3] = (byte)(data_read[1] >> 4);
            data_time[4] = (byte)(data_read[2] >> 4);
            data_time[5] = (byte)(data_read[3] & 0xF);
            data_time[6] = (byte)(data_read[3] >> 4);
            data_time[7] = (byte)(data_read[4] & 0xF);
            data_time[8] = (byte)(data_read[4] >> 4);
            data_time[9] = (byte)(data_read[5] & 0xF);

            if ((minmax & RESET_MIN) == RESET_MIN) // minimum
            {
                // Set min value to current value
                address = 0x3FD;
                number = 2;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set min value timestamp to current time
                address = 0x401;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            if ((minmax & RESET_MAX) == RESET_MAX) // maximum
            {
                // Set max value to current value
                address = 0x3FF;
                number = 2;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set max value timestamp to current time
                address = 0x40B;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            return 1;
        }


        /********************************************************************
         * humidity_outdoor
         * Read relative humidity, current value only
         * 
         * Input: Handle to weatherstation
         * Returns: relative humidity in percent (integer)
         *
         ********************************************************************/
        public int humidity_outdoor()
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x419;
            int bytes = 1;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            return ((data[0] >> 4) * 10 + (data[0] & 0xF));
        }


        /********************************************************************
         * humidity_outdoor_all
         * Read both current outdoor humidity and min/max values with timestamps
         * 
         * Input: Handle to weatherstation
         * Output: Relative humidity in % hum_min and hum_max (integers)
         *         Timestamps for hum_min and hum_max in pointers to
         *                timestamp structures for time_min and time_max
         *
         * Returns: releative humidity current value in % (integer)
         *
         ********************************************************************/
        public int humidity_outdoor_all(
                                 ref int hum_min,
                                 ref int hum_max,
                                 ref timestamp time_min,
                                 ref timestamp time_max)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x419;
            int bytes = 13;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            hum_min = (data[1] >> 4) * 10 + (data[1] & 0xF);
            hum_max = (data[2] >> 4) * 10 + (data[2] & 0xF);

            time_min.minute = ((data[3] >> 4) * 10) + (data[3] & 0xF);
            time_min.hour = ((data[4] >> 4) * 10) + (data[4] & 0xF);
            time_min.day = ((data[5] >> 4) * 10) + (data[5] & 0xF);
            time_min.month = ((data[6] >> 4) * 10) + (data[6] & 0xF);
            time_min.year = 2000 + ((data[7] >> 4) * 10) + (data[7] & 0xF);

            time_max.minute = ((data[8] >> 4) * 10) + (data[8] & 0xF);
            time_max.hour = ((data[9] >> 4) * 10) + (data[9] & 0xF);
            time_max.day = ((data[10] >> 4) * 10) + (data[10] & 0xF);
            time_max.month = ((data[11] >> 4) * 10) + (data[11] & 0xF);
            time_max.year = 2000 + ((data[12] >> 4) * 10) + (data[12] & 0xF);

            return ((data[0] >> 4) * 10 + (data[0] & 0xF));
        }


        /********************************************************************/
        /* humidity_outdoor_reset
         * Reset min/max outdoor humidity with timestamps
         * 
         * Input: Handle to weatherstation
         *        minmax - char (8 bit integer) that controls if minimum,
         *                 maximum or both are reset
         * Output: None
         *
         * Returns: 1 if success
         *
         ********************************************************************/
        public int humidity_outdoor_reset(byte minmax)
        {
            byte[] data_read = new byte[20];
            byte[] data_value = new byte[20];
            byte[] data_time = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address;
            int number;

            // First read current humidity into data_value
            address = 0x419;
            number = 1;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_value[0] = (byte)(data_read[0] & 0xF);
            data_value[1] = (byte)(data_read[0] >> 4);

            // Get current time from station
            address = 0x23B;
            number = 6;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_time[0] = (byte)(data_read[0] & 0xF);
            data_time[1] = (byte)(data_read[0] >> 4);
            data_time[2] = (byte)(data_read[1] & 0xF);
            data_time[3] = (byte)(data_read[1] >> 4);
            data_time[4] = (byte)(data_read[2] >> 4);
            data_time[5] = (byte)(data_read[3] & 0xF);
            data_time[6] = (byte)(data_read[3] >> 4);
            data_time[7] = (byte)(data_read[4] & 0xF);
            data_time[8] = (byte)(data_read[4] >> 4);
            data_time[9] = (byte)(data_read[5] & 0xF);

            if ((minmax & RESET_MIN) == RESET_MIN) // minimum
            {
                // Set min value to current value
                address = 0x41B;
                number = 2;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set min value timestamp to current time
                address = 0x41F;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            if ((minmax & RESET_MAX) == RESET_MAX) // maximum
            {
                // Set max value to current value
                address = 0x41D;
                number = 2;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set max value timestamp to current time
                address = 0x429;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            return 1;
        }



        /********************************************************************
         * wind_current
         * Read wind speed, wind direction and last 5 wind directions
         *
         * Input: Handle to weatherstation
         *        wind_speed_conv_factor controlling convertion to other
         *             units than m/s
         *
         * Output: winddir - pointer to double in degrees
         *
         * Returns: Wind speed (double) in the unit given in the loaded config
         *
         ********************************************************************/
        public double wind_current(
                            double wind_speed_conv_factor,
                            double winddir)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int i;
            int address = 0x527; //Windspeed and direction
            int bytes = 3;

            for (i = 0; i < MAXWINDRETRIES; i++)
            {
                if (read_safe(address, bytes, data, command) != bytes) //Wind
                    read_error_exit();

                if ((data[0] != 0x00) ||                            //Invalid wind data
                    ((data[1] == 0xFF) && (((data[2] & 0xF) == 0) || ((data[2] & 0xF) == 1))))
                {
                    sleep_long(10); //wait 10 seconds for new wind measurement
                    continue;
                }
                else
                {
                    break;
                }
            }

            //Calculate wind directions
            winddir = (data[2] >> 4) * 22.5;

            //Calculate raw wind speed 	- convert from m/s to whatever
            return ((((data[2] & 0xF) << 8) + (data[1])) / 10.0 * wind_speed_conv_factor);
        }


        /********************************************************************
         * wind_all
         * Read wind speed, wind direction and last 5 wind directions
         *
         * Input: Handle to weatherstation
         *        wind_speed_conv_factor controlling convertion to other
         *             units than m/s
         *
         * Output: winddir_index
         *              Current wind direction expressed as ticks from North
         *              where North=0. Used to convert to direction string
         *         winddir
         *              Array of doubles containing current winddirection
         *              in winddir[0] and the last 5 in the following
         *              positions all given in degrees
         *
         * Returns: Wind speed (double) in the unit given in the loaded config
         *
         ********************************************************************/
        public double wind_all(
                        double wind_speed_conv_factor,
                ref int winddir_index,
                ref short[] winddir)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int i;
            int address = 0x527; //Windspeed and direction
            int bytes = 6;

            for (i = 0; i < MAXWINDRETRIES; i++)
            {
                if (read_safe(address, bytes, data, command) != bytes) //Wind
                    read_error_exit();

                if ((data[0] != 0x00) ||                             //Invalid wind data
                   ((data[1] == 0xFF) && (((data[2] & 0xF) == 0) || ((data[2] & 0xF) == 1))))
                {
                    sleep_long(10); //wait 10 seconds for new wind measurement
                    continue;
                }
                else
                {
                    break;
                }
            }

            //Calculate wind directions
            winddir_index = (data[2] >> 4);
            winddir[0] = (short)((data[2] >> 4) * 22.5);
            winddir[1] = (short)((data[3] & 0xF) * 22.5);
            winddir[2] = (short)((data[3] >> 4) * 22.5);
            winddir[3] = (short)((data[4] & 0xF) * 22.5);
            winddir[4] = (short)((data[4] >> 4) * 22.5);
            winddir[5] = (short)((data[5] & 0xF) * 22.5);

            //Calculate raw wind speed - convert from m/s to whatever
            return ((((data[2] & 0xF) << 8) + (data[1])) / 10.0 * wind_speed_conv_factor);
        }


        /********************************************************************
         * wind_minmax
         * Read min/max wind speeds with timestamps
         * 
         * Input: Handle to weatherstation
         *        wind_speed_conv_factor controlling convertion to other
         *             units than m/s
         *
         * Output: Wind wind_min and wind_max (double)
         *                unit defined by config conversion factor
         *         Timestamps for wind_min and wind_max in pointers to
         *                timestamp structures for time_min and time_max
         *
         * Returns: wind max (double)
         *
         * Note: The function is made so that if a pointer to
         *       wind_min/max and time_min/max is a NULL pointer the function
         *       ignores this parameter. Example: if you only need wind_max
         *       use the function like this..
         *       windmax = wind_minmax(METERS_PER_SECOND,NULL,NULL,NULL,NULL);
         *
         ********************************************************************/
        public double wind_minmax(
                           double wind_speed_conv_factor,
                   ref double wind_min,
                   ref double wind_max,
                   ref timestamp time_min,
                   ref timestamp time_max)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x4EE;
            int bytes = 15;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            //if (wind_min != null)
            wind_min = (data[1] * 256 + data[0]) / 360.0 * wind_speed_conv_factor;
            //if (wind_max != null)
            wind_max = (data[4] * 256 + data[3]) / 360.0 * wind_speed_conv_factor;

            //if (time_min.year != null)
            //{
            time_min.minute = ((data[5] >> 4) * 10) + (data[5] & 0xF);
            time_min.hour = ((data[6] >> 4) * 10) + (data[6] & 0xF);
            time_min.day = ((data[7] >> 4) * 10) + (data[7] & 0xF);
            time_min.month = ((data[8] >> 4) * 10) + (data[8] & 0xF);
            time_min.year = 2000 + ((data[9] >> 4) * 10) + (data[9] & 0xF);
            //}

            //if (time_max.year != null)
            //{
            time_max.minute = ((data[10] >> 4) * 10) + (data[10] & 0xF);
            time_max.hour = ((data[11] >> 4) * 10) + (data[11] & 0xF);
            time_max.day = ((data[12] >> 4) * 10) + (data[12] & 0xF);
            time_max.month = ((data[13] >> 4) * 10) + (data[13] & 0xF);
            time_max.year = 2000 + ((data[14] >> 4) * 10) + (data[14] & 0xF);
            //}

            return ((data[4] * 256 + data[3]) / 360.0 * wind_speed_conv_factor);
        }


        /********************************************************************/
        /* wind_reset
         * Reset min/max wind with timestamps
         * 
         * Input: Handle to weatherstation
         *        minmax - char (8 bit integer) that controls if minimum,
         *                 maximum or both are reset
         * Output: None
         *
         * Returns: 1 if success
         *
         ********************************************************************/
        public int wind_reset(byte minmax)
        {
            byte[] data_read = new byte[20];
            byte[] data_value = new byte[20];
            byte[] data_time = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address;
            int number;
            int i;
            int current_wind;

            address = 0x527; //Windspeed
            number = 3;

            for (i = 0; i < MAXWINDRETRIES; i++)
            {
                if (read_safe(address, number, data_read, command) != number)
                    read_error_exit();

                if ((data_read[0] != 0x00) ||                            //Invalid wind data
                    ((data_read[1] == 0xFF) && (((data_read[2] & 0xF) == 0) || ((data_read[2] & 0xF) == 1))))
                {
                    sleep_long(10); //wait 10 seconds for new wind measurement
                    continue;
                }
                else
                {
                    break;
                }
            }

            current_wind = (((data_read[2] & 0xF) << 8) + (data_read[1])) * 36;

            data_value[0] = (byte)(current_wind & 0xF);
            data_value[1] = (byte)((current_wind >> 4) & 0xF);
            data_value[2] = (byte)((current_wind >> 8) & 0xF);
            data_value[3] = (byte)((current_wind >> 12) & 0xF);

            // Get current time from station
            address = 0x23B;
            number = 6;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_time[0] = (byte)(data_read[0] & 0xF);
            data_time[1] = (byte)(data_read[0] >> 4);
            data_time[2] = (byte)(data_read[1] & 0xF);
            data_time[3] = (byte)(data_read[1] >> 4);
            data_time[4] = (byte)(data_read[2] >> 4);
            data_time[5] = (byte)(data_read[3] & 0xF);
            data_time[6] = (byte)(data_read[3] >> 4);
            data_time[7] = (byte)(data_read[4] & 0xF);
            data_time[8] = (byte)(data_read[4] >> 4);
            data_time[9] = (byte)(data_read[5] & 0xF);

            if ((minmax & RESET_MIN) == RESET_MIN) // minimum
            {
                // Set min value to current value
                address = 0x4EE;
                number = 4;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set min value timestamp to current time
                address = 0x4F8;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            if ((minmax & RESET_MAX) == RESET_MAX) // maximum
            {
                // Set max value to current value
                address = 0x4F4;
                number = 4;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set max value timestamp to current time
                address = 0x502;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            return 1;
        }


        /********************************************************************
         * windchill
         * Read wind chill, current value only
         * 
         * Input: Handle to weatherstation
         *        temperature_conv flag (integer) controlling
         *            convertion to deg F
         *
         * Returns: wind chill  (deg C if config.temperature_conv is not set)
         *                      (deg F if config.temperature_conv is set)
         *
         * It is recommended to run this right after a wind speed reading
         * to enhance the likelyhood that the wind speed is valid
         *
         ********************************************************************/
        public double windchill(int temperature_conv)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x3A0;
            int bytes = 2;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            if (temperature_conv == 1)
                return ((((data[1] >> 4) * 10 + (data[1] & 0xF) +
                          (data[0] >> 4) / 10.0 + (data[0] & 0xF) / 100.0) -
                          30.0) * 9 / 5 + 32);
            else
                return ((((data[1] >> 4) * 10 + (data[1] & 0xF) +
                          (data[0] >> 4) / 10.0 + (data[0] & 0xF) / 100.0) - 30.0));
        }


        /********************************************************************
         * windchill_minmax
         * Read wind chill min/max with timestamps
         * 
         * Input: Handle to weatherstation
         *        temperature_conv flag (integer) controlling
         *            convertion to deg F
         *
         * Output: Windchill wc_min and wc_max
         *                (deg C if config.temperature_conv is not set)
         *                (deg F if config.temperature_conv is set)
         *         Timestamps for wc_min and wc_max in pointers to
         *                timestamp structures for time_min and time_max
         *
         * Returns: Nothing
         *
         ********************************************************************/
        public void windchill_minmax(
                              int temperature_conv,
                      ref double wc_min,
                      ref double wc_max,
                      ref timestamp time_min,
                      ref timestamp time_max)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x3A5;
            int bytes = 15;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            wc_min = ((data[1] >> 4) * 10 + (data[1] & 0xF) + (data[0] >> 4) / 10.0 +
                        (data[0] & 0xF) / 100.0) - 30.0;

            wc_max = ((data[4] & 0xF) * 10 + (data[3] >> 4) + (data[3] & 0xF) / 10.0 +
                        (data[2] >> 4) / 100.0) - 30.0;

            if (temperature_conv == 1)
            {
                wc_min = wc_min * 9 / 5 + 32;
                wc_max = wc_max * 9 / 5 + 32;
            }

            time_min.minute = ((data[5] & 0xF) * 10) + (data[4] >> 4);
            time_min.hour = ((data[6] & 0xF) * 10) + (data[5] >> 4);
            time_min.day = ((data[7] & 0xF) * 10) + (data[6] >> 4);
            time_min.month = ((data[8] & 0xF) * 10) + (data[7] >> 4);
            time_min.year = 2000 + ((data[9] & 0xF) * 10) + (data[8] >> 4);

            time_max.minute = ((data[10] & 0xF) * 10) + (data[9] >> 4);
            time_max.hour = ((data[11] & 0xF) * 10) + (data[10] >> 4);
            time_max.day = ((data[12] & 0xF) * 10) + (data[11] >> 4);
            time_max.month = ((data[13] & 0xF) * 10) + (data[12] >> 4);
            time_max.year = 2000 + ((data[14] & 0xF) * 10) + (data[13] >> 4);

            return;
        }


        /********************************************************************/
        /* windchill_reset
         * Reset min/max windchill with timestamps
         * 
         * Input: Handle to weatherstation
         *        minmax - char (8 bit integer) that controls if minimum,
         *                 maximum or both are reset
         * Output: None
         *
         * Returns: 1 if success
         *
         ********************************************************************/
        public int windchill_reset(byte minmax)
        {
            byte[] data_read = new byte[20];
            byte[] data_value = new byte[20];
            byte[] data_time = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address;
            int number;

            // First read current windchill into data_value
            address = 0x3A0;
            number = 2;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_value[0] = (byte)(data_read[0] & 0xF);
            data_value[1] = (byte)(data_read[0] >> 4);
            data_value[2] = (byte)(data_read[1] & 0xF);
            data_value[3] = (byte)(data_read[1] >> 4);

            // Get current time from station
            address = 0x23B;
            number = 6;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_time[0] = (byte)(data_read[0] & 0xF);
            data_time[1] = (byte)(data_read[0] >> 4);
            data_time[2] = (byte)(data_read[1] & 0xF);
            data_time[3] = (byte)(data_read[1] >> 4);
            data_time[4] = (byte)(data_read[2] >> 4);
            data_time[5] = (byte)(data_read[3] & 0xF);
            data_time[6] = (byte)(data_read[3] >> 4);
            data_time[7] = (byte)(data_read[4] & 0xF);
            data_time[8] = (byte)(data_read[4] >> 4);
            data_time[9] = (byte)(data_read[5] & 0xF);

            if ((minmax & RESET_MIN) == RESET_MIN) // minimum
            {
                // Set min value to current value
                address = 0x3A5;
                number = 4;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set min value timestamp to current time
                address = 0x3AE;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            if ((minmax & RESET_MAX) == RESET_MAX) // maximum
            {
                // Set max value to current value
                address = 0x3AA;
                number = 4;

                if (write_safe(address, number, WRITENIB, data_value, command) != number)
                    write_error_exit();

                // Set max value timestamp to current time
                address = 0x3B8;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            return 1;
        }


        /********************************************************************
         * rain_1h
         * Read rain last 1 hour, current value only
         * 
         * Input: Handle to weatherstation
         *        rain_conv_factor controlling convertion to other
         *             units than mm
         *
         * Returns: rain (double) converted to unit given in config
         *
         ********************************************************************/
        public double rain_1h(double rain_conv_factor)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x4B4;
            int bytes = 3;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            return (((data[2] >> 4) * 1000 + (data[2] & 0xF) * 100 +
                      (data[1] >> 4) * 10 + (data[1] & 0xF) + (data[0] >> 4) / 10.0 +
                      (data[0] & 0xF) / 100.0) / rain_conv_factor);
        }

        /********************************************************************
         * rain_1h_all
         * Read rain last 1 hourand maximum with timestamp
         * 
         * Input: Handle to weatherstation
         *        rain_conv_factor controlling convertion to other
         *             units than mm
         *
         * Output: Rain maximum in rain_max (double)
         *                unit defined by config conversion factor
         *         Timestamps for rain_max in pointers to
         *                timestamp structures for time_min and time_max
         *
         * Returns: rain (double) converted to unit given in config
         *
         ********************************************************************/
        public double rain_1h_all(
                           double rain_conv_factor,
                   ref double rain_max,
                   ref timestamp time_max)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x4B4;
            int bytes = 11;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            rain_max = ((data[5] >> 4) * 1000 + (data[5] & 0xF) * 100 +
                         (data[4] >> 4) * 10 + (data[4] & 0xF) + (data[3] >> 4) / 10.0 +
                         (data[3] & 0xF) / 100.0) / rain_conv_factor;

            time_max.minute = ((data[6] >> 4) * 10) + (data[6] & 0xF);
            time_max.hour = ((data[7] >> 4) * 10) + (data[7] & 0xF);
            time_max.day = ((data[8] >> 4) * 10) + (data[8] & 0xF);
            time_max.month = ((data[9] >> 4) * 10) + (data[9] & 0xF);
            time_max.year = 2000 + ((data[10] >> 4) * 10) + (data[10] & 0xF);

            return (((data[2] >> 4) * 1000 + (data[2] & 0xF) * 100 +
                     (data[1] >> 4) * 10 + (data[1] & 0xF) + (data[0] >> 4) / 10.0 +
                     (data[0] & 0xF) / 100.0) / rain_conv_factor);
        }


        /********************************************************************/
        /* rain_1h_max_reset
         * Reset max rain 1h with timestamps
         * 
         * Input: Handle to weatherstation
         *        minmax - char (8 bit integer) that controls if minimum,
         *                 maximum or both are reset
         * Output: None
         *
         * Returns: 1 if success
         *
         ********************************************************************/
        public int rain_1h_max_reset()
        {
            byte[] data_read = new byte[20];
            byte[] data_value = new byte[20];
            byte[] data_time = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address;
            int number;

            // First read current rain 1h into data_value
            address = 0x4B4;
            number = 3;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_value[0] = (byte)(data_read[0] & 0xF);
            data_value[1] = (byte)(data_read[0] >> 4);
            data_value[2] = (byte)(data_read[1] & 0xF);
            data_value[3] = (byte)(data_read[1] >> 4);
            data_value[4] = (byte)(data_read[2] & 0xF);
            data_value[5] = (byte)(data_read[2] >> 4);

            // Get current time from station
            address = 0x23B;
            number = 6;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_time[0] = (byte)(data_read[0] & 0xF);
            data_time[1] = (byte)(data_read[0] >> 4);
            data_time[2] = (byte)(data_read[1] & 0xF);
            data_time[3] = (byte)(data_read[1] >> 4);
            data_time[4] = (byte)(data_read[2] >> 4);
            data_time[5] = (byte)(data_read[3] & 0xF);
            data_time[6] = (byte)(data_read[3] >> 4);
            data_time[7] = (byte)(data_read[4] & 0xF);
            data_time[8] = (byte)(data_read[4] >> 4);
            data_time[9] = (byte)(data_read[5] & 0xF);

            // Set max value to current value
            address = 0x4BA;
            number = 6;

            if (write_safe(address, number, WRITENIB, data_value, command) != number)
                write_error_exit();

            // Set max value timestamp to current time
            address = 0x4C0;
            number = 10;

            if (write_safe(address, number, WRITENIB, data_time, command) != number)
                write_error_exit();

            return 1;
        }

        /********************************************************************/
        /* rain_1h_reset
         * Reset current rain 1h
         * 
         * Input: Handle to weatherstation
         *        minmax - char (8 bit integer) that controls if minimum,
         *                 maximum or both are reset
         * Output: None
         *
         * Returns: 1 if success
         *
         ********************************************************************/
        public int rain_1h_reset()
        {
            byte[] data = new byte[50];
            byte[] command = new byte[60];	//room for write data also
            int address;
            int number;

            // First overwrite the 1h rain history with zeros
            address = 0x479;
            number = 30;
            memset(data, 0);

            if (write_safe(address, number, WRITENIB, data, command) != number)
                write_error_exit();

            // Set value to zero
            address = 0x4B4;
            number = 6;

            if (write_safe(address, number, WRITENIB, data, command) != number)
                write_error_exit();

            return 1;
        }


        /********************************************************************
         * rain_24h
         * Read rain last 24 hours, current value only
         * 
         * Input: Handle to weatherstation
         *        rain_conv_factor controlling convertion to other
         *             units than mm
         *
         * Returns: rain (double) converted to unit given in config
         *
         ********************************************************************/
        public double rain_24h(double rain_conv_factor)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x497;
            int bytes = 3;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            return (((data[2] >> 4) * 1000 + (data[2] & 0xF) * 100 +
                     (data[1] >> 4) * 10 + (data[1] & 0xF) + (data[0] >> 4) / 10.0 +
                     (data[0] & 0xF) / 100.0) / rain_conv_factor);
        }


        /********************************************************************
         * rain_24h_all
         * Read rain last 24 hours and maximum with timestamp
         * 
         * Input: Handle to weatherstation
         *        rain_conv_factor controlling convertion to other
         *             units than mm
         *
         * Output: Rain maximum in rain_max (double)
         *                unit defined by config conversion factor
         *         Timestamp for rain_max in pointers to
         *                timestamp structures for time_min and time_max
         *
         * Returns: rain (double) converted to unit given in config
         *
         ********************************************************************/
        public double rain_24h_all(
                           double rain_conv_factor,
                   ref double rain_max,
                   ref timestamp time_max)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x497;
            int bytes = 11;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            rain_max = ((data[5] >> 4) * 1000 + (data[5] & 0xF) * 100 +
                         (data[4] >> 4) * 10 + (data[4] & 0xF) + (data[3] >> 4) / 10.0 +
                         (data[3] & 0xF) / 100.0) / rain_conv_factor;

            time_max.minute = ((data[6] >> 4) * 10) + (data[6] & 0xF);
            time_max.hour = ((data[7] >> 4) * 10) + (data[7] & 0xF);
            time_max.day = ((data[8] >> 4) * 10) + (data[8] & 0xF);
            time_max.month = ((data[9] >> 4) * 10) + (data[9] & 0xF);
            time_max.year = 2000 + ((data[10] >> 4) * 10) + (data[10] & 0xF);

            return (((data[2] >> 4) * 1000 + (data[2] & 0xF) * 100 +
                     (data[1] >> 4) * 10 + (data[1] & 0xF) + (data[0] >> 4) / 10.0 +
                     (data[0] & 0xF) / 100.0) / rain_conv_factor);
        }


        /********************************************************************/
        /* rain_24h_max_reset
         * Reset max rain 24h with timestamps
         * 
         * Input: Handle to weatherstation
         *        minmax - char (8 bit integer) that controls if minimum,
         *                 maximum or both are reset
         * Output: None
         *
         * Returns: 1 if success
         *
         ********************************************************************/
        public int rain_24h_max_reset()
        {
            byte[] data_read = new byte[20];
            byte[] data_value = new byte[20];
            byte[] data_time = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address;
            int number;

            // First read current rain 24h into data_value
            address = 0x497;
            number = 3;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_value[0] = (byte)(data_read[0] & 0xF);
            data_value[1] = (byte)(data_read[0] >> 4);
            data_value[2] = (byte)(data_read[1] & 0xF);
            data_value[3] = (byte)(data_read[1] >> 4);
            data_value[4] = (byte)(data_read[2] & 0xF);
            data_value[5] = (byte)(data_read[2] >> 4);

            // Get current time from station
            address = 0x23B;
            number = 6;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_time[0] = (byte)(data_read[0] & 0xF);
            data_time[1] = (byte)(data_read[0] >> 4);
            data_time[2] = (byte)(data_read[1] & 0xF);
            data_time[3] = (byte)(data_read[1] >> 4);
            data_time[4] = (byte)(data_read[2] >> 4);
            data_time[5] = (byte)(data_read[3] & 0xF);
            data_time[6] = (byte)(data_read[3] >> 4);
            data_time[7] = (byte)(data_read[4] & 0xF);
            data_time[8] = (byte)(data_read[4] >> 4);
            data_time[9] = (byte)(data_read[5] & 0xF);

            // Set max value to current value
            address = 0x49D;
            number = 6;

            if (write_safe(address, number, WRITENIB, data_value, command) != number)
                write_error_exit();

            // Set max value timestamp to current time
            address = 0x4A3;
            number = 10;

            if (write_safe(address, number, WRITENIB, data_time, command) != number)
                write_error_exit();

            return 1;
        }


        /********************************************************************/
        /* rain_24h_reset
         * Reset current rain 24h
         * 
         * Input: Handle to weatherstation
         *        minmax - char (8 bit integer) that controls if minimum,
         *                 maximum or both are reset
         * Output: None
         *
         * Returns: 1 if success
         *
         ********************************************************************/
        public int rain_24h_reset()
        {
            byte[] data = new byte[50];
            byte[] command = new byte[60];	//room for write data also
            int address;
            int number;

            // First overwrite the 24h rain history with zeros
            address = 0x446;
            number = 48;
            memset(data, 0);

            if (write_safe(address, number, WRITENIB, data, command) != number)
                write_error_exit();

            // Set value to zero
            address = 0x497;
            number = 6;

            if (write_safe(address, number, WRITENIB, data, command) != number)
                write_error_exit();

            return 1;
        }

        /********************************************************************
         * rain_total
         * Read rain accumulated total, current value only
         * 
         * Input: Handle to weatherstation
         *        rain_conv_factor controlling convertion to other
         *             units than mm
         *
         * Returns: rain (double) converted to unit given in config
         *
         ********************************************************************/
        public double rain_total(double rain_conv_factor)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x4D2;
            int bytes = 3;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            return (((data[2] >> 4) * 1000 + (data[2] & 0xF) * 100 +
                     (data[1] >> 4) * 10 + (data[1] & 0xF) +
                     (data[0] >> 4) / 10.0 + (data[0] & 0xF) / 100.0) /
                     rain_conv_factor);
        }


        /********************************************************************
         * rain_total_all
         * Read rain total accumulated with timestamp
         * 
         * Input: Handle to weatherstation
         *        rain_conv_factor controlling convertion to other
         *             units than mm
         *
         * Output: Timestamp for rain total in pointers to
         *                timestamp structures for time_since
         *
         * Returns: rain (double) converted to unit given in config
         *
         ********************************************************************/
        public double rain_total_all(
                           double rain_conv_factor,
                   ref timestamp time_since)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address = 0x4D2;
            int bytes = 8;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            time_since.minute = ((data[3] >> 4) * 10) + (data[3] & 0xF);
            time_since.hour = ((data[4] >> 4) * 10) + (data[4] & 0xF);
            time_since.day = ((data[5] >> 4) * 10) + (data[5] & 0xF);
            time_since.month = ((data[6] >> 4) * 10) + (data[6] & 0xF);
            time_since.year = 2000 + ((data[7] >> 4) * 10) + (data[7] & 0xF);

            return (((data[2] >> 4) * 1000 + (data[2] & 0xF) * 100 +
                     (data[1] >> 4) * 10 + (data[1] & 0xF) +
                     (data[0] >> 4) / 10.0 + (data[0] & 0xF) / 100.0) /
                     rain_conv_factor);
        }


        /********************************************************************/
        /* rain_total_reset
         * Reset current total rain
         * 
         * Input: Handle to weatherstation
         *        minmax - char (8 bit integer) that controls if minimum,
         *                 maximum or both are reset
         * Output: None
         *
         * Returns: 1 if success
         *
         ********************************************************************/
        public int rain_total_reset()
        {
            byte[] data_read = new byte[20];
            byte[] data_value = new byte[20];
            byte[] data_time = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address;
            int number;

            // Get current time from station
            address = 0x23B;
            number = 6;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_time[0] = (byte)(data_read[0] & 0xF);
            data_time[1] = (byte)(data_read[0] >> 4);
            data_time[2] = (byte)(data_read[1] & 0xF);
            data_time[3] = (byte)(data_read[1] >> 4);
            data_time[4] = (byte)(data_read[2] >> 4);
            data_time[5] = (byte)(data_read[3] & 0xF);
            data_time[6] = (byte)(data_read[3] >> 4);
            data_time[7] = (byte)(data_read[4] & 0xF);
            data_time[8] = (byte)(data_read[4] >> 4);
            data_time[9] = (byte)(data_read[5] & 0xF);

            // Set value to zero
            address = 0x4D1;
            number = 7;
            memset(data_value, 0);

            if (write_safe(address, number, WRITENIB, data_value, command) != number)
                write_error_exit();

            // Set max value timestamp to current time
            address = 0x4D8;
            number = 10;

            if (write_safe(address, number, WRITENIB, data_time, command) != number)
                write_error_exit();

            return 1;
        }


        /********************************************************************
         * rel_pressure
         * Read relaive air pressure, current value only
         * 
         * Input: Handle to weatherstation
         *        pressure_conv_factor controlling convertion to other
         *             units than hPa
         *
         * Returns: pressure (double) converted to unit given in config
         *
         ********************************************************************/
        public double rel_pressure(double pressure_conv_factor)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];
            int address = 0x5E2;
            int bytes = 3;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();


            return (((data[2] & 0xF) * 1000 + (data[1] >> 4) * 100 +
                     (data[1] & 0xF) * 10 + (data[0] >> 4) +
                     (data[0] & 0xF) / 10.0) / pressure_conv_factor);
        }


        /********************************************************************
         * rel_pressure_minmax
         * Read relative pressure min/max with timestamps
         * 
         * Input: Handle to weatherstation
         *        pressure_conv_factor controlling convertion to other
         *             units than hPa
         *
         * Output: Pressure pres_min and pres_max (double)
         *                unit defined by config conversion factor
         *         Timestamps for pres_min and pres_max in pointers to
         *                timestamp structures for time_min and time_max
         *
         * Returns: nothing
         *
         ********************************************************************/
        public void rel_pressure_minmax(
                                 double pressure_conv_factor,
                         ref double pres_min,
                         ref double pres_max,
                         ref timestamp time_min,
                         ref timestamp time_max)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];
            int address = 0x600;
            int bytes = 13;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            pres_min = ((data[2] & 0xF) * 1000 + (data[1] >> 4) * 100 +
                        (data[1] & 0xF) * 10 + (data[0] >> 4) +
                        (data[0] & 0xF) / 10.0) / pressure_conv_factor;

            pres_max = ((data[12] & 0xF) * 1000 + (data[11] >> 4) * 100 +
                        (data[11] & 0xF) * 10 + (data[10] >> 4) +
                        (data[10] & 0xF) / 10.0) / pressure_conv_factor;

            address = 0x61E; //Relative pressure time and date for min/max
            bytes = 10;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            time_min.minute = ((data[0] >> 4) * 10) + (data[0] & 0xF);
            time_min.hour = ((data[1] >> 4) * 10) + (data[1] & 0xF);
            time_min.day = ((data[2] >> 4) * 10) + (data[2] & 0xF);
            time_min.month = ((data[3] >> 4) * 10) + (data[3] & 0xF);
            time_min.year = 2000 + ((data[4] >> 4) * 10) + (data[4] & 0xF);

            time_max.minute = ((data[5] >> 4) * 10) + (data[5] & 0xF);
            time_max.hour = ((data[6] >> 4) * 10) + (data[6] & 0xF);
            time_max.day = ((data[7] >> 4) * 10) + (data[7] & 0xF);
            time_max.month = ((data[8] >> 4) * 10) + (data[8] & 0xF);
            time_max.year = 2000 + ((data[9] >> 4) * 10) + (data[9] & 0xF);

            return;
        }


        /********************************************************************
         * abs_pressure
         * Read absolute air pressure, current value only
         * 
         * Input: Handle to weatherstation
         *        pressure_conv_factor controlling convertion to other
         *             units than hPa
         *
         * Returns: pressure (double) converted to unit given in config
         *
         ********************************************************************/
        public double abs_pressure(double pressure_conv_factor)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];
            int address = 0x5D8;
            int bytes = 3;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();


            return (((data[2] & 0xF) * 1000 + (data[1] >> 4) * 100 +
                     (data[1] & 0xF) * 10 + (data[0] >> 4) +
                     (data[0] & 0xF) / 10.0) / pressure_conv_factor);
        }


        /********************************************************************
         * abs_pressure_minmax
         * Read absolute pressure min/max with timestamps
         * 
         * Input: Handle to weatherstation
         *        pressure_conv_factor controlling convertion to other
         *             units than hPa
         *
         * Output: Pressure pres_min and pres_max (double)
         *                unit defined by config conversion factor
         *         Timestamps for pres_min and pres_max in pointers to
         *                timestamp structures for time_min and time_max
         *
         * Returns: nothing
         *
         ********************************************************************/
        public void abs_pressure_minmax(
                                 double pressure_conv_factor,
                         ref double pres_min,
                         ref double pres_max,
                         ref timestamp time_min,
                         ref timestamp time_max)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];
            int address = 0x5F6;
            int bytes = 13;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            pres_min = ((data[2] & 0xF) * 1000 + (data[1] >> 4) * 100 +
                        (data[1] & 0xF) * 10 + (data[0] >> 4) +
                        (data[0] & 0xF) / 10.0) / pressure_conv_factor;

            pres_max = ((data[12] & 0xF) * 1000 + (data[11] >> 4) * 100 +
                        (data[11] & 0xF) * 10 + (data[10] >> 4) +
                        (data[10] & 0xF) / 10.0) / pressure_conv_factor;

            address = 0x61E; //Relative pressure time and date for min/max
            bytes = 10;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            time_min.minute = ((data[0] >> 4) * 10) + (data[0] & 0xF);
            time_min.hour = ((data[1] >> 4) * 10) + (data[1] & 0xF);
            time_min.day = ((data[2] >> 4) * 10) + (data[2] & 0xF);
            time_min.month = ((data[3] >> 4) * 10) + (data[3] & 0xF);
            time_min.year = 2000 + ((data[4] >> 4) * 10) + (data[4] & 0xF);

            time_max.minute = ((data[5] >> 4) * 10) + (data[5] & 0xF);
            time_max.hour = ((data[6] >> 4) * 10) + (data[6] & 0xF);
            time_max.day = ((data[7] >> 4) * 10) + (data[7] & 0xF);
            time_max.month = ((data[8] >> 4) * 10) + (data[8] & 0xF);
            time_max.year = 2000 + ((data[9] >> 4) * 10) + (data[9] & 0xF);

            return;
        }

        /********************************************************************/
        /* pressure_reset
         * Reset min/max pressure (relative and absolute) with timestamps
         * 
         * Input: Handle to weatherstation
         *        minmax - char (8 bit integer) that controls if minimum,
         *                 maximum or both are reset
         * Output: None
         *
         * Returns: 1 if success
         *
         ********************************************************************/
        public int pressure_reset(byte minmax)
        {
            byte[] data_read = new byte[20];
            byte[] data_value_abs = new byte[20];
            byte[] data_value_rel = new byte[20];
            byte[] data_time = new byte[20];
            byte[] command = new byte[25];	//room for write data also
            int address;
            int number;

            // First read current abs/rel pressure into data_value_abs/rel
            address = 0x5D8;
            number = 8;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_value_abs[0] = (byte)(data_read[0] & 0xF);
            data_value_abs[1] = (byte)(data_read[0] >> 4);
            data_value_abs[2] = (byte)(data_read[1] & 0xF);
            data_value_abs[3] = (byte)(data_read[1] >> 4);
            data_value_abs[4] = (byte)(data_read[2] & 0xF);

            data_value_rel[0] = (byte)(data_read[5] & 0xF);
            data_value_rel[1] = (byte)(data_read[5] >> 4);
            data_value_rel[2] = (byte)(data_read[6] & 0xF);
            data_value_rel[3] = (byte)(data_read[6] >> 4);
            data_value_rel[4] = (byte)(data_read[7] & 0xF);

            // Get current time from station
            address = 0x23B;
            number = 6;

            if (read_safe(address, number, data_read, command) != number)
                read_error_exit();

            data_time[0] = (byte)(data_read[0] & 0xF);
            data_time[1] = (byte)(data_read[0] >> 4);
            data_time[2] = (byte)(data_read[1] & 0xF);
            data_time[3] = (byte)(data_read[1] >> 4);
            data_time[4] = (byte)(data_read[2] >> 4);
            data_time[5] = (byte)(data_read[3] & 0xF);
            data_time[6] = (byte)(data_read[3] >> 4);
            data_time[7] = (byte)(data_read[4] & 0xF);
            data_time[8] = (byte)(data_read[4] >> 4);
            data_time[9] = (byte)(data_read[5] & 0xF);

            if ((minmax & RESET_MIN) == RESET_MIN) // minimum
            {
                // Set min abs value to current abs value
                address = 0x5F6;
                number = 5;

                if (write_safe(address, number, WRITENIB, data_value_abs, command) != number)
                    write_error_exit();

                // Set min rel value to current rel value
                address = 0x600;
                number = 5;

                if (write_safe(address, number, WRITENIB, data_value_rel, command) != number)
                    write_error_exit();

                // Set min value timestamp to current time
                address = 0x61E;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            if ((minmax & RESET_MAX) == RESET_MAX) // maximum
            {
                // Set max abs value to current abs value
                address = 0x60A;
                number = 5;

                if (write_safe(address, number, WRITENIB, data_value_abs, command) != number)
                    write_error_exit();

                // Set max rel value to current rel value
                address = 0x614;
                number = 5;

                if (write_safe(address, number, WRITENIB, data_value_rel, command) != number)
                    write_error_exit();

                // Set max value timestamp to current time
                address = 0x628;
                number = 10;

                if (write_safe(address, number, WRITENIB, data_time, command) != number)
                    write_error_exit();
            }

            return 1;
        }


        /********************************************************************
         * pressure_correction
         * Read the correction from absolute to relaive air pressure
         * 
         * Input: Handle to weatherstation
         *        pressure_conv_factor controlling convertion to other
         *             units than hPa
         *
         * Returns: pressure (double) converted to unit given in conv factor
         *
         ********************************************************************/
        public double pressure_correction(double pressure_conv_factor)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];
            int address = 0x5EC;
            int bytes = 3;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();


            return ((data[2] & 0xF) * 1000 +
                    (data[1] >> 4) * 100 +
                    (data[1] & 0xF) * 10 +
                    (data[0] >> 4) +
                    (data[0] & 0xF) / 10.0 -
                    1000
                   ) / pressure_conv_factor;
        }


        /********************************************************************
         * tendency_forecast
         * Read Tendency and Forecast
         * 
         * Input: Handle to weatherstation
         *
         * Output: tendency - string Steady, Rising or Falling
         *         forecast - string Rainy, Cloudy or Sunny
         *
         * Returns: nothing
         *
         ********************************************************************/
        public void tendency_forecast(ref string tendency, ref string forecast)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];
            int address = 0x26B;
            int bytes = 1;
            string[] tendency_values = new string[] { "Steady", "Rising", "Falling" };
            string[] forecast_values = new string[] { "Rainy", "Cloudy", "Sunny" };

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            tendency = tendency_values[data[0] >> 4];
            forecast = forecast_values[data[0] & 0xF];

            return;
        }


        /********************************************************************
         * read_history_info
         * Read the history information like interval, countdown, time
         * of last record, pointer to last record.
         * 
         * Input:  Handle to weatherstation
         *        
         * Output: interval - Current interval in minutes (integer)
         *         countdown - Countdown to next measurement (integer)
         *         timelast - Time/Date for last measurement (timestamp struct)
         *         no_records - number of valid records (integer)
         *
         * Returns: interger pointing to last written record. [0x00-0xAE]
         *
         ********************************************************************/
        public int read_history_info(ref int interval, ref int countdown,
                 ref timestamp time_last, ref int no_records)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];
            int address = 0x6B2;
            int bytes = 10;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            interval = (data[1] & 0xF) * 256 + data[0] + 1;
            countdown = data[2] * 16 + (data[1] >> 4) + 1;
            time_last.minute = ((data[3] >> 4) * 10) + (data[3] & 0xF);
            time_last.hour = ((data[4] >> 4) * 10) + (data[4] & 0xF);
            time_last.day = ((data[5] >> 4) * 10) + (data[5] & 0xF);
            time_last.month = ((data[6] >> 4) * 10) + (data[6] & 0xF);
            time_last.year = 2000 + ((data[7] >> 4) * 10) + (data[7] & 0xF);
            no_records = data[9];

            return data[8];

        }


        /********************************************************************
         * read_history_record
         * Read the history information like interval, countdown, time
         * of last record, pointer to last record.
         * 
         * Input:  Handle to weatherstation
         *         config structure with conversion factors
         *         record - record index number to be read [0x00-0xAE]
         *        
         * Output: temperature_indoor (double)
         *         temperature_indoor (double)
         *         pressure (double)
         *         humidity_indoor (integer)
         *         humidity_outdoor (integer)
         *         raincount (double)
         *         windspeed (double)
         *         windir_degrees (double)
         *         dewpoint (double) - calculated
         *         windchill (double) - calculated, new post 2001 formula
         *
         * Returns: integer index number pointing to next record 
         *
         ********************************************************************/
        /// <summary>
        /// Read the history record, calculate windchill and dew point
        /// </summary>
        /// <param name="record">record to be read</param>
        /// <param name="config"></param>
        /// <param name="temperature_indoor"></param>
        /// <param name="temperature_outdoor"></param>
        /// <param name="pressure"></param>
        /// <param name="humidity_indoor"></param>
        /// <param name="humidity_outdoor"></param>
        /// <param name="raincount"></param>
        /// <param name="windspeed"></param>
        /// <param name="winddir_degrees"></param>
        /// <param name="dewpoint"></param>
        /// <param name="windchill"></param>
        /// <returns>pointer to next history record</returns>
        public int read_history_record(
                                int record,
                        ref config_type config,
                        ref double temperature_indoor,
                        ref double temperature_outdoor,
                        ref double pressure,
                        ref int humidity_indoor,
                        ref int humidity_outdoor,
                        ref double raincount,
                        ref double windspeed,
                        ref double winddir_degrees,
                        ref double dewpoint,
                        ref double windchill)
        {
            byte[] data = new byte[20];
            byte[] command = new byte[25];
            int address;
            int bytes = 10;
            long tempint;
            double A, B, C; // Intermediate values used for dewpoint calculation
            double wind_kmph;

            address = 0x6C6 + record * 19;

            if (read_safe(address, bytes, data, command) != bytes)
                read_error_exit();

            tempint = (data[4] << 12) + (data[3] << 4) + (data[2] >> 4);

            pressure = 1000 + (tempint % 10000) / 10.0;

            if (pressure >= 1502.2)
                pressure = pressure - 1000;

            pressure = pressure / config.pressure_conv_factor;

            humidity_indoor = Convert.ToInt16((tempint - (tempint % 10000)) / 10000.0);

            humidity_outdoor = (data[5] >> 4) * 10 + (data[5] & 0xF);

            raincount = ((data[7] & 0xF) * 256 + data[6]) * 0.518 / config.rain_conv_factor;

            windspeed = (data[8] * 16 + (data[7] >> 4)) / 10.0; //Need metric for WC

            winddir_degrees = (data[9] & 0xF) * 22.5;

            // Temperatures	in Celsius. Cannot convert until WC is calculated
            tempint = ((data[2] & 0xF) << 16) + (data[1] << 8) + data[0];
            temperature_indoor = (tempint % 1000) / 10.0 - 30.0;
            temperature_outdoor = (tempint - (tempint % 1000)) / 10000.0 - 30.0;

            // Calculate windchill using new post 2001 USA/Canadian formula
            // Twc = 13.112 + 0.6215*Ta -11.37*V^0.16 + 0.3965*Ta*V^0.16 [Celsius and km/h] 

            wind_kmph = 3.6 * windspeed;
            if (wind_kmph > 4.8)
            {
                windchill = 13.12 + 0.6215 * temperature_outdoor -
                             11.37 * Math.Pow(wind_kmph, 0.16) +
                             0.3965 * temperature_outdoor * Math.Pow(wind_kmph, 0.16);
            }
            else
            {
                windchill = temperature_outdoor;
            }

            // Calculate dewpoint
            // REF http://www.faqs.org/faqs/meteorology/temp-dewpoint/             
            A = 17.2694;
            B = (temperature_outdoor > 0) ? 237.3 : 265.5;
            C = (A * temperature_outdoor) / (B + temperature_outdoor) + Math.Log((double)humidity_outdoor / 100);
            dewpoint = B * C / (A - C);

            // Now that WC/DP is calculated we can convert all temperatures and winds
            if (config.temperature_conv == 1)
            {
                temperature_indoor = temperature_indoor * 9 / 5 + 32;
                temperature_outdoor = temperature_outdoor * 9 / 5 + 32;
                windchill = windchill * 9 / 5 + 32;
                dewpoint = dewpoint * 9 / 5 + 32;
            }

            windspeed *= config.wind_speed_conv_factor;

            return (++record) % 0xAF;
        }

        /********************************************************************
         * light
         * Turns display light on and off
         *
         * Input: control - integer -   0 = off, Anything else = on
         *
         * Returns: Nothing
         *
         ********************************************************************/
        public void light(int control)
        {
            byte[] data = new byte[1];
            byte[] command = new byte[25];  //Data returned is just ignored
            int address = 0x016;
            int number = 1;
            byte encode_constant;

            data[0] = 0;
            encode_constant = UNSETBIT;
            if (control != 0)
                encode_constant = SETBIT;

            if (write_safe(address, number, encode_constant, data, command) != number)
                write_error_exit();

            return;
        }
        #endregion

        #region util
        private void memset(byte[] data, int value)
        {
            data.SetValue((byte)value, 0, data.Length);
        }

        private void Sleep(int p)
        {
            System.Threading.Thread.Sleep(p);
        }

        private void printf(string p)
        {
            System.Console.WriteLine(p);
            throw new Exception("Errore " + p.ToString());
        }

        private void printf(string p, bool debug)
        {
            System.Console.WriteLine(p);
        }

        private void read_error_exit()
        {
            System.Console.WriteLine("read_safe() error");
            throw new Exception("Errore read_safe()");
        }

        private void write_error_exit()
        {
            System.Console.WriteLine("write_safe() error");
            throw new Exception("write_safe() error");
        }

        private void exit(int p)
        {
            System.Console.WriteLine("EXIT");
            throw new Exception("Errore " + p.ToString());
        }

        public string status()
        {
            return wsdevice.status();
        }
        #endregion
    }
}
