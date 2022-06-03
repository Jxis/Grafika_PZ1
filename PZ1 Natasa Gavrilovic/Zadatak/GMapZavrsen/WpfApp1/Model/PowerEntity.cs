using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Model
{
    public class PowerEntity
    {
        private long id;
        private string name;
        private double x;
        private double y;
        private System.Windows.Media.Brush color;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        private string toolTip;

        public PowerEntity()
        {

        }



        public long Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public string ToolTip { get => toolTip; set => toolTip = value; }

        public System.Windows.Media.Brush Color { get => color; set => color = value; }


    }
}
