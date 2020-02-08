using System;
using System.Collections.Generic;
using System.Diagnostics;
using Ensembl;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS.Files
{
    public class RefSequenceFile : NamedFileProvider
    {
        public RefSequenceFile(string name) : base(name)
        {
        }

        public override Errno OnReadHandle(ExpandedPath file, PathInfo info, byte[] buf, long offset, out int bytesRead)
        {
            // Trace.WriteLine($"*** RefSequenceFile.OnReadHandle({file.FullPath}, offset={offset}, buf_len={buf.Length})");

            var species = file.Components[0];
            var chromosome = file.Components[2];
            var slice = new Slice(species, chromosome);
            var start = offset + 1;
            var end = start + buf.Length - 1;
            var data = slice.GetSequenceString((int)start, (int)end);

            int toBeReadCount = buf.Length;
            bytesRead = CopyBuf(data, buf, toBeReadCount);
            return 0;
        }
    }
}