using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCapitalizationBoraGUI
{
    public class GUIViewItem
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public long HighCap { get; set; }
        public DateTime HighCapDatetime { get; set; }
        public long LowCap { get; set; }
        public DateTime LowCapDatetime { get; set; }


    }
}
