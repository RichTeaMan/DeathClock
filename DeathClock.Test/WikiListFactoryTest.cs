using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DeathClock.Test
{
    [TestClass]
    public class WikiListFactoryTest
    {
        private WikiListFactory wikiListFactory;

        [TestInitialize]
        public void Setup()
        {
            var logger = new LoggerFactory().AddConsole().AddDebug().CreateLogger<WikiListFactory>();
            wikiListFactory = new WikiListFactory(logger);
        }

        [TestMethod]
        public void CheckLists()
        {
            string content = File.ReadAllText("WikiJson/ListsOfAmericans.json");

            var wikiList = wikiListFactory.CreateFromContent(content);

            var expectedLists = new List<string>();

            #region expected

            expectedLists.Add("List of Afghan Americans");
            expectedLists.Add("List of African Americans");
            expectedLists.Add("List of Albanian Americans");
            expectedLists.Add("List of Amish and their descendants");
            expectedLists.Add("List of Angolan Americans");
            expectedLists.Add("List of Antiguan and Barbudan Americans");
            expectedLists.Add("List of Arab Americans");
            expectedLists.Add("List of Argentine Americans");
            expectedLists.Add("List of Armenian Americans");
            expectedLists.Add("List of Asian Americans");
            expectedLists.Add("List of Australian Americans");
            expectedLists.Add("List of Austrian Americans");
            expectedLists.Add("List of Azerbaijani Americans");
            expectedLists.Add("List of Bahamian Americans");
            expectedLists.Add("List of Bangladeshi Americans");
            expectedLists.Add("List of Barbadian Americans");
            expectedLists.Add("List of Belarusian Americans");
            expectedLists.Add("List of Belgian Americans");
            expectedLists.Add("List of Belizean Americans");
            expectedLists.Add("List of Beninese Americans");
            expectedLists.Add("List of Bermudian Americans");
            expectedLists.Add("List of Bolivian Americans");
            expectedLists.Add("List of Bosnian Americans");
            expectedLists.Add("List of Brazilian Americans");
            expectedLists.Add("List of Bulgarian Americans");
            expectedLists.Add("List of Burmese Americans");
            expectedLists.Add("List of Cajuns");
            expectedLists.Add("List of Cambodian Americans");
            expectedLists.Add("List of Cameroonian Americans");
            expectedLists.Add("List of Canadian Americans");
            expectedLists.Add("List of Catalan Americans");
            expectedLists.Add("List of Chilean Americans");
            expectedLists.Add("List of Chinese Americans");
            expectedLists.Add("List of Colombian Americans");
            expectedLists.Add("List of Coptic Americans");
            expectedLists.Add("List of Cossack Americans");
            expectedLists.Add("List of Costa Rican Americans");
            expectedLists.Add("List of Croatian Americans");
            expectedLists.Add("List of Cuban Americans");
            expectedLists.Add("List of Cypriot Americans");
            expectedLists.Add("List of Czech Americans");
            expectedLists.Add("List of Danish Americans");
            expectedLists.Add("List of Dominican Americans (Dominica)");
            expectedLists.Add("List of Dominican Americans (Dominican Republic)");
            expectedLists.Add("List of Dutch Americans");
            expectedLists.Add("List of Ecuadorian Americans");
            expectedLists.Add("List of Egyptian Americans");
            expectedLists.Add("List of Emirati Americans");
            expectedLists.Add("List of Americans of English descent");
            expectedLists.Add("List of Estonian Americans");
            expectedLists.Add("List of Ethiopian Americans");
            expectedLists.Add("List of Fijian Americans");
            expectedLists.Add("List of Filipino Americans");
            expectedLists.Add("List of Finnish Americans");
            expectedLists.Add("List of French Americans");
            expectedLists.Add("List of Fuzhounese Americans");
            expectedLists.Add("List of Gambian Americans");
            expectedLists.Add("List of Georgian Americans");
            expectedLists.Add("List of German Americans");
            expectedLists.Add("List of Ghanaian Americans");
            expectedLists.Add("List of Greek Americans");
            expectedLists.Add("List of Guatemalan Americans");
            expectedLists.Add("List of Guinean Americans");
            expectedLists.Add("List of Guyanese Americans");
            expectedLists.Add("List of Haitian Americans");
            expectedLists.Add("List of Hakka Americans");
            expectedLists.Add("List of Hispanic and Latino Americans");
            expectedLists.Add("List of Hmong Americans");
            expectedLists.Add("List of Honduran Americans");
            expectedLists.Add("List of Hong Kong Americans");
            expectedLists.Add("List of Hungarian Americans");
            expectedLists.Add("List of Icelandic Americans");
            expectedLists.Add("List of Indian Americans");
            expectedLists.Add("List of Indo-Caribbean Americans");
            expectedLists.Add("List of Indonesian Americans");
            expectedLists.Add("List of Iranian Americans");
            expectedLists.Add("List of Iraqi Americans");
            expectedLists.Add("List of Irish Americans");
            expectedLists.Add("List of Israeli Americans");
            expectedLists.Add("List of Italian Americans");
            expectedLists.Add("List of Ivorian Americans");
            expectedLists.Add("List of Jamaican Americans");
            expectedLists.Add("List of Japanese Americans");
            expectedLists.Add("Lists of American Jews");
            expectedLists.Add("List of Kazakh Americans");
            expectedLists.Add("List of Kenyan Americans");
            expectedLists.Add("List of Korean Americans");
            expectedLists.Add("List of Kuwaiti Americans");
            expectedLists.Add("List of Laotian Americans");
            expectedLists.Add("List of Latvian Americans");
            expectedLists.Add("List of Lebanese Americans");
            expectedLists.Add("List of Liberian Americans");
            expectedLists.Add("List of Lithuanian Americans");
            expectedLists.Add("List of Louisiana Creoles");
            expectedLists.Add("List of Luxembourg Americans");
            expectedLists.Add("List of Macedonian Americans");
            expectedLists.Add("List of Malawian Americans");
            expectedLists.Add("List of Malian Americans");
            expectedLists.Add("List of Māori Americans");
            expectedLists.Add("List of Mexican Americans");
            expectedLists.Add("List of Monegasque Americans");
            expectedLists.Add("List of Montenegrin Americans");
            expectedLists.Add("List of Moroccan Americans");
            expectedLists.Add("List of Native Americans of the United States");
            expectedLists.Add("List of Native Hawaiians");
            expectedLists.Add("List of Nepalese Americans");
            expectedLists.Add("List of New Zealand Americans");
            expectedLists.Add("List of Nicaraguan Americans");
            expectedLists.Add("List of Nigerian Americans");
            expectedLists.Add("List of Norwegian Americans");
            expectedLists.Add("List of Pakistani Americans");
            expectedLists.Add("List of Palauan Americans");
            expectedLists.Add("List of Palestinian Americans");
            expectedLists.Add("List of Panamanian Americans");
            expectedLists.Add("List of Paraguayan Americans");
            expectedLists.Add("List of Peruvian Americans");
            expectedLists.Add("List of Polish Americans");
            expectedLists.Add("List of Portuguese Americans");
            expectedLists.Add("List of Stateside Puerto Ricans");
            expectedLists.Add("List of Romani Americans");
            expectedLists.Add("List of Romanian Americans");
            expectedLists.Add("List of Russian Americans");
            expectedLists.Add("List of Rusyn Americans");
            expectedLists.Add("List of Salvadoran Americans");
            expectedLists.Add("List of Scotch-Irish Americans");
            expectedLists.Add("List of Scottish Americans");
            expectedLists.Add("List of Senegalese Americans");
            expectedLists.Add("List of Serbian Americans");
            expectedLists.Add("List of Sicilian Americans");
            expectedLists.Add("List of Singaporean Americans");
            expectedLists.Add("List of Slovak Americans");
            expectedLists.Add("List of Slovene Americans");
            expectedLists.Add("List of Somali Americans");
            expectedLists.Add("List of South African Americans");
            expectedLists.Add("List of Spanish Americans");
            expectedLists.Add("List of Sri Lankan Americans");
            expectedLists.Add("List of Sudanese Americans");
            expectedLists.Add("List of Surinamese Americans");
            expectedLists.Add("List of Swedish Americans");
            expectedLists.Add("List of Swiss Americans");
            expectedLists.Add("List of Syrian Americans");
            expectedLists.Add("List of Taiwanese Americans");
            expectedLists.Add("List of Tajikistani Americans");
            expectedLists.Add("List of Tanzanian Americans");
            expectedLists.Add("List of Thai Americans");
            expectedLists.Add("List of Tibetan Americans");
            expectedLists.Add("List of Tongan Americans");
            expectedLists.Add("List of Trinidadian and Tobagonian Americans");
            expectedLists.Add("List of Turkish Americans");
            expectedLists.Add("List of Ugandan Americans");
            expectedLists.Add("List of Ukrainian Americans");
            expectedLists.Add("List of Uruguayan Americans");
            expectedLists.Add("List of Uzbek Americans");
            expectedLists.Add("List of Venezuelan Americans");
            expectedLists.Add("List of Vietnamese Americans");
            expectedLists.Add("List of Welsh Americans");
            expectedLists.Add("List of people from Alabama");
            expectedLists.Add("List of people from Alaska");
            expectedLists.Add("List of people from Arizona");
            expectedLists.Add("List of people from Arkansas");
            expectedLists.Add("List of people from California");
            expectedLists.Add("List of people from Colorado");
            expectedLists.Add("List of people from Connecticut");
            expectedLists.Add("List of people from Delaware");
            expectedLists.Add("List of people from Florida");
            expectedLists.Add("List of people from Georgia (U.S. state)");
            expectedLists.Add("List of people from Hawaii");
            expectedLists.Add("List of people from Idaho");
            expectedLists.Add("List of people from Illinois");
            expectedLists.Add("List of people from Indiana");
            expectedLists.Add("List of people from Iowa");
            expectedLists.Add("List of people from Kansas");
            expectedLists.Add("List of people from Kentucky");
            expectedLists.Add("List of people from Louisiana");
            expectedLists.Add("List of people from Maine");
            expectedLists.Add("List of people from Maryland");
            expectedLists.Add("List of people from Massachusetts");
            expectedLists.Add("List of people from Michigan");
            expectedLists.Add("List of people from Minnesota");
            expectedLists.Add("List of people from Mississippi");
            expectedLists.Add("List of people from Missouri");
            expectedLists.Add("List of people from Montana");
            expectedLists.Add("List of people from Nebraska");
            expectedLists.Add("List of people from Nevada");
            expectedLists.Add("List of people from New Hampshire");
            expectedLists.Add("List of people from New Jersey");
            expectedLists.Add("List of people from New Mexico");
            expectedLists.Add("List of people from New York");
            expectedLists.Add("List of people from North Carolina");
            expectedLists.Add("List of people from North Dakota");
            expectedLists.Add("List of people from Ohio");
            expectedLists.Add("List of people from Oklahoma");
            expectedLists.Add("List of people from Oregon");
            expectedLists.Add("List of people from Pennsylvania");
            expectedLists.Add("List of people from Rhode Island");
            expectedLists.Add("List of people from South Carolina");
            expectedLists.Add("List of people from South Dakota");
            expectedLists.Add("List of people from Tennessee");
            expectedLists.Add("List of people from Texas");
            expectedLists.Add("List of people from Utah");
            expectedLists.Add("List of people from Vermont");
            expectedLists.Add("List of people from Virginia");
            expectedLists.Add("List of people from Washington");
            expectedLists.Add("List of people from West Virginia");
            expectedLists.Add("List of people from Wisconsin");
            expectedLists.Add("List of people from Wyoming");
            expectedLists.Add("List of people from American Samoa");
            expectedLists.Add("List of people from Guam");
            expectedLists.Add("List of people from Puerto Rico");
            expectedLists.Add("List of people from the United States Virgin Islands");
            expectedLists.Add("List of atheist Americans");
            expectedLists.Add("List of American Muslims");
            expectedLists.Add("List of naturalized American citizens");
            expectedLists.Add("List of Ellis Island immigrants");

            #endregion

            CollectionAssert.AreEquivalent(expectedLists, wikiList.ListTitles.ToArray());
        }

    }
}
