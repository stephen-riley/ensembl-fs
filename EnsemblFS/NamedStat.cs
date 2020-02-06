using Mono.Unix.Native;

namespace EnsemblFS
{
    public class NamedStat
    {
        public string Name { get; private set; }

        public Stat Stat { get; private set; }

        public NamedStat(string name, Stat stat)
        {
            Name = name;
            Stat = stat;
        }

        public NamedStat()
        {
            Name = string.Empty;
            Stat = new Stat();
        }
    }
}