using System;
using System.Collections.Generic;

namespace Esthar.Data.Transform
{
    public sealed class AsmEvent : List<AsmOperation>
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

        public AsmEvent(AsmModule module, IEnumerable<AsmOperation> operations)
            : base(operations)
        {
            Module = module;
        }

        public AsmModule Module;
        public AsmSegments Segments;

        public AsmOperation GetOperation(JsmCommand command, int offset)
        {
            AsmOperation operation = this[offset];
            if (operation.Command != command)
                throw new ArgumentException("offset");
            return operation;
        }

        public int GetOperationArgument(JsmCommand command, int offset)
        {
            AsmOperation operation = GetOperation(command, offset);
            if (operation.Argument == null)
                throw new ArgumentException("command");
            return operation.Argument.Value;
        }

        public AsmOperation GetOperation(int offset)
        {
            return this[offset];
        }

        public int GetOperationArgument(int offset)
        {
            AsmOperation operation = GetOperation(offset);
            if (operation.Argument == null)
                throw new ArgumentException("offset");

            switch (operation.Command)
            {
                case JsmCommand.PSHN_L:
                    return operation.Argument.Value;
            }
            return operation.Argument.Value;
        }

        public void SetOperationArgument(int offset, int value)
        {
            AsmOperation operation = GetOperation(offset);
            if (operation.Argument == null)
                throw new ArgumentException("offset");

            switch (operation.Command)
            {
                case JsmCommand.PSHN_L:
                    operation.Argument = value;
                    break;
            }

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