using System;

namespace Esthar.Data.Transform
{
    public interface IAsmReadableCommandStack
    {
        AsmValueSource Pop();
    }

    public interface IAsmWriteableCommandStack
    {
        void Push(AsmValueSource source);
    }

    public sealed class AsmCommandStack : IAsmReadableCommandStack, IAsmWriteableCommandStack
    {
        private readonly AsmEvent _script;
        private int _offset;

        public AsmCommandStack(AsmEvent script, int offset)
        {
            _script = script;
            _offset = offset;
        }

        void IAsmWriteableCommandStack.Push(AsmValueSource source)
        {
            source.Write(_script, _offset++);
        }

        AsmValueSource IAsmReadableCommandStack.Pop()
        {
            return AsmValueSource.Create(_script, _offset++);
        }
    }
}