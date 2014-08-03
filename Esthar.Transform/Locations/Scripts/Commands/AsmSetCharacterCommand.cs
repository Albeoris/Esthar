namespace Esthar.Data.Transform
{
    public sealed class AsmSetCharacterCommand : AsmCommand
    {
        private const int StackSize = 1;

        public AsmValueSource CharactedId;

        public AsmSetCharacterCommand(AsmEvent script, int offset)
            : base(JsmCommand.SETPC, script, offset)
        {
            IAsmReadableCommandStack stack = GetReadableCommandStack(StackSize);
            CharactedId = stack.Pop();
        }

        public AsmCharacterId TryResolveCharacterId()
        {
            return (AsmCharacterId)(CharactedId.TryResolveValue() ?? (int)AsmCharacterId.Unknown);
        }

        public override void CommitChanges()
        {
            IAsmWriteableCommandStack stack = GetWriteableCommandStack(StackSize);
            stack.Push(CharactedId);
        }
    }
}