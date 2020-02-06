using System;
using System.Diagnostics;
using System.IO;
using FuseSharp;

namespace EnsemblFS
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine("Usage: AlphaNumericFS <mount point>");
                return -1;
            }

            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            string mountPoint;
            if (!Directory.Exists(mountPoint = Path.GetFullPath(args[0])))
                Directory.CreateDirectory(mountPoint);

            Console.WriteLine("Mount point:{0}", mountPoint);

            string[] actualArgs = { "-s", "-f", mountPoint };

            int status = -1;

            using (FileSystem fs = new EnsemblFileSystem())
            using (FileSystemHandler fsh = new FileSystemHandler(fs, actualArgs))
            {
                status = fsh.Start();
            }

            Console.WriteLine(status);
            return status;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception;
            if (exception == null)
            {
                return;
            }

            Console.WriteLine(exception.Message);
            Debug.WriteLine(exception.Message);
        }
    }
}