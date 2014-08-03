using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class ArchiveInformationAccessor
    {
        private static readonly Guid MagicNumber = new Guid("3BAC2A90-85A6-429F-AACD-A6BA0B520A6F");

        private readonly FileInfo _fileInfo;

        public ArchiveInformationAccessor(string path)
        {
            Exceptions.CheckFileNotFoundException(path);

            _fileInfo = new FileInfo(path);
        }

        public ArchiveInformation Create()
        {
            _fileInfo.Refresh();

            return new ArchiveInformation(_fileInfo.FullName, _fileInfo.Length, _fileInfo.LastWriteTimeUtc, false);
        }

        public ArchiveInformation Read()
        {
            long position = FindPosition();
            if (position < 0)
                return null;

            using (FileStream fs = new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                fs.Seek(position, SeekOrigin.Begin);

                BinaryFormatter bf = new BinaryFormatter();
                ArchiveInformation info = (ArchiveInformation)bf.Deserialize(fs);
                info.FileSize = _fileInfo.Length;

                if (!Check(info, fs))
                    return null;

                return info;
            }
        }

        public ArchiveInformation ReadOrCreate()
        {
            try
            {
                return Read() ?? Create();
            }
            catch
            {
                return Create();
            }
        }

        public void Write(ArchiveInformation info)
        {
            _fileInfo.Refresh();
            info.FileTime = _fileInfo.LastWriteTimeUtc;

            long position = FindPosition();

            using (FileStream fs = new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                if (position < 0)
                    position = _fileInfo.Length;

                fs.Seek(position, SeekOrigin.Begin);

                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, info);

                bw.Write((int)(fs.Position + 24));
                bw.Write((int)(fs.Position - position));
                bw.Write(MagicNumber);

                fs.SetLength(fs.Position);
            }
            
            info.FileSize = _fileInfo.Length;
            _fileInfo.LastWriteTimeUtc = info.FileTime;
        }

        private bool Check(ArchiveInformation info, FileStream fs)
        {
            _fileInfo.Refresh();

            BinaryReader br = fs.GetBinaryReader();
            fs.Seek(-24, SeekOrigin.End);
            int oldSize = br.ReadInt32();
            if (oldSize != _fileInfo.Length)
                return false;

            if (_fileInfo.LastWriteTimeUtc != info.FileTime)
                return false;
            if (String.Compare(Path.GetFullPath(_fileInfo.FullName), Path.GetFullPath(info.ContentFilePath), StringComparison.InvariantCultureIgnoreCase) != 0)
                return false;

            return true;
        }

        private long FindPosition()
        {
            using (FileStream fs = new FileStream(_fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (BinaryReader br = new BinaryReader(fs))
            {
                if (fs.Length < 20)
                    return -1;

                fs.Seek(-20, SeekOrigin.End);

                int length = br.ReadInt32();

                if (br.ReadGuid() != MagicNumber)
                    return -1;

                return Math.Max(-1, _fileInfo.Length - length - 20);
            }
        }
    }
}