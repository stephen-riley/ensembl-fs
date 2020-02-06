using System;
using System.Collections.Generic;
using System.Linq;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS.Directories
{
    public class StructuresDir : NodeProvider
    {
        private IList<NamedStat> subdirs = new List<NamedStat> {
            new NamedStat("chromosomes", Extensions.StandardDir()),
            new NamedStat("proteins", Extensions.StandardDir()),
            new NamedStat("features", Extensions.StandardDir())
        };

        public StructuresDir(params NodeProvider[] children) : base(children)
        {
        }

        public override ExpandedPath.Action HandlePath(ExpandedPath ep)
        {
            Console.WriteLine($"*** StructuresDir.HandlePath {ep.Path}");

            if (ep.Level > ep.Components.Count)
            {
                return ExpandedPath.Action.Unknown;
            }
            else if (ep.Level == ep.Components.Count)
            {
                return ExpandedPath.Action.Handle;
            }
            else
            {
                return ExpandedPath.Action.PassThrough;
            }
        }

        public override Errno OnGetPathStatus(ExpandedPath path, out NamedStat entry)
        {
            var desiredDir = path.Components.Last();
            var selectedDir = subdirs.Where(s => s.Name == desiredDir).FirstOrDefault();

            if (selectedDir != null)
            {
                entry = selectedDir;
                return 0;
            }
            else
            {
                entry = new NamedStat();
                return Errno.ENOENT;
            }
        }

        public override Errno OnOpenHandle(ExpandedPath file, PathInfo info)
        {
            throw new System.NotImplementedException();
        }

        public override Errno OnReadDirectory(ExpandedPath directory, PathInfo info, out IEnumerable<NamedStat> paths)
        {
            if (directory.Components.Count < Level)
            {
                paths = subdirs;
                return 0;
            }
            else
            {
                return Children[0].OnReadDirectory(directory, info, out paths);
            }
        }

        public override Errno OnReadHandle(ExpandedPath file, PathInfo info, byte[] buf, long offset, out int bytesRead)
        {
            throw new System.NotImplementedException();
        }
    }
}