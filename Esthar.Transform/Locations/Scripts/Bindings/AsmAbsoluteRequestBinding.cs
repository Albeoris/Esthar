namespace Esthar.Data.Transform
{
    public sealed class AsmAbsoluteRequestBinding : AsmBinding
    {
        public readonly ushort TargetModuleIndex;
        public readonly AsmValueSource TargetEventLabel;

        public AsmAbsoluteRequestBinding(AsmSegment source, int sourceOffset, int targetModuleIndex, AsmValueSource targetEventLabel)
            : base(AsmBindingType.AbsoluteRequest, source, sourceOffset)
        {
            TargetModuleIndex = (ushort)targetModuleIndex;
            TargetEventLabel = targetEventLabel;
        }

        public override AsmSegment[] ResolveTargets()
        {
            int? eventLabel = TargetEventLabel.TryResolveValue();
            if (eventLabel == null)
                return null;

            AsmSegment target = SourceSegment.Event.Module.ParentCollection.GetModuleByIndex(TargetModuleIndex)[(ushort)eventLabel].Segments[0];
            return new[] { target };
        }

        public override bool? ResolveCondition()
        {
            return true;
        }
    }
}