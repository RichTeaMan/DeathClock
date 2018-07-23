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

        [TestMethod]
        public void CheckBirthDate3()
        {
            string content = File.ReadAllText("WikiJson/Zozimus.json");

            var person = personFactory.CreateFromContent(content);

            var expectedBirthDate = new DateTime(1794, 01, 01);

            Assert.AreEqual(expectedBirthDate, person?.BirthDate);
        }

        [TestMethod]
        public void CheckBirthDate4()
        {
            string content = File.ReadAllText("WikiJson/JamesEKelly.json");

            var person = personFactory.CreateFromContent(content);

            var expectedBirthDate = new DateTime(1855, 07, 30);

            Assert.AreEqual(expectedBirthDate, person?.BirthDate);
        }

        [TestMethod]
        public void CheckBirthDate5()
        {
            string content = File.ReadAllText("WikiJson/ZinaidaSerebriakova.json");

            var person = personFactory.CreateFromContent(content);

            var expectedBirthDate = new DateTime(1884, 12, 12);

            Assert.AreEqual(expectedBirthDate, person?.BirthDate);
        }
    }
}
