namespace Esthar.Data.Transform
{
    /// <summary>
    /// Opens a field message window and lets player choose a single line.
    /// ASK saves the chosen line index into a temp variable which you can retrieve with PSHI_L 0.
    /// AASK is an upgrade that also lets you set the window position. 
    /// </summary>
    public sealed class AskAC : AsmCommandOld
    {
        public AskAC(int offset, AsmEvent script)
            : base(JsmCommand.ASK, offset, script)
        {
            AsmCommandStackOld stackOld = GetCommandStackOld(6);
            MessageChanel = stackOld.Pop();
            MessageId = stackOld.Pop();
            FirstAnswerLine = stackOld.Pop();
            LastAnswerLine = stackOld.Pop();
            DefaultAnswerLine = stackOld.Pop();
            CancelAnswerLine = stackOld.Pop();
        }

        public override void CommitChanges()
        {
            AsmCommandStackOld stackOld = GetCommandStackOld(6);
            stackOld.Push(MessageChanel);
            stackOld.Push(MessageId);
            stackOld.Push(FirstAnswerLine);
            stackOld.Push(LastAnswerLine);
            stackOld.Push(DefaultAnswerLine);
            stackOld.Push(CancelAnswerLine);
        }

        public int MessageChanel { get; set; }
        public int MessageId { get; set; }

        public int FirstAnswerLine { get; set; }
        public int LastAnswerLine { get; set; }
        public int DefaultAnswerLine { get; set; }
        public int CancelAnswerLine { get; set; }
    }
}