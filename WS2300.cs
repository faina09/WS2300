using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;

/*  C# code based on OpenSource Project open2300 */

namespace WS2300
{
    public partial class WS2300 : Form
    {
        private WS2300base ws2300;
        private WS2300base.config_type config;
        private StreamWriter logsr;
        private readonly string LogFile;
        private const string VER = "WS2300 1.0 - 2008-12-19";
        /* Revision History
        1.0 2007-04-26 initial release
         * 2007-05-02 sample using threads
         * 2007-05-04 history using threads; history writes History.sql
         * 2007-05-07 graphics added
         * 2007-05-08 minor changes
         * 2007-05-11 history save to and read from a CSV file
         * 2007-05-13 Mean max min T_ext
         * 2007-05-14 Mean and mean of means T_ext
         * 2007-05-22 No line symbol; line color pickup
         * 2007-05-24 Open/Close COM
         * 2008-06-17 Converted to VS2008
         *            max/min/mean on each year (2007, 2008)
         *            comboBox to choice COM1, COM2,...COM6
         * 2008-11-25 read and reset min/max
         * 2008-12-19 set time scale to last 2 months
         * @@ TODO: read/write config values from an ini/cfg file
        */

        public WS2300()
        {
            InitializeComponent();
            this.Text = VER;
            config = new WS2300base.config_type();
            // TODO: values to be read from a config file
            config.wind_speed_conv_factor = 1.0;                   // Speed dimention, m/s is default
            config.temperature_conv = 0;                           // Temperature in Celsius
            config.rain_conv_factor = 1.0;                         // Rain in mm
            config.pressure_conv_factor = 1.0;                     // Pressure in hPa (same as millibar)
            config.history_log_type = "csv";
            //config.history_log_type = "sql";
            config.port = "COM1";

            LogFile = Application.StartupPath + "\\History";
            if (config.history_log_type == "csv")
            {
                lblHistory.Text += ".csv";
                LogFile += ".csv";
            }
            else
            {
                lblHistory.Text += ".sql";
                LogFile += ".sql";
            }
            this.toolStripStatusLabel.Text = "Click on OPEN COM";
            lblIntTemp.Text = "";
            DateTime dt = this.dateTimePicker1.Value;
            long s;
            // set max val all'ora precedente
            // s = 3600 + dt.Minute * 60 + dt.Second
            // set max val al minuto precedente
            s = 60 + dt.Second;
            this.dateTimePicker1.Value = new DateTime(dt.Ticks - s * 10000000);
            this.dateTimePicker1.MaxDate = this.dateTimePicker1.Value;
        }

        private void Display(string p)
        {
            this.textBoxReceive.Text += p + Environment.NewLine;
        }

        private void BtnInTmp_Click(object sender, EventArgs e)
        {
            try
            {
                double ret = ws2300.temperature_indoor(config.temperature_conv);
                lblIntTemp.Text = "TEMP=" + ret.ToString();
            }
            catch (Exception ex)
            {
                lblIntTemp.Text = "ERROR: " + ex.Message;
            }
        }

