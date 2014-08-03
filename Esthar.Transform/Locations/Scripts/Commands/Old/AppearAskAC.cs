namespace Esthar.Data.Transform
{
    /// <summary>
    /// Opens a field message window and lets player choose a single line.
    /// AASK saves the chosen line index into a temp variable which you can retrieve with PSHI_L 0. 
    /// </summary>
    public sealed class AppearAskAC : AsmCommandOld
    {
        public AppearAskAC(int offset, AsmEvent script)
            : base(JsmCommand.AASK, offset, script)
        {
            AsmCommandStackOld stackOld = GetCommandStackOld(8);
            MessageChanel = stackOld.Pop();
            MessageId = stackOld.Pop();
            FirstAnswerLine = stackOld.Pop();
            LastAnswerLine = stackOld.Pop();
            DefaultAnswerLine = stackOld.Pop();
            CancelAnswerLine = stackOld.Pop();
            X = stackOld.Pop();
            Y = stackOld.Pop();
        }

        public override void CommitChanges()
        {
            AsmCommandStackOld stackOld = GetCommandStackOld(8);
            stackOld.Push(MessageChanel);
            stackOld.Push(MessageId);
            stackOld.Push(FirstAnswerLine);
            stackOld.Push(LastAnswerLine);
            stackOld.Push(DefaultAnswerLine);
            stackOld.Push(CancelAnswerLine);
            stackOld.Push(X);
            stackOld.Push(Y);
        }

        public int MessageChanel { get; set; }
        public int MessageId { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int FirstAnswerLine { get; set; }
        public int LastAnswerLine { get; set; }
        public int DefaultAnswerLine { get; set; }
        public int CancelAnswerLine { get; set; }
    }
}