using System;
using Mono.Unix.Native;

namespace EnsemblFS
{
    public static class Extensions
    {
        public static Stat StandardDir()
        {
            long timeNow = 0;
            Syscall.time(out timeNow);

            var stat = new Stat();
            stat.st_uid = Syscall.getuid();
            stat.st_gid = Syscall.getgid();
            stat.st_atime = timeNow;
            stat.st_mtime = timeNow;
            stat.st_mode = FilePermissions.S_IFDIR | (FilePermissions)Convert.ToInt32("0755", 8);
            stat.st_nlink = 2;

            return stat;
        }

        public static Stat StandardFile()
        {
            long timeNow = 0;
            Syscall.time(out timeNow);

            var stat = new Stat();
            stat.st_uid = Syscall.getuid();
            stat.st_gid = Syscall.getgid();
            stat.st_atime = timeNow;
            stat.st_mtime = timeNow;
            stat.st_mode = FilePermissions.S_IFREG | (FilePermissions)Convert.ToInt32("0644", 8);
            stat.st_nlink = 1;
            stat.st_size = 0;

            return stat;
        }
    }
}