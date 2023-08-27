using SimaiConverter.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaiConverter.Data
{
    public class Slide : Note
    {
        public int InitialDuration { get; protected set; } = 0;
        public int Duration { get; protected set; } = 0;
        public int TargetKey { get; protected set; } = 0;

        public double Length => GetLength();

        public Slide SetDuration(int duration)
        {
            Duration = duration;
            return this;
        }

        public Slide SetInitial(int initial)
        {
            InitialDuration = initial;
            return this;
        }

        public Slide SetTarget(int target)
        {
            TargetKey = target;
            return this;
        }

        public override string Compose()
        {
            return $"{Style}{Type}\t{Section}\t{Tick}\t{Key}\t{InitialDuration}\t{Duration}\t{TargetKey}";
        }

        double GetLength()
        {
            const double PI = 3.14159;

            //by default, diameter's length is 20
            if (Type == NoteType.SI_)
            {
                int step = Math.Min(GetStepClockwise(), GetStepCounterClockwise());
                if (step == 4) return 20.0;
                else if (step == 3) return 18.0;
                else if (step == 2) return 14.0;
            }
            else if (Type == NoteType.SV_)
            {
                int step = Math.Min(GetStepClockwise(), GetStepCounterClockwise());
                if (step == 3) return 20.0;
                else if (step == 2) return 20.0;
                else if (step == 1) return 20.0;
            }
            else if (Type == NoteType.SF_)
            {
                int step = Math.Min(GetStepClockwise(), GetStepCounterClockwise());
                if (step == 4) return 20.0;
            }
            else if (Type == NoteType.SSL || Type == NoteType.SSR)
            {
                int step = Math.Min(GetStepClockwise(), GetStepCounterClockwise());
                if (step == 4) return 30.0;
            }
            else if (Type == NoteType.SCL)
            {
                int step = GetStepClockwise();
                if (step == 0) return 20.0 * PI;
                else return 20.0 * PI * step / 8.0;
            }
            else if (Type == NoteType.SCR)
            {
                int step = GetStepCounterClockwise();
                if (step == 0) return 20.0 * PI;
                else return 20.0 * PI * step / 8.0;
            }
            else if (Type == NoteType.SUL)
            {
                int step = GetStepClockwise();
                if (step == 0) return 33.0;
                else if (step == 1) return 30.0;
                else if (step == 2) return 27.0;
                else if (step == 3) return 24.0;
                else if (step == 4) return 21.0;
                else if (step == 5) return 42.0;
                else if (step == 6) return 39.0;
                else if (step == 7) return 36.0;
            }
            else if (Type == NoteType.SUR)
            {
                int step = GetStepCounterClockwise();
                if (step == 0) return 33.0;
                else if (step == 1) return 30.0;
                else if (step == 2) return 27.0;
                else if (step == 3) return 24.0;
                else if (step == 4) return 21.0;
                else if (step == 5) return 42.0;
                else if (step == 6) return 39.0;
                else if (step == 7) return 36.0;
            }
            else if (Type == NoteType.SXL)
            {
                int step = GetStepClockwise();
                if (step == 0) return 35.0;
                else if (step == 1) return 28.0;
                else if (step == 2) return 22.0;
                else if (step == 3) return 49.0;
                else if (step == 4) return 49.0;
                else if (step == 5) return 48.0;
                else if (step == 6) return 46.0;
                else if (step == 7) return 41.0;
            }
            else if (Type == NoteType.SXR)
            {
                int step = GetStepCounterClockwise();
                if (step == 0) return 35.0;
                else if (step == 1) return 28.0;
                else if (step == 2) return 22.0;
                else if (step == 3) return 49.0;
                else if (step == 4) return 49.0;
                else if (step == 5) return 48.0;
                else if (step == 6) return 46.0;
                else if (step == 7) return 41.0;
            }
            else if (Type == NoteType.SLL)
            {
                int step = GetStepCounterClockwise();
                if (step == 4) return 28.0;
                else if (step == 5) return 32.0;
                else if (step == 6) return 34.0;
                else if (step == 7) return 32.0;
            }
            else if (Type == NoteType.SLR)
            {
                int step = GetStepClockwise();
                if (step == 4) return 28.0;
                else if (step == 5) return 32.0;
                else if (step == 6) return 34.0;
                else if (step == 7) return 32.0;
            }

            throw new Exception("Invalid slide target key in maidata.txt");
        }

        int GetStepClockwise()
        {
            int tempKey = Key;
            int step = 0;
            while (tempKey != TargetKey)
            {
                tempKey++;
                if (tempKey > 7) tempKey = 0;
                step++;
            }

            return step;
        }

        int GetStepCounterClockwise()
        {
            int tempKey = Key;
            int step = 0;
            while (tempKey != TargetKey)
            {
                tempKey--;
                if (tempKey < 0) tempKey = 7;
                step++;
            }

            return step;
        }
    }
}
