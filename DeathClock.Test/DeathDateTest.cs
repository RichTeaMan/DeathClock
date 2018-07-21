using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DeathClock.Test
{
[TestClass]
    public class DeathDateTest
    {
        private PersonFactory personFactory;

        [TestInitialize]
        public void Setup()
        {
            personFactory = new PersonFactory(null);
        }

        [TestMethod]
        public void CheckDeathDate1()
        {
            string content = File.ReadAllText("WikiJson/EricLiddell.json");

            var person = personFactory.CreateFromContent(content);
            
            var expectedDeathDate = new DateTime(1945, 2, 21);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate2()
        {
            string content = File.ReadAllText("WikiJson/HHPrice.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1984, 11, 26);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate3()
        {
            string content = File.ReadAllText("WikiJson/JamesEKelly.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1933, 05, 25);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate4()
        {
            string content = File.ReadAllText("WikiJson/MalcolmCowley.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1989, 03, 27);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }
    }
}
