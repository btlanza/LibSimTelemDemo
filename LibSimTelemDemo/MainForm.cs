/* Benjamin Lanza
 * MainForm.cs
 * October 15th, 2018 */

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using static LibSimTelem.EventDefs;
using static LibSimTelem.ACDefs;
using static LibSimTelem.F117Defs;
using static LibSimTelem.F118Defs;

namespace LibSimTelemDemo
{
    public partial class MainForm : Form
    {
        /* Constant or static readonly fields */
        private static readonly List<Bitmap> TEXT_BITMAPS = new List<Bitmap> {
            Properties.Resources.g0,
            Properties.Resources.g1,
            Properties.Resources.g2,
            Properties.Resources.g3,
            Properties.Resources.g4,
            Properties.Resources.g5,
            Properties.Resources.g6,
            Properties.Resources.g7,
            Properties.Resources.g8,
            Properties.Resources.g9
        };

        /* Delegates */
        private delegate void SetGameDelegate(int gameIndex);
        private delegate void StopSearchDelegate();
        private delegate void DrawAxesUpdateDelegate(AxesEventArgs args);
        private delegate void DrawPowertrainUpdateDelegate(PowertrainEventArgs args);

        /* Fields */
        private LibSimTelem.TelemReader reader;
        private LibSimTelem.TelemData data;

        private Thread gameDetectThread;
        private static Mutex flagMtx = new Mutex();
        private volatile bool isSearching = false;
        private volatile bool isRunning = false;

        private float maxDirRotation = 1.0f;

        /* Form constructor */
        public MainForm()
        {
            InitializeComponent();
        }

        /* Runs when the form loads */
        private void MainForm_Load(object sender, EventArgs e)
        {
            /* Disable the stop button until isRunning is true */
            btnStop.Enabled = false;

            /* Automatically disable components for automatic game detection */
            chkAutoStart_CheckedChanged(null, null);

            /* Populate cbxSelectGame and default to Assetto Corsa */
            cbxSelectGame.Items.Add("Assetto Corsa");
            cbxSelectGame.Items.Add("F1 2017");
            cbxSelectGame.Items.Add("F1 2018");
            cbxSelectGame.SelectedIndex = 0;

            /* Sets up UI widget images */
            picSteeringWheel = new InterpolatedPictureBox();
            picSteeringWheel.InterMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            picSteeringWheel.SizeMode = PictureBoxSizeMode.StretchImage;
            picSteeringWheel.Size = new Size(135, 135);
            picSteeringWheel.Location = new Point(12, 86);
            picSteeringWheel.Image = Properties.Resources.steering_wheel_complete;
            picSteeringWheel.Visible = true;
            this.Controls.Add(picSteeringWheel);

            picPedals = new InterpolatedPictureBox();
            picPedals.InterMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            picPedals.SizeMode = PictureBoxSizeMode.StretchImage;
            picPedals.Size = new Size(121, 57);
            picPedals.Location = new Point(156, 164);
            picPedals.Image = Properties.Resources.pedal_set;
            picPedals.Visible = true;
            this.Controls.Add(picPedals);

            picSpeedRPM = new InterpolatedPictureBox();
            picSpeedRPM.InterMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            picSpeedRPM.SizeMode = PictureBoxSizeMode.StretchImage;
            picSpeedRPM.Size = new Size(118, 54);
            picSpeedRPM.Location = new Point(153, 86);
            picSpeedRPM.Image = Properties.Resources.speed_rpm_panel;
            picSpeedRPM.Visible = true;
            this.Controls.Add(picSpeedRPM);
        }

        /* Runs when the AutoStart checkbox's value is changed */
        private void chkAutoStart_CheckedChanged(object sender, EventArgs e)
        {
            cbxSelectGame.Enabled = !chkAutoDetect.Checked;
            btnStart.Enabled = !chkAutoDetect.Checked;

            if(chkAutoDetect.Checked)
            {
                isSearching = true;
                gameDetectThread = new Thread(DetectGame);
                gameDetectThread.Start();
            }
            else
            {
                flagMtx.WaitOne();
                isSearching = false;
                flagMtx.ReleaseMutex();
                gameDetectThread.Join();
            }
        }

        /* Runs on a thread to detect a running supported game */
        private void DetectGame()
        {
            string[] supportedGames = new string[]
            {
                LibSimTelem.ACDefs.GAME_PROCESS_NAME,
                LibSimTelem.F117Defs.GAME_PROCESS_NAME,
                LibSimTelem.F118Defs.GAME_PROCESS_NAME
            };

            while (true)
            {
                flagMtx.WaitOne();
                if (isSearching) { flagMtx.ReleaseMutex(); }
                else { flagMtx.ReleaseMutex(); break; }

                for (int i = 0; i < supportedGames.Length; ++i)
                {
                    if (Process.GetProcessesByName(supportedGames[i]).Length > 0)
                    {
                        DialogResult result = MessageBox.Show("Would you like to set the game to supported application " + supportedGames[i] + "?",
                            "Supported game detected!", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            this.BeginInvoke(new SetGameDelegate(SetGame), i);
                            return;
                        }
                        else
                        {
                            this.BeginInvoke(new StopSearchDelegate(StopGameSearch));
                            return;
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }

        /* Sets the supported game */
        private void SetGame(int gameIndex)
        {
            cbxSelectGame.SelectedIndex = gameIndex;
            chkAutoDetect.Checked = false;
        }

        /* Cancels automatic search for a supported game */
        private void StopGameSearch()
        {
            chkAutoDetect.Checked = false;
        }

        /* Redraws the UI using the passed AxesEventArgs object */
        private void DrawAxesUpdate(AxesEventArgs args) { 

            Image imgWheelComponents = Properties.Resources.steering_wheel_components;
            Image imgWheel = Properties.Resources.steering_wheel;
            int width = imgWheelComponents.Width;
            int height = imgWheelComponents.Height;

            Image drawWheelTo = new Bitmap(width, height);

            Graphics gWheel = Graphics.FromImage(drawWheelTo);
            gWheel.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            gWheel.DrawImage(imgWheelComponents, new Rectangle(0, 0, width, height));
            gWheel.TranslateTransform(width / 2, height / 2);
            gWheel.RotateTransform(args.steering * maxDirRotation);
            gWheel.TranslateTransform(-1 * width / 2, -1 * height / 2);
            gWheel.DrawImage(imgWheel, new Rectangle(0, 0, width, height));
            gWheel.Flush();
            gWheel.Dispose();

            picSteeringWheel.Image = drawWheelTo;
            imgWheelComponents.Dispose();
            imgWheel.Dispose();

            Image imgPedals = Properties.Resources.pedal_set;
            width = imgPedals.Width;
            height = imgPedals.Height;

            Image drawPedalsTo = new Bitmap(width, height);
            Graphics gPedals = Graphics.FromImage(drawPedalsTo);
            gPedals.FillRectangle(new SolidBrush(Color.Blue), 0, height - args.clutch * 22, 16, args.clutch * 22);
            gPedals.FillRectangle(new SolidBrush(Color.Red), 21, height - args.brake * 22, 16, args.brake * 22);
            gPedals.FillRectangle(new SolidBrush(Color.LawnGreen), 42, height - args.throttle * 28, 18, args.throttle * 28);
            gPedals.DrawImage(imgPedals, new Rectangle(0, 0, width, height));
            gPedals.Flush();
            gPedals.Dispose();

            picPedals.Image = drawPedalsTo;
            imgPedals.Dispose();
        }

        /* Redraws the UI using the passed PowertrainEventArgs object */
        private void DrawPowertrainUpdate(PowertrainEventArgs args)
        {

            Image imgSpeedRPMPanel = Properties.Resources.speed_rpm_panel;
            List<Bitmap> imgSpeedText = new List<Bitmap>();

            string strSpeedText = ((int)args.kmh).ToString();
            if (strSpeedText.Length > 3) strSpeedText = "999";

            int width = imgSpeedRPMPanel.Width;
            int height = imgSpeedRPMPanel.Height;
            int maxRpm = data.GetMaxRPM();

            Image drawPanelTo = new Bitmap(width, height);
            Graphics gPanel = Graphics.FromImage(drawPanelTo);
            gPanel.FillRectangle(new SolidBrush(Color.OrangeRed), 1.0f, 1.0f, 27.0f * ((float)args.rpm / (float)maxRpm), 5.0f);
            gPanel.DrawImage(imgSpeedRPMPanel, new Rectangle(0, 0, width, height));
            for (int i = 0; i < strSpeedText.Length; ++i) gPanel.DrawImage(TEXT_BITMAPS[strSpeedText[i] - '0'], new Rectangle((i + 1) * 6, 6, 5, 5));
            gPanel.Flush();
            gPanel.Dispose();

            picSpeedRPM.Image = drawPanelTo;
            imgSpeedRPMPanel.Dispose();
        }

        /* Runs when the start button is clicked */
        private void btnStart_Click(object sender, EventArgs e)
        {
            isRunning = true;
            grpGetGame.Enabled = false;
            int gameIndex = cbxSelectGame.SelectedIndex;

            if(gameIndex == 0)
            {
                reader = new LibSimTelem.ACReader();
                data = new LibSimTelem.ACData();
            }else if(gameIndex == 1)
            {
                reader = new LibSimTelem.F117Reader();
                data = new LibSimTelem.F117Data();
            }
            else
            {
                reader = new LibSimTelem.F118Reader();
                data = new LibSimTelem.F118Data();
            }

            data.SubscribeToReader(reader);
            data.AxesReceivedEvent += OnGotAxes;
            data.PowertrainReceivedEvent += OnGotPowertrain;
            btnStop.Enabled = true;
            isRunning = true;
            reader.Start();
        }

        /* Runs when the stop button is clicked */
        private void btnStop_Click(object sender, EventArgs e)
        {
            reader.Stop();
            btnStop.Enabled = false;
            isRunning = false;
            grpGetGame.Enabled = true;
        }

        /* Runs when the program receives axes information from the game */
        private void OnGotAxes(object sender, AxesEventArgs args)
        {
            this.BeginInvoke(new DrawAxesUpdateDelegate(DrawAxesUpdate), args);
        }

        /* Runs when the program receives powertrain information from the game */
        private void OnGotPowertrain(object sender, PowertrainEventArgs args)
        {
            this.BeginInvoke(new DrawPowertrainUpdateDelegate(DrawPowertrainUpdate), args);
        }

        /* Runs before the main form closes */
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isRunning) reader.Stop();
            StopGameSearch();
        }

        /* Runs when the game selection box's value changes */
        private void cbxSelectGame_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cbxSelectGame.SelectedIndex;
            if(index == 0) maxDirRotation = 1.0f;
            else if(index == 1) maxDirRotation = 180.0f;
            else if(index == 2) maxDirRotation = 180.0f;
        }
    }
}
