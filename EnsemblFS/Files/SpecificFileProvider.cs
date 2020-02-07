using System.Linq;
using Ensembl;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS.Files
{
    public abstract class SpecificFileProvider : FileProvider
    {
        public NamedStat FileNamedStat { get; private set; }

        public SpecificFileProvider(string name) : base()
        {
            FileNamedStat = new NamedStat(name, Extensions.StandardFile());
        }

        public override sealed ExpandedPath.Action HandlePath(ExpandedPath ep)
        {
            if (ep.Components.Last() == FileNamedStat.Name)
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
            var species = path.Components[0];
            var chromosome = path.Components[2];

            if (FileNamedStat.Stat.st_size == 0)
            {
                var slice = new Slice(species, chromosome);
                var stat = FileNamedStat.Stat;
                stat.st_size = slice.Length;
                FileNamedStat.Stat = stat;
                // copying FileNamedStat.Stat into a local variable is to dodge error CS1612
            }

            entry = FileNamedStat;
            return 0;
        }
    }
}