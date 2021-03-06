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

        [TestMethod]
        public void CheckDeathDate5()
        {
            string content = File.ReadAllText("WikiJson/AnniAlbers.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1994, 05, 09);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate6()
        {
            string content = File.ReadAllText("WikiJson/GladysTantaquidgeon.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(2005, 11, 01);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate7()
        {
            string content = File.ReadAllText("WikiJson/EricDWalrond.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1966, 08, 08);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate8()
        {
            string content = File.ReadAllText("WikiJson/LesterJMaitland.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1990, 03, 27);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate9()
        {
            string content = File.ReadAllText("WikiJson/EllamaeEllisLeague.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1991, 03, 04);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate10()
        {
            string content = File.ReadAllText("WikiJson/RousasRushdoony.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(2001, 02, 08);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate11()
        {
            string content = File.ReadAllText("WikiJson/HaroldOsborn.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1975, 04, 05);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate12()
        {
            string content = File.ReadAllText("WikiJson/UkichiroNakaya.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1962, 04, 11);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate13()
        {
            string content = File.ReadAllText("WikiJson/BirendranathSircar.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1980, 11, 28);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate14()
        {
            string content = File.ReadAllText("WikiJson/RichardEberhart.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(2005, 06, 09);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate15()
        {
            string content = File.ReadAllText("WikiJson/AdolphMurie.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1974, 08, 16);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate16()
        {
            string content = File.ReadAllText("WikiJson/GenrikhLyushkov.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1945, 08, 01);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate17()
        {
            string content = File.ReadAllText("WikiJson/TahirDizdari.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1972, 05, 09);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate18()
        {
            string content = File.ReadAllText("WikiJson/TonySisti.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1983, 01, 01);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate19()
        {
            string content = File.ReadAllText("WikiJson/SonHouse.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1988, 10, 19);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate20()
        {
            string content = File.ReadAllText("WikiJson/WilliamGrover-Williams.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1945, 03, 01);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate21()
        {
            string content = File.ReadAllText("WikiJson/PopeCyrilVIofAlexandria.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1971, 03, 09);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate22()
        {
            string content = File.ReadAllText("WikiJson/GlennMiller.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1944, 12, 15);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate23()
        {
            string content = File.ReadAllText("WikiJson/AnneValliantBurnettTandy.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1980, 01, 01);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate24()
        {
            string content = File.ReadAllText("WikiJson/PremendraMitra.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1988, 05, 02);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }

        [TestMethod]
        public void CheckDeathDate25()
        {
            string content = File.ReadAllText("WikiJson/MikeMilligan.json");

            var person = personFactory.CreateFromContent(content);

            var expectedDeathDate = new DateTime(1978, 05, 08);

            Assert.AreEqual(expectedDeathDate, person.DeathDate);
        }
    }
}
