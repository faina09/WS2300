using System;
using System.Threading;
using System.Windows.Forms;

namespace WS2300
{
    class ws2300bg
    {
        private WS2300base m_ws2300;
        private WS2300base.config_type m_config;
        private string m_variables;
        private DateTime m_time_lastrecord;
        private double m_pressure_term;
        private int m_new_records, m_interval, m_lastlog_record;

        /// <summary>
        /// Usually a form or a winform control that implements "Invoke/BeginInvode"
        /// </summary>
        ContainerControl m_sender = null;

        /// <summary>
        /// The delegate method (callback) on the sender to call
        /// </summary>
        Delegate m_senderDelegate = null;

        // class constructor logic here
        public ws2300bg(ContainerControl sender, Delegate senderDelegate, params object[] list)
        {
            m_sender = sender;
            m_senderDelegate = senderDelegate;
            m_ws2300 = (WS2300base)list.GetValue(0);
            m_config = (WS2300base.config_type)list.GetValue(1);
            m_variables = (string)list.GetValue(2);
        }

        /// <summary>
        /// Method for ThreadStart delegate
        /// </summary>
        public void RunProcess()
        {
            string[] var;
            string rVal;
            double ret;
            int tempint;
            string[] directions = new string[]{"N","NNE","NE","ENE","E","ESE","SE","SSE",
	                           "S","SSW","SW","WSW","W","WNW","NW","NNW"};
            short[] winddir = new short[6];

            Thread.CurrentThread.IsBackground = true; //make them a daemon
            try
            {
                var = m_variables.Split(',');
                rVal = "undef";
                foreach (string s in var)
                {
                    switch (s.Trim().ToUpper())
                    {
                        case "T_IN":
                            rVal = m_ws2300.temperature_indoor(m_config.temperature_conv).ToString("F2");
                            break;
                        case "T_OUT":
                            rVal = m_ws2300.temperature_outdoor(m_config.temperature_conv).ToString("F2");
                            break;
                        case "DEW":
                            rVal = m_ws2300.dewpoint(m_config.temperature_conv).ToString("F2");
                            break;
                        case "H_IN":
                            rVal = m_ws2300.humidity_indoor().ToString();
                            break;
                        case "H_OUT":
                            rVal = m_ws2300.humidity_outdoor().ToString();
                            break;
                        case "WIND":
                            tempint = -1;
                            winddir[0] = 0xFFF;
                            ret = m_ws2300.wind_all(m_config.wind_speed_conv_factor, ref tempint, ref winddir);
                            rVal = "Wind=" + ret.ToString() + " dir:" + winddir[0].ToString() + "° " + directions[tempint];
                            break;
                        case "W_CHILL":
                            rVal = m_ws2300.windchill(m_config.temperature_conv).ToString("F2");
                            break;
                        case "PRESS":
                            rVal = m_ws2300.rel_pressure(m_config.pressure_conv_factor).ToString("F2");
                            break;
                        case "HISTORY":
                            rVal = HistoryRecord();
                            break;
                    }
                    if (rVal != "")
                        UpdateMessage(rVal, false);
                }
                UpdateMessage("read ENDED", true);
            }
            catch (Exception ex)
            {
                UpdateMessage("ERROR: " + ex.Message, true);
            }
        }

        public DateTime LastRecordDateTime
        {
            set
            {
                m_time_lastrecord = value;
            }
            get
            {
                return m_time_lastrecord;
            }
        }

        public double PressureTerm
        {
            set
            {
                m_pressure_term = value;
            }
            get
            {
                return m_pressure_term;
            }
        }
        
        public int NewRecords
        {
            set
            {
                m_new_records = value;
            }
            get
            {
                return m_new_records;
            }
        }
        
        public int Interval
        {
            set
            {
                m_interval = value;
            }
            get
            {
                return m_interval;
            }
        }

        public int LastLogRecord
        {
            set
            {
                m_lastlog_record = value;
            }
            get
            {
                return m_lastlog_record;
            }
        }
        