        private void BtnRead_Click(object sender, EventArgs e)
        {
            double ret;
            int tempint;
            string[] directions = new string[]{"N","NNE","NE","ENE","E","ESE","SE","SSE",
                               "S","SSW","SW","WSW","W","WNW","NW","NNW"};
            short[] winddir = new short[6];
            try
            {
                /* READ TEMPERATURE INDOOR */
                ret = ws2300.temperature_indoor(config.temperature_conv);
                Display("TEMP in=" + ret.ToString());
                /* READ TEMPERATURE OUTDOOR */
                ret = ws2300.temperature_outdoor(config.temperature_conv);
                Display("TEMP out=" + ret.ToString());
                /* READ DEWPOINT */
                ret = ws2300.dewpoint(config.temperature_conv);
                Display("Dew point=" + ret.ToString());
                /* READ RELATIVE HUMIDITY INDOOR */
                ret = ws2300.humidity_indoor();
                Display("HUM in=" + ret.ToString());
                /* READ RELATIVE HUMIDITY OUTDOOR */
                ret = ws2300.humidity_outdoor();
                Display("HUM out=" + ret.ToString());
                /* READ WIND SPEED AND DIRECTION */
                tempint = -1;
                winddir[0] = 0xFFF;
                ret = ws2300.wind_all(config.wind_speed_conv_factor, ref tempint, ref winddir);
                Display("Wind=" + ret.ToString() + " dir:" + winddir[0].ToString() + "° " + directions[tempint]);
                //sprintf(logline, "%s%.1f %s ", logline, winddir[0], directions[tempint]);
                /* READ WINDCHILL */
                ret = ws2300.windchill(config.temperature_conv);
                Display("Windchill=" + ret.ToString());
                /* READ RAIN 1H */
                ret = ws2300.rain_1h(config.rain_conv_factor);
                Display("RAIN 1h=" + ret.ToString());
                /* READ RAIN 24H */
                ret = ws2300.rain_24h(config.rain_conv_factor);
                Display("RAIN 24h=" + ret.ToString());
                /* READ RAIN TOTAL */
                ret = ws2300.rain_total(config.rain_conv_factor);
                Display("RAIN tot=" + ret.ToString());
                /* READ RELATIVE PRESSURE */
                ret = ws2300.rel_pressure(config.pressure_conv_factor);
                Display("PRESS rel=" + ret.ToString());
                /* READ TENDENCY AND FORECAST */
                string tendency = "";
                string forecast = "";
                ws2300.tendency_forecast(ref tendency, ref forecast);
                Display("   tendency=" + tendency);
                Display("   forecast=" + forecast);
            }
            catch (Exception ex)
            {
                Display("ERROR: " + ex.Message);
            }
            Display("");
        }

        private void BtnMinMax_Click(object sender, EventArgs e)
        {
            double retmin, retmax;
            int retmini, retmaxi;
            WS2300base.timestamp timemin, timemax;

            retmin = -1;
            retmax = -1;
            retmini = -1;
            retmaxi = -1;
            timemin = new WS2300base.timestamp();
            timemax = new WS2300base.timestamp();
            try
            {
                ws2300.temperature_indoor_minmax(config.temperature_conv, ref retmin, ref retmax, ref timemin, ref timemax);
                Display("TEMP in min=" + retmin.ToString("F2") + Time(timemin));
                Display("TEMP in max=" + retmax.ToString("F2") + Time(timemax));
                ws2300.temperature_outdoor_minmax(config.temperature_conv, ref retmin, ref retmax, ref timemin, ref timemax);
                Display("TEMP out min=" + retmin.ToString("F2") + Time(timemin));
                Display("TEMP out max=" + retmax.ToString("F2") + Time(timemax));
                ws2300.humidity_indoor_all(ref retmini, ref retmaxi, ref timemin, ref timemax);
                Display("HUM in min=" + retmin.ToString("F2") + Time(timemin));
                Display("HUM in max=" + retmax.ToString("F2") + Time(timemax));
                ws2300.humidity_outdoor_all(ref retmini, ref retmaxi, ref timemin, ref timemax);
                Display("HUM out min=" + retmin.ToString("F2") + Time(timemin));
                Display("HUM out max=" + retmax.ToString("F2") + Time(timemax));
                ws2300.rel_pressure_minmax(config.pressure_conv_factor, ref retmin, ref retmax, ref timemin, ref timemax);
                Display("PRESS rel min=" + retmin.ToString("F2") + Time(timemin));
                Display("PRESS rel max=" + retmax.ToString("F2") + Time(timemax));
            }
            catch (Exception ex)
            {
                Display("ERROR: " + ex.Message);
            }
            Display("");
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            /*
            double s;
            s = 13.4562333333;
            display(" "+ s.ToString("N");
            display(" " + s.ToString("F");
            //display(" " + s.ToString("D");
            display(" " + s.ToString("G");
            */
            try
            {
                //ws.temperature_indoor_reset(ws2300, WS2300base.RESET_MIN);
                //ws.temperature_indoor_reset(ws2300, WS2300base.RESET_MAX);
                ws2300.temperature_indoor_reset(WS2300base.RESET_MIN | WS2300base.RESET_MAX);
                Display("TEMP in min/max resetted");
                ws2300.temperature_outdoor_reset(WS2300base.RESET_MIN | WS2300base.RESET_MAX);
                Display("TEMP out min/max resetted");
                ws2300.humidity_indoor_reset(WS2300base.RESET_MIN | WS2300base.RESET_MAX);
                Display("HUM in min/max resetted");
                ws2300.humidity_outdoor_reset(WS2300base.RESET_MIN | WS2300base.RESET_MAX);
                Display("HUM out min/max resetted");
                ws2300.pressure_reset(WS2300base.RESET_MIN | WS2300base.RESET_MAX);
                Display("PRESSURE min/max resetted");
                ws2300.rain_total_reset();
                Display("RAIN total resetted");
            }
            catch (Exception ex)
            {
                Display("ERROR: " + ex.Message);
            }
            Display("");
        }

