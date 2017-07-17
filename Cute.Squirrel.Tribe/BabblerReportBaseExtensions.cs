using Cute.Squirrel.Babbler;

namespace Cute.Squirrel.Tribe
{
    public static class BabblerReportBaseExtensions
    {
        public static TribeMemberIdentifier AsTribeIdentifier(this IBabblerReportBase report)
        {
            return TribeMemberIdentifier.FromIBabblerReportBase(report);
        }
    }
}