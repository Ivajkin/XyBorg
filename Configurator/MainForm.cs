using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Configurator
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        const string config_file_name = "config.ini";
        const string game_exe_path = "XyBorg.exe";
        private void MainForm_Load(object sender, EventArgs e)
        {
            Config.Read(config_file_name);
            fullscreenChckBx.Checked = Config.fullscreen;
            hdrEnabledChckBx.Checked = Config.use_hdr;
            screenSizeChLBx.Text = Config.screen_x.ToString() + "x" + Config.screen_y.ToString();
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SaveNExitBtn_Click(object sender, EventArgs e)
        {
            Config.Write(config_file_name);
            Application.Exit();
        }

        private void SaveNRunBtn_Click(object sender, EventArgs e)
        {
            try
            {
                Config.Write(config_file_name);
            }
            finally
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo = new System.Diagnostics.ProcessStartInfo(game_exe_path);
                p.Start();
                Application.Exit();
            }
        }


        private void fullscreenChckBx_CheckedChanged(object sender, EventArgs e)
        {
            Config.fullscreen = fullscreenChckBx.Checked;
        }

        private void hdrEnabledChckBx_CheckedChanged(object sender, EventArgs e)
        {
            Config.use_hdr = hdrEnabledChckBx.Checked;
        }

        private void screenSizeChLBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            string size = screenSizeChLBx.Text;
            Regex r = new Regex(@"(\d+)x(\d+)");
            Match m = r.Match(size);
            if (m.Success)
            {
                Config.screen_x = int.Parse(m.Groups[1].Value);
                Config.screen_y = int.Parse(m.Groups[2].Value);
            }
        }
    }
}
