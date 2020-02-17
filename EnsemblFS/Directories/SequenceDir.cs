using System.Collections.Generic;
using System.Linq;
using EnsemblFS.Files;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS.Directories
{
    public class SequenceDir : DirectoryProvider
    {
        private static IDictionary<string, NamedStat> sequenceFiles = new Dictionary<string, NamedStat>();

        public SequenceDir(params NodeProvider[] children) : base(children)
        {
            foreach (var child in children.OfType<NamedFileProvider>())
            {
                sequenceFiles[child.Name] = new NamedStat(child.Name, Extensions.StandardFile());
            }
        }

        public override ExpandedPath.Action HandlePath(ExpandedPath ep)
        {
            var desiredFile = ep.Components.Last();

            if (ep.Components.Count < Level)
            {
                return ExpandedPath.Action.Handle;
            }
            else
            {
                // we're asking about files in the dir
                foreach (var child in Children)
                {
                    var action = child.HandlePath(ep);
                    if (action == ExpandedPath.Action.Handle)
                    {
                        return ExpandedPath.Action.PassThrough;
                    }
                }

                return ExpandedPath.Action.NotFound;
            }
        }

        public override Errno OnGetPathStatus(ExpandedPath path, out NamedStat entry)
        {
            var desiredFile = path.Components.Last();
            if (sequenceFiles.TryGetValue(desiredFile, out var ns))
            {
                entry = ns;
                return 0;
            }
            else
            {
                entry = new NamedStat();
                return Errno.ENOENT;
            }
        }

        public override Errno OnReadDirectory(ExpandedPath directory, PathInfo info, out IEnumerable<NamedStat> paths)
        {
            paths = sequenceFiles.Values;
            return 0;
        }
    }
}