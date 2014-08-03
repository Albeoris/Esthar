namespace Esthar.Data.Transform
{
    /// <summary>
    /// Изменяет название локации.
    /// </summary>
    public sealed class SetPlaceAC : AsmCommandOld
    {
        public SetPlaceAC(int offset, AsmEvent script)
            : base(JsmCommand.SETPLACE, offset, script)
        {
            AsmCommandStackOld stackOld = GetCommandStackOld(1);
            TextId = stackOld.Pop();
        }

        public override void CommitChanges()
        {
            AsmCommandStackOld stackOld = GetCommandStackOld(1);
            stackOld.Push(TextId);
        }

        public int TextId { get; set; }
    }
}