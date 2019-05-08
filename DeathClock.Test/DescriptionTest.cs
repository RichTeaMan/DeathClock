using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DeathClock.Test
{
    [TestClass]
    public class DescriptionTest
    {
        private WikiUtility wikiUtility;

        [TestInitialize]
        public void Setup()
        {
            wikiUtility = new WikiUtility(null, null);
        }

        [TestMethod]
        public void CheckDescription1()
        {
            string content = File.ReadAllText("WikiJson/RichardEberhart.json");

            var person = wikiUtility.CreateFromContent(content);

            var expectedDescription = "Poet";

            Assert.AreEqual(expectedDescription, person.KnownFor);
        }

        [TestMethod]
        public void CheckDescription2()
        {
            string content = File.ReadAllText("WikiJson/ElizabethII.json");

            var person = wikiUtility.CreateFromContent(content);

            var expectedDescription = "Queen of the United Kingdom and the other Commonwealth realms";

            Assert.AreEqual(expectedDescription, person.KnownFor);
        }

        [TestMethod]
        public void CheckDescription3()
        {
            string content = File.ReadAllText("WikiJson/SonHouse.json");

            var person = wikiUtility.CreateFromContent(content);

            var expectedDescription = "Musician";

            Assert.AreEqual(expectedDescription, person.KnownFor);
        }

        [TestMethod]
        public void CheckDescription4()
        {
            string content = File.ReadAllText("WikiJson/WilliamGrover-Williams.json");

            var person = wikiUtility.CreateFromContent(content);

            var expectedDescription = "Racing driver";

            Assert.AreEqual(expectedDescription, person.KnownFor);
        }

        [TestMethod]
        public void CheckDescription5()
        {
            string content = File.ReadAllText("WikiJson/PopeCyrilVIofAlexandria.json");

            var person = wikiUtility.CreateFromContent(content);

            var expectedDescription = "Pope of the Coptic Orthodox Church of Alexandria";

            Assert.AreEqual(expectedDescription, person.KnownFor);
        }
    }
}
