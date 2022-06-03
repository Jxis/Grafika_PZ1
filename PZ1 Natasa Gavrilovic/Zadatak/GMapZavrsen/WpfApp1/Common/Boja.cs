using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Common
{
    public class Boja
    {
        public Boja(PropertyInfo info)
        {
            Name = info.Name;
            Bojica = (System.Windows.Media.Color)info.GetValue(null);
        }

        public string Name { get; }
        public System.Windows.Media.Color Bojica { get; }

    }
}
