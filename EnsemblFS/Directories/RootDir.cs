using System;
using System.Collections.Generic;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS.Directories
{
    public class RootDir : DirectoryProvider
    {
        private NamedStat rootDirEntry;

        public RootDir(params NodeProvider[] children) : base(children)
        {
            rootDirEntry = new NamedStat("/", Extensions.StandardDir());
        }

        public override ExpandedPath.Action HandlePath(ExpandedPath ep)
        {
            return ep.Level == ep.Components.Count ? ExpandedPath.Action.Handle : ExpandedPath.Action.PassThrough;
        }

        public override Errno OnGetPathStatus(ExpandedPath path, out NamedStat entry)
        {
            entry = rootDirEntry;
            return 0;
        }

        public override Errno OnReadDirectory(ExpandedPath directory, PathInfo info, out IEnumerable<NamedStat> paths)
        {
            return Children[0].OnReadDirectory(directory.NextLevel(), info, out paths);
        }
    }
}