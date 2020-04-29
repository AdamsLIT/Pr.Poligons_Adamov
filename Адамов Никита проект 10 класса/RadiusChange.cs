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
 
    public  delegate void RadiusDelegate(object sender, RadiusEventArgs e);
    
    public partial class RadiusChange : Form
    {
        int Old_Radius;
        public int Rad_Value
        {
            get { return trackBar1.Value; }
            set { trackBar1.Value = value; }
        }
        public event RadiusDelegate  OnRadiuschanged;
        public event RadiusEndChanging OnRadiudEndChanging;
        public RadiusChange(int value)
        {
            InitializeComponent();
            trackBar1.Value = value;
            Old_Radius = value;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            OnRadiuschanged(this, new RadiusEventArgs(trackBar1.Value));
        }

        private void RadiusChange_Shown(object sender, EventArgs e)
        {

        }

        private void RadiusChange_Load(object sender, EventArgs e)
        {

        }

        private void trackBar1_MouseUp(object sender, MouseEventArgs e)
        {
            OnRadiudEndChanging(this, new RadiusChangingEventArgs(trackBar1.Value, Old_Radius));
            Old_Radius = trackBar1.Value;
        }
    }
    public class RadiusEventArgs : EventArgs
    {
        int Radius;
        public RadiusEventArgs(int radius)
        {
            Radius = radius;

        }
        public  int radius
        {
            get { return Radius; }
            set { Radius = value; }
        }
    }
    public delegate void RadiusEndChanging(object sender, RadiusChangingEventArgs e);
    public class RadiusChangingEventArgs : EventArgs
    {
        int Old_Radius;
        int New_Radius;
        public RadiusChangingEventArgs(int radius_N,int radius_O)
        {
            Old_Radius=radius_O;
            New_Radius=radius_N;

        }
        
        public int radius_Old
        {
            get { return Old_Radius; }
            set { Old_Radius = value; }
        }
        public int radius_New
        {
            get { return New_Radius; }
            set { New_Radius = value; }
        }
    }
   
    }
