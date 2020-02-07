using System;
using System.Collections.Generic;
using System.Linq;
using Ensembl;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS.Directories
{
    public class SpeciesDir : DirectoryProvider
    {
        private List<NamedStat> species = new List<NamedStat>();

        private static IList<NamedStat> entries = new List<NamedStat>
        {
            new NamedStat("chromosomes",Extensions.StandardDir())
        };

        public SpeciesDir(params NodeProvider[] children) : base(children)
        {
        }

        public override Errno OnGetPathStatus(ExpandedPath path, out NamedStat entry)
        {
            LoadSpeciesList();

            var dirname = path.Components.Last();
            if (species.Where(ns => ns.Name == dirname).FirstOrDefault() != null)
            {
                entry = new NamedStat(dirname, Extensions.StandardDir());
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
            LoadSpeciesList();

            if (directory.Components.Count < Level)
            {
                paths = species;
                return 0;
            }
            else
            {
                return Children[0].OnReadDirectory(directory.NextLevel(), info, out paths);
            }
        }

        public override ExpandedPath.Action HandlePath(ExpandedPath ep)
        {
            LoadSpeciesList();

            if (ep.Level == 0)
            {
                throw new ArgumentOutOfRangeException("ExpandedPath.Level cannot be 0 here");
            }

            var requestedSpecies = species.Where(s => s.Name == ep.Components[ep.Level - 1]).FirstOrDefault();

            if (requestedSpecies == null)
            {
                return ExpandedPath.Action.NotFound;
            }

            if (ep.Level == ep.Components.Count)
            {
                return ExpandedPath.Action.Handle;
            }
            else
            {
                return ExpandedPath.Action.PassThrough;
            }
        }

        private void LoadSpeciesList()
        {
            if (species.Count != 0)
            {
                return;
            }

            var speciesDbNames = Slice.GetSpeciesDbNames();

            species = speciesDbNames
                .Select(name =>
                {
                    var stat = new Stat();
                    long timeNow = 0;
                    Syscall.time(out timeNow);

                    stat.st_uid = Syscall.getuid();
                    stat.st_gid = Syscall.getgid();
                    stat.st_atime = timeNow;
                    stat.st_mtime = timeNow;
                    stat.st_mode = FilePermissions.S_IFDIR | (FilePermissions)Convert.ToInt32("0755", 8);
                    stat.st_nlink = 2;

                    return new NamedStat(name, stat);
                }).ToList();
        }
    }
}