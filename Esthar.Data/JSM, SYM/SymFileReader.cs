using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class SymFileReader : GameFileReader
    {
        private static readonly string[] Splitter = { "::" };

        public string[] Labels;
        //public string[] GroupLabels;
        //public string[][] ScriptLabels;
        //public List<string> ScriptLabelsList = new List<string>();

        public SymFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);
        }

        public override void Open()
        {
            Close();

            if (IOStream.IsEndOfStream())
                return;

            var list = new List<string>(128);
           
            StreamReader sr = IOStream.GetStreamReader();
            
            // Пропускаем заголовок
            string line = sr.ReadLine();
            while (!sr.EndOfStream)
            {
                if (line != sr.ReadLine())
                    continue;

                list.Add(line.TrimEnd(' '));
                break;
            }

            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                if (string.IsNullOrEmpty(line))
                    continue;

                string[] parts = line.TrimEnd(' ').Split(Splitter, 2, StringSplitOptions.RemoveEmptyEntries);
                list.Add(parts.Length == 1 ? parts[0] : parts[1]);
            }
            
            Labels = list.ToArray();
        }
    }
}