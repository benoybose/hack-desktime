using System;
using System.Drawing;

namespace DeskTime
{
    internal class DTLogData
    {
        internal int ItemId;

        internal int Id;

        internal int seconds;

        internal string Title;

        internal string session;

        internal string App = "Idle";

        internal bool IsApp = true;

        internal string Path;

        internal string Text;

        internal string Url;

        internal string LastInputBecause = "";

        internal DateTime Start = DateTime.UtcNow;

        internal DateTime Stop = DateTime.UtcNow;

        internal string Cmd;

        internal Rectangle Rect;

        internal bool Visible = true;

        public override string ToString()
        {
            return "|Start:" + Start.ToString() + "|Stop:" + Stop.ToString() + " |App:" + App + " |Url:" + Url + " |Title:" + Title + " |IsApp:" + IsApp + " |Cmd:" + Cmd + " |Path:" + Path + " |Visible:" + Visible + " |LastInputBecause:" + LastInputBecause;
        }
    }
}
