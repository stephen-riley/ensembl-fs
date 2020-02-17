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

        protected unsafe int CopyBuf(string src, byte[] dest, int length, int srcOffset = 0, int destOffset = 0)
        {
            if (srcOffset + length > src.Length)
            {
                length = src.Length - (int)srcOffset;
            }

            fixed (byte* pBuf = dest)
            {
                var charBuf = Encoding.UTF8.GetBytes(src.Substring((int)srcOffset, length));
                for (int i = 0; i < charBuf.Length; i++)
                {
                    pBuf[i + destOffset] = charBuf[i];
                }

                return charBuf.Length;
            }
        }
    }
}