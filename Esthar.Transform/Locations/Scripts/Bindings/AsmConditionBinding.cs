namespace Esthar.Data.Transform
{
    public sealed class AsmConditionBinding : AsmBinding
    {
        public readonly AsmSegment TargetSegment;
        public readonly AsmValueSource ConditionSource;

        public AsmConditionBinding(AsmSegment source, AsmSegment target, AsmValueSource conditionSource, bool isTrue)
            : base(isTrue ? AsmBindingType.ConditionTrue : AsmBindingType.ConditionFalse, source, source.Length - 1)
        {
            TargetSegment = target;
            ConditionSource = conditionSource;
        }

        public override AsmSegment[] ResolveTargets()
        {
            return new[] { TargetSegment };
        }

        public override bool? ResolveCondition()
        {
            int? value = ConditionSource.TryResolveValue();
            if (value == null)
                return null;

            return Type == AsmBindingType.ConditionTrue ? value == 1 : value == 0;
        }
    }
}