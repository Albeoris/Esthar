using System.Linq;

namespace Esthar.Data.Transform
{
    public sealed class AsmRelativeRequestBinding : AsmBinding
    {
        public readonly int PartyMember;
        public readonly AsmValueSource TargetEventSource;

        public AsmRelativeRequestBinding(AsmSegment source, int sourceOffset, int partyMember, AsmValueSource targetEventSource)
            : base(AsmBindingType.RelativeRequest, source, sourceOffset)
        {
            PartyMember = partyMember;
            TargetEventSource = targetEventSource;
        }

        public override AsmSegment[] ResolveTargets()
        {
            int? value = TargetEventSource.TryResolveValue();
            if (value == null)
                return null;

            return SourceSegment.Event.Module.ParentCollection
                .GetOrderedModules()
                .Where(m => m.Type == AsmModuleType.Object && ((AsmObject)m).CharacterId != AsmCharacterId.None)
                .SelectMany(m => m.GetOrderedEvents().Skip(value.Value - 1).Take(1))
                .Select(e => e.Segments[0])
                .ToArray();
        }

        public override bool? ResolveCondition()
        {
            return true;
        }
    }
}