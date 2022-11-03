using System.Collections.Generic;
using System.Linq;

namespace DeskTime
{
    internal class LogDataList : List<DTLogData>
    {
        public new void Add(DTLogData item)
        {
            if (base.Count > 0 && this.Last().IsApp)
            {
                this.Last().Stop = item.Start;
            }
            base.Add(item);
        }
    }
}
