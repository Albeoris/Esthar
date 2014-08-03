namespace Esthar.Data.Transform
{
    /// <summary>
    /// Pop up a message window and pauses script execution until the player dismisses the window. 
    /// </summary>
    public sealed class AppearMessageAC : AsmCommandOld
    {
        public AppearMessageAC(int offset, AsmEvent script)
            : base(JsmCommand.AMES, offset, script)
        {
            AsmCommandStackOld stackOld = GetCommandStackOld(4);
            MessageChanel = stackOld.Pop();
            MessageId = stackOld.Pop();
            X = stackOld.Pop();
            Y = stackOld.Pop();
        }

        public override void CommitChanges()
        {
            AsmCommandStackOld stackOld = GetCommandStackOld(4);
            stackOld.Push(MessageChanel);
            stackOld.Push(MessageId);
            stackOld.Push(X);
            stackOld.Push(Y);
        }

        public int MessageChanel { get; set; }
        public int MessageId { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
    }
}