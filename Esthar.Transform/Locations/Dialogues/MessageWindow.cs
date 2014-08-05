using System;
using System.Windows;
using System.Windows.Media;

namespace Esthar.Data.Transform
{
    public sealed class MessageWindow
    {
        #region Static

        public static MessageWindow FromCommand(AsmCommand command)
        {
            switch (command.NativeOperation.Command)
            {
                case JsmCommand.MES: return FromCommand((AsmMessageCommand)command);
                case JsmCommand.AMES: return FromCommand((AsmAppearMessageCommand)command);
                case JsmCommand.AMESW: return FromCommand((AsmAppearMessageAndWaitCommand)command);
                case JsmCommand.RAMESW: return FromCommand((AsmResumeScriptAppearMessageAndWaitCommand)command);
                case JsmCommand.ASK: return FromCommand((AsmAskCommand)command);
                case JsmCommand.AASK: return FromCommand((AsmAppearAskCommand)command);
                default: throw new NotImplementedException();
            }
        }

        public static MessageWindow FromCommand(AsmMessageCommand command)
        {
            return new MessageWindow(command.MessageChanel, command.MessageId);
        }

        public static MessageWindow FromCommand(AsmAppearMessageCommand command)
        {
            return new MessageWindow(command.MessageChanel, command.MessageId, command.X, command.Y);
        }

        public static MessageWindow FromCommand(AsmAppearMessageAndWaitCommand command)
        {
            return new MessageWindow(command.MessageChanel, command.MessageId, command.X, command.Y);
        }

        public static MessageWindow FromCommand(AsmResumeScriptAppearMessageAndWaitCommand command)
        {
            return new MessageWindow(command.MessageChanel, command.MessageId, command.X, command.Y);
        }

        public static MessageWindow FromCommand(AsmAskCommand command)
        {
            return new MessageWindow(command.MessageChanel, command.MessageId, null, null,
                command.FirstAnswerLine, command.LastAnswerLine, command.DefaultAnswerLine, command.CancelAnswerLine);
        }

        public static MessageWindow FromCommand(AsmAppearAskCommand command)
        {
            return new MessageWindow(command.MessageChanel, command.MessageId, command.X, command.Y,
                command.FirstAnswerLine, command.LastAnswerLine, command.DefaultAnswerLine, command.CancelAnswerLine);
        }

        #endregion

        private MessageWindow(AsmValueSource messageChanel, AsmValueSource messageId,
            AsmValueSource x = null, AsmValueSource y = null,
            AsmValueSource firstAnswerLine = null, AsmValueSource lastAnswerLine = null,
            AsmValueSource defaultAnswerLine = null, AsmValueSource cancelAnswerLine = null)
        {
            _messageChanel = messageChanel;
            _messageId = messageId;

            if (x != null && y != null)
            {
                _x = x;
                _y = y;
                IsMovable = true;
                IsDynamic = IsDynamic || (x.Type != AsmValueSourceType.Static || y.Type != AsmValueSourceType.Static);
            }

            if (firstAnswerLine != null && lastAnswerLine != null && defaultAnswerLine != null && cancelAnswerLine != null)
            {
                _firstAnswerLine = firstAnswerLine;
                _lastAnswerLine = lastAnswerLine;
                _defaultAnswerLine = defaultAnswerLine;
                _cancelAnswerLine = cancelAnswerLine;
                IsQuestion = true;
            }
        }

        private const int DefaultX = 160;
        private const int DefaultY = 112;

        private readonly AsmValueSource _messageChanel;
        private readonly AsmValueSource _messageId;
        private readonly AsmValueSource _x;
        private readonly AsmValueSource _y;
        private readonly AsmValueSource _firstAnswerLine;
        private readonly AsmValueSource _lastAnswerLine;
        private readonly AsmValueSource _defaultAnswerLine;
        private readonly AsmValueSource _cancelAnswerLine;

        public bool IsDynamic { get; private set; }
        public bool IsMovable { get; private set; }
        public bool IsQuestion { get; private set; }

        public int MessageChanel
        {
            get { return _messageChanel.ResolveValue(); }
        }

        public int MessageId
        {
            get { return _messageId.ResolveValue(); }
        }

        public LocalizableString Message { get; set; }

        public int Order
        {
            get { return Message.Order; }
            set { Message.Order = value; }
        }

        public bool IsIndent
        {
            get { return Message.IsIndent; }
            set { Message.IsIndent = value; }
        }

        public int X
        {
            get { return IsMovable && !IsDynamic ? _x.ResolveValue() : DefaultX; }
            set
            {
                if (!IsMovable || IsDynamic) throw new InvalidOperationException();
                _x.SetAbsoluteValue(value);
            }
        }

        public int Y
        {
            get { return IsMovable && !IsDynamic ? _y.ResolveValue() : DefaultY; }
            set
            {
                if (!IsMovable || IsDynamic) throw new InvalidOperationException();
                _y.SetAbsoluteValue(value);
            }
        }

        public int FirstAnswerLine
        {
            get { return IsQuestion ? _firstAnswerLine.ResolveValue() : -1; }
            set
            {
                if (!IsQuestion) throw new InvalidOperationException();
                _firstAnswerLine.SetAbsoluteValue(value);
            }
        }

        public int LastAnswerLine
        {
            get { return IsQuestion ? _lastAnswerLine.ResolveValue() : -1; }
            set
            {
                if (!IsQuestion) throw new InvalidOperationException();
                _lastAnswerLine.SetAbsoluteValue(value);
            }
        }

        public int DefaultAnswerLine
        {
            get { return IsQuestion ? _defaultAnswerLine.ResolveValue() : -1; }
            set
            {
                if (!IsQuestion) throw new InvalidOperationException();
                _defaultAnswerLine.SetAbsoluteValue(value);
            }
        }

        public int CancelAnswerLine
        {
            get { return IsQuestion ? _cancelAnswerLine.ResolveValue() : -1; }
            set
            {
                if (!IsQuestion) throw new InvalidOperationException();
                _cancelAnswerLine.SetAbsoluteValue(value);
            }
        }

        public string DisplayName
        {
            get { return Message.Current.Substring(0, Math.Min(Message.Current.Length, 25)) + "..."; }
        }

        public Brush DisplayBrush
        {
            get { return Message.Current == Message.Original ? Brushes.Black : Brushes.ForestGreen; }
        }

        public Thickness DisplayMargin
        {
            get { return IsIndent ? new Thickness(0, 16, 0, 0) : new Thickness(); }
        }
    }
}