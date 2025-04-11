#define DEV_MODE

namespace LuminaryEngine.Engine.Core.ResourceManagement;

public class DevModeConfig
{
#if DEV_MODE
    public static bool ShowCollisionBoxes => false;
    public static bool MuteAllSounds => true;
#else
    public static bool ShowCollisionBoxes => false;
    public static bool MuteAllSounds => false;
#endif
}