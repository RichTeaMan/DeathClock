using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DeathClock.Test
{
    [TestClass]
    public class BirthDateTest
    {
        private PersonFactory personFactory;

        [TestInitialize]
        public void Setup()
        {
            personFactory = new PersonFactory(null);
        }

        [TestMethod]
        public void CheckBirthDate1()
        {
            string content = File.ReadAllText("WikiJson/ZygmuntGorgolewski.json");

            var person = personFactory.CreateFromContent(content);

            var expectedBirthDate = new DateTime(1845, 02, 14);

            Assert.AreEqual(expectedBirthDate, person?.BirthDate);
        }

        [TestMethod]
        public void CheckBirthDate2()
        {
            string content = File.ReadAllText("WikiJson/Zulfiya.json");

            var person = personFactory.CreateFromContent(content);

            var expectedBirthDate = new DateTime(1915, 03, 14);

            Assert.AreEqual(expectedBirthDate, person?.BirthDate);
        }
    }
}
