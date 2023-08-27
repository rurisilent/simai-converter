using SimaiConverter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaiConverter.Data
{
    public class TouchHold : Note
    {
        public int Duration { get; protected set; } = 0;
        public string Area { get; protected set; } = "C";
        public bool IsHanabi { get; protected set; } = false;
        public NoteSize Size { get; protected set; } = NoteSize.M1;

        public TouchHold SetDuration(int duration)
        {
            Duration = duration;
            return this;
        }

        public TouchHold SetArea(string area)
        {
            Area = area;
            return this;
        }

        public TouchHold SetIsHanabi(bool isHanabi)
        {
            IsHanabi = isHanabi;
            return this;
        }

        public TouchHold SetSize(NoteSize size)
        {
            Size = size;
            return this;
        }

        public override string Compose()
        {
            return $"{Style}{Type}\t{Section}\t{Tick}\t{Key}\t{Duration}\t{Area}\t{(IsHanabi ? 1 : 0)}";
        }
    }
}
