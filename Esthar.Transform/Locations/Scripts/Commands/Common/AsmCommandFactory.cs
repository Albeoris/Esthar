using System;
using System.Collections.Generic;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class AsmCommandFactory
    {
        private readonly AsmEvent _event;
        private readonly int _offset;
        private readonly int _length;

        private readonly Dictionary<Type, Action<AsmCommand>> _createdHandlers = new Dictionary<Type, Action<AsmCommand>>();

        public AsmCommandFactory(AsmEvent evt)
        {
            _event = evt;
            _offset = 0;
            _length = evt.Count;
        }

        public AsmCommandFactory(AsmSegment segment)
        {
            _event = segment.Event;
            _offset = segment.Offset;
            _length = segment.Length;
        }

        public T TryCreate<T>(int offset, JsmCommand command) where T : AsmCommand
        {
            Action<AsmCommand> handler;
            T result = InternalTryCreate<T>(offset, command);
            if (result != null && _createdHandlers.TryGetValue(result.GetType(), out handler))
                handler(result);
            return result;
        }

        private T InternalTryCreate<T>(int offset, JsmCommand command) where T : AsmCommand
        {
            offset += _offset;

            if (_event[offset].Command != command)
                return null;

            switch (command)
            {
                case JsmCommand.SETPC:
                    return (T)(AsmCommand)new AsmSetCharacterCommand(_event, offset);
                case JsmCommand.SETPLACE:
                    return (T)(AsmCommand)new AsmSetPlaceCommand(_event, offset);
                case JsmCommand.ASK:
                    return (T)(AsmCommand)new AsmAskCommand(_event, offset);
                case JsmCommand.AASK:
                    return (T)(AsmCommand)new AsmAppearAskCommand(_event, offset);
                case JsmCommand.MES:
                    return (T)(AsmCommand)new AsmMessageCommand(_event, offset);
                case JsmCommand.AMES:
                    return (T)(AsmCommand)new AsmAppearMessageCommand(_event, offset);
                case JsmCommand.AMESW:
                    return (T)(AsmCommand)new AsmAppearMessageAndWaitCommand(_event, offset);
                case JsmCommand.RAMESW:
                    return (T)(AsmCommand)new AsmResumeScriptAppearMessageAndWaitCommand(_event, offset);
            }

            throw new NotImplementedException();
        }

        public T TryFind<T>(JsmCommand command) where T : AsmCommand
        {
            for (int i = 0; i < _length; i++)
            {
                T result = TryCreate<T>(i, command);
                if (result != null)
                    return result;
            }
            return null;
        }

        public List<AsmCommand> FindAll(params JsmCommand[] commands)
        {
            List<AsmCommand> result = new List<AsmCommand>();
            for (int i = 0; i < _length; i++)
            {
                foreach (JsmCommand command in commands)
                {
                    AsmCommand cmd = TryCreate<AsmCommand>(i, command);
                    if (cmd != null)
                    {
                        result.Add(cmd);
                        break;
                    }
                }
            }
            return result;
        }

        public void RegisterCreateHandler<T>(Action<T> handler) where T : AsmCommand
        {
            _createdHandlers.Add(TypeCache<T>.Type, command => handler((T)command));
        }

        public void RegisterCreateHandler<T>(Action<AsmCommand> handler) where T : AsmCommand
        {
            _createdHandlers.Add(TypeCache<T>.Type, handler);
        }
    }
}