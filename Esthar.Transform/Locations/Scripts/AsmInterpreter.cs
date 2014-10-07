using System;
using System.Collections.Generic;
using System.Linq;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class AsmInterpreter
    {
        private readonly AsmCollection _scripts;
        private readonly Stack<int> _stack = new Stack<int>(128);

        public AsmInterpreter(AsmCollection scripts)
        {
            _scripts = Exceptions.CheckArgumentNull(scripts, "scripts");
        }

        public void BindSegments()
        {
            CollectSegmentsData();
//            CreateExternalBindings();
        }

        private void CollectSegmentsData()
        {
            foreach (AsmModule module in _scripts.GetOrderedModules())
            {
                Construct(module);
                foreach (AsmEvent evt in module.GetOrderedEvents())
                {
                    List<int> jumpFrom, jumpTo;
                    GetJumps(evt, out jumpFrom, out jumpTo);

                    CreateSegmentsByJumps(evt, jumpFrom, jumpTo);
                    CreateInternalBindings(evt.Segments, jumpFrom, jumpTo);
                }
            }
        }

        private void Construct(AsmModule module)
        {
            AsmCommandFactory factory = new AsmCommandFactory(module.Construct);
            switch (module.Type)
            {
                case JsmModuleType.Object:
                {
                    AsmSetCharacterCommand cmd = factory.TryFind<AsmSetCharacterCommand>(JsmCommand.SETPC);
                    if (cmd != null) ((AsmObject)module).CharacterId = cmd.TryResolveCharacterId();
                    break;
                }
            }
        }

        private void GetJumps(AsmEvent evt, out List<int> jumpFrom, out List<int> jumpTo)
        {
            jumpFrom = new List<int>();
            jumpTo = new List<int>();

            for (int i = 0; i < evt.Count; i++)
            {
                JsmOperation opetation = evt[i];
                switch (opetation.Command)
                {
                    case JsmCommand.JMP:
                    case JsmCommand.JPF:
                        jumpFrom.Add(i);
                        jumpTo.Add(Math.Max(0, i + opetation.Argument)); // JMP -5479
                        break;
                }
            }
        }

        private void CreateSegmentsByJumps(AsmEvent evt, List<int> jumpFrom, List<int> jumpTo)
        {
            jumpFrom = jumpFrom.Order().ToList();
            jumpTo = jumpTo.Order().ToList();

            AsmSegments segments = new AsmSegments(jumpFrom.Count);
            if (segments.Capacity == 0)
            {
                segments.Add(new AsmSegment(evt, 0, evt.Count));
                evt.Segments = segments;
                return;
            }

            int left = 0;
            int fromIndex = 0;
            int toIndex = 0;
            while (true)
            {
                int right;
                if (fromIndex < jumpFrom.Count)
                {
                    if (toIndex < jumpTo.Count)
                    {
                        int fromValue = jumpFrom[fromIndex];
                        int toValue = jumpTo[toIndex];
                        if (fromValue < toValue)
                        {
                            right = fromValue;
                            fromIndex++;
                        }
                        else if (toValue < fromValue)
                        {
                            right = toValue - 1;
                            toIndex++;
                        }
                        else
                        {
                            right = toValue - 1;
                            toIndex++;
                        }
                    }
                    else
                    {
                        right = jumpFrom[fromIndex++];
                    }
                }
                else if (toIndex < jumpTo.Count)
                {
                    right = jumpTo[toIndex++];
                    if (left == right)
                        continue;
                }
                else
                {
                    if (left < evt.Count)
                        segments.Add(new AsmSegment(evt, left, evt.Count - left));
                    break;
                }

                int length = right - left + 1;
                if (length != 0)
                    segments.Add(new AsmSegment(evt, left, length));
                left = right + 1;
            }

            evt.Segments = segments;
        }

        private void CreateInternalBindings(AsmSegments segments, List<int> jumpFrom, List<int> jumpTo)
        {
            for (int s = 0; s < segments.Count - 1; s++)
            {
                AsmSegment source = segments[s];
                AsmSegment target = segments[s + 1];
                JsmOperation last = source[source.Length - 1];
                if (last.Command == JsmCommand.JPF)
                {
                    AsmConditionBinding binding = new AsmConditionBinding(source, target, AsmValueSource.Create(source, source.Length - 2), true);
                    source.OutputBindings.Add(binding);
                    target.InputBindings.Add(binding);
                }
                else if (last.Command != JsmCommand.JMP)
                {
                    AsmHardlinkBinding binding = new AsmHardlinkBinding(source, target);
                    source.OutputBindings.Add(binding);
                    target.InputBindings.Add(binding);
                }
            }

            for (int i = 0; i < jumpFrom.Count; i++)
            {
                int fromIndex = jumpFrom[i];
                int toIndex = jumpTo[i];

                AsmSegment source = segments.GetSegmentByOffset(fromIndex);
                AsmSegment target = segments.GetSegmentByOffset(toIndex);

                JsmOperation last = source[source.Length - 1];
                if (last.Command == JsmCommand.JMP)
                {
                    AsmHardlinkBinding binding = new AsmHardlinkBinding(source, target);
                    source.OutputBindings.Add(binding);
                    target.InputBindings.Add(binding);
                }
                else if (last.Command == JsmCommand.JPF)
                {
                    AsmConditionBinding binding = new AsmConditionBinding(source, target, AsmValueSource.Create(source, source.Length - 2), false);
                    source.OutputBindings.Add(binding);
                    target.InputBindings.Add(binding);
                }
            }
        }

        private void CreateExternalBindings()
        {
            foreach (AsmEvent evt in _scripts.GetOrderedModules().SelectMany(module => module.GetOrderedEvents()))
            {
                for (int i = 0; i < evt.Count; i++)
                {
                    JsmOperation operation = evt[i];
                    switch (operation.Command)
                    {
                        case JsmCommand.REQ:
                        case JsmCommand.REQEW:
                        case JsmCommand.REQSW:
                        {
                            AsmSegment source = evt.Segments.GetSegmentByOffset(i);
                            AsmValueSource labelTargetEvent = AsmValueSource.Create(evt, i - 1);
                            AsmAbsoluteRequestBinding binding = new AsmAbsoluteRequestBinding(source, i - source.Offset, operation.Argument, labelTargetEvent);
                            source.OutputBindings.Add(binding);
                            AsmSegment[] targets = binding.ResolveTargets();
                            if (targets != null)
                                foreach (AsmSegment target in targets)
                                    target.InputBindings.Add(binding);
                            break;
                        }

                        case JsmCommand.PREQ:
                        case JsmCommand.PREQEW:
                        case JsmCommand.PREQSW:
                        {
                            AsmSegment source = evt.Segments.GetSegmentByOffset(i);
                            AsmValueSource labelTargetEvent = AsmValueSource.Create(evt, i - 1);
                            AsmRelativeRequestBinding binding = new AsmRelativeRequestBinding(source, i - source.Offset, operation.Argument, labelTargetEvent);
                            source.OutputBindings.Add(binding);
                            AsmSegment[] targets = binding.ResolveTargets();
                            if (targets != null)
                                foreach (AsmSegment target in targets)
                                    target.InputBindings.Add(binding);
                            break;
                        }
                    }
                }
            }
        }
    }
}