        private void BtnHistory_Click(object sender, EventArgs e)
        {
            // console only:
            //History(0);
            History(1);
        }

        private void BtnHistorySave_Click(object sender, EventArgs e)
        {
            History(2);
            /*
             *  Table structure for table `weather`
             *
             CREATE TABLE `weather` (
            `timestamp` bigint(14) NOT NULL default '0',
            `rec_date` date NOT NULL default '0000-00-00',
            `rec_time` time NOT NULL default '00:00:00',
            `temp_in` decimal(3,1) NOT NULL default '0.0',
            `temp_out` decimal(3,1) default '0.0',
            `dewpoint` decimal(3,1) default '0.0',
            `rel_hum_in` tinyint(3) NOT NULL default '0',
            `rel_hum_out` tinyint(3) default '0',
            `windspeed` decimal(4,1) default '0.0',
            `wind_angle` decimal(4,1) default '0.0',
            `wind_direction` char(3) default NULL,
            `wind_chill` decimal(3,1) default '0.0',
            `rain_1h` decimal(4,1) NOT NULL default '0.0',
            `rain_24h` decimal(4,1) NOT NULL default '0.0',
            `rain_total` decimal(5,1) NOT NULL default '0.0',
            `rel_pressure` decimal(5,1) NOT NULL default '0.0',
            `tendency` varchar(7) NOT NULL default '',
            `forecast` varchar(6) NOT NULL default '',
            UNIQUE KEY `timestamp` (`timestamp`)
            ) ENGINE=MyISAM;
             */
        }

