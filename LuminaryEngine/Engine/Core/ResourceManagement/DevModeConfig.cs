namespace LuminaryEngine.Engine.Core.ResourceManagement;

public class DevModeConfig
{
#if DEBUG
    public static bool EnableDevMode = true;
#elif RELEASE
    public static bool EnableDevMode = false;
#endif
}