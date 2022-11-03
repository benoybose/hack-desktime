using System;

namespace DeskTime
{
    public class DesktimeRemote : MarshalByRefObject
    {
        public static int iconState;

        static DesktimeRemote()
        {
        }

        public int Status()
        {
            return iconState;
        }
    }
}
