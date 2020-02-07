using System;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS.Files
{
    public class PatchSequenceFile : FileProvider
    {
        public PatchSequenceFile() : base()
        {
        }

        public override ExpandedPath.Action HandlePath(ExpandedPath ep)
        {
            throw new NotImplementedException();
        }

        public override Errno OnOpenHandle(ExpandedPath file, PathInfo info)
        {
            throw new NotImplementedException();
        }

        public override Errno OnReadHandle(ExpandedPath file, PathInfo info, byte[] buf, long offset, out int bytesRead)
        {
            throw new NotImplementedException();
        }
    }
}