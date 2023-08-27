using ManyConsole;
using SimaiConverter.Convert.Simai;
using SimaiConverter.Data;

namespace SimaiConverter
{
    class Program
    {
        public static int Main(string[] args)
        {
            var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
            return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
        }

        public class Convert : ConsoleCommand
        {
            const int CODE_SUCCESS = 0;
            const int CODE_ERROR = 2;
            const string MA2VERSION = "1.04.00"; //Targeted Ma2 Version

            public string? InputPath { get; set; }
            public string? OutputPath { get; set; }

            public Convert()
            {
                IsCommand("convert", $"convert simai chart to ma2 (supported ma2 version {MA2VERSION})");
                HasLongDescription("IMPORTANT: utage style chart is currently NOT supported");
                HasRequiredOption("i|input=", "input path of maidata.txt", p => InputPath = p);
                HasRequiredOption("o|output=", "output path of maidata.ma2", p => OutputPath = p);
            }

            public override int Run(string[] remainingArguments)
            {
                try
                {
                    if (InputPath == null || OutputPath == null) throw new FileNotFoundException("File not found");

                    var chart = SimaiReader.ReadChart(InputPath);
                    Console.WriteLine("");
                    Console.WriteLine("Succesfully converted! Exporting...");

                    var convertedChart = chart.Compose();
                    Console.Write(convertedChart);

                    File.WriteAllText(OutputPath, convertedChart);

                    Console.WriteLine("Exported to " + OutputPath);

                    return CODE_SUCCESS;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    return CODE_ERROR;
                }
            }
        }
    }
}