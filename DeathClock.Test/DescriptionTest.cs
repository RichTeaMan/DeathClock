using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DeathClock.Test
{
    [TestClass]
    public class DescriptionTest
    {
        private PersonFactory personFactory;

        [TestInitialize]
        public void Setup()
        {
            personFactory = new PersonFactory(null);
        }

        [TestMethod]
        public void CheckDescription1()
        {
            string content = File.ReadAllText("WikiJson/RichardEberhart.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDescription = "Poet";

            Assert.AreEqual(expectedDescription, person.Description);
        }

        [TestMethod]
        public void CheckDescription2()
        {
            string content = File.ReadAllText("WikiJson/ElizabethII.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDescription = "Queen of the United Kingdom and the other Commonwealth realms";

            Assert.AreEqual(expectedDescription, person.Description);
        }

        [TestMethod]
        public void CheckDescription3()
        {
            string content = File.ReadAllText("WikiJson/SonHouse.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDescription = "Musician";

            Assert.AreEqual(expectedDescription, person.Description);
        }
    }
}
