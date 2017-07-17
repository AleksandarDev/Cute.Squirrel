using System;
using Cute.Squirrel.Babbler;

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


        public static TribeMemberIdentifier FromIBabblerReportBase(IBabblerReportBase report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            return new TribeMemberIdentifier(report.Source, report.Identifier);
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
}