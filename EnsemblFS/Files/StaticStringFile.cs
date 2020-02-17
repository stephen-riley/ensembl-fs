using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ensembl;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS.Files
{
    public class StaticStringFile : NamedFileProvider
    {
        private string Content { get; set; }

        public StaticStringFile(string name, string content) : base(name)
        {
            Content = content;
        }

        public override Errno OnGetPathStatus(ExpandedPath path, out NamedStat entry)
        {
            var result = base.OnGetPathStatus(path, out entry);
            var stat = entry.Stat;
            stat.st_size = Content.Length;
            entry.Stat = stat;
            return result;
        }

        public override Errno OnReadHandle(ExpandedPath file, PathInfo info, byte[] buf, long offset, out int bytesRead)
        {
            Trace.WriteLine($"*** StaticStringFile.OnReadHandle({file.FullPath}, offset={offset}, buf_len={buf.Length})");

            var start = (int)offset;
            var length = Math.Min(buf.Length, Content.Length);
            var data = Content.Substring(start, length);
            bytesRead = CopyBuf(data, buf, length);
            return 0;
        }
    }
}