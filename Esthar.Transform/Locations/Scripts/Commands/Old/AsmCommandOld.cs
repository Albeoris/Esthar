using System;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public abstract class AsmCommandOld
    {
        protected readonly AsmEvent Script;
        protected readonly int Offset;

        public JsmOperation Operation { get; private set; }
        public abstract void CommitChanges();

        protected AsmCommandOld(JsmCommand command, int offset, AsmEvent script)
        {
            if (offset < 0 || offset >= script.Count)
                throw new ArgumentOutOfRangeException("offset");

            Script = Exceptions.CheckArgumentNull(script, "script");
            Offset = offset;

            Operation = Script.GetOperation(command, offset);
        }

        protected AsmCommandStackOld GetCommandStackOld(int size)
        {
            return new AsmCommandStackOld(Script, Offset - size);
        }

        protected sealed class AsmCommandStackOld
        {
            private readonly AsmEvent _script;
            private int _offset;

            public AsmCommandStackOld(AsmEvent script, int offset)
            {
                _script = script;
                _offset = offset;
            }

            public void Push(int value)
            {
                _script.SetOperationArgument(_offset++, value);
            }

            public int Pop()
            {
                return _script.GetOperationArgument(_offset++);
            }
        }
    }
}