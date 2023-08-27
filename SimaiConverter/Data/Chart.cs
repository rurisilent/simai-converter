using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaiConverter.Data
{
    public class Chart
    {
        public string? InternalVersion { get; private set; } = "0.00.00";
        public string? Version { get; private set; } = "1.04.00";
        public bool FesMode { get; private set; } = false;
        public double[] BPM { get; private set; } = new double[4] { 0.000, 0.000, 0.000, 0.000 };
        public (int, int) Measurement { get; private set; } = (4, 4);
        public int Resolution { get; private set; } = 384;
        public int ClockDefinition { get; private set; } = 384;
        public string? CompatibleCode => "MA2";

        public List<Note>? Notes { get; private set; }

        public Chart()
        {
            Notes = new List<Note>();
        }

        public void SetBPM(params double[] bpm)
        {
            BPM = bpm;
        }

        public string Compose()
        {
            //sort chart
            Notes.Sort((x, y) =>
            {
                ulong xTick = (ulong)x.Section * (ulong)ClockDefinition + (ulong)x.Tick;
                ulong yTick = (ulong)y.Section * (ulong)ClockDefinition + (ulong)y.Tick;
                if (xTick < yTick) return -1;
                else if (xTick > yTick) return 1;
                else
                {
                    if (x.Type == Enum.NoteType.STR && y.Type != Enum.NoteType.STR) return -1;
                    else if (y.Type == Enum.NoteType.STR && x.Type != Enum.NoteType.STR) return 1;
                    else return 0;
                }
            });

            StringBuilder chart = new StringBuilder();

            //build header
            chart.AppendLine($"VERSION\t{InternalVersion}\t{Version}");
            chart.AppendLine($"FES_MODE\t{(FesMode ? 1 : 0)}");
            chart.AppendLine($"BPM_DEF\t{BPM[0]:0.000}\t{BPM[1]:0.000}\t{BPM[2]:0.000}\t{BPM[3]:0.000}"); //Need variable length
            chart.AppendLine($"MET_DEF\t{Measurement.Item1}\t{Measurement.Item2}");
            chart.AppendLine($"RESOLUTION\t{Resolution}");
            chart.AppendLine($"CLK_DEF\t{ClockDefinition}");
            chart.AppendLine($"COMPATIBLE_CODE\t{CompatibleCode}");
            chart.AppendLine();

            //build bpm / measurement definitions
            chart.AppendLine($"BPM\t{0}\t{0}\t{BPM[0]:0.000}"); //Variable BPM is NOT implemented
            chart.AppendLine($"MET\t{0}\t{0}\t{Measurement.Item1}\t{Measurement.Item2}"); //Variable BPM is NOT implemented
            chart.AppendLine();

            //build note
            foreach (var note in Notes)
            {
                chart.AppendLine(note.Compose());
            }
            chart.AppendLine();

            return chart.ToString();
        }
    }
}
