namespace Esthar.Data.Transform
{
    /// <summary>
    /// Popup a message window.
    /// This is usually used on lines to popup text when the player crosses a certain point on a screen.
    /// The size of the message window can be set with WINSIZE.
    /// </summary>
    public sealed class MessageAC : AsmCommandOld
    {
        public MessageAC(int offset, AsmEvent script)
            : base(JsmCommand.MES, offset, script)
        {
            AsmCommandStackOld stackOld = GetCommandStackOld(2);
            MessageChanel = stackOld.Pop();
            MessageId = stackOld.Pop();
        }

        public override void CommitChanges()
        {
            AsmCommandStackOld stackOld = GetCommandStackOld(2);
            stackOld.Push(MessageChanel);
            stackOld.Push(MessageId);
        }

        public int MessageChanel { get; set; }
        public int MessageId { get; set; }
    }
}
