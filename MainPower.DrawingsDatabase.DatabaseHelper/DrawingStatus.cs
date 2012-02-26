using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MainPower.DrawingsDatabase.DatabaseHelper
{
    public enum DrawingStatus
    {
        [Description("Planned")]
        Planned,
        [Description("As Built")]
        AsBuilt,
        [Description("Cancelled")]
        Canceled,
        [Description("Superseded")]
        Superseded,
        [Description("Current")]
        Current,
        [Description("Undefined")]
        Undefined,
    }
}
