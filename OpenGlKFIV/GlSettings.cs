using FormatKFIV.Utility;

namespace ToolsForKFIV;

public class Settings
{
    public Colour mtBgCC = Colour.FromARGB(255, 40, 22, 46);
    public Colour mtXAxC = Colour.FromARGB(255, 244, 67, 54);
    public Colour mtYAxC = Colour.FromARGB(255, 3, 169, 244);
    public Colour mtZAxC = Colour.FromARGB(255, 76, 175, 80);
    public bool mtShowGridAxis = true;
    
    private static Settings _instance = new();

    public static Settings Instance => _instance;
}