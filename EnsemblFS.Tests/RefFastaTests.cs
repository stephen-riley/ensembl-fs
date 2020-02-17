using System.Text;
using Ensembl;
using EnsemblFS.Directories;
using EnsemblFS.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnsemblFS.Tests
{
    [TestClass]
    public class RefFastaTests
    {
        ExpandedPath ep = new ExpandedPath("/homo_sapiens_core_99_38/chromosomes/1/REF.fasta");
        Slice slice = new Slice("homo_sapiens_core_99_38", "1");

        [TestMethod]
        public void GetHeaderOnlyRange()
        {
            var fastaFile = new RefFastaFile("REF.fasta");
            var header = fastaFile.GetFastaHeader(slice);

            var bytesRead = 0;
            var buf = new byte[10];

            var result = fastaFile.OnReadHandle(ep, null, buf, 0, out bytesRead);

            Assert.AreEqual(header.Substring(0, 10), Encoding.ASCII.GetString(buf));
        }

        [TestMethod]
        public void GetDataOnlyRange()
        {
            var fastaFile = new RefFastaFile("REF.fasta");
            var header = fastaFile.GetFastaHeader(slice);

            var bytesRead = 0;
            var buf = new byte[10];

            var result = fastaFile.OnReadHandle(ep, null, buf, 9999 + header.Length, out bytesRead);

            Assert.AreEqual("NTAACCCTAA", Encoding.ASCII.GetString(buf));
        }

        [TestMethod]
        public void GetMixedRange()
        {
            var fastaFile = new RefFastaFile("REF.fasta");
            var header = fastaFile.GetFastaHeader(slice);

            var bytesRead = 0;
            var buf = new byte[header.Length + 5];

            var result = fastaFile.OnReadHandle(ep, null, buf, 0, out bytesRead);

            Assert.AreEqual(header + "NNNNN", Encoding.ASCII.GetString(buf));
        }

        [DataRow(1, 1)]
        [DataRow(7, 5)]
        [DataRow(10, 8)]
        [DataRow(15, 11)]
        [DataRow(25, 19)]
        [DataTestMethod]
        public void TestFastaDataLengthCalculator(int requestedLength, int desiredDataLength)
        {
            var actual = RefFastaFile.CalculateDataLength(requestedLength, 5);
            Assert.AreEqual(desiredDataLength, actual);
        }
    }
}
