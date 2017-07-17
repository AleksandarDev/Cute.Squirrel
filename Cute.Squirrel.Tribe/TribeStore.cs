using System;
using System.Collections.Generic;
using System.Linq;
using Cute.Squirrel.Babbler;

namespace Cute.Squirrel.Tribe
{
    public class TribeStore<TMember, TData> 
        where TMember : ITribeMember<TData> 
        where TData : IBabblerReportBase
    {
        private readonly object membersLock = new object();
        private readonly Dictionary<TribeMemberIdentifier, TMember> members = 
            new Dictionary<TribeMemberIdentifier, TMember>();


        public void RegisterMember(TMember member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            lock (this.membersLock)
            {
                if (this.IsRegisteredMember(member.Identifier))
                    this.members[member.Identifier] = member;
                else this.members.Add(member.Identifier, member);
            }
        }

        public void Unregister(TribeMemberIdentifier identifier)
        {
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            lock (this.membersLock)
            {
                if (this.IsRegisteredMember(identifier))
                    this.members.Remove(identifier);
            }
        }

        public IEnumerable<TMember> GetMembers()
        {
            return this.members.Values;
        }

        public IEnumerable<TMember> GetMembersForSource(string source)
        {
            return this.members.Values.Where(member => member.Identifier.Source == source);
        }

        public TMember GetMember(TribeMemberIdentifier identifier)
        {
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            lock (this.membersLock)
            {
                if (this.IsRegisteredMember(identifier))
                    return this.members[identifier];
            }

            return default(TMember);
        }

        public bool IsRegisteredMember(TribeMemberIdentifier identifier)
        {
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            lock (this.members)
            {
                return this.members.ContainsKey(identifier);
            }
        }
    }
}
