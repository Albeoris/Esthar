using System;
using System.Collections;
using System.Collections.Generic;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class AsmSegment : IEnumerable<JsmOperation>, IEquatable<AsmSegment>
    {
        public readonly Guid Id = Guid.NewGuid();
        public readonly AsmEvent Event;
        public readonly int Offset;
        public readonly int Length;
        public readonly List<AsmBinding> OutputBindings = new List<AsmBinding>();
        public readonly List<AsmBinding> InputBindings = new List<AsmBinding>();

        public AsmSegment(AsmEvent evt, int offset, int length)
        {
            Exceptions.CheckArgumentOutOfRangeException(length, "length", 1, 10000);

            Event = evt;
            Offset = offset;
            Length = length;
        }

        public JsmOperation this[int index]
        {
            get { return Event[Offset + index]; }
            set { Event[Offset + index] = value; }
        }

        public IEnumerator<JsmOperation> GetEnumerator()
        {
            for (int i = Offset; i < Length; i++)
                yield return Event[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #region IEquatable<AsmSegment>

        public override bool Equals(object obj)
        {
            return Equals(obj as AsmSegment);
        }

        public bool Equals(AsmSegment other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Event, other.Event) && Offset == other.Offset && Length == other.Length;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Event != null ? Event.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Offset;
                hashCode = (hashCode * 397) ^ Length;
                return hashCode;
            }
        }

        public static bool operator ==(AsmSegment left, AsmSegment right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AsmSegment left, AsmSegment right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}