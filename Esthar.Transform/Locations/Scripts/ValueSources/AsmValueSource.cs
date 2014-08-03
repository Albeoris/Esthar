using System;

namespace Esthar.Data.Transform
{
    public abstract class AsmValueSource
    {
        public readonly AsmValueSourceType Type;

        protected AsmValueSource(AsmValueSourceType type)
        {
            Type = type;
        }

        public virtual int? TryResolveValue()
        {
            return null;
        }

        public virtual int ResolveValue()
        {
            int? result = TryResolveValue();
            if (result == null)
                throw new NotSupportedException();
            return result.Value;
        }

        public virtual void SetAbsoluteValue(int value)
        {
            throw new NotSupportedException();
        }

        public static AsmValueSource Create(AsmSegment segment, int offset)
        {
            return Create(segment.Event, segment.Offset + offset);
        }

        public static AsmValueSource Create(AsmEvent evt, int offset)
        {
            AsmOperation operation = evt[offset];
            switch (operation.Command)
            {
                case JsmCommand.PSHN_L:
                    return new AsmStaticValueSource(operation);
                case JsmCommand.PSHI_L:
                case JsmCommand.PSHM_B:
                case JsmCommand.PSHM_W:
                case JsmCommand.PSHM_L:
                case JsmCommand.PSHSM_B:
                case JsmCommand.PSHSM_W:
                case JsmCommand.PSHSM_L:
                case JsmCommand.PSHAC:
                case JsmCommand.CAL:
                    return new AsmUnknownValueSource(evt, offset);
                default:
                    return new AsmUnknownValueSource(evt, offset);
            }
        }

        public abstract void Write(AsmEvent evt, int offset);
    }
}