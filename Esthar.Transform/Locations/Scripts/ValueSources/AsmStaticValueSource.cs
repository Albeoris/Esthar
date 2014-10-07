namespace Esthar.Data.Transform
{
    public sealed class AsmStaticValueSource : AsmValueSource
    {
        private readonly JsmOperation _operation;

        public AsmStaticValueSource(JsmOperation operation)
            : base(AsmValueSourceType.Static)
        {
            _operation = operation;
        }

        public override int? TryResolveValue()
        {
            return _operation.Argument;
        }

        public override void SetAbsoluteValue(int value)
        {
            _operation.Argument = value;
        }

        public override void Write(AsmEvent evt, int offset)
        {
            evt[offset] = _operation;
        }
    }
}