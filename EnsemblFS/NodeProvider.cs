using System;
using System.Collections.Generic;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS
{
    public abstract class NodeProvider
    {
        internal int Level { get; set; }

        public IList<NodeProvider> Children { get; private set; }

        public NodeProvider(params NodeProvider[] children)
        {
            Children = new List<NodeProvider>(children);
        }

        public abstract Errno OnGetPathStatus(
            ExpandedPath path,
            out NamedStat entry);

        public abstract Errno OnReadDirectory(
            ExpandedPath directory,
            PathInfo info,
            out IEnumerable<NamedStat> paths);

        public abstract Errno OnOpenHandle(
            ExpandedPath file,
            PathInfo info);

        public abstract Errno OnReadHandle(
            ExpandedPath file,
            PathInfo info,
            byte[] buf,
            long offset,
            out int bytesRead);

        public abstract ExpandedPath.Action HandlePath(ExpandedPath ep);
    }
}