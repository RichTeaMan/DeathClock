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

            var expectedBirthDate = new DateTime(1845, 2, 14);

            Assert.AreEqual(expectedBirthDate, person?.BirthDate);
        }
    }
}
