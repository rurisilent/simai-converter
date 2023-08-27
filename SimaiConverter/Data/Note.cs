using SimaiConverter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaiConverter.Data
{
    public abstract class Note
    {
        public NoteStyle Style { get; protected set; } = NoteStyle.NM;
        public NoteType Type { get; protected set; } = NoteType.TAP;
        public int Section { get; protected set; } = 0;
        public int Tick { get; protected set; } = 0;
        public int Key { get; protected set; } = 0;


        public virtual T SetStyle<T>(NoteStyle style) where T : Note
        {
            Style = style;
            return (T)this;
        }

        public virtual T SetType<T>(NoteType type) where T : Note
        {
            Type = type;
            return (T)this;
        }

        public virtual T SetPosition<T>(int section, int tick, int key) where T : Note
        {
            Section = section;
            Tick = tick;
            Key = key;
            return (T)this;
        }

        public virtual T AddTick<T>(int tick, int clockDef) where T : Note
        {
            Tick += tick;
            while (Tick > clockDef)
            {
                Tick -= clockDef;
                Section++;
            }
            return (T)this;
        }

        public abstract string Compose();
    }
}
