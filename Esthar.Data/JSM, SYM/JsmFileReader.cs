using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class JsmFileReader : GameFileReader
    {
        public JsmGroup[] Groups;
        public JsmScript[] Scripts;
        public JsmOperation[] Opertations;

        public JsmFileReader(Stream stream)
            : base(stream)
        {
        }

        public override void Close()
        {
            Groups = null;
            Scripts = null;
            Opertations = null;
        }

        public override void Open()
        {
            BinaryReader br = new BinaryReader(IOStream);

            JsmHeader header = IOStream.ReadStruct<JsmHeader>();

            int groupCount = (header.ScriptsOffset - 8) / 2;
            int scriptCount = (header.OperationsOffset - header.ScriptsOffset) / 2;
            int operationsCount = (int)((IOStream.Length - header.OperationsOffset) / 4);

            ushort index = 0;
            SortedList<ushort, JsmGroup> groups = new SortedList<ushort, JsmGroup>(groupCount);
            for (ushort i = 0; i < header.CountAreas; ++i)
            {
                JsmGroup group = ReadJsmGroup(br, index++, JsmModuleType.Area);
                groups.Add(group.Label, group);
            }
            for (ushort i = 0; i < header.CountDoors; ++i)
            {
                JsmGroup group = ReadJsmGroup(br, index++, JsmModuleType.Door);
                groups.Add(group.Label, group);
            }
            for (ushort i = 0; i < header.CountModules; ++i)
            {
                JsmGroup group = ReadJsmGroup(br, index++, JsmModuleType.Module);
                groups.Add(group.Label, group);
            }
            for (ushort i = 0; i < header.CountObjects; ++i)
            {
                JsmGroup group = ReadJsmGroup(br, index++, JsmModuleType.Object);
                groups.Add(group.Label, group);
            }

            JsmScript[] scripts = new JsmScript[scriptCount - 1];
            for (int i = 0; i < scriptCount; ++i)
            {
                ushort pos = br.ReadUInt16();
                bool flag = pos >> 15 == 1;
                pos &= 0x7FFF;
                if (i > 0)
                    scripts[i - 1].OperationsCount = (pos - scripts[i - 1].Position);
                
                if (i < scriptCount - 1)
                    scripts[i] = new JsmScript(pos, flag);
            }

            JsmOperation[] opertations = new JsmOperation[operationsCount];
            for (int i = 0; i < operationsCount; i++)
                opertations[i] = new JsmOperation(br.ReadUInt32());

            Groups = groups.Values.ToArray();
            Scripts = scripts;
            Opertations = opertations;
        }

        private JsmGroup ReadJsmGroup(BinaryReader br, ushort index, JsmModuleType type)
        {
            ushort group = br.ReadUInt16();
            ushort label = (ushort)(group >> 7);
            byte count = (byte)((group & 0x7F) + 1);

            return new JsmGroup(index, label, count, type);
        }

        public int GetMapId()
        {
            for (int i = 0; i < Opertations.Length; i++)
                if (Opertations[i].Command == JsmCommand.SETPLACE)
                    if (Opertations[i - 1].Command == JsmCommand.PSHN_L)
                        return Opertations[i - 1].Argument;
                    else
                        throw new Exception();

            return -1;
        }
    }
}