        private string HistoryRecord()
        {
            double t_in, t_out, press;
            int h_in, h_out;
            double rain, w_speed, w_deg, dew, w_chill;
            int nextHistoryRecord;
            string strO;

            strO = "";
            t_in = t_out = press = -1;
            h_in = h_out = 1;
            rain = w_speed = w_deg = dew = w_chill = -1;
            for (int i = 1; i <= m_new_records; i++)
            {
                nextHistoryRecord = (i + m_lastlog_record) % 0xAF;
                m_ws2300.read_history_record(nextHistoryRecord, ref m_config, ref t_in, ref t_out, ref press, ref h_in, ref h_out, ref rain, ref w_speed, ref w_deg, ref dew, ref w_chill);
                m_time_lastrecord = m_time_lastrecord.AddMinutes((double)m_interval);
                if (m_config.history_log_type == "csv")
                {
                    if (h_out > 100)
                    {
                        // out-of-range values:
                        // outdoor values invalid due to communication error with external station
                        strO = timeDB(m_time_lastrecord, 0) + "," + timeDB(m_time_lastrecord, 1) + "," + timeDB(m_time_lastrecord, 2) + "," + decimalDB(t_in) + ",null" +
                            ",null," + h_in.ToString() + ",null, null, null, null, null" +
                            ",0,0," + decimalDB(rain) + "," + decimalDB(press + m_pressure_term) + ",0,0";
                    }
                    else
                    {
                        strO = timeDB(m_time_lastrecord, 0) + "," + timeDB(m_time_lastrecord, 1) + "," + timeDB(m_time_lastrecord, 2) + "," + decimalDB(t_in) + "," + decimalDB(t_out) +
                            "," + decimalDB(dew) + "," + h_in.ToString() + "," + h_out.ToString() + "," + decimalDB(w_speed) + "," + decimalDB(w_deg) + ",," + decimalDB(w_chill) +
                            ",0,0," + decimalDB(rain) + "," + decimalDB(press + m_pressure_term) + ",0,0";
                    }
                }
                else
                {
                    strO = "INSERT INTO weather (timestamp, rec_date, rec_time, temp_in, temp_out, " +
                        "dewpoint, rel_hum_in, rel_hum_out, windspeed, wind_angle, wind_direction, wind_chill, " +
                        "rain_1h, rain_24h, rain_total, rel_pressure, tendency, forecast) VALUES (";
                    if (h_out > 100)
                    {
                        // out-of-range values:
                        // outdoor values invalid due to communication error with external station
                        strO += timeDB(m_time_lastrecord, 0) + ",'" + timeDB(m_time_lastrecord, 1) + "','" + timeDB(m_time_lastrecord, 2) + "','" + decimalDB(t_in) + "',null" +
                            ", null,'" + h_in.ToString() + "',null, null, null, null, null" +
                            ",'0','0','" + decimalDB(rain) + "','" + decimalDB(press + m_pressure_term) + "','0','0');";
                    }
                    else
                    {
                        strO += timeDB(m_time_lastrecord, 0) + ",'" + timeDB(m_time_lastrecord, 1) + "','" + timeDB(m_time_lastrecord, 2) + "','" + decimalDB(t_in) + "','" + decimalDB(t_out) +
                            "','" + decimalDB(dew) + "','" + h_in.ToString() + "','" + h_out.ToString() + "','" + decimalDB(w_speed) + "','" + decimalDB(w_deg) + "','','" + decimalDB(w_chill) +
                            "','0','0','" + decimalDB(rain) + "','" + decimalDB(press + m_pressure_term) + "','0','0');";
                    }
                }
                UpdateMessage(strO, false, i);
            }
            return "";
        }

        private string decimalDB(double val)
        {
            return val.ToString("F1").Replace(',', '.');
        }

        private string timeDB(DateTime timestr, int type)
        {
            switch (type)
            {
                case 0:
                    return timestr.Ticks.ToString();
                case 1:
                    return timestr.Year + timestr.Month.ToString("D2") + timestr.Day.ToString("D2");
                case 2:
                    return timestr.Hour.ToString("D2") + ":" + timestr.Minute.ToString("D2");
            }
            return "";
        }

        private void UpdateMessage(string msg, bool done)
        {
            m_sender.BeginInvoke(m_senderDelegate, new object[] { msg, done, -1 });
        }

        private void UpdateMessage(string msg, bool done, int cnt)
        {
            m_sender.BeginInvoke(m_senderDelegate, new object[] { msg, done, cnt });
        }
    }
}
