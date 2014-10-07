using System;
using System.Collections.Generic;

namespace Esthar.Data.Transform
{
    public sealed class AsmEvent : List<JsmOperation>
    {
        public ushort Index;
        public ushort Label;
        public string Title;
        public bool Flag;

        public AsmEvent(AsmModule module)
        {
            Module = module;
        }

        public AsmEvent(AsmModule module, int capacity)
            : base(capacity)
        {
            Module = module;
        }

        public AsmEvent(AsmModule module, IEnumerable<JsmOperation> operations)
            : base(operations)
        {
            Module = module;
        }

        public AsmModule Module;
        public AsmSegments Segments;

        public JsmOperation GetOperation(JsmCommand command, int offset)
        {
            JsmOperation operation = this[offset];
            if (operation.Command != command)
                throw new ArgumentException("offset");
            return operation;
        }

        public int GetOperationArgument(JsmCommand command, int offset)
        {
            JsmOperation operation = GetOperation(command, offset);
            if (!operation.HasArgument)
                throw new ArgumentException("command");
            return operation.Argument;
        }

        public JsmOperation GetOperation(int offset)
        {
            return this[offset];
        }

        public int GetOperationArgument(int offset)
        {
            JsmOperation operation = GetOperation(offset);
            if (!operation.HasArgument)
                throw new ArgumentException("offset");

            switch (operation.Command)
            {
                case JsmCommand.PSHN_L:
                    return operation.Argument;
            }

            throw new NotImplementedException();
            return operation.Argument;
        }

        public void SetOperationArgument(int offset, int value)
        {
            JsmOperation operation = GetOperation(offset);
            if (!operation.HasArgument)
                throw new ArgumentException("offset");

            switch (operation.Command)
            {
                case JsmCommand.PSHN_L:
                    operation.Argument = value;
                    return;
            }

            throw new NotImplementedException();
            operation.Argument = value;
        }

        public IAsmReadableCommandStack GetReadableCommandStack(int offset, int size)
        {
            return new AsmCommandStack(this, offset - size);
        }

        public IAsmWriteableCommandStack GetWriteableCommandStack(int offset, int size)
        {
            return new AsmCommandStack(this, offset - size);
        }
    }
}