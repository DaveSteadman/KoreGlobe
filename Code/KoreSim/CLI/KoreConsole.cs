using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;

using KoreCommon;

namespace KoreSim;

#nullable enable

// Class to run a thread for command line input.
// This class can't do anything, it must delgate all actual processing to the KoreEventDriver class.
public class KoreConsole
{
    // Thread control
    private Thread? consoleThread = null;
    private bool running;

    // List of command handlers (with a signature) we can select and exeute.
    private readonly List<KoreCommand> commandHandlers = new List<KoreCommand>();

    // Two lists to hold input and output strings for the console.
    private KoreThreadsafeStringList InputQueue = new KoreThreadsafeStringList();
    private KoreThreadsafeStringList OutputQueue = new KoreThreadsafeStringList();

    // Event to set on new input, to unblock the console thread to process new commands.
    private AutoResetEvent InputEvent = new AutoResetEvent(false);

    // ---------------------------------------------------------------------------------------------
    // MARK: Constructor
    // ---------------------------------------------------------------------------------------------

    public KoreConsole()
    {
        running = false;

        // Register command handlers
        InitializeCommands();
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Thread control
    // ---------------------------------------------------------------------------------------------

    public void Start()
    {
        if (running == false)
        {
            KoreCentralLog.AddEntry($"Starting console thread... ({commandHandlers.Count} Commands.)");
            running = true;
            consoleThread = new Thread(ConsoleLoop);
            consoleThread?.Start();
        }
    }

    public void Stop()
    {
        running = false;
        consoleThread = null;
    }

    public void WaitForExit()
    {
        KoreCentralLog.AddEntry("Waiting on Join()...");
        consoleThread?.Join(); // This will block until consoleThread finishes execution
        KoreCentralLog.AddEntry("Join() returned.");
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: InitializeCommands
    // ---------------------------------------------------------------------------------------------

    private void InitializeCommands()
    {
        // Register commands and their handlers here
        KoreCentralLog.AddEntry("KoreConsole: Initializing commands...");

        // General app control commands
        commandHandlers.Add(new KoreCommandVersion());
        commandHandlers.Add(new KoreCommandExit());

        // Sim control
        commandHandlers.Add(new KoreCommandSimClock());
        commandHandlers.Add(new KoreCommandSimStart());
        commandHandlers.Add(new KoreCommandSimStop());
        commandHandlers.Add(new KoreCommandSimReset());
        commandHandlers.Add(new KoreCommandSimPause());
        commandHandlers.Add(new KoreCommandSimResume());

        // Network
        commandHandlers.Add(new KoreCommandNetworkReport());
        commandHandlers.Add(new KoreCommandNetworkInjectIncoming());
        commandHandlers.Add(new KoreCommandNetworkEndConnection());

        //commandHandlers.Add(new KoreCommandModelJsonRead());
        //commandHandlers.Add(new KoreCommandModelJsonWrite());

        // Entity control
        commandHandlers.Add(new KoreCommandEntityTestScenario());
        commandHandlers.Add(new KoreCommandEntityAdd());
        commandHandlers.Add(new KoreCommandEntityDelete());
        commandHandlers.Add(new KoreCommandEntityDeleteAll());

        // Entity details
        commandHandlers.Add(new KoreCommandEntityPosition());
        commandHandlers.Add(new KoreCommandEntityCourse());
        commandHandlers.Add(new KoreCommandEntityCourseDelta());

        // Entity Report
        commandHandlers.Add(new KoreCommandEntityReportElem());
        commandHandlers.Add(new KoreCommandEntityReportPos());

        // Element Control
        //commandHandlers.Add(new KoreCommandEntityDeleteAllEmitters());

        // MapServer
        commandHandlers.Add(new KoreCommandElePrep());
        commandHandlers.Add(new KoreCommandEleLoadArc());
        commandHandlers.Add(new KoreCommandEleSaveTile());
        commandHandlers.Add(new KoreCommandEleSaveTileSet());
        commandHandlers.Add(new KoreCommandEleLoadTile());
        commandHandlers.Add(new KoreCommandEleForPos());
        commandHandlers.Add(new KoreCommandEleReport());
        commandHandlers.Add(new KoreCommandElePatchLoad());
        commandHandlers.Add(new KoreCommandElePatchSave());

        // Tile Images
        commandHandlers.Add(new KoreCommandSatCollate());
        commandHandlers.Add(new KoreCommandSatDivide());
        commandHandlers.Add(new KoreCommandSatDivideTo());




        // commandHandlers.Add("help", CmdHelp);
        // commandHandlers.Add("version", CmdVersion);
        // commandHandlers.Add("#", CmdComment);
        // commandHandlers.Add("runfile", CmdRunFile);

        // // MapUtils
        // commandHandlers.Add("tilecode", CmdTileCode);

        // // Map handlers
        // commandHandlers.Add("setroot", CmdSetRoot);
        // commandHandlers.Add("getroot", CmdGetRoot);
        // commandHandlers.Add("create", CmdCreate);

        // // Networking
        // commandHandlers.Add("network", CmdNetwork);
    }

    private void ConsoleLoop()
    {
        KoreCentralLog.AddEntry("Console thread starting...");

        // delay for a second to allow the main thread to start up
        System.Threading.Thread.Sleep(1000);

        while (running)
        {
            // Wait for input trigger
            InputEvent.WaitOne();
            ProcessCommand();
        }
        KoreCentralLog.AddEntry("Console thread exiting...");
    }

    private void ProcessCommand()
    {
        // Loop while we have commands to process
        while (!InputQueue.IsEmpty())
        {
            // Get the string a space-delimit the parts
            string inputLine = InputQueue.RetrieveString();
            //var inputParts = inputLine.Trim().Split(' ').ToList();

            // Split the input line into parts, removing any empty entries
            var inputParts = inputLine.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            // Check for internal commands - true if executed.
            if (RunInternalCommand(inputParts))
                continue;

            // Go through each of the registered command handlers looking for a match
            foreach (var currCmd in commandHandlers)
            {
                if (currCmd.Matches(inputParts))
                {
                    // Pass remaining parts as parameters to the command
                    string responseStr = currCmd.Execute(inputParts.Skip(currCmd.SignatureCount).ToList());
                    OutputQueue.AddString(responseStr);
                    break; // leave the current command loop - move back out to the next input line
                }
            }
        }
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Input
    // ---------------------------------------------------------------------------------------------

    public void AddInput(string input)
    {
        InputQueue.AddString(input);
        InputEvent.Set();
    }

    // ---------------------------------------------------------------------------------------------
    // MARK: Output
    // ---------------------------------------------------------------------------------------------

    public bool HasOutput()
    {
        return !OutputQueue.IsEmpty();
    }

    public string GetOutput()
    {
        StringBuilder output = new StringBuilder();

        while (!OutputQueue.IsEmpty())
            output.AppendLine(OutputQueue.RetrieveString());

        return output.ToString();
    }

    // ---------------------------------------------------------------------------------------------
    // Command functions:
    // ---------------------------------------------------------------------------------------------
    // - private void Cmd<Name>(string[] args)

    private bool RunInternalCommand(List<string> inputParts)
    {
        if (inputParts.Count == 0)
            return false;

        string command = inputParts[0];

        switch (command)
        {
            case "help":
                {
                    StringBuilder helpStr = new StringBuilder();
                    helpStr.AppendLine("Available commands:");
                    foreach (var cmd in commandHandlers)
                    {
                        helpStr.AppendLine($"- {cmd.HelpString}");
                    }
                    OutputQueue.AddString(helpStr.ToString());
                    return true;
                }

            case "runfile":
                {
                    if (inputParts.Count < 2)
                    {
                        OutputQueue.AddString("Usage: runfile <filename>");
                        return true;
                    }

                    string filename = inputParts[1];

                    if (!System.IO.File.Exists(filename))
                    {
                        OutputQueue.AddString($"File does not exist: {filename}");
                        return true;
                    }

                    OutputQueue.AddString($"Running file: {filename}");

                    string[] lines = System.IO.File.ReadAllLines(filename);

                    foreach (string line in lines)
                    {
                        // trim line to 100 characters
                        if (line.Length > 100)
                        {
                            string shortLine = line.Substring(0, 100);
                            shortLine += "...";
                            OutputQueue.AddString($"FILE>> {shortLine}");
                        }
                        else
                        {
                            OutputQueue.AddString($"FILE>> {line}");
                        }

                        InputQueue.AddString(line);
                    }
                    return true;
                }

            default:
                return false;
        }
    }
}

