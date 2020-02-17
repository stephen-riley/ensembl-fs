using System;
using System.Collections.Generic;
using System.Linq;
using EnsemblFS.Directories;
using EnsemblFS.Files;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS
{
    public class EnsemblFileSystem : FileSystem
    {
        private NodeDispatcher dispatcher;

        public EnsemblFileSystem()
        {
            dispatcher = new NodeDispatcher(
                new RootDir(
                    new SpeciesDir(
                        new StructuresDir(
                            new ChromosomeDir(
                                new SequenceDir(
                                    new RefSequenceFile("REF"),
                                    new StaticStringFile("PATCH", "The PATCH file has not yet been implemented.\r\n")
                                )
                            )
                        )
                    )
                )
            );
        }

        public override Errno OnGetPathStatus(string path, out Stat stat)
        {
            return dispatcher.OnGetPathStatus(path, out stat);
        }

        public override Errno OnReadDirectory(
            string directory,
            PathInfo info,
            out IEnumerable<DirectoryEntry> paths)
        {
            return dispatcher.OnReadDirectory(directory, info, out paths);
        }

        public override Errno OnOpenHandle(string file, PathInfo info)
        {
            return dispatcher.OnOpenHandle(file, info);
        }

        public override Errno OnReadHandle(
            string file,
            PathInfo info,
            byte[] buf,
            long offset,
            out int bytesRead)
        {
            return dispatcher.OnReadHandle(file, info, buf, offset, out bytesRead);
        }
    }
}