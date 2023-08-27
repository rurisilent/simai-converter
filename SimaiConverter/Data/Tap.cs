using SimaiConverter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaiConverter.Data
{
    public class Tap : Note
    {
        public override string Compose()
        {
            return $"{Style}{Type}\t{Section}\t{Tick}\t{Key}";
        }
    }
}
