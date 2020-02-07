using System.Collections.Generic;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS.Files
{
    public class RefSequenceFile : FileProvider
    {
        public override ExpandedPath.Action HandlePath(ExpandedPath ep)
        {
            throw new System.NotImplementedException();
        }

        public override Errno OnOpenHandle(ExpandedPath file, PathInfo info)
        {
            throw new System.NotImplementedException("FileProvider.OnOpenHandle marked as sealed, not implemented");
        }

        public override Errno OnReadHandle(ExpandedPath file, PathInfo info, byte[] buf, long offset, out int bytesRead)
        {
            throw new System.NotImplementedException("FileProvider.OnReadHandle marked as sealed, not implemented");
        }
    }
}