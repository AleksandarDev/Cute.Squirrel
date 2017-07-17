using System;
using Cute.Squirrel.Babbler;

namespace Cute.Squirrel.Tribe
{
    public abstract class TribeMember<TData> : ITribeMember<TData> 
        where TData : IBabblerReportBase
    {
        public TribeMemberIdentifier Identifier { get; set; }

        public bool IsReporting { get; set; }

        public TData Data { get; set; }


        protected TribeMember(TribeMemberIdentifier identifier) : this(identifier, default(TData))
        {
        }

        protected TribeMember(TribeMemberIdentifier identifier, TData data)
        {
            this.Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            this.Data = data;
        }
    }
}