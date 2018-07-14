﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            personFactory = new PersonFactory();
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
    }
}
