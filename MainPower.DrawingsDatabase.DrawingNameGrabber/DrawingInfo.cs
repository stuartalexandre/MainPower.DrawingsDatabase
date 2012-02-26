using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MainPower.DrawingsDatabase.DrawingNameGrabber
{
    public class DrawingInfo
    {

        public string Name { get; set; }
        public string Path { get; set; }
        public bool Include { get; set; }
        public bool IsDir { get; set; }
    }
}
