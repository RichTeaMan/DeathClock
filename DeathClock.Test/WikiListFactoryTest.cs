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

            expectedLists.Add("List_of_Afghan_Americans");
            expectedLists.Add("List_of_African_Americans");
            expectedLists.Add("List_of_Albanian_Americans");
            expectedLists.Add("List_of_Amish_and_their_descendants");
            expectedLists.Add("List_of_Angolan_Americans");
            expectedLists.Add("List_of_Antiguan_and_Barbudan_Americans");
            expectedLists.Add("List_of_Arab_Americans");
            expectedLists.Add("List_of_Argentine_Americans");
            expectedLists.Add("List_of_Armenian_Americans");
            expectedLists.Add("List_of_Asian_Americans");
            expectedLists.Add("List_of_Australian_Americans");
            expectedLists.Add("List_of_Austrian_Americans");
            expectedLists.Add("List_of_Azerbaijani_Americans");
            expectedLists.Add("List_of_Bahamian_Americans");
            expectedLists.Add("List_of_Bangladeshi_Americans");
            expectedLists.Add("List_of_Barbadian_Americans");
            expectedLists.Add("List_of_Belarusian_Americans");
            expectedLists.Add("List_of_Belgian_Americans");
            expectedLists.Add("List_of_Belizean_Americans");
            expectedLists.Add("List_of_Beninese_Americans");
            expectedLists.Add("List_of_Bermudian_Americans");
            expectedLists.Add("List_of_Bolivian_Americans");
            expectedLists.Add("List_of_Bosnian_Americans");
            expectedLists.Add("List_of_Brazilian_Americans");
            expectedLists.Add("List_of_Bulgarian_Americans");
            expectedLists.Add("List_of_Burmese_Americans");
            expectedLists.Add("List_of_Cajuns");
            expectedLists.Add("List_of_Cambodian_Americans");
            expectedLists.Add("List_of_Cameroonian_Americans");
            expectedLists.Add("List_of_Canadian_Americans");
            expectedLists.Add("List_of_Catalan_Americans");
            expectedLists.Add("List_of_Chilean_Americans");
            expectedLists.Add("List_of_Chinese_Americans");
            expectedLists.Add("List_of_Colombian_Americans");
            expectedLists.Add("List_of_Coptic_Americans");
            expectedLists.Add("List_of_Cossack_Americans");
            expectedLists.Add("List_of_Costa_Rican_Americans");
            expectedLists.Add("List_of_Croatian_Americans");
            expectedLists.Add("List_of_Cuban_Americans");
            expectedLists.Add("List_of_Cypriot_Americans");
            expectedLists.Add("List_of_Czech_Americans");
            expectedLists.Add("List_of_Danish_Americans");
            expectedLists.Add("List_of_Dominican_Americans_(Dominica)");
            expectedLists.Add("List_of_Dominican_Americans_(Dominican_Republic)");
            expectedLists.Add("List_of_Dutch_Americans");
            expectedLists.Add("List_of_Ecuadorian_Americans");
            expectedLists.Add("List_of_Egyptian_Americans");
            expectedLists.Add("List_of_Emirati_Americans");
            expectedLists.Add("List_of_Americans_of_English_descent");
            expectedLists.Add("List_of_Estonian_Americans");
            expectedLists.Add("List_of_Ethiopian_Americans");
            expectedLists.Add("List_of_Fijian_Americans");
            expectedLists.Add("List_of_Filipino_Americans");
            expectedLists.Add("List_of_Finnish_Americans");
            expectedLists.Add("List_of_French_Americans");
            expectedLists.Add("List_of_Fuzhounese_Americans");
            expectedLists.Add("List_of_Gambian_Americans");
            expectedLists.Add("List_of_Georgian_Americans");
            expectedLists.Add("List_of_German_Americans");
            expectedLists.Add("List_of_Ghanaian_Americans");
            expectedLists.Add("List_of_Greek_Americans");
            expectedLists.Add("List_of_Guatemalan_Americans");
            expectedLists.Add("List_of_Guinean_Americans");
            expectedLists.Add("List_of_Guyanese_Americans");
            expectedLists.Add("List_of_Haitian_Americans");
            expectedLists.Add("List_of_Hakka_Americans");
            expectedLists.Add("List_of_Hispanic_and_Latino_Americans");
            expectedLists.Add("List_of_Hmong_Americans");
            expectedLists.Add("List_of_Honduran_Americans");
            expectedLists.Add("List_of_Hong_Kong_Americans");
            expectedLists.Add("List_of_Hungarian_Americans");
            expectedLists.Add("List_of_Icelandic_Americans");
            expectedLists.Add("List_of_Indian_Americans");
            expectedLists.Add("List_of_Indo-Caribbean_Americans");
            expectedLists.Add("List_of_Indonesian_Americans");
            expectedLists.Add("List_of_Iranian_Americans");
            expectedLists.Add("List_of_Iraqi_Americans");
            expectedLists.Add("List_of_Irish_Americans");
            expectedLists.Add("List_of_Israeli_Americans");
            expectedLists.Add("List_of_Italian_Americans");
            expectedLists.Add("List_of_Ivorian_Americans");
            expectedLists.Add("List_of_Jamaican_Americans");
            expectedLists.Add("List_of_Japanese_Americans");
            expectedLists.Add("Lists_of_American_Jews");
            expectedLists.Add("List_of_Kazakh_Americans");
            expectedLists.Add("List_of_Kenyan_Americans");
            expectedLists.Add("List_of_Korean_Americans");
            expectedLists.Add("List_of_Kuwaiti_Americans");
            expectedLists.Add("List_of_Laotian_Americans");
            expectedLists.Add("List_of_Latvian_Americans");
            expectedLists.Add("List_of_Lebanese_Americans");
            expectedLists.Add("List_of_Liberian_Americans");
            expectedLists.Add("List_of_Lithuanian_Americans");
            expectedLists.Add("List_of_Louisiana_Creoles");
            expectedLists.Add("List_of_Luxembourg_Americans");
            expectedLists.Add("List_of_Macedonian_Americans");
            expectedLists.Add("List_of_Malawian_Americans");
            expectedLists.Add("List_of_Malian_Americans");
            expectedLists.Add("List_of_Māori_Americans");
            expectedLists.Add("List_of_Mexican_Americans");
            expectedLists.Add("List_of_Monegasque_Americans");
            expectedLists.Add("List_of_Montenegrin_Americans");
            expectedLists.Add("List_of_Moroccan_Americans");
            expectedLists.Add("List_of_Native_Americans_of_the_United_States");
            expectedLists.Add("List_of_Native_Hawaiians");
            expectedLists.Add("List_of_Nepalese_Americans");
            expectedLists.Add("List_of_New_Zealand_Americans");
            expectedLists.Add("List_of_Nicaraguan_Americans");
            expectedLists.Add("List_of_Nigerian_Americans");
            expectedLists.Add("List_of_Norwegian_Americans");
            expectedLists.Add("List_of_Pakistani_Americans");
            expectedLists.Add("List_of_Palauan_Americans");
            expectedLists.Add("List_of_Palestinian_Americans");
            expectedLists.Add("List_of_Panamanian_Americans");
            expectedLists.Add("List_of_Paraguayan_Americans");
            expectedLists.Add("List_of_Peruvian_Americans");
            expectedLists.Add("List_of_Polish_Americans");
            expectedLists.Add("List_of_Portuguese_Americans");
            expectedLists.Add("List_of_Stateside_Puerto_Ricans");
            expectedLists.Add("List_of_Romani_Americans");
            expectedLists.Add("List_of_Romanian_Americans");
            expectedLists.Add("List_of_Russian_Americans");
            expectedLists.Add("List_of_Rusyn_Americans");
            expectedLists.Add("List_of_Salvadoran_Americans");
            expectedLists.Add("List_of_Scotch-Irish_Americans");
            expectedLists.Add("List_of_Scottish_Americans");
            expectedLists.Add("List_of_Senegalese_Americans");
            expectedLists.Add("List_of_Serbian_Americans");
            expectedLists.Add("List_of_Sicilian_Americans");
            expectedLists.Add("List_of_Singaporean_Americans");
            expectedLists.Add("List_of_Slovak_Americans");
            expectedLists.Add("List_of_Slovene_Americans");
            expectedLists.Add("List_of_Somali_Americans");
            expectedLists.Add("List_of_South_African_Americans");
            expectedLists.Add("List_of_Spanish_Americans");
            expectedLists.Add("List_of_Sri_Lankan_Americans");
            expectedLists.Add("List_of_Sudanese_Americans");
            expectedLists.Add("List_of_Surinamese_Americans");
            expectedLists.Add("List_of_Swedish_Americans");
            expectedLists.Add("List_of_Swiss_Americans");
            expectedLists.Add("List_of_Syrian_Americans");
            expectedLists.Add("List_of_Taiwanese_Americans");
            expectedLists.Add("List_of_Tajikistani_Americans");
            expectedLists.Add("List_of_Tanzanian_Americans");
            expectedLists.Add("List_of_Thai_Americans");
            expectedLists.Add("List_of_Tibetan_Americans");
            expectedLists.Add("List_of_Tongan_Americans");
            expectedLists.Add("List_of_Trinidadian_and_Tobagonian_Americans");
            expectedLists.Add("List_of_Turkish_Americans");
            expectedLists.Add("List_of_Ugandan_Americans");
            expectedLists.Add("List_of_Ukrainian_Americans");
            expectedLists.Add("List_of_Uruguayan_Americans");
            expectedLists.Add("List_of_Uzbek_Americans");
            expectedLists.Add("List_of_Venezuelan_Americans");
            expectedLists.Add("List_of_Vietnamese_Americans");
            expectedLists.Add("List_of_Welsh_Americans");
            expectedLists.Add("List_of_people_from_Alabama");
            expectedLists.Add("List_of_people_from_Alaska");
            expectedLists.Add("List_of_people_from_Arizona");
            expectedLists.Add("List_of_people_from_Arkansas");
            expectedLists.Add("List_of_people_from_California");
            expectedLists.Add("List_of_people_from_Colorado");
            expectedLists.Add("List_of_people_from_Connecticut");
            expectedLists.Add("List_of_people_from_Delaware");
            expectedLists.Add("List_of_people_from_Florida");
            expectedLists.Add("List_of_people_from_Georgia_(U.S._state)");
            expectedLists.Add("List_of_people_from_Hawaii");
            expectedLists.Add("List_of_people_from_Idaho");
            expectedLists.Add("List_of_people_from_Illinois");
            expectedLists.Add("List_of_people_from_Indiana");
            expectedLists.Add("List_of_people_from_Iowa");
            expectedLists.Add("List_of_people_from_Kansas");
            expectedLists.Add("List_of_people_from_Kentucky");
            expectedLists.Add("List_of_people_from_Louisiana");
            expectedLists.Add("List_of_people_from_Maine");
            expectedLists.Add("List_of_people_from_Maryland");
            expectedLists.Add("List_of_people_from_Massachusetts");
            expectedLists.Add("List_of_people_from_Michigan");
            expectedLists.Add("List_of_people_from_Minnesota");
            expectedLists.Add("List_of_people_from_Mississippi");
            expectedLists.Add("List_of_people_from_Missouri");
            expectedLists.Add("List_of_people_from_Montana");
            expectedLists.Add("List_of_people_from_Nebraska");
            expectedLists.Add("List_of_people_from_Nevada");
            expectedLists.Add("List_of_people_from_New_Hampshire");
            expectedLists.Add("List_of_people_from_New_Jersey");
            expectedLists.Add("List_of_people_from_New_Mexico");
            expectedLists.Add("List_of_people_from_New_York");
            expectedLists.Add("List_of_people_from_North_Carolina");
            expectedLists.Add("List_of_people_from_North_Dakota");
            expectedLists.Add("List_of_people_from_Ohio");
            expectedLists.Add("List_of_people_from_Oklahoma");
            expectedLists.Add("List_of_people_from_Oregon");
            expectedLists.Add("List_of_people_from_Pennsylvania");
            expectedLists.Add("List_of_people_from_Rhode_Island");
            expectedLists.Add("List_of_people_from_South_Carolina");
            expectedLists.Add("List_of_people_from_South_Dakota");
            expectedLists.Add("List_of_people_from_Tennessee");
            expectedLists.Add("List_of_people_from_Texas");
            expectedLists.Add("List_of_people_from_Utah");
            expectedLists.Add("List_of_people_from_Vermont");
            expectedLists.Add("List_of_people_from_Virginia");
            expectedLists.Add("List_of_people_from_Washington");
            expectedLists.Add("List_of_people_from_West_Virginia");
            expectedLists.Add("List_of_people_from_Wisconsin");
            expectedLists.Add("List_of_people_from_Wyoming");
            expectedLists.Add("List_of_people_from_American_Samoa");
            expectedLists.Add("List_of_people_from_Guam");
            expectedLists.Add("List_of_people_from_Puerto_Rico");
            expectedLists.Add("List_of_people_from_the_United_States_Virgin_Islands");
            expectedLists.Add("List_of_atheist_Americans");
            expectedLists.Add("List_of_American_Muslims");
            expectedLists.Add("List_of_naturalized_American_citizens");
            expectedLists.Add("List_of_Ellis_Island_immigrants");

            #endregion

            CollectionAssert.AreEquivalent(expectedLists, wikiList.ListTitles.ToArray());
        }

        [TestMethod]
        public void CheckPersonList1()
        {
            string content = File.ReadAllText("WikiJson/ListOfAfghanAmericans.json");
            //string content = File.ReadAllText("WikiJson/1.json");

            var wikiList = wikiListFactory.CreateFromContent(content);

            var expectedPersonList = new List<string>();

            #region expected

            expectedPersonList.Add("Stephen_J._Townsend");
            expectedPersonList.Add("Ehsan_Aman");
            expectedPersonList.Add("Tamim_Ansary");
            expectedPersonList.Add("Zohra_Daoud");
            expectedPersonList.Add("Farhad_Darya");
            expectedPersonList.Add("Qader_Eshpari");
            expectedPersonList.Add("Azita_Ghanizada");
            expectedPersonList.Add("Abdul_W._Haqiqi");
            expectedPersonList.Add("Khaled_Hosseini");
            expectedPersonList.Add("Ali_Ahmad_Jalali");
            expectedPersonList.Add("Zalmay_Khalilzad");
            expectedPersonList.Add("Wallace_Fard_Muhammad");
            expectedPersonList.Add("Naim_Popal");
            expectedPersonList.Add("Ahmad_Khan_Rahami");
            expectedPersonList.Add("Haidar_Salim");
            expectedPersonList.Add("Nazif_Shahrani");
            expectedPersonList.Add("Nabil_Miskinyar");
            expectedPersonList.Add("Fariba_Nawa");
            expectedPersonList.Add("Vida_Samadzai");
            expectedPersonList.Add("Jawed_Wassel");
            expectedPersonList.Add("Mariam_Wafa");
            expectedPersonList.Add("Najibullah_Zazi");

            #endregion

            CollectionAssert.AreEquivalent(expectedPersonList, wikiList.PersonTitles.ToArray());
        }

    }
}
