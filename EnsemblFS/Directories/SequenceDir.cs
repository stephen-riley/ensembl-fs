using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS.Directories
{
    public class SequenceDir : DirectoryProvider
    {
        private static IDictionary<string, NamedStat> sequenceFiles = new Dictionary<string, NamedStat>
        {
            ["REF"] = new NamedStat("REF", Extensions.StandardFile()),
            ["PATCHED"] = new NamedStat("PATCHED", Extensions.StandardFile())
        };

        public SequenceDir(params NodeProvider[] children) : base(children)
        {
        }

        public override ExpandedPath.Action HandlePath(ExpandedPath ep)
        {
            Trace.WriteLine($"SequenceDir.HandlePath {ep.Path}");

            var desiredFile = ep.Components.Last();
            if (sequenceFiles.ContainsKey(desiredFile))
            {
                return ExpandedPath.Action.Handle;
            }
            else
            {
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