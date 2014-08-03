namespace Esthar.Data.Transform
{
    public abstract class AsmBinding
    {
        public readonly AsmBindingType Type;
        public readonly AsmSegment SourceSegment;
        public readonly int SourceOffset;

        public AsmBinding(AsmBindingType type, AsmSegment source, int sourceOffset)
        {
            Type = type;
            SourceSegment = source;
            SourceOffset = sourceOffset;
        }

        public virtual AsmSegment[] ResolveTargets()
        {
            return null;
        }

        public virtual bool? ResolveCondition()
        {
            return null;
        }
    }
}