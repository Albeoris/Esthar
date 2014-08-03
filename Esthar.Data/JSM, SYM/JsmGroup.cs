namespace Esthar.Data
{
    public sealed class JsmGroup
    {
        public readonly ushort ExecutionOrder;
        public readonly ushort Label;
        public readonly byte ScriptsCount;
        public readonly AsmModuleType Type;

        public JsmGroup(ushort executionOrder, ushort label, byte scriptsCount, AsmModuleType type)
        {
            ExecutionOrder = executionOrder;
            Label = label;
            ScriptsCount = scriptsCount;
            Type = type;
        }
    }
}