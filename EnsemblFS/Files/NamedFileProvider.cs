using System.Collections.Generic;
using System.Linq;
using Ensembl;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS.Files
{
    public abstract class NamedFileProvider : FileProvider
    {
        public string Name { get; private set; }

        private IDictionary<string, NamedStat> fileCache;

        public NamedFileProvider(string name) : base()
        {
            Name = name;
            fileCache = new Dictionary<string, NamedStat>();
        }

        public override sealed ExpandedPath.Action HandlePath(ExpandedPath ep)
        {
            if (ep.Components.Last() == Name)
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
            if (!fileCache.ContainsKey(path.FullPath))
            {
                var species = path.Components[0];
                var chromosome = path.Components[2];
                var slice = new Slice(species, chromosome);

                var stat = Extensions.StandardFile();
                stat.st_size = slice.Length;
                var ns = new NamedStat(Name, stat);
                fileCache[path.FullPath] = ns;
            }

            entry = fileCache[path.FullPath];
            return 0;
        }
    }
}