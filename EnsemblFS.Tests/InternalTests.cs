using EnsemblFS.Directories;
using EnsemblFS.Files;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnsemblFS.Tests
{
    [TestClass]
    public class InternalTests
    {
        private static NodeDispatcher dispatcher;

        [ClassInitialize]
        public static void Init(TestContext _)
        {
            dispatcher = new NodeDispatcher(
                new RootDir(
                    new SpeciesDir(
                        new StructuresDir(
                            new ChromosomeDir(
                                new SequenceDir(
                                    new RefSequenceFile("REF"),
                                    new PatchSequenceFile("PATCH")))))));
        }

        [TestMethod]
        public void DispatcherLocatesCorrectSpecificFileHandlingNode()
        {
            var ep = new ExpandedPath("/homo_sapiens_core_99_38/chromosomes/1/REF");
            NodeProvider handlingNode;

            var res = dispatcher.LocateNodeByExpandedPath(ep, dispatcher.Root, out handlingNode);

            Assert.AreEqual(ExpandedPath.Action.Handle, res);
            Assert.AreEqual(typeof(RefSequenceFile), handlingNode.GetType());
        }
    }
}
