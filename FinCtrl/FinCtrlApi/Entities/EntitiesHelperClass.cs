using MongoDB.Driver;

namespace FinCtrlApi.Entities
{
    public static class EntitiesHelperClass
    {
        public static bool DebugBuild
        {
#if DEBUG
            get => true;
#else
            get => false;
#endif
        }
    }
}
