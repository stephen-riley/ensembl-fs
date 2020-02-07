using System;
using System.Collections.Generic;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS
{
    public abstract class FileProvider : NodeProvider
    {
        public FileProvider() : base()
        {
        }

        public override sealed Errno OnGetPathStatus(ExpandedPath path, out NamedStat entry)
        {
            throw new System.NotImplementedException("FileProvider.OnGetPathStatus marked as sealed, not implemented");
        }

        public override sealed Errno OnReadDirectory(ExpandedPath directory, PathInfo info, out IEnumerable<NamedStat> paths)
        {
            throw new System.NotImplementedException("FileProvider.OnReadDirectory marked as sealed, not implemented");
        }
    }
}