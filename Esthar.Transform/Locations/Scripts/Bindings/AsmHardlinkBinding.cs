namespace Esthar.Data.Transform
{
    public sealed class AsmHardlinkBinding : AsmBinding
    {
        public readonly AsmSegment TargetSegment;

        public AsmHardlinkBinding(AsmSegment source, AsmSegment target)
            : base(AsmBindingType.Hardlink, source, source.Length - 1)
        {
            TargetSegment = target;
        }

        public override AsmSegment[] ResolveTargets()
        {
            return new[] { TargetSegment };
        }

        public override bool? ResolveCondition()
        {
            return true;
        }
    }
}