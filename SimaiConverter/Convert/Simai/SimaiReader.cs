using SimaiConverter.Data;
using SimaiConverter.Enum;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimaiConverter.Convert.Simai
{
    public class SimaiReader
    {
        public static Chart ReadChart(string inputPath)
        {
            if (!File.Exists(inputPath))
            {
                throw new FileNotFoundException();
            }

            string rawData = File.ReadAllText(inputPath);
            string[] tokenizedData = Tokenizer(rawData);
            return AnalyzeChart(tokenizedData);
        }

        static string[] Tokenizer(string input)
        {
            return input.Replace("\r\n", "").Replace("\n", "").Split(',');
        }

        static Chart AnalyzeChart(string[] data)
        {
            Chart result = new Chart();

            int loggerIndex = 0;

            double bpm = 0;
            int measure = 0;
            int section = 0;
            int tick = 0;

            int state = 0;
            const int STATE_NORMAL = 0;
            const int STATE_READ_BPM = 1;
            const int STATE_READ_MEASURE = 2;

            const int clockDefinition = 384;

            StringBuilder sb = new StringBuilder();
            string simplifiedToken;

            foreach (var token in data)
            {
                sb.Clear();
                simplifiedToken = "";
                foreach (var c in token)
                {
                    switch (state)
                    {
                        case STATE_NORMAL:
                            if (c == '(')
                            {
                                if (sb.Length > 0) simplifiedToken = sb.ToString();
                                state = STATE_READ_BPM;
                                sb.Clear();
                            }
                            else if (c == '{')
                            {
                                if (sb.Length > 0) simplifiedToken = sb.ToString();
                                state = STATE_READ_MEASURE;
                                sb.Clear();
                            }
                            else
                            {
                                sb.Append(c);
                            }
                            break;
                        case STATE_READ_BPM:
                            if (c == ')')
                            {
                                state = STATE_NORMAL;
                                if (double.TryParse(sb.ToString(), out var newBpm))
                                {
                                    bpm = newBpm;
                                    sb.Clear();
                                }
                                else
                                {
                                    throw new Exception("Error resolving bpm/measure in maidata.txt");
                                }
                            }
                            else
                            {
                                sb.Append(c);
                            }
                            break;
                        case STATE_READ_MEASURE:
                            if (c == '}')
                            {
                                state = STATE_NORMAL;
                                if (int.TryParse(sb.ToString(), out var newMeasure))
                                {
                                    measure = newMeasure;
                                    sb.Clear();
                                }
                                else
                                {
                                    throw new Exception("Error resolving bpm/measure in maidata.txt");
                                }
                            }
                            else
                            {
                                sb.Append(c);
                            }
                            break;
                    }
                }

                if (state != STATE_NORMAL) throw new Exception("Error resolving bpm/measure in maidata.txt");
                if (sb.Length > 0) simplifiedToken = sb.ToString();

                //analyze simplified token
                //separate each
                string[] eachTokens = simplifiedToken.Split('/');
                foreach (var eachTk in eachTokens)
                {
                    if (eachTk.Contains("A") ||
                        eachTk.Contains("B") ||
                        eachTk.Contains("C") ||
                        eachTk.Contains("D") ||
                        eachTk.Contains("E"))
                    {
                        //touch hold
                        if (eachTk.Contains("h")) result.Notes?.Add(GenerateTouchHold(section, tick, eachTk, clockDefinition));
                        //touch
                        else result.Notes?.Add(GenerateTouch(section, tick, eachTk));
                    }
                    else if (eachTk.Contains("h"))
                    {
                        //hold
                        result.Notes?.Add(GenerateHold(section, tick, eachTk, clockDefinition));
                    }
                    else if (
                        eachTk.Contains("-") ||
                        eachTk.Contains("v") ||
                        eachTk.Contains("w") ||
                        eachTk.Contains("<") ||
                        eachTk.Contains(">") ||
                        eachTk.Contains("p") ||
                        eachTk.Contains("q") ||
                        eachTk.Contains("s") ||
                        eachTk.Contains("z") ||
                        eachTk.Contains("V") ||
                        eachTk.Contains("^"))
                    {
                        //slide
                        result.Notes?.AddRange(GenerateSlideGroup(section, tick, eachTk, clockDefinition));
                    }
                    else if (
                        eachTk.Contains("1") ||
                        eachTk.Contains("2") ||
                        eachTk.Contains("3") ||
                        eachTk.Contains("4") ||
                        eachTk.Contains("5") ||
                        eachTk.Contains("6") ||
                        eachTk.Contains("7") ||
                        eachTk.Contains("8"))
                    {
                        //tap
                        result.Notes?.Add(GenerateTap(section, tick, eachTk));
                    }
                }

                //debug log
                /*while (loggerIndex < result.Notes.Count)
                {
                    Console.WriteLine(result.Notes[loggerIndex].Compose());
                    loggerIndex++;
                }*/

                //update ticks
                tick += (int)Math.Round(clockDefinition * (1.0 / measure));
                while (tick >= clockDefinition)
                {
                    section++;
                    tick -= clockDefinition;
                }
            }

            result.SetBPM(bpm, bpm, bpm, bpm);
            return result;
        }

        static Tap GenerateTap(int section, int tick, string data)
        {
            NoteStyle style = NoteStyle.NM;
            if (data.Contains("x") && data.Contains("b")) style = NoteStyle.BX;
            else if (data.Contains("x")) style = NoteStyle.EX;
            else if (data.Contains("b")) style = NoteStyle.BR;

            if (int.TryParse(data.Replace("b", "").Replace("x", ""), out var key))
            {
                return new Tap()
                .SetStyle<Tap>(style)
                .SetType<Tap>(NoteType.TAP)
                .SetPosition<Tap>(section, tick, key - 1);
            }
            else
            {
                throw new Exception("Invalid tap definition in maidata.txt");
            }
        }

        static Hold GenerateHold(int section, int tick, string data, int clockDefinition)
        {
            NoteStyle style = NoteStyle.NM;
            if (data.Contains("x") && data.Contains("b")) style = NoteStyle.BX;
            else if (data.Contains("x")) style = NoteStyle.EX;
            else if (data.Contains("b")) style = NoteStyle.BR;
            Hold result;

            if (int.TryParse(data.Replace("h", "").Replace("b", "").Replace("x", "")[0].ToString(), out var key))
            {
                result = new Hold()
                    .SetStyle<Hold>(style)
                    .SetType<Hold>(NoteType.HLD)
                    .SetPosition<Hold>(section, tick, key - 1);
            }
            else
            {
                throw new Exception("Invalid tap definition in maidata.txt");
            }

            //get duration
            int durationIndex = data.IndexOf("[");
            int durationEndIndex = data.IndexOf("]");
            int duration = GetDuration(data, durationIndex, durationEndIndex, clockDefinition);
            result.SetDuration(duration);
            return result;
        }

        static Touch GenerateTouch(int section, int tick, string data)
        {
            NoteStyle style = NoteStyle.NM;

            if (data.Contains("C"))
            {
                return new Touch()
                    .SetStyle<Touch>(style)
                    .SetType<Touch>(NoteType.TTP)
                    .SetPosition<Touch>(section, tick, 0)
                    .SetArea(data[0].ToString())
                    .SetIsHanabi(data.Contains("f"));
            }
            else
            {
                if (data.Length > 1 && int.TryParse(data.Replace("f", "")[1].ToString(), out var key))
                {
                    return new Touch()
                        .SetStyle<Touch>(style)
                        .SetType<Touch>(NoteType.TTP)
                        .SetPosition<Touch>(section, tick, key - 1)
                        .SetArea(data[0].ToString())
                        .SetIsHanabi(data.Contains("f"));
                }
                else
                {
                    throw new Exception("Invalid touch definition in maidata.txt");
                }
            }
        }

        static TouchHold GenerateTouchHold(int section, int tick, string data, int clockDefinition)
        {
            NoteStyle style = NoteStyle.NM;
            TouchHold result;

            result = new TouchHold()
                    .SetStyle<TouchHold>(style)
                    .SetType<TouchHold>(NoteType.THO)
                    .SetPosition<TouchHold>(section, tick, 0)
                    .SetArea(data[0].ToString())
                    .SetIsHanabi(data.Contains("f"));

            //get duration
            int durationIndex = data.IndexOf("[");
            int durationEndIndex = data.IndexOf("]");
            int duration = GetDuration(data, durationIndex, durationEndIndex, clockDefinition);
            result.SetDuration(duration);
            return result;
        }

        static List<Note> GenerateSlideGroup(int section, int tick, string data, int clockDefinition)
        {
            List<Note> result = new List<Note>();

            bool IsNextFragment(char c, char last)
            {
                if (IsNextSlide(last)) return false;
                if (c == '-' ||
                    c == 'v' ||
                    c == 'w' ||
                    c == '<' ||
                    c == '>' ||
                    c == 's' ||
                    c == 'z' ||
                    c == 'V' ||
                    c == '^')
                {
                    return true;
                }
                else if (c == 'p')
                {
                    if (last != 'p') return true;
                    else return false;
                }
                else if (c == 'q')
                {
                    if (last != 'q') return true;
                    else return false;
                }
                else
                {
                    return false;
                }
            }

            bool IsNextSlide(char c)
            {
                if (c == '*') return true;
                else return false;
            }

            Tap GetSlideStart(int section, int tick, string data)
            {
                var note = GenerateTap(section, tick, data);
                return note.SetType<Tap>(NoteType.STR);
            }

            int state = 0;
            const int STATE_TOP = 0;
            const int STATE_SLIDE = 1;
            char lastChar = ' ';
            StringBuilder sb = new StringBuilder();

            Tap slideStart = new Tap();
            List<Slide> generatedSlides = new List<Slide>();
            List<Slide> processingSlides = new List<Slide>();

            void ProcessSlide()
            {
                //process slide duration & connection
                //we only process count > 1
                if (processingSlides.Count > 1)
                {
                    //set style
                    processingSlides[0].SetStyle<Slide>(processingSlides[^1].Style);
                    for (int i = 1; i < processingSlides.Count; i++)
                    {
                        processingSlides[i].SetStyle<Slide>(NoteStyle.CN);
                    }

                    //check duration property
                    int lastDefinedDuration = processingSlides[^1].Duration;
                    int durationDefinitionCount = 0;
                    foreach (var slide in processingSlides)
                    {
                        if (slide.Duration > 0) durationDefinitionCount++;
                    }

                    if (durationDefinitionCount == processingSlides.Count)
                    {
                        int tickIncrement = 0;
                        for (int i = 1; i < processingSlides.Count; i++)
                        {
                            tickIncrement += processingSlides[i - 1].Duration + processingSlides[i - 1].InitialDuration;
                            processingSlides[i].SetInitial(0);
                            processingSlides[i].AddTick<Slide>(tickIncrement, clockDefinition);
                        }
                    }
                    else if (lastDefinedDuration != 0 && durationDefinitionCount == 1)
                    {
                        //get slide total length
                        double totalLength = 0;
                        foreach (var slide in processingSlides)
                        {
                            totalLength += slide.Length;
                        }
                        //set start duration
                        processingSlides[0].SetDuration((int)(processingSlides[0].Length / totalLength * lastDefinedDuration));
                        int tickIncrement = 0;
                        for (int i = 1; i < processingSlides.Count; i++)
                        {
                            tickIncrement += processingSlides[i - 1].Duration + processingSlides[i - 1].InitialDuration;
                            processingSlides[i].SetInitial(0);
                            processingSlides[i].SetDuration((int)(processingSlides[i].Length / totalLength * lastDefinedDuration));
                            processingSlides[i].AddTick<Slide>(tickIncrement, clockDefinition);
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid connected slide definition in maidata.txt");
                    }
                }

                //each slide, init another group for this, and save the generated group
                generatedSlides.AddRange(processingSlides);
                processingSlides.Clear();
            }

            foreach (char c in data)
            {
                switch (state)
                {
                    case STATE_TOP:
                        if (IsNextFragment(c, lastChar))
                        {
                            slideStart = GetSlideStart(section, tick, sb.ToString());
                            sb.Clear();
                            state = STATE_SLIDE;
                        }
                        sb.Append(c);
                        break;
                    case STATE_SLIDE:
                        if (IsNextFragment(c, lastChar) || IsNextSlide(c))
                        {
                            //generate
                            var slide = GenerateSlide(section, tick, sb.ToString(), clockDefinition, processingSlides.Count == 0 ? slideStart.Key : processingSlides[^1].TargetKey);
                            sb.Clear();
                            processingSlides.Add(slide);
                        }

                        if (IsNextSlide(c))
                        {
                            ProcessSlide();
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }

                lastChar = c;
            }

            if (sb.Length > 0)
            {
                //generate
                var slide = GenerateSlide(section, tick, sb.ToString(), clockDefinition, processingSlides.Count == 0 ? slideStart.Key : processingSlides[^1].TargetKey);
                sb.Clear();
                processingSlides.Add(slide);

                ProcessSlide();
            }
            result.Add(slideStart);
            result.AddRange(generatedSlides);
            return result;
        }

        static Slide GenerateSlide(int section, int tick, string data, int clockDefinition, int sourceKey)
        {
            NoteStyle style = NoteStyle.NM;
            if (data.Contains("b")) style = NoteStyle.BR;

            Slide result = new Slide().SetStyle<Slide>(style).SetPosition<Slide>(section, tick, sourceKey);

            //get slide shape
            string simplifiedData = data;

            if (data.Contains("-"))
            {
                result.SetType<Slide>(NoteType.SI_);
                simplifiedData = data.Substring(data.IndexOf("-") + 1);
            }
            else if (data.Contains("pp"))
            {
                result.SetType<Slide>(NoteType.SXL);
                simplifiedData = data.Substring(data.IndexOf("pp") + 2);
            }
            else if (data.Contains("qq"))
            {
                result.SetType<Slide>(NoteType.SXR);
                simplifiedData = data.Substring(data.IndexOf("qq") + 2);
            }
            else if (data.Contains("p"))
            {
                result.SetType<Slide>(NoteType.SUL);
                simplifiedData = data.Substring(data.IndexOf("p") + 1);
            }
            else if (data.Contains("q"))
            {
                result.SetType<Slide>(NoteType.SUR);
                simplifiedData = data.Substring(data.IndexOf("q") + 1);
            }
            else if (data.Contains("v"))
            {
                result.SetType<Slide>(NoteType.SV_);
                simplifiedData = data.Substring(data.IndexOf("v") + 1);
            }
            else if (data.Contains("w"))
            {
                result.SetType<Slide>(NoteType.SF_);
                simplifiedData = data.Substring(data.IndexOf("w") + 1);
            }
            else if (data.Contains("s"))
            {
                result.SetType<Slide>(NoteType.SSL);
                simplifiedData = data.Substring(data.IndexOf("s") + 1);
            }
            else if (data.Contains("z"))
            {
                result.SetType<Slide>(NoteType.SSR);
                simplifiedData = data.Substring(data.IndexOf("z") + 1);
            }
            else if (data.Contains("z"))
            {
                result.SetType<Slide>(NoteType.SSR);
                simplifiedData = data.Substring(data.IndexOf("z") + 1);
            }
            else if (data.Contains("<"))
            {
                if (sourceKey == 0 ||
                    sourceKey == 1 ||
                    sourceKey == 6 ||
                    sourceKey == 7)
                    result.SetType<Slide>(NoteType.SCL);
                else result.SetType<Slide>(NoteType.SCR);
                simplifiedData = data.Substring(data.IndexOf("<") + 1);
            }
            else if (data.Contains(">"))
            {
                if (sourceKey == 0 ||
                    sourceKey == 1 ||
                    sourceKey == 6 ||
                    sourceKey == 7)
                    result.SetType<Slide>(NoteType.SCR);
                else result.SetType<Slide>(NoteType.SCL);
                simplifiedData = data.Substring(data.IndexOf(">") + 1);
            }
            else if (data.Contains("^"))
            {
                simplifiedData = data.Substring(data.IndexOf("^") + 1);
                //we need target key to determine direction
                if (int.TryParse(simplifiedData[0].ToString(), out var tempTarget))
                {
                    int startPt = sourceKey;
                    int step = 0;
                    tempTarget -= 1; //to convert key to target

                    while (startPt != tempTarget)
                    {
                        startPt++;
                        if (startPt > 7) startPt = 0;
                        step++;
                    }

                    if (step == 0 || step == 4) throw new Exception("Invalid arc slide definition in maidata.txt");

                    if (step > 4) result.SetType<Slide>(NoteType.SCL);
                    else result.SetType<Slide>(NoteType.SCR);
                }
                else
                {
                    throw new Exception("Invalid slide definition in maidata.txt");
                }
            }
            else if (data.Contains("V"))
            {
                simplifiedData = data.Substring(data.IndexOf("V") + 2);
                var interceptedKeyString = data.Substring(data.IndexOf("V") + 1)[0].ToString();

                //we need intercepted key to determine direction
                if (int.TryParse(interceptedKeyString, out var interceptedKey))
                {
                    interceptedKey -= 1; //convert key to index

                    int leftKey = sourceKey - 2;
                    int rightKey = sourceKey + 2;

                    if (leftKey < 0) leftKey += 8;
                    if (rightKey > 7) rightKey -= 8;

                    if (leftKey == interceptedKey) result.SetType<Slide>(NoteType.SLL);
                    else if (rightKey == interceptedKey) result.SetType<Slide>(NoteType.SLR);
                    else throw new Exception("Invalid V slide definition in maidata.txt");
                }
                else
                {
                    throw new Exception("Invalid slide definition in maidata.txt");
                }
            }

            //get target key
            if (int.TryParse(simplifiedData[0].ToString(), out var targetKey))
            {
                result.SetTarget(targetKey - 1);
            }
            else
            {
                throw new Exception("Invalid slide definition in maidata.txt");
            }

            //get duration
            int durationIndex = data.IndexOf("[");
            int durationEndIndex = data.IndexOf("]");
            int duration = GetDuration(data, durationIndex, durationEndIndex, clockDefinition);
            result.SetDuration(duration);

            //set initial, by default it is 96 (1/4)
            result.SetInitial(96);
            return result;
        }

        static int GetDuration(string data, int leftPt, int rightPt, int clockDefinition)
        {
            if (leftPt == -1 && rightPt == -1) { return 0; }
            else if (leftPt == -1 || rightPt == -1 || rightPt - leftPt < 4) throw new Exception("Invalid duration definition in maidata.txt");
            string[] durationPair = data.Substring(leftPt + 1, rightPt - leftPt - 1).Split(':');
            if (durationPair.Length != 2) throw new Exception("Invalid duration definition in maidata.txt");
            if (int.TryParse(durationPair[0], out var measure) && int.TryParse(durationPair[1], out var count))
            {
                return (int)Math.Round(clockDefinition * (1.0 / measure) * count);
            }
            else
            {
                throw new Exception("Invalid duration definition in maidata.txt");
            }
        }
    }
}
