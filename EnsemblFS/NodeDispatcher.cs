using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FuseSharp;
using Mono.Unix.Native;

using static EnsemblFS.ExpandedPath.Action;

[assembly: InternalsVisibleTo("EnsemblFS.Tests")]

namespace EnsemblFS
{
    public class NodeDispatcher
    {
        public NodeProvider Root { get; private set; }

        public NodeDispatcher(NodeProvider root)
        {
            Root = root;
            SetLevel(Root);
        }

        private static void SetLevel(NodeProvider node, int level = 0)
        {
            node.Level = level;
            foreach (var child in node.Children)
            {
                SetLevel(child, level + 1);
            }
        }

        internal ExpandedPath.Action LocateNodeByExpandedPath(ExpandedPath ep, NodeProvider node, out NodeProvider? handlingNode)
        {
            var action = node.HandlePath(ep);

            switch (action)
            {
                case Unknown:
                    throw new NotImplementedException($"handler for {node.GetType().ToString()} returned Unknown");
                case NotFound:
                    // TODO
                    handlingNode = null;
                    return ExpandedPath.Action.NotFound;
                case PassThrough:
                    // TODO 
                    foreach (var child in node.Children)
                    {
                        var res = LocateNodeByExpandedPath(ep.NextLevel(), child, out handlingNode);
                        if (res == ExpandedPath.Action.Handle)
                        {
                            return ExpandedPath.Action.Handle;
                        }
                    }
                    handlingNode = null;
                    return ExpandedPath.Action.NotFound;
                case Handle:
                    handlingNode = node;
                    return ExpandedPath.Action.Handle;
            }

            handlingNode = null;
            return ExpandedPath.Action.Unknown;
        }

        public Errno OnGetPathStatus(
            string path,
            out Stat entry)
        {
            var ep = new ExpandedPath(path);
            NodeProvider? node;
            var action = LocateNodeByExpandedPath(ep, Root, out node);

            if (action == ExpandedPath.Action.Handle && node != null)
            {
                NamedStat nstat;
                var res = node.OnGetPathStatus(ep, out nstat);
                entry = nstat.Stat;
                return res;
            }
            else
            {
                entry = new Stat();
                return Errno.ENOENT;
            }
        }

        public Errno OnReadDirectory(
            string directory,
            PathInfo info,
            out IEnumerable<DirectoryEntry> paths)
        {
            var ep = new ExpandedPath(directory);
            NodeProvider? node;
            var action = LocateNodeByExpandedPath(ep, Root, out node);

            if (action == ExpandedPath.Action.Handle && node != null)
            {
                IEnumerable<NamedStat> nstats;
                var res = node.OnReadDirectory(ep, info, out nstats);
                paths = nstats.Select(ns => new DirectoryEntry(ns.Name));
                return res;
            }
            else
            {
                paths = new List<DirectoryEntry>();
                return Errno.ENOENT;
            }

        }

        public Errno OnOpenHandle(
            string file,
            PathInfo info)
        {
            var ep = new ExpandedPath(file);
            NodeProvider? node;
            var action = LocateNodeByExpandedPath(ep, Root, out node);

            if (action == ExpandedPath.Action.Handle && node != null)
            {
                return node.OnOpenHandle(ep, info);
            }
            else
            {
                return Errno.ENOENT;
            }

        }

        public Errno OnReadHandle(
            string file,
            PathInfo info,
            byte[] buf,
            long offset,
            out int bytesRead)
        {
            var ep = new ExpandedPath(file);
            NodeProvider? node;
            var action = LocateNodeByExpandedPath(ep, Root, out node);

            if (action == ExpandedPath.Action.Handle && node != null)
            {
                return node.OnReadHandle(ep, info, buf, offset, out bytesRead);
            }
            else
            {
                bytesRead = 0;
                return Errno.ENOENT;
            }

        }
    }
}