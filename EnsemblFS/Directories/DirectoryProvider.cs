using System;
using System.Collections.Generic;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS
{
    public abstract class DirectoryProvider : NodeProvider
    {
        public DirectoryProvider(params NodeProvider[] children) : base(children)
        {
        }

        public override sealed Errno OnOpenHandle(ExpandedPath file, PathInfo info)
        {
            throw new System.NotImplementedException("FileProvider.OnOpenHandle marked as sealed, not implemented");
        }

        public override sealed Errno OnReadHandle(ExpandedPath file, PathInfo info, byte[] buf, long offset, out int bytesRead)
        {
            throw new System.NotImplementedException("FileProvider.OnReadHandle marked as sealed, not implemented");
        }
    }
}