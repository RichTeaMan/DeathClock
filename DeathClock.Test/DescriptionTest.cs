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
        public void CheckDescription()
        {
            string content = File.ReadAllText("WikiJson/RichardEberhart.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDescription = "Poet";

            Assert.AreEqual(expectedDescription, person.Description);
        }
    }
}
