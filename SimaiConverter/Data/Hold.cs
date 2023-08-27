using SimaiConverter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaiConverter.Data
{
    public class Hold : Note
    {
        public int Duration { get; protected set; } = 0;

        public Hold SetDuration(int duration)
        {
            Duration = duration;
            return this;
        }

        public override string Compose()
        {
            return $"{Style}{Type}\t{Section}\t{Tick}\t{Key}\t{Duration}";
        }
    }
}
