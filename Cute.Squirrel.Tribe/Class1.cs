using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cute.Squirrel.Tribe
{
    public sealed class TribeMemberIdentifier : IEquatable<TribeMemberIdentifier>
    {
        public string Identifier { get; }

        public string Source { get; }


        public TribeMemberIdentifier(string source, string identifier)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(source));
            if (string.IsNullOrWhiteSpace(identifier))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(identifier));

            this.Source = source;
            this.Identifier = identifier;
        }


        public override bool Equals(object obj)
        {
            // ReSharper disable once BaseObjectEqualsIsObjectEquals
            return base.Equals(obj);
        }

        public bool Equals(TribeMemberIdentifier other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Identifier, other.Identifier) && string.Equals(Source, other.Source);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Identifier != null ? Identifier.GetHashCode() : 0) * 397) ^ (Source != null ? Source.GetHashCode() : 0);
            }
        }
    }

    public abstract class TribeMember<T> 
    {
        public TribeMemberIdentifier Identifier { get; set; }

        public T Data { get; set; }


        protected TribeMember(TribeMemberIdentifier identifier) : this(identifier, default(T))
        {
        }

        protected TribeMember(TribeMemberIdentifier identifier, T data)
        {
            this.Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
            this.Data = data;
        }
    }

    public class TribeTracker<T>
    {
        private readonly Dictionary<TribeMemberIdentifier, TribeMember<T>> members = 
            new Dictionary<TribeMemberIdentifier, TribeMember<T>>();


        public void RegisterMember(TribeMember<T> member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            if (this.IsRegisteredMember(member.Identifier))
                this.members[member.Identifier] = member;
            else this.members.Add(member.Identifier, member);
        }

        public void Unregister(TribeMemberIdentifier identifier)
        {
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            if (this.IsRegisteredMember(identifier))
                this.members.Remove(identifier);
        }

        public IEnumerable<TribeMember<T>> GetMembers()
        {
            return this.members.Values;
        }

        public IEnumerable<TribeMember<T>> GetMembersForSource(string source)
        {
            return this.members.Values.Where(member => member.Identifier.Source == source);
        }

        public TribeMember<T> GetMember(TribeMemberIdentifier identifier)
        {
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            if (this.IsRegisteredMember(identifier))
                return this.members[identifier];

            return null;
        }

        public bool IsRegisteredMember(TribeMemberIdentifier identifier)
        {
            if (identifier == null) throw new ArgumentNullException(nameof(identifier));

            return this.members.ContainsKey(identifier);
        }
    }
}
