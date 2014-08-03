using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Threading;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class ArchiveOptimizer
    {
        private readonly ArchiveInformation _info;
        
        public event EventHandler<ProgressArgs> Progress;

        private int _totalProgress, _processedSize;

        public ArchiveOptimizer(ArchiveInformation info)
        {
            Exceptions.CheckArgumentNull(info, "info");
            
            _info = info;
        }

        public ArchiveInformation Optimize()
        {
            ArchiveInformation targetInfo;
            ArchiveAnalizer analizer = new ArchiveAnalizer(_info);
            analizer.ReadNativeInformation();

            using (MemoryMappedFile memoryFile = GetSourceMemoryMappedFile())
            {
                analizer.AnalizeUncompressedArchive(memoryFile);
                targetInfo = CreateTargetArchiveInformation();

                // Правка смещений
                CorrectOffsets(memoryFile, _info.RootArchive.Childs.OfType<ArchiveArchiveEntry>().ToArray(), targetInfo.RootArchive.Childs.OfType<ArchiveArchiveEntry>().ToArray());

                WriteMetrics(targetInfo);
                using (Stream output = new FileStream(targetInfo.ContentFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    WriteContent(memoryFile, output, _info, targetInfo);
            }

            targetInfo.IsOptimized = true;
            targetInfo.MoveFiles(_info);

            ArchiveInformationAccessor accessor = new ArchiveInformationAccessor(targetInfo.ContentFilePath);
            accessor.Write(targetInfo);

            return targetInfo;
        }

        private MemoryMappedFile GetSourceMemoryMappedFile()
        {
            if (_info.RootArchive.IsUncompressed())
            {
                _totalProgress = (int)_info.FileSize;
                return MemoryMappedFile.CreateFromFile(_info.ContentFilePath, FileMode.Open);
            }

            using (FileStream input = new FileStream(_info.ContentFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                return UncompressSourceFile(input);
        }

        private MemoryMappedFile UncompressSourceFile(FileStream input)
        {
            int uncompressedSize = ArchiveUncompressor.CalcUncompressedSize(_info);
            _totalProgress = uncompressedSize * 2;
            MemoryMappedFile memoryFile = MemoryMappedFile.CreateNew(Guid.NewGuid().ToString(), uncompressedSize);
            try
            {
                using (MemoryMappedViewStream memory = memoryFile.CreateViewStream())
                {
                    ArchiveUncompressor uncompressor = new ArchiveUncompressor(_info, input, memory);
                    uncompressor.Progress += (s, e) => Progress.NullSafeInvoke(s, new ProgressArgs(_info.ContentFilePath, Interlocked.Add(ref _processedSize, (int)e.ProcessedSize), _totalProgress));
                    uncompressor.Uncompress();
                }
            }
            catch
            {
                memoryFile.Dispose();
                throw;
            }
            return memoryFile;
        }

        private void CorrectOffsets(MemoryMappedFile memoryFile, ArchiveArchiveEntry[] sourceArchives, ArchiveArchiveEntry[] targetArchives)
        {
            for (int i = 0; i < sourceArchives.Length; i++)
            {
                ArchiveArchiveEntry source = sourceArchives[i];
                ArchiveArchiveEntry target = targetArchives[i];

                if (source.Name != target.Name)
                    throw new Exception("Порядок элементов в иерархиях изменился.");

                using (MemoryMappedViewStream memory = memoryFile.CreateViewStream(source.MetricsEntry.GetAbsoluteOffset(), source.MetricsEntry.UncompressedContentSize))
                using (BinaryWriter bw = new BinaryWriter(memory))
                {
                    foreach (ArchiveFileEntry file in target.Childs.OfType<ArchiveFileEntry>())
                    {
                        bw.Write(file.UncompressedContentSize);
                        bw.Write(file.ContentOffset);
                        bw.Write((int)Compression.None);
                    }

                    ArchiveArchiveEntry[] childSourceArrays = source.Childs.OfType<ArchiveArchiveEntry>().ToArray();
                    ArchiveArchiveEntry[] childTargetArrays = target.Childs.OfType<ArchiveArchiveEntry>().ToArray();

                    foreach (ArchiveFileEntry file in childTargetArrays.Select(a => a.MetricsEntry))
                    {
                        bw.Write(file.UncompressedContentSize);
                        bw.Write(file.ContentOffset);
                        bw.Write((int)Compression.None);
                    }
                    foreach (ArchiveFileEntry file in childTargetArrays.Select(a => a.ListingEntry))
                    {
                        bw.Write(file.UncompressedContentSize);
                        bw.Write(file.ContentOffset);
                        bw.Write((int)Compression.None);
                    }
                    foreach (ArchiveFileEntry file in childTargetArrays.Select(a => a.ContentEntry))
                    {
                        bw.Write(file.UncompressedContentSize);
                        bw.Write(file.ContentOffset);
                        bw.Write((int)Compression.None);
                    }

                    CorrectOffsets(memoryFile, childSourceArrays, childTargetArrays);
                }
            }
        }

        private ArchiveInformation CreateTargetArchiveInformation()
        {
            ArchiveInformation result = _info.GetTempArchiveInformation();

            foreach (ArchiveFileEntry oldChild in _info.RootArchive.Childs.OfType<ArchiveFileEntry>())
                MoveFile(oldChild, result.RootArchive, result.RootDirectory);

            MoveArchive(_info.RootArchive, result.RootArchive, result.RootDirectory);
            return result;
        }

        private void MoveArchive(ArchiveArchiveEntry sourceArchive, ArchiveArchiveEntry targetArchvie, ArchiveDirectoryEntry rootDirectory)
        {
            ArchiveArchiveEntry[] oldChildArchives = sourceArchive.Childs.OfType<ArchiveArchiveEntry>().ToArray();
            ArchiveArchiveEntry[] archiveEntries = MoveArchive(oldChildArchives, targetArchvie);
            ArchiveFileEntry[] metricsEntries = MoveMetrics(oldChildArchives, archiveEntries, targetArchvie);
            ArchiveFileEntry[] listingEntries = MoveListing(oldChildArchives, archiveEntries, targetArchvie);
            MoveContent(oldChildArchives, archiveEntries, metricsEntries, listingEntries, targetArchvie, rootDirectory);
        }

        private ArchiveArchiveEntry[] MoveArchive(ArchiveArchiveEntry[] oldArchives, ArchiveArchiveEntry newParent)
        {
            List<ArchiveArchiveEntry> result = new List<ArchiveArchiveEntry>(oldArchives.Length);
            foreach (ArchiveArchiveEntry oldArchive in oldArchives)
            {
                ArchiveArchiveEntry newArchive = new ArchiveArchiveEntry(oldArchive.Name);
                newParent.AddEntry(newArchive);
                result.Add(newArchive);
            }
            return result.ToArray();
        }

        private ArchiveFileEntry[] MoveMetrics(ArchiveArchiveEntry[] oldArchives, ArchiveArchiveEntry[] newArchives, ArchiveArchiveEntry newParent)
        {
            List<ArchiveFileEntry> result = new List<ArchiveFileEntry>(oldArchives.Length);
            for (int i = 0; i < oldArchives.Length; i++)
            {
                ArchiveArchiveEntry oldArchive = oldArchives[i];
                ArchiveArchiveEntry newArchive = newArchives[i];

                ArchiveFileEntry newMetric = oldArchive.MetricsEntry.Clone();
                {
                    newMetric.Compression = Compression.None;
                    newMetric.ContentOffset = newParent.CurrentPosition;
                    newArchive.MetricsEntry = newMetric;

                    SetCapacity(newMetric);
                    newParent.CurrentPosition += newMetric.ContentCapacity;
                }
                result.Add(newMetric);
            }
            return result.ToArray();
        }

        private ArchiveFileEntry[] MoveListing(ArchiveArchiveEntry[] oldArchives, ArchiveArchiveEntry[] newArchives, ArchiveArchiveEntry newParent)
        {
            List<ArchiveFileEntry> result = new List<ArchiveFileEntry>(oldArchives.Length);
            for (int i = 0; i < oldArchives.Length; i++)
            {
                ArchiveArchiveEntry oldArchive = oldArchives[i];
                ArchiveArchiveEntry newArchive = newArchives[i];

                ArchiveFileEntry newListing = oldArchive.ListingEntry.Clone();
                {
                    newListing.Compression = Compression.None;
                    newListing.ContentOffset = newParent.CurrentPosition;
                    newArchive.ListingEntry = newListing;

                    SetCapacity(newListing);
                    newParent.CurrentPosition += newListing.ContentCapacity;
                }
                result.Add(newListing);
            }
            return result.ToArray();
        }

        private void MoveContent(ArchiveArchiveEntry[] oldArchives, ArchiveArchiveEntry[] newArchives, ArchiveFileEntry[] newMetrics, ArchiveFileEntry[] newListing, ArchiveArchiveEntry newParent, ArchiveDirectoryEntry newDirectory)
        {
            for (int i = 0; i < oldArchives.Length; i++)
            {
                ArchiveArchiveEntry oldArchive = oldArchives[i];
                ArchiveArchiveEntry newArchive = newArchives[i];

                ArchiveFileEntry newContent = oldArchive.ContentEntry.Clone();
                {
                    newContent.Compression = Compression.None;
                    newContent.ContentOffset = newParent.CurrentPosition;
                    newArchive.ContentEntry = newContent;
                }

                newArchive.MetricsEntry = newMetrics[i];
                newArchive.ListingEntry = newListing[i];
                newArchive.ContentEntry = newContent;

                foreach (ArchiveFileEntry oldChild in oldArchive.Childs.OfType<ArchiveFileEntry>())
                    MoveFile(oldChild, newArchive, newDirectory);
                foreach (ArchiveArchiveEntry oldChild in oldArchive.Childs.OfType<ArchiveArchiveEntry>())
                    MoveArchive(oldChild, newArchive, newDirectory);

                newContent.UncompressedContentSize = newArchive.CurrentPosition;
                SetCapacity(newContent);
                newParent.CurrentPosition += newContent.ContentCapacity;
            }
        }

        private void MoveFile(ArchiveFileEntry oldFile, ArchiveArchiveEntry newParent, ArchiveDirectoryEntry newDirectory)
        {
            ArchiveFileEntry newFile = oldFile.Clone();
            {
                newFile.Compression = Compression.None;
                newFile.ContentOffset = newParent.CurrentPosition;
                SetCapacity(newFile);
                newParent.CurrentPosition += newFile.ContentCapacity;
            }

            newParent.AddEntry(newFile);
            newDirectory.AddEntry(oldFile.GetFullPath(), newFile);
        }

        private void WriteMetrics(ArchiveInformation targetInfo)
        {
            using (Stream output = new FileStream(targetInfo.MetricFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            using (BinaryWriter bw = new BinaryWriter(output))
            {
                foreach (ArchiveFileEntry file in targetInfo.RootArchive.Childs.OfType<ArchiveFileEntry>())
                {
                    bw.Write(file.UncompressedContentSize);
                    bw.Write(file.ContentOffset);
                    bw.Write((int)Compression.None);
                }

                ArchiveArchiveEntry[] childTargetArrays = targetInfo.RootArchive.Childs.OfType<ArchiveArchiveEntry>().ToArray();
                foreach (ArchiveFileEntry file in childTargetArrays.Select(a => a.MetricsEntry))
                {
                    bw.Write(file.UncompressedContentSize);
                    bw.Write(file.ContentOffset);
                    bw.Write((int)Compression.None);
                }
                foreach (ArchiveFileEntry file in childTargetArrays.Select(a => a.ListingEntry))
                {
                    bw.Write(file.UncompressedContentSize);
                    bw.Write(file.ContentOffset);
                    bw.Write((int)Compression.None);
                }
                foreach (ArchiveFileEntry file in childTargetArrays.Select(a => a.ContentEntry))
                {
                    bw.Write(file.UncompressedContentSize);
                    bw.Write(file.ContentOffset);
                    bw.Write((int)Compression.None);
                }
            }
        }

        private void WriteContent(MemoryMappedFile memoryFile, Stream outputStream, ArchiveInformation inputInfo, ArchiveInformation outputInfo)
        {
            List<ArchiveFileEntry> inputFiles = inputInfo.RootArchive.Expand();
            List<ArchiveFileEntry> outputFiles = outputInfo.RootArchive.Expand();

            for (int i = 0; i < inputFiles.Count; i++)
            {
                ArchiveFileEntry inputEntry = inputFiles[i];
                ArchiveFileEntry outputEntry = outputFiles[i];

                if (inputEntry.Name != outputEntry.Name)
                    throw new Exception("Порядок элементов в иерархиях изменился.");

                using (Stream inputStream = memoryFile.CreateViewStream(inputEntry.GetAbsoluteOffset(), inputEntry.UncompressedContentSize))
                {
                    outputStream.Seek(outputEntry.GetAbsoluteOffset(), SeekOrigin.Begin);
                    inputStream.CopyTo(outputStream);
                }
                Progress.NullSafeInvoke(this, new ProgressArgs(_info.ContentFilePath, Interlocked.Add(ref _processedSize, inputEntry.UncompressedContentSize), _totalProgress));
            }
        }

        private void SetCapacity(ArchiveFileEntry entry)
        {
            entry.ContentCapacity = (int)(Options.AbsoluteReserve + Options.RelativeReserve * entry.UncompressedContentSize / 100 + entry.UncompressedContentSize);
        }
    }
}