        private void History(int type)
        {
            // based on histlog2300.c
            WS2300base.timestamp time_last;
            DateTime time_lastrecord_tm, time_lastrecord;
            int interval, countdown, no_records;
            double pressure_term;
#if noTread
            double t_in, t_out, press;
            int h_in, h_out;
            double w_speed, w_deg, rain, dew, w_chill;
            string strO;
            int next_record;
            h_in = -1; h_out = -1;
            w_speed = -1; w_deg = -1;
            rain = -1; dew = -1; w_chill = -1;
            t_in = -1; t_out = -1;
            press = -1;
            strO = "";
            string[] directions = new string[]{"N","NNE","NE","ENE","E","ESE","SE","SSE",
	                           "S","SSW","SW","WSW","W","WNW","NW","NNW"};
#endif
            time_last = new WS2300base.timestamp();
            int current_record, lastlog_record, new_records;
            interval = -1;
            countdown = -1;
            no_records = -1;
            try
            {
                current_record = ws2300.read_history_info(ref interval, ref countdown, ref time_last, ref no_records);
                if (type != 2)
                {
                    Display("HISTORY last=" + current_record.ToString() + " interval:" + interval.ToString() +
                        " countdown:" + countdown.ToString() + " recno:" + no_records.ToString() + " timelast:" + Time(time_last));
                }
                time_lastrecord_tm = new DateTime(time_last.year, time_last.month, time_last.day,
                    time_last.hour, time_last.minute, 0);
                pressure_term = ws2300.pressure_correction(config.pressure_conv_factor);

                // da inizializzare con la data dell'ultima registrazione
                //time_lastrecord = new DateTime(1900, 1, 1, 0, 0, 0);
                time_lastrecord = new DateTime(2007, 4, 26, 14, 0, 0);
                //time_lastrecord_tm = new DateTime(2007, 4, 5, 7, 30, 0);
                DateTime dt = this.dateTimePicker1.Value;
                long s = dt.Minute * 60 + dt.Second;
                time_lastrecord = new DateTime(dt.Ticks - s * 10000000);

                DateTime d = new DateTime(time_lastrecord_tm.Ticks - time_lastrecord.Ticks);
                System.TimeSpan ds = time_lastrecord_tm.Subtract(time_lastrecord);
                new_records = (int)(ds.TotalMinutes / interval);

                if (new_records > 0xAF)
                {
                    new_records = 0xAF;
                }

                if (new_records > no_records)
                {
                    new_records = no_records;
                }

                lastlog_record = current_record - new_records;

                if (lastlog_record < 0)
                {
                    lastlog_record = 0xAE + lastlog_record + 1;
                }

                time_lastrecord_tm = time_lastrecord_tm.Subtract(System.TimeSpan.FromMinutes(new_records * interval));

#if noTread
                for (int i = 1; i <= new_records; i++)
                {
                    next_record = (i + lastlog_record) % 0xAF;
                    ws2300.read_history_record(next_record, ref config, ref t_in, ref t_out, ref press, ref h_in, ref h_out, ref rain, ref w_speed, ref w_deg, ref dew, ref w_chill);

                    time_lastrecord_tm = time_lastrecord_tm.AddMinutes((double)interval);

                    if (type == 0)
                    {
                        strO = time(time_lastrecord_tm);
                        strO += " T_in=" + t_in.ToString() + " T_out=" + t_out.ToString() + " P=" + (press + pressure_term).ToString() +
                            " h_in=" + h_in.ToString() + " h_out=" + h_out.ToString() + " w=" + w_speed.ToString() + " " + w_deg.ToString() +
                            " rain=" + rain.ToString() +
                            " w_chill=" + w_chill.ToString() + " dew=" + dew.ToString("F");
                        System.Console.WriteLine(next_record.ToString() + " " + strO);
                    }
                    if (type == 1)
                    {
                        //1011,1	18,3	44	21,6	36	5,9	20,9	2,1	SSW	0,0	22:39	25.05.2006
                        strO = (press + pressure_term).ToString("F1") + '\t' + t_in.ToString("F1") + '\t' + h_in.ToString();
                        strO += '\t' + t_out.ToString("F1") + '\t' + h_out.ToString() + '\t' + dew.ToString("F1") + '\t' +
                            w_chill.ToString("F1") + '\t' + w_speed.ToString() + '\t' + w_deg.ToString() + '\t' + rain.ToString("F1") + '\t' + time(time_lastrecord_tm);
                    }
                    if (type == 2)
                    {
                        strO = "INSERT INTO weather (timestamp, rec_date, rec_time, temp_in, temp_out, " +
                            "dewpoint, rel_hum_in, rel_hum_out, windspeed, wind_angle, wind_direction, wind_chill, " +
                            "rain_1h, rain_24h, rain_total, rel_pressure, tendency, forecast) VALUES (";
                        if (h_out > 100)
                        {
                            // out-of-range values:
                            // outdoor values invalid due to communication error with external station
                            strO += timeDB(time_lastrecord_tm, 0) + ",'" + timeDB(time_lastrecord_tm, 1) + "','" + timeDB(time_lastrecord_tm, 2) + "','" + decimalDB(t_in) + "',null" +
                                ", null,'" + h_in.ToString() + "',null, null, null, null, null" +
                                ",'0','0','" + decimalDB(rain) + "','" + decimalDB(press + pressure_term) + "','0','0');";
                        }
                        else
                        {
                            strO += timeDB(time_lastrecord_tm, 0) + ",'" + timeDB(time_lastrecord_tm, 1) + "','" + timeDB(time_lastrecord_tm, 2) + "','" + decimalDB(t_in) + "','" + decimalDB(t_out) +
                                "','" + decimalDB(dew) + "','" + h_in.ToString() + "','" + h_out.ToString() + "','" + decimalDB(w_speed) + "','" + decimalDB(w_deg) + "','','" + decimalDB(w_chill) +
                                "','0','0','" + decimalDB(rain) + "','" + decimalDB(press + pressure_term) + "','0','0');";
                        }
                    }
                    display(strO);
                }
#else
                Historybg(new_records, interval, lastlog_record, time_lastrecord_tm, pressure_term);
#endif
            }
            catch (Exception ex)
            {
                Display("ERROR: " + ex.Message);
            }
            Display("");
        }

        private string DecimalDB(double val)
        {
            return val.ToString("F1").Replace(',', '.');
        }

        private string TimeDB(DateTime timestr, int type)
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

        private string Time(WS2300base.timestamp timestr)
        {
            string retStr;
            retStr = " " + timestr.day.ToString() + "/" + timestr.month.ToString() + "/" + timestr.year.ToString();
            retStr += " " + timestr.hour.ToString() + ":" + timestr.minute.ToString();
            return retStr;
        }

