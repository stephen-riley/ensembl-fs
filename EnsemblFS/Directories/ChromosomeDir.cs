using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ensembl;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS
{
    public class ChromosomeDir : DirectoryProvider
    {
        private Dictionary<string, IList<NamedStat>> chromosomes = new Dictionary<string, IList<NamedStat>>();

        public ChromosomeDir(params NodeProvider[] children) : base(children)
        {
        }

        public override ExpandedPath.Action HandlePath(ExpandedPath ep)
        {
            Trace.WriteLine($"StructuresDir.HandlePath {ep.Path}");

            LoadChromosomeList(ep);

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

        public override Errno OnReadDirectory(ExpandedPath directory, PathInfo info, out IEnumerable<NamedStat> paths)
        {
            LoadChromosomeList(directory);

            if (directory.Components.Count < Level)
            {
                if (chromosomes.ContainsKey(directory.Components[0]))
                {
                    paths = chromosomes[directory.Components[0]];
                    return 0;
                }
                else
                {
                    paths = new List<NamedStat>();
                    return Errno.ENOENT;
                }
            }
            else
            {
                return Children[0].OnReadDirectory(directory, info, out paths);
            }
        }

        public override Errno OnGetPathStatus(ExpandedPath path, out NamedStat entry)
        {
            LoadChromosomeList(path);

            var species = path.Components[0];
            var chromosome = path.Components.Last();

            if (chromosomes.ContainsKey(species))
            {
                entry = chromosomes[species].Where(ns => ns.Name == chromosome).First();
                return 0;
            }
            else
            {
                entry = new NamedStat();
                return Errno.ENOENT;
            }
        }

        public void LoadChromosomeList(ExpandedPath ep)
        {
            if (chromosomes.ContainsKey(ep.Components[0]))
            {
                return;
            }

            var speciesDbName = ep.Components[0];
            var chromosomeList = Slice.GetSpeciesChromosomeList(speciesDbName);

            var chromosomeForSpecies = chromosomeList
                .Select(name =>
                {
                    return new NamedStat(name, Extensions.StandardDir());
                }).ToList();

            chromosomes[speciesDbName] = chromosomeForSpecies;
        }
    }
}