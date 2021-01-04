using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZedGraph;

namespace WS2300
{
    public partial class Graph : Form
    {
        private int a;
        private readonly string LogFile;
        private readonly string history_log_type;
        private readonly Color[] col = new Color[13] { Color.Red, Color.DarkViolet, Color.Green, Color.Blue,
            Color.DarkOrange, Color.Gold,  Color.LawnGreen,  Color.White, Color.DeepSkyBlue,
            Color.White, Color.White, Color.Red, Color.DarkViolet};

        public Graph(string _VER, string _LogFile)
        {
            InitializeComponent();
            LogFile = _LogFile;
            history_log_type = _LogFile.Substring(_LogFile.LastIndexOf(".") + 1, 3).ToLower();
            this.Text += " - " + _VER;
        }

        private void Graph_Load(object sender, EventArgs e)
        {
            colorPicker1.Color = col[1];
            colorPicker2.Color = col[2];
            colorPicker3.Color = col[3];
            colorPicker4.Color = col[4];
            colorPicker5.Color = col[5];
            colorPicker6.Color = col[6];
            colorPicker7.Color = col[8];
            colorPicker8.Color = col[11];
            colorPicker9.Color = col[12];
            colorPickerTin.Color = col[0];
            a = 4; // Temp out
            CreateGraph(zedGraph, a++, false, true);
            // set scal to last 2 months
            GraphPane myPane = zedGraph.GraphPane;
            // DateTime (long ticks) A date and time expressed in 100-nanosecond units.
            //myPane.XAxis.Scale.Max = 39814; // 1-1-2009
            //633663648000000000 = 01gen2009 ore 0:00
            double di = 633663648000000000.0 / 39814.0; //thatis 15915598734113.628371929472045009
            myPane.XAxis.Scale.Max = DateTime.Now.Ticks / di + 1;
            myPane.XAxis.Scale.Min = myPane.XAxis.Scale.Max - 60;
            zedGraph.AxisChange();
            zedGraph.Refresh();
            // resize
            SetSize();
        }

        private void Graph_Resize(object sender, EventArgs e)
        {
            SetSize();
        }

        private void SetSize()
        {
            zedGraph.Location = new Point(10, 10);
            int xw = this.ClientRectangle.Width - btn1.Width - colorPickerTin.Width - 10;
            // Leave a small margin around the outside of the control
            zedGraph.Size = new Size(xw - 20, this.ClientRectangle.Height - 42);
            int yw = 20;
            btn1.Location = new Point(xw, 10);
            chkT_in.Location = new Point(xw, 15 + yw);
            chkT_out.Location = new Point(xw, 15 + 2 * yw);
            chkDew.Location = new Point(xw, 15 + 3 * yw);
            chkH_in.Location = new Point(xw, 15 + 4 * yw);
            chkH_out.Location = new Point(xw, 15 + 5 * yw);
            chkW_speed.Location = new Point(xw, 15 + 6 * yw);
            chkW_angle.Location = new Point(xw, 15 + 7 * yw);
            chkW_chill.Location = new Point(xw, 15 + 8 * yw);
            chkRain.Location = new Point(xw, 15 + 9 * yw);
            chkPress.Location = new Point(xw, 15 + 10 * yw);
            btnRedraw.Location = new Point(xw, 15 + 11 * yw);
            colorPickerTin.Location = new Point(xw + 80, 15 + yw - 2);
            colorPicker1.Location = new Point(xw + 80, 15 + 2 * yw - 2);
            colorPicker2.Location = new Point(xw + 80, 15 + 3 * yw - 2);
            colorPicker3.Location = new Point(xw + 80, 15 + 4 * yw - 2);
            colorPicker4.Location = new Point(xw + 80, 15 + 5 * yw - 2);
            colorPicker5.Location = new Point(xw + 80, 15 + 6 * yw - 2);
            colorPicker6.Location = new Point(xw + 80, 15 + 7 * yw - 2);
            colorPicker7.Location = new Point(xw + 80, 15 + 8 * yw - 2);
            colorPicker8.Location = new Point(xw + 80, 15 + 9 * yw - 2);
            colorPicker9.Location = new Point(xw + 80, 15 + 10 * yw - 2);
            statusStrip1.Width = this.ClientRectangle.Width;
        }

        private void CreateGraph(ZedGraphControl zgc, int meteovar, bool clear, bool clearall)
        {
            meteovar -= 3;
            string[] name = new string[] { "Temp in", "Temp out", "dewpoint", "Rel humidity in", "Rel humidity out", "Wind speed", "Wind angle", "Wind Direction", "Wind chill", "rain_1h", "rain_24h", "Rain total", "Air pressure" };
            string[] vname = new string[] { "°C", "°C", "°C", "%", "%", "m/s", "deg", "", "°C", "", "", "mm", "hPa" };
            SymbolType[] sym = new SymbolType[] { SymbolType.None };
            //to use symbols: SymbolType.Plus, SymbolType.None, SymbolType.Star, SymbolType.Square, SymbolType.Triangle, SymbolType.Diamond

            GraphPane myPane = zgc.GraphPane;
            if (clear)
            {
                if (myPane.CurveList[name[meteovar]] != null)
                    for (int i = 0; i < myPane.CurveList.Count; i++)
                    {
                        //myPane.CurveList[name[meteovar]].Clear();
                        if (myPane.CurveList[i].Label.Text == name[meteovar])
                        {
                            myPane.CurveList[i].Clear();
                        }
                    }
            }
            else
            {
                // Set the titles and axis labels
                myPane.Title.Text = "MeteoGraph";
                myPane.XAxis.Title.Text = "Date";
                myPane.XAxis.Type = AxisType.Date;
                myPane.XAxis.MajorGrid.IsVisible = true;
                myPane.YAxis.Title.Text = name[meteovar] + " (" + vname[meteovar] + ")";
                myPane.YAxis.MajorGrid.IsVisible = true;

                // Make up data points
                PointPairList list = new PointPairList();
                StreamReader sr = new StreamReader(LogFile);
                string line;
                string[] lval;
                string lv;
                double y;
                XDate xd;
                while ((line = sr.ReadLine()) != null)
                {
                    lv = "";
                    if (history_log_type == "csv")
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
                        list.Add(xd, y);
                    }
                }
                sr.Close();
                if (clearall)
                {
                    myPane.CurveList.Clear();
                }

                LineItem myCurve = myPane.AddCurve(name[meteovar], list, col[meteovar % col.Length], sym[meteovar % sym.Length]);
                // Make the symbols opaque by filling them with white
                myCurve.Symbol.Fill = new Fill(Color.White);

                // Calculate the Axis Scale Ranges
                zgc.AxisChange();
            }
            zgc.Refresh();
        }

        private void Btn1_Click(object sender, EventArgs e)
        {
            //DATA:   0,    1,    2,       3,        4,        5,          6,           7,         8,          9,          -- 10,         11,   -- 12,    -- 13,         14,           15,       --,       --
            //timestamp, date, time, temp_in, temp_out, dewpoint, rel_hum_in, rel_hum_out, windspeed, wind_angle, wind_direction, wind_chill, rain_1h, rain_24h, rain_total, rel_pressure, tendency, forecast
            if (a == 16) a = 3;
            if (a == 10) a++;
            if (a == 12) a++;
            if (a == 13) a++;
            CreateGraph(zedGraph, a++, false, true);
        }

        private void ChkT_in_CheckedChanged(object sender, EventArgs e)
        {
            CreateGraph(zedGraph, 3, !chkT_in.Checked, false);
        }

        private void ChkT_out_CheckedChanged(object sender, EventArgs e)
        {
            CreateGraph(zedGraph, 4, !chkT_out.Checked, false);
        }

        private void ChkH_in_CheckedChanged(object sender, EventArgs e)
        {
            CreateGraph(zedGraph, 6, !chkH_in.Checked, false);
        }

        private void ChkH_out_CheckedChanged(object sender, EventArgs e)
        {
            CreateGraph(zedGraph, 7, !chkH_out.Checked, false);
        }

        private void ChkDew_CheckedChanged(object sender, EventArgs e)
        {
            CreateGraph(zedGraph, 5, !chkDew.Checked, false);
        }

        private void ChkW_speed_CheckedChanged(object sender, EventArgs e)
        {
            CreateGraph(zedGraph, 8, !chkW_speed.Checked, false);
        }

        private void ChkW_angle_CheckedChanged(object sender, EventArgs e)
        {
            CreateGraph(zedGraph, 9, !chkW_angle.Checked, false);
        }

        private void ChkW_chill_CheckedChanged(object sender, EventArgs e)
        {
            CreateGraph(zedGraph, 11, !chkW_chill.Checked, false);
        }

        private void ChkRain_CheckedChanged(object sender, EventArgs e)
        {
            CreateGraph(zedGraph, 14, !chkRain.Checked, false);
        }

        private void ChkPress_CheckedChanged(object sender, EventArgs e)
        {
            CreateGraph(zedGraph, 15, !chkPress.Checked, false);
        }

        private void BtnRedraw_Click(object sender, EventArgs e)
        {
            GraphPane myPane = zedGraph.GraphPane;
            myPane.CurveList.Clear();
            if (chkT_in.Checked)
                CreateGraph(zedGraph, 3, false, false);
            if (chkT_out.Checked)
                CreateGraph(zedGraph, 4, false, false);
            if (chkDew.Checked)
                CreateGraph(zedGraph, 5, false, false);
            if (chkH_in.Checked)
                CreateGraph(zedGraph, 6, false, false);
            if (chkH_out.Checked)
                CreateGraph(zedGraph, 7, false, false);
            if (chkW_speed.Checked)
                CreateGraph(zedGraph, 8, false, false);
            if (chkW_angle.Checked)
                CreateGraph(zedGraph, 9, false, false);
            if (chkW_chill.Checked)
                CreateGraph(zedGraph, 11, false, false);
            if (chkRain.Checked)
                CreateGraph(zedGraph, 14, false, false);
            if (chkPress.Checked)
                CreateGraph(zedGraph, 15, false, false);
            zedGraph.AxisChange();
            zedGraph.Refresh();
        }

        private void ColorPickerTin_ColorChanged(object sender, PJLControls.ColorChangedEventArgs e)
        {
            col[0] = colorPickerTin.Color;
        }

        private void ColorPicker1_ColorChanged(object sender, PJLControls.ColorChangedEventArgs e)
        {
            col[1] = colorPicker1.Color;
        }

        private void ColorPicker2_ColorChanged(object sender, PJLControls.ColorChangedEventArgs e)
        {
            col[2] = colorPicker2.Color;
        }

        private void ColorPicker3_ColorChanged(object sender, PJLControls.ColorChangedEventArgs e)
        {
            col[3] = colorPicker3.Color;
        }

        private void ColorPicker4_ColorChanged(object sender, PJLControls.ColorChangedEventArgs e)
        {
            col[4] = colorPicker4.Color;
        }

        private void ColorPicker5_ColorChanged(object sender, PJLControls.ColorChangedEventArgs e)
        {
            col[5] = colorPicker5.Color;
        }

        private void ColorPicker6_ColorChanged(object sender, PJLControls.ColorChangedEventArgs e)
        {
            col[6] = colorPicker6.Color;
        }

        private void ColorPicker7_ColorChanged(object sender, PJLControls.ColorChangedEventArgs e)
        {
            col[8] = colorPicker7.Color;
        }

        private void ColorPicker8_ColorChanged(object sender, PJLControls.ColorChangedEventArgs e)
        {
            col[11] = colorPicker8.Color;
        }

        private void ColorPicker9_ColorChanged(object sender, PJLControls.ColorChangedEventArgs e)
        {
            col[12] = colorPicker9.Color;
        }

        private bool ZedGraph_MouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            // Save the mouse location
            PointF mousePt = new PointF(e.X, e.Y);

            // Find the Chart rect that contains the current mouse location
            GraphPane pane = sender.MasterPane.FindChartRect(mousePt);

            // If pane is non-null, we have a valid location.  Otherwise, the mouse is not
            // within any chart rect.
            if (pane != null)
            {
                // Convert the mouse location to X, and Y scale values
                pane.ReverseTransform(mousePt, out double x, out double y);
                // Format the status label text
                toolStripStatusXY.Text = "(" + x.ToString("f2") + ", " + y.ToString("f2") + ")";
            }
            else
            {
                // If there is no valid data, then clear the status label text
                toolStripStatusXY.Text = string.Empty;
            }

            // Return false to indicate we have not processed the MouseMoveEvent
            // ZedGraphControl should still go ahead and handle it
            return false;
        }
    }
}
