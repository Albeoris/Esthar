namespace Esthar.Data.Transform
{
    public sealed class AsmUnknownValueSource : AsmValueSource
    {
        private readonly AsmEvent _event;
        private readonly int _offset;

        public AsmUnknownValueSource(AsmEvent evt, int offset) : base(AsmValueSourceType.Unknown)
        {
            _event = evt;
            _offset = offset;
        }

        public override void Write(AsmEvent evt, int offset)
        {
            evt[offset] = _event[_offset];
        }
    }
}