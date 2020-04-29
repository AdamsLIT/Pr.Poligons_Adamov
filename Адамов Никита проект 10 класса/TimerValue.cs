using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Адамов_Никита_проект_10_класса
{
    public delegate void TimerDelegate(object sender, TimerEventArgs e);
    public partial class TimerValue : Form
    {
        public event TimerDelegate OnTimerChanged;

        public TimerValue(int value)
        {
            InitializeComponent();
            trackBar1.Value = value;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            OnTimerChanged(this, new TimerEventArgs(trackBar1.Value));
        }
       

        private void TimerValue_Load(object sender, EventArgs e)
        {

        }

        
    }
    public class TimerEventArgs : EventArgs
    {
        int timerTicks;
        public TimerEventArgs(int tics)
        {
            timerTicks = tics;

        }
        public int TimerTics
        {
            get { return timerTicks; }
            set { timerTicks = value; }
        }
    }
}
