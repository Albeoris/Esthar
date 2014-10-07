namespace Esthar.Data
{
    public sealed class JsmGroup
    {
        public readonly ushort ExecutionOrder;
        public readonly ushort Label;
        public readonly byte ScriptsCount;
        public readonly JsmModuleType Type;

        public JsmGroup(ushort executionOrder, ushort label, byte scriptsCount, JsmModuleType type)
        {
            ExecutionOrder = executionOrder;
            Label = label;
            ScriptsCount = scriptsCount;
            Type = type;
        }
    }
}