using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
            ControlBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            guna2ProgressBar1.Style = ProgressBarStyle.Blocks;
        }

        public void UpdateProgress(int value)
        {
            guna2ProgressBar1.Value = value;
        }
    }
}