        private string Time(DateTime timestr)
        {
            //17:00	24.04.2007
            string retStr;
            retStr = " " + timestr.Hour.ToString("D2") + ":" + timestr.Minute.ToString("D2") +
                " " + timestr.Day.ToString("D2") + "." + timestr.Month.ToString("D2") + "." + timestr.Year;
            //retStr = timestr.ToLongDateString();
            return retStr;
        }

        #region THREAD

        private void BtnReadTh_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text = "data reading...";
            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);
            ws2300bg ws = new ws2300bg(this, showProgress, new object[] { ws2300, config, "T_IN,T_OUT,DEW,H_IN,H_OUT,WIND,W_CHILL,PRESS" });
            Thread t = new Thread(new ThreadStart(ws.RunProcess));
            t.IsBackground = true; //make them a daemon - prevent thread callback issues
            btnInTmp.Enabled = false;
            btnMinMax.Enabled = false;
            btnRead.Enabled = false;
            btnReset.Enabled = false;
            btnReadTh.Enabled = false;
            btnHistorySave.Enabled = false;
            t.Start();
        }

        private delegate void ShowProgressDelegate(string msg, bool done, int cnt);

        private void ShowProgress(string msg, bool done, int cnt)
        {
            try
            {
                if (done)
                {
                    btnInTmp.Enabled = true;
                    btnMinMax.Enabled = true;
                    btnRead.Enabled = true;
                    btnReset.Enabled = true;
                    btnReadTh.Enabled = true;
                    btnHistorySave.Enabled = true;
                    if (logsr != null)
                    {
                        toolStripStatusLabel.Text = "Read history ENDED";
                        logsr.Close();
                        logsr = null;
                        btnGRAPH.Enabled = true;
                    }
                    else
                        toolStripStatusLabel.Text = msg;
                }
                else
                {
                    if (logsr != null)
                    {
                        logsr.WriteLine(msg);
                        toolStripStatusLabel.Text = "Read history record" + cnt.ToString();
                    }
                    else
                    {
                        Display(msg);
                    }
                }
            }
            catch (Exception ex)
            {
                Display("ERROR: " + ex.Message);
            }
        }

        private void Historybg(int new_records, int interval, int lastlog_record, DateTime time_lastrecord_tm, double pressure_term)
        {
            toolStripStatusLabel.Text = "data reading...";
            ShowProgressDelegate showProgress = new ShowProgressDelegate(ShowProgress);
            ws2300bg ws = new ws2300bg(this, showProgress, new object[] { ws2300, config, "HISTORY" });
            ws.NewRecords = new_records;
            ws.Interval = interval;
            ws.LastLogRecord = lastlog_record;
            ws.LastRecordDateTime = time_lastrecord_tm;
            ws.PressureTerm = pressure_term;
            Thread t = new Thread(new ThreadStart(ws.RunProcess));
            t.IsBackground = true; //make them a daemon - prevent thread callback issues
            btnInTmp.Enabled = false;
            btnMinMax.Enabled = false;
            btnRead.Enabled = false;
            btnReset.Enabled = false;
            btnReadTh.Enabled = false;
            btnHistorySave.Enabled = false;
            t.Start();
        }

        #endregion THREAD

        #region HistoryLog

        private void BtnViewLog_Click(object sender, EventArgs e)
        {
            string txtBody;
            int lstTimestampIdx;
            string lstTimestamp;
            try
            {
                if (File.Exists(LogFile))
                {
                    if (logsr != null)
                    {
                        // ferma il log
                        logsr.Close();
                    }
                    // open to read history log file
                    StreamReader sr = new StreamReader(LogFile);
                    // read the whole file
                    txtBody = sr.ReadToEnd();
                    sr.Close();
                    lstTimestampIdx = -1;
                    if (config.history_log_type == "csv")
                    {
                        // \r\n633138768000000000, .... \r\n
                        int idx, idxp;
                        int cnt;
                        // cerco la penultima occorrenza di \r\n, max lung riga 118 caratteri, min 60
                        idx = txtBody.LastIndexOf("\r\n") - 120;
                        cnt = 0;
                        idxp = idx;
                        while ((idx = txtBody.IndexOf("\r\n", ++idx)) > -1)
                            cnt++;
                        if (cnt == 2)
                            lstTimestampIdx = txtBody.IndexOf("\r\n", idxp) + 2;
                    }
                    else
                    {
                        // VALUES (633138768000000000,'
                        lstTimestampIdx = txtBody.LastIndexOf("VALUES (") + 8;
                    }
                    if (lstTimestampIdx > -1)
                    {
                        lstTimestamp = txtBody.Substring(lstTimestampIdx, 18);
                        dateTimePicker1.MaxDate = new DateTime(Convert.ToInt64(lstTimestamp));
                        dateTimePicker1.Value = new DateTime(Convert.ToInt64(lstTimestamp));
                    }
                    // riapre il log
                    logsr = File.AppendText(LogFile);
                }
                else
                {
                    logsr = File.CreateText(LogFile);
                }
                toolStripStatusLabel.Text = "Log file ready to be write!";
                btnGRAPH.Enabled = false;
            }
            catch (Exception ex)
            {
                toolStripStatusLabel.Text = "Log file invalid; delete it!";
                Display("ERROR: " + ex.Message);
            }
        }

        #endregion HistoryLog

        private void GRAPH_Click(object sender, EventArgs e)
        {
            Graph graph = new Graph(VER, LogFile);
            graph.ShowDialog();
            graph.Dispose();
        }

        private void BtnMean_Click(object sender, EventArgs e)
        {
            // media delle temperature esterne
            MaxMinList list = new MaxMinList();

            StreamReader sr = new StreamReader(LogFile);
            string line;
            string[] lval;
            string lv;
            double y;
            double ysum = 0;
            int cnt = 0;
            DateTime xd;
            int day = 0;
            int month = 0;
            int year = 0;
            int meteovar = 1; //T_out
            while ((line = sr.ReadLine()) != null)
            {
                lv = "";
                if (config.history_log_type == "csv")
                {
                    lval = line.Split(',');
                    if (lval.Length >= (meteovar + 3))
                    {
                        lv = lval[meteovar + 3].Replace('.', ',').Trim();
                    }
                }
                else
                {
                    line = line.Substring(line.LastIndexOf("(") + 1);
                    lval = line.Split(',');
                    if (lval.Length >= (meteovar + 3))
                    {
                        lv = lval[meteovar + 3].Replace('.', ',').Replace('\'', ' ').Trim();
                    }
                }
                if (lv != "null" && lv != "" && !line.StartsWith("--")) // ignore invalid points and comments
                {
                    y = Convert.ToDouble(lv);
                    long v = Convert.ToInt64(lval[0]);
                    xd = Convert.ToDateTime(new DateTime(v));
                    if (xd.Day == day && xd.Month == month && xd.Year == year)
                    {
                        if (y < list.Min(day, month, year))
                        {
                            list.SetMin(day, month, year, y);
                        }
                        if (y > list.Max(day, month, year))
                        {
                            list.SetMax(day, month, year, y);
                        }
                        cnt++;
                        ysum += y;
                    }
                    else
                    {
                        if (year > 0)
                        {
                            list.SetMean(day, month, year, ysum / cnt);
                        }

                        day = xd.Day;
                        month = xd.Month;
                        year = xd.Year;
                        list.Add(day, month, year, y);
                        cnt = 0;
                        ysum = 0;
                    }
                }
            }
            if (year > 0)
            {
                list.SetMean(day, month, year, ysum / cnt);
            }

            sr.Close();
            month = 0;
            int monthnew, yearnew;
            this.textBoxReceive.Clear();
            Display("Temperatures: Max, Min, Mean");
            Display("ALL 2007 data Means: " + list.EvaluateMeanMax(0, 2007).ToString("F1") + " "
                + list.EvaluateMeanMin(0, 2007).ToString("F1") + " " + list.EvaluateMeanMean(0, 2007).ToString("F1"));
            Display("ALL 2008 data Means: " + list.EvaluateMeanMax(0, 2008).ToString("F1") + " "
                + list.EvaluateMeanMin(0, 2008).ToString("F1") + " " + list.EvaluateMeanMean(0, 2008).ToString("F1"));
            for (int i = 0; i < list.Count(); i++)
            {
                monthnew = list.Maxmin(i).Month;
                yearnew = list.Maxmin(i).Year;
                if (month != monthnew)
                {
                    Display("Month " + monthnew.ToString() + "/" + yearnew.ToString() + " Means: " + list.EvaluateMeanMax(monthnew, yearnew).ToString("F1") + " "
                        + list.EvaluateMeanMin(monthnew, yearnew).ToString("F1") + " " + list.EvaluateMeanMean(monthnew, yearnew).ToString("F1"));
                    month = monthnew;
                }
                if (checkBoxValues.Checked)
                {
                    Display(list.Maxmin(i).Date + ": " + list.Maxmin(i).Max.ToString("F1") + " "
                        + list.Maxmin(i).Min.ToString("F1") + " " + list.Maxmin(i).Mean.ToString("F1"));
                }
            }
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                config.port = comboBoxCom.Text;
                ws2300 = new WS2300base(config.port);
                string str = ws2300.status();
                this.toolStripStatusLabel.Text = str;
            }
            catch (Exception ex)
            {
                this.toolStripStatusLabel.Text = ex.Message;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            try
            {
                ws2300.close();
                this.toolStripStatusLabel.Text = "COM closed";
            }
            catch (Exception)
            {
                this.toolStripStatusLabel.Text = "COM never opened";
            }
        }
    }

    public class MaxMinList
    {
        private readonly List<MaxMin> maxmin = new List<MaxMin>();

        public MaxMinList()
        {
        }

        public void Add(int day, int month, int year, double y)
        {
            maxmin.Add(new MaxMin(day, month, year, y, y));
        }

        public void SetMax(int day, int month, int year, double y)
        {
            foreach (MaxMin p in maxmin)
                if (p.IsSameDate(day, month, year))
                    p.Max = y;
        }

        public void SetMin(int day, int month, int year, double y)
        {
            foreach (MaxMin p in maxmin)
                if (p.IsSameDate(day, month, year))
                    p.Min = y;
        }

        public void SetMean(int day, int month, int year, double y)
        {
            foreach (MaxMin p in maxmin)
                if (p.IsSameDate(day, month, year))
                    p.Mean = y;
        }

        public double Max(int day, int month, int year)
        {
            double ret = -1000;
            foreach (MaxMin p in maxmin)
            {
                if (p.IsSameDate(day, month, year) && ret < p.Max)
                {
                    ret = p.Max;
                }
            }
            return ret;
        }

        public double Min(int day, int month, int year)
        {
            double ret = 1000;
            foreach (MaxMin p in maxmin)
            {
                if (p.IsSameDate(day, month, year) && ret > p.Min)
                {
                    ret = p.Min;
                }
            }
            return ret;
        }

        public double Mean(int day, int month, int year)
        {
            double ret = 1000;
            foreach (MaxMin p in maxmin)
            {
                if (p.IsSameDate(day, month, year) && ret > p.Mean)
                {
                    ret = p.Mean;
                }
            }
            return ret;
        }

        public int Count()
        {
            return maxmin.Count;
        }

        public MaxMin Maxmin(int idx)
        {
            return maxmin[idx];
        }

        public double EvaluateMeanMax(int month, int year)
        {
            double valSum = 0;
            int valCnt = 0;
            foreach (MaxMin p in maxmin)
            {
                if (p.Year == year && (p.Month == month || month == 0))
                {
                    valSum += p.Max;
                    valCnt++;
                }
            }
            return valCnt > 0 ? valSum / valCnt : 1000;
        }

        public double EvaluateMeanMin(int month, int year)
        {
            double valSum = 0;
            int valCnt = 0;
            foreach (MaxMin p in maxmin)
            {
                if (p.Year == year && (p.Month == month || month == 0))
                {
                    valSum += p.Min;
                    valCnt++;
                }
            }
            return valCnt > 0 ? valSum / valCnt : 1000;
        }

        public double EvaluateMeanMean(int month, int year)
        {
            double valSum = 0;
            int valCnt = 0;
            foreach (MaxMin p in maxmin)
            {
                if (p.Year == year && (p.Month == month || month == 0))
                {
                    valSum += p.Mean;
                    valCnt++;
                }
            }
            return valCnt > 0 ? valSum / valCnt : 1000;
        }
    }

    public class MaxMin
    {
        private readonly int Day;

        public MaxMin(int _day, int _month, int _year, double _Max, double _Min)
        {
            Day = _day;
            Month = _month;
            Year = _year;
            Max = _Max;
            Min = _Min;
        }

        public double Max { get; set; }

        public double Min { get; set; }

        public double Mean { get; set; }

        public int Month { get; }

        public int Year { get; }

        public string Date
        {
            get
            {
                return Year.ToString("D4") + Month.ToString("D2") + Day.ToString("D2");
            }
        }

        internal bool IsSameDate(int day, int month, int year)
        {
            return Day == day && this.Month == month && this.Year == year;
        }
    }
}
