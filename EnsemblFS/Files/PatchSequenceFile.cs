using System;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS.Files
{
    public class PatchSequenceFile : SpecificFileProvider
    {
        public PatchSequenceFile(string name) : base(name)
        {
        }

        public override Errno OnReadHandle(ExpandedPath file, PathInfo info, byte[] buf, long offset, out int bytesRead)
        {
            throw new NotImplementedException();
        }
    }
}