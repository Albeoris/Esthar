namespace Esthar.Data.Transform
{
    public sealed class AsmSetPlaceCommand : AsmCommand
    {
        private const int StackSize = 1;

        public AsmValueSource TextId;

        public AsmSetPlaceCommand(AsmEvent script, int offset)
            : base(JsmCommand.SETPLACE, script, offset)
        {
            IAsmReadableCommandStack stack = GetReadableCommandStack(StackSize);
            TextId = stack.Pop();
        }

        public override void CommitChanges()
        {
            IAsmWriteableCommandStack stack = GetWriteableCommandStack(StackSize);
            stack.Push(TextId);
        }
    }
}