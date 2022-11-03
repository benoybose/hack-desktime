using System;
using System.Collections.Generic;

namespace DeskTime
{
    public class ProjectItem
    {
        public string name;

        public int id;

        public int createdAt;

        public List<TaskItem> tasks;

        public override string ToString()
        {
            return "|ID:" + id + " |Created:" + createdAt + " |Name:" + name.ToString() + " |Tasks:" + tasks.Count;
        }

        public string serializeX()
        {
            return id + "|" + createdAt + "|" + name;
        }

        public bool unserializeX(string ser)
        {
            bool result = false;
            int num = ser.IndexOf("|");
            if (num >= 0)
            {
                string value = ser.Substring(0, num);
                id = Convert.ToInt32(value);
                ser = ser.Substring(num + 1);
                num = ser.IndexOf("|");
                if (num >= 0)
                {
                    value = ser.Substring(0, num);
                    createdAt = Convert.ToInt32(value);
                    ser = ser.Substring(num + 1);
                    if (ser.Length > 0)
                    {
                        name = ser;
                        result = true;
                    }
                }
            }
            return result;
        }
    }
}
