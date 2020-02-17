using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Ensembl;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS.Files
{
    public class RefFastaFile : NamedFileProvider
    {
        const int LINE_WIDTH = 60;
        const string CRLF = "\r\n";

        // This dictionary works in conjuction with NamedFileProvider.fileCache.
        // The key is the ExpandedPath.FullPath value.
        public RefFastaFile(string name) : base(name)
        {
        }

        internal string GetFastaHeader(Slice slice)
            => $">{slice.ChromosomeName} dna:chromosome chromosome:{slice.Version}:{slice.ChromosomeName}:1:{slice.Length}:1 REF\r\n";

        public override Errno OnGetPathStatus(ExpandedPath path, out NamedStat entry)
        {
            var errno = base.OnGetPathStatus(path, out entry);

            if (errno == 0)
            {
                var stat = entry.Stat;

                var species = path.Components[0];
                var chromosome = path.Components[2];
                var slice = new Slice(species, chromosome);

                // pad the size for the FASTA header
                stat.st_size += GetFastaHeader(slice).Length;

                // pad the size for a "\r\n" after each LINE_WIDTH chars
                stat.st_size += 2 * (long)Math.Ceiling(slice.Length / (double)LINE_WIDTH);

                entry.Stat = stat;
            }

            return errno;
        }

        public override Errno OnReadHandle(ExpandedPath file, PathInfo info, byte[] buf, long offset, out int bytesRead)
        {
            Trace.WriteLine($"*** RefSequenceFile.OnReadHandle({file.FullPath}, offset={offset}, buf_len={buf.Length})");

            var species = file.Components[0];
            var chromosome = file.Components[2];
            var slice = new Slice(species, chromosome);

            var header = GetFastaHeader(slice);

            var reqStart = offset;
            var reqEnd = offset + buf.Length - 1;

            if (reqStart <= header.Length && reqEnd <= header.Length) // start and end inside header
            {
                var length = (int)(reqEnd - reqStart + 1);
                bytesRead = CopyBuf(header.Substring((int)reqStart, length), buf, length);
            }
            else if (reqStart > header.Length) // start and end in data
            {
                reqStart -= header.Length;
                reqEnd -= header.Length;

                var dataLength = CalculateDataLength((int)reqEnd - (int)reqStart + 1);
                var adjEnd = (int)reqStart + dataLength - 1;

                var data = slice.GetSequenceString((int)reqStart + 1, adjEnd);
                data = InsertNewlines(data, (int)reqStart + 1);
                bytesRead = CopyBuf(data, buf, buf.Length);
            }
            else // start in header, end in data
            {
                var totalLength = reqEnd - reqStart + 1;

                var headPart = header.Substring((int)reqStart);
                var dataStart = 1;
                var dataEnd = CalculateDataLength((int)totalLength - headPart.Length);

                var dataPart = slice.GetSequenceString(dataStart, dataEnd);
                dataPart = InsertNewlines(dataPart, dataStart);

                bytesRead = CopyBuf(headPart, buf, headPart.Length);
                bytesRead += CopyBuf(dataPart, buf, dataPart.Length, 0, headPart.Length);
            }

            return 0;
        }

        internal static int CalculateDataLength(int requestedLength, int lineWidth = LINE_WIDTH)
        {
            int segLength = lineWidth + CRLF.Length;
            int count = (requestedLength / segLength) * lineWidth + (requestedLength % segLength);
            return count;
        }

        internal string InsertNewlines(string data, int startIndex, int lineWidth = LINE_WIDTH)
        {
            var sb = new StringBuilder();

            var offset = 0;
            var length = (startIndex - 1) % lineWidth;

            do
            {
                if (length > 0)
                {
                    var maxLen = Math.Min(length, data.Length - offset);
                    sb.Append(data.Substring(offset, maxLen));
                    sb.Append("\r\n");
                }

                offset += length;
                length = offset + length < data.Length ? LINE_WIDTH : data.Length - offset;
            } while (offset < data.Length);

            return sb.ToString();
        }
    }
}