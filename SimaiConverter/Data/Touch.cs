using SimaiConverter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaiConverter.Data
{
    public class Touch : Note
    {
        public string Area { get; protected set; } = "C";
        public bool IsHanabi { get; protected set; } = false;
        public NoteSize Size { get; protected set; } = NoteSize.M1;

        public Touch SetArea(string area)
        {
            Area = area;
            return this;
        }

        public Touch SetIsHanabi(bool isHanabi)
        {
            IsHanabi = isHanabi;
            return this;
        }

        public Touch SetSize(NoteSize size)
        {
            Size = size;
            return this;
        }

        public override string Compose()
        {
            return $"{Style}{Type}\t{Section}\t{Tick}\t{Key}\t{Area}\t{(IsHanabi ? 1 : 0)}\t{Size}";
        }
    }
}
