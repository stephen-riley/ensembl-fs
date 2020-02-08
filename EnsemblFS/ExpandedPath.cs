using System;
using System.Collections.Generic;

namespace EnsemblFS
{
    public class ExpandedPath
    {
        public enum Action
        {
            Unknown,
            Handle,
            PassThrough,
            NotFound
        }

        public string FullPath { get; private set; }

        public IList<string> Components { get; private set; }

        public int Level { get; private set; }

        public ExpandedPath(string path)
        {
            FullPath = path;
            Level = 0;

            Components = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }

        public ExpandedPath(ExpandedPath ep, int newLevel)
        {
            FullPath = ep.FullPath;
            Level = newLevel;
            Components = ep.Components;
        }

        public ExpandedPath NextLevel()
        {
            return new ExpandedPath(this, Level + 1);
        }
    }
}