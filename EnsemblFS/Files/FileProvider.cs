using System;
using System.Collections.Generic;
using System.Text;
using FuseSharp;
using Mono.Unix.Native;

namespace EnsemblFS
{
    public abstract class FileProvider : NodeProvider
    {
        public FileProvider() : base()
        {
        }

        public override sealed Errno OnOpenHandle(ExpandedPath file, PathInfo info)
        {
            var descriptor = 0x2a000000 + new Random().Next(0x1000000);
            info.Handle = new IntPtr(descriptor);
            return 0;
        }

        public override sealed Errno OnReadDirectory(ExpandedPath directory, PathInfo info, out IEnumerable<NamedStat> paths)
        {
            throw new System.NotImplementedException("FileProvider.OnReadDirectory marked as sealed, not implemented");
        }

        protected unsafe int CopyBuf(string src, byte[] dest, int length, int offset = 0)
        {
            if (offset + length > src.Length)
            {
                length = src.Length - (int)offset;
            }

            fixed (byte* pBuf = dest)
            {
                var charBuf = Encoding.UTF8.GetBytes(src.Substring((int)offset, length));
                for (int i = 0; i < charBuf.Length; i++)
                {
                    pBuf[i] = charBuf[i];
                }

                return charBuf.Length;
            }
        }
    }
}