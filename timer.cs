
using System.Timers;
using System.Collections.Generic;
using System.Text.RegularExpressions;

//GetResult(string stringifiedNumber) returns string threewindowTypeResponse

// Can uncomment these for simple testing in a CLI with dotnet
string[] arguments = Environment.GetCommandLineArgs();
string stringResponse = Timer.StartTimersTest(arguments[2], arguments[3]);
Console.Write(stringResponse);
// File.WriteAllText("output.txt", stringResponse); //Sometimes easier to copy text for testing from a file.

public class Timer
{
    static Dictionary<string, (long, System.Timers.Timer)>  CustomTimers = new Dictionary<string, (long, System.Timers.Timer)>();
    public static (int, string) CreateTimer(string name, string duration)
    {   
        // long durationLimit = 2 * 3600000; // Do you want a limit?
        duration = duration.Trim(' ');
        long totalDuration = 0;
        List<string> uniqueDenotationsUsed = [];
        
        // Split the string into sections based on hour minute and second.
        string[] durationSections = Regex.Split(duration, @"(?<=[hm])");
        foreach (string section in durationSections)
        {
            if(section == string.Empty) // Clean up hanging empty elements in array
                continue;

            string denotation = section[^1].ToString();
            string durationOnly = section[..^1];

            // Ensure the format doesnt have duplicates like 1h1h1s
            if (!uniqueDenotationsUsed.Contains(denotation))
                uniqueDenotationsUsed.Add(denotation);
            else
                return (-1, "Duplicate denotation used. Please used format 1h10m30s");

            bool validDuration = Int64.TryParse(durationOnly, out long durationTime);
            if (!validDuration)
                return (-1, "Invalid duration. Please use format 1h10m30s");
            if (durationTime < 0)
                return (-1, "Duration must be greater than 0. Please use format 1h10m30s");

            // multiply duration milliseconds appropriately
            switch (denotation.ToLower())
            {
                case "s":
                    durationTime *= 1000;
                    break;
                case "m":
                    durationTime *= 60000;
                    break;
                case "h":
                    durationTime *= 3600000;
                    break;
                default:
                    return (-1, "Invalid duration type. Please use format 1h10m30s");
            }
            totalDuration += durationTime;
        }

        // Keep if you want a limit
        // if(totalDuration > durationLimit)
        // {
        //     return "No No No, I want to be alive when this timer ends.";
        // }

        long timerEnd = DateTimeOffset.Now.ToUnixTimeMilliseconds() + totalDuration;

        // Create new timer and pass name into callback for proper termination once timer ends
        System.Timers.Timer newTimer = new(totalDuration);
        newTimer.Elapsed += (sender, e) => ExpireTimer(name);
        newTimer.AutoReset = false;
        newTimer.Start();

        CustomTimers.Add(name, (timerEnd ,newTimer));
        return (0, $"Created timer for {name} Successfully");
    }

    public static string GetTimers()
    {
        string highlightColor = "#FFFF00";
        string alertColor = "#FF0000";
        string textColor = "#FFFFFF";
        string responseText = "<a href=\"text://Active Timers:<br><br>";
        foreach(KeyValuePair<string, (long, System.Timers.Timer)> timer in CustomTimers)
        {
            string secondsColor = textColor;
            long remaining = timer.Value.Item1 - DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if(remaining <= 60000)
                secondsColor = alertColor;
                
            responseText += $"    <font color={highlightColor}>{timer.Key}</font>: <font color={secondsColor}>{ParseTimeRemaining(timer.Value.Item1 - DateTimeOffset.Now.ToUnixTimeMilliseconds())}</font> remaining<br><br>";
        }
        responseText += "\">Current Timers</a>";
        return responseText;
    } 
    static void ExpireTimer(string name)
    {
        CustomTimers[name].Item2.Dispose();
        Console.WriteLine($"{name} timer has been disposed"); // Left in for testing functionality
        CustomTimers.Remove(name);
        GetTimers();
    }

    // Convert time into readable string
    static string ParseTimeRemaining(long remaining)
    {
        TimeSpan t = TimeSpan.FromMilliseconds(remaining);

        remaining /= 1000;
        switch (remaining)
        {
            case >= 3600: // Hours
                return $"{(int)t.Hours}h {t.Minutes}m {t.Seconds}s";
            case >= 60: // Minutes
                return $"{t.Minutes}m {t.Seconds}s";
            default:
                return $"{t.Seconds}s";
        }
    }

    // Simply a test function that will stay alive while timers run
    // Wait 3 seconds to see times start being disposed
    // the for loop items will still run and succeed even if initial one fails
    // Suggested cli command: dotnet script timer.cs newTimer 1h78m137s
    public static string StartTimersTest(string name, string duration)
    {
        (int status, string responseText) = CreateTimer(name, duration);
        for(var i=1; i<3; i++)
        {
          CreateTimer(name+i.ToString(), (i*3).ToString()+"s");
        }        
        if(status<0) // Only returning if theres an error, remove check to return success also
            return responseText; 

        Console.ReadLine(); // Press enter to see results
        return GetTimers();
    }
}