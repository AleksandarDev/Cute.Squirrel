using Cute.Squirrel.Babbler;

namespace Cute.Squirrel.Tribe
{
    public interface ITribeMember<TData> 
        where TData : IBabblerReportBase
    {
        TribeMemberIdentifier Identifier { get; set; }

        bool IsReporting { get; set; }

        TData Data { get; set; }

    }
}