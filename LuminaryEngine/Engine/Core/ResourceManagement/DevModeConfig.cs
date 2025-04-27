#define DEV_MODE

namespace LuminaryEngine.Engine.Core.ResourceManagement;

public class DevModeConfig
{
#if DEV_MODE
    public static bool ShowCollisionBoxes => false;
    public static bool MuteAllSounds => true;
    public static bool IsDebugEnabled => true;
    public static bool IgnoreSaves => true;
#else
    public static bool ShowCollisionBoxes => false;
    public static bool MuteAllSounds => false;
    public static bool IsDebugEnabled => false;
    public static bool IgnoreSaves => false;
#endif
}