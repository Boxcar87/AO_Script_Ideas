
//GetResult(string stringifiedNumber) returns string threewindowTypeResponse

// Can uncomment these for simple testing in a CLI with dotnet
string[] arguments = Environment.GetCommandLineArgs();
string stringResponse = MA.GetResult(arguments[2]);
Console.Write(stringResponse);
File.WriteAllText("output.txt", stringResponse); //Sometimes easier to copy text for testing from a file.

public class MA
{
    public static string GetResult(string value)
    {
        bool valid = Int32.TryParse(value, out int numValue);
        if (!valid)
            return "Error! Check your input value";
        if (numValue < 0)
            return "Error! Value must be > 0";
        if (numValue > 3000)
            numValue = 3000;

        double qualityLevel = 0;
        switch (numValue)
        {
            case < 1001:
                qualityLevel = Math.Floor(numValue / 2.0);
                break;
            case < 2001:
                qualityLevel = Math.Floor((numValue - 1000) / 2.0);
                break;
            case < 3001:
                qualityLevel = Math.Floor((numValue - 2000) / 2.0);
                break;
        }

        int[] thresholds = [200, 1000, 2000];
        (int, int)[] martialArtistIDs = [(211352, 211353),(211353, 211354), (211357, 211358), (211363, 211364)];
        (int, int)[] shadeIDs = [(211349, 211350), (211350, 211351), (211359, 211360), (211365, 211366)];
        (int, int)[] othersIDs = [(43712, 144745), (144745, 43713), (211355, 211356), (211361, 211362)];

        int range = 0;
        if (numValue < 2001){
            for(int i = 0; i<thresholds.Length; i++)
            {   
                range = i;
                if(numValue<thresholds[range])
                    break;
            }
        }
        else
            range = 3;

        string textColor = "#FFFFFF";
        string highlightColor = "#FFFF00";

        string responseText = $"<font color={textColor}><font color={highlightColor}>{numValue}</font> MA</font> - </font><font color={highlightColor}>";
        responseText += $"<a href=\"itemref://{martialArtistIDs[range].Item1}/{martialArtistIDs[range].Item2}/{qualityLevel}\">Martial Artist</a> | ";
        responseText += $"<a href=\"itemref://{shadeIDs[range].Item1}/{shadeIDs[range].Item2}/{qualityLevel}\">Shade</a> | ";
        responseText += $"<a href=\"itemref://{othersIDs[range].Item1}/{othersIDs[range].Item2}/{qualityLevel}\">Others</a></font>";
        return responseText;
    }
}