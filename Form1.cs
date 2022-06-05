using System;
using System.IO;
using System.Windows.Forms;

namespace ProgrammableWallpaper
{
    public partial class Form1 : Form
    {
        Form2 drawform;
        public Form1()
        {
            InitializeComponent();
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            var fullpaths = Directory.GetFiles(Directory.GetCurrentDirectory() + "/DLLs", "*.dll", SearchOption.AllDirectories);
            foreach (string path in fullpaths)
            {
                comboBox1.Items.Add(Path.GetFileName(path));
            }
            comboBox1.SelectedIndex = 0;
        }
        private void LoadButton_Click(object sender, EventArgs e)
        {
            if (Program.drawing)
            {
                drawform.Close();
            }
            else
            {
                //this.Visible = false;
                if (comboBox1.SelectedIndex > -1)
                {
                    drawform = new Form2();
                    drawform.Show();
                    drawform.DLLLoadPlay("DLLs/" + comboBox1.Text);
                }
            }
        }
        private void FrameRateTextBox_TextChanged(object sender, EventArgs e)
        {
            int value=10;
            int.TryParse(FrameRateTextBox.Text, out value);
            if (value > 0) Program.framerate = 1000 / value;
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.drawing = false;
        }
    }
}
