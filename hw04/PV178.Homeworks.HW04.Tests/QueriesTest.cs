using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PV178.Homeworks.HW04.Model;

namespace PV178.Homeworks.HW04.Tests
{
    [TestClass]
    public class QueriesTest
    {
        private Queries queries;
        public Queries Queries => queries ?? (queries = new Queries());

        [TestMethod]
        public void FourAttacksWithLightestSharksInVenezuelaQueryTest_ReturnsCorrectResult()
        {
            var expectedResult = new List<int>
            {
                2892, 3267, 5081, 22
            };

            var fourAttacksWithLightestSharksInVenezuela =
                Queries.FourAttacksWithLightestSharksInVenezuelaQuery();

            var res = fourAttacksWithLightestSharksInVenezuela
                .SequenceEqual(expectedResult);
            AssertBoolEqualsTrue(res);
        }

        [TestMethod]
        public void AreAllLongSharksGenderIgnoringQueryTest_ReturnsCorrectResult()
        {
            var areAllLongSharksGenderIgnoring = Queries.AreAllLongSharksGenderIgnoringQuery();

            AssertBoolEqualsTrue(areAllLongSharksGenderIgnoring.Equals(false));
        }

        [TestMethod]
        public void FortunateSharkAttacksSumWithinMonarchyOrTerritoryQueryTest_ReturnsCorrectResult()
        {
            // act
            var sum = Queries.FortunateSharkAttacksSumWithinMonarchyOrTerritoryQuery();

            // assert
            Assert.AreEqual(1157, sum, "Expected total sum: 1157");
        }

        [TestMethod]
        public void SharksWithoutNicknameAttacksQueryTest_ReturnsCorrectResult()
        {
            var expectedResult = new Dictionary<string, int>
            {
                {"Nurse shark", 35},
                {"Spinner shark", 34},
                {"Mako shark", 30},
                {"Carpet shark", 33},
                {"Hammerhead shark", 46},
                {"Dusky shark", 31},
                {"Grey reef shark", 37},
                {"Lemon shark", 27},
                {"Blue shark", 18},
                {"Salmon shark", 17},
                {"Blacktip shark", 19}
            };

            var sharksWithoutNicknameAttacks = Queries.SharksWithoutNicknameAttacksQuery();

            AssertBoolEqualsTrue(sharksWithoutNicknameAttacks.OrderBy(x => x.Key).SequenceEqual(expectedResult.OrderBy(x => x.Key)));
        }

        [TestMethod]
        public void WhiteDeathSurvivorsStartingWithKQueryTest_ReturnsCorrectResult()
        {
            // arrange
            var personNamesExpectedResult = new List<string> { "K. Tracy", "Karl Kuchnow", "Kazuhiho Kato", "Kevin Thompson" };

            // act
            var whiteDeathSurvivorsStartingWithK = Queries.WhiteDeathSurvivorsStartingWithKQuery();

            // assert
            var res = personNamesExpectedResult.SequenceEqual(whiteDeathSurvivorsStartingWithK);
            AssertBoolEqualsTrue(res);
        }

        [TestMethod]
        public void FiveSharkSpeciesWithMostFatalitiesQueryTest_ReturnsCorrectResult()
        {
            // arrange
            var fiveSharkSpeciesWithMostFatalitiesExpectedResult = new Dictionary<string, int>
            {
                {"White shark", 283},
                {"Hammerhead shark", 148},
                {"Sevengill shark", 131},
                {"Bronze whaler shark", 129},
                {"Wobbegong shark", 126}
            };

            // act
            var fiveSharkSpeciesWithMostFatalities = Queries.FiveSharkSpeciesWithMostFatalitiesQuery();

            // assert
            var res = fiveSharkSpeciesWithMostFatalitiesExpectedResult.SequenceEqual(fiveSharkSpeciesWithMostFatalities);
            AssertBoolEqualsTrue(res);
        }

        [TestMethod]
        public void GetSecretCodeQueryTest_ReturnsCorrectResult()
        {
            var expectedCode = "238";

            var secretCode = Queries.GetSecretCodeQuery();

            AssertBoolEqualsTrue(secretCode.Equals(expectedCode));
        }

        [TestMethod]
        public void GovernmentTypePercentagesQueryTest_ReturnsCorrectResult()
        {
            // arrange
            var expectedMessage = "Republic: 59,9%, Monarchy: 18,6%, Territory: 15,8%, AutonomousRegion: 2,0%, ParliamentaryDemocracy: 1,6%, AdministrativeRegion: 0,8%, OverseasCommunity: 0,8%, Federation: 0,4%";

            // act
            var governmentTypePercentages = Queries.GovernmentTypePercentagesQuery().Replace('.', ',');

            // assert
            Assert.AreEqual(expectedMessage, governmentTypePercentages, "Actual output message does not correspond to expected one");
        }

        [TestMethod]
        public void VeryLuckyPeopleCountQueryTest_ReturnsCorrectResult()
        {
            var veryLuckyPeopleCount = Queries.VeryLuckyPeopleCountQuery();

            AssertBoolEqualsTrue(veryLuckyPeopleCount.Equals(2));
        }

        [TestMethod]
        public void LightestSharksInSouthAmericaQueryTest_ReturnsCorrectResult()
        {
            var lightestSharksInSouthAmerica = Queries.LightestSharksInSouthAmericaQuery();

            AssertBoolEqualsTrue(lightestSharksInSouthAmerica != null);

            var sharkIds = lightestSharksInSouthAmerica
                .SelectMany(tuple => tuple.Item2)
                .Select(species => species.Id)
                .ToList();

            var expectedSharkIds = new List<int>
            {
                15, 13, 12, 14, 7, 2, 17, 15, 4, 4, 7, 3, 13, 17, 4, 13, 3, 17, 15
            };

            AssertBoolEqualsTrue(sharkIds.OrderBy(s => s).SequenceEqual(expectedSharkIds.OrderBy(s => s)));

            var sizes = lightestSharksInSouthAmerica
                .Select(tuple => tuple.Item2.Count)
                .ToList();

            var expectedSizes = new List<int>
            {
                1, 0, 8, 3, 2, 0, 0, 1, 0, 0, 0, 0, 1,3
            };

            AssertBoolEqualsTrue(sizes.OrderBy(s => s).SequenceEqual(expectedSizes.OrderBy(s => s)));

            var countries = lightestSharksInSouthAmerica
                .Select(tuple => tuple.Item1).ToList();

            var expectedCountries = new List<string>
            {
                "Argentina",
                "Bolivia",
                "Brazil",
                "Chile",
                "Ecuador",
                "Falkland Islands",
                "French Guiana",
                "Guyana",
                "Colombia",
                "Paraguay",
                "Peru",
                "Suriname",
                "Uruguay",
                "Venezuela"
            };

            AssertBoolEqualsTrue(countries.OrderBy(c => c).SequenceEqual(expectedCountries.OrderBy(c => c)));
        }

        [TestMethod]
        public void ContinentInfoAboutSurfersQueryTest_ReturnsCorrectResult()
        {
            var expectedResult = new Dictionary<string, Tuple<int, double>>
            {
                { "Central America", new Tuple<int, double>(4, 20.25) },
                { "Australia", new Tuple<int, double>(23, 24.96) },
                { "Asia", new Tuple<int, double>(2, 58) },
                { "Africa", new Tuple<int, double>(8, 22.33) },
                { "Europe", new Tuple<int, double>(1, 47) },
                { "South America", new Tuple<int, double>(4, 19.67) }
            };

            var continentInfoAboutSurfers = Queries.ContinentInfoAboutSurfersQuery();

            AssertBoolEqualsTrue(expectedResult.OrderBy(x => x.Key).SequenceEqual(continentInfoAboutSurfers.OrderBy(x => x.Key)));
        }

        [TestMethod]
        public void InfoAboutPeopleThatNamesStartsWithCAndWasInBahamasQueryTest_ReturnsCorrectResult()
        {
            var expectedResult = new List<string>
            {
                "Captain Masson was attacked in Bahamas by Rhincodon typus",
                "C.D. Dollar was attacked in Bahamas by Carcharhinus brachyurus",
                "Carl James Harth was attacked in Bahamas by Carcharhinus brachyurus",
                "Carl Starling was attacked in Bahamas by Carcharhinus amblyrhynchos"
            };

            var infoAboutPeopleThatNamesStartsWithCAndWasInBahamas =
                Queries.InfoAboutPeopleThatNamesStartsWithCAndWasInBahamasQuery();

            var res = infoAboutPeopleThatNamesStartsWithCAndWasInBahamas.OrderBy(x => x)
                .SequenceEqual(expectedResult.OrderBy(x => x));
            AssertBoolEqualsTrue(res);
        }

        [TestMethod]
        public void InfoAboutPeopleThatWasInBahamasHeroicModeQueryTest_ReturnsCorrectResult()
        {
            var expectedResult = new List<string>
            {
                #region PeopleInBahamasAttackedBySharks
                "male was attacked by Isurus oxyrinchus",
                "Patricia Hodge was attacked by Sphyrna lewini",
                "Karl Kuchnow was attacked by Carcharodon carcharias",
                "Doug Perrine was attacked by Carcharhinus brevipinna",
                "Captain Masson was attacked by Rhincodon typus",
                "Kevin G. Schlusemeyer was attacked by Carcharhinus obscurus",
                "14' boat, occupant: Jonathan Leodorn was attacked by Ginglymostoma cirratum",
                "Jerry Greenberg was attacked by Carcharhinus obscurus",
                "Bruce Johnson, rescuer was attacked by Isurus oxyrinchus",
                "Philip Sweeting was attacked by Isurus oxyrinchus",
                "C.D. Dollar was attacked by Carcharhinus brachyurus",
                "Stanton Waterman was attacked by Carcharodon carcharias",
                "Francisco Edmund Blanc, a scientist from National Museum in Paris was attacked by Carcharodon carcharias",
                "Roy Pinder was attacked by Carcharhinus brevipinna",
                "Joanie Regan was attacked by Carcharodon carcharias",
                "Richard  Winer was attacked by Carcharodon carcharias",
                "young girl was attacked by Orectolobus hutchinsi",
                "12' skiff, occupant: E.R.F. Johnson was attacked by Carcharodon carcharias",
                "E.F. MacEwan was attacked by Carcharhinus limbatus",
                "Nick Raich was attacked by Carcharodon carcharias",
                "Krishna Thompson was attacked by Isurus oxyrinchus",
                "Kevin King was attacked by Isurus oxyrinchus",
                "James Douglas Munn was attacked by Sphyrna lewini",
                "John DeBry was attacked by Rhincodon typus",
                "John Petty was attacked by Notorynchus cepedianus",
                "male was attacked by Isurus oxyrinchus",
                "Sean Connelly was attacked by Carcharodon carcharias",
                "Mr. Wichman was attacked by Sphyrna lewini",
                "Tip Stanley was attacked by Carcharias taurus",
                "Roger Yost was attacked by Orectolobus hutchinsi",
                "Luis Hernandez was attacked by Carcharodon carcharias",
                "Max Briggs was attacked by Carcharhinus amblyrhynchos",
                "Markus Groh was attacked by Prionace glauca",
                "male, a sponge Diver was attacked by Carcharhinus brachyurus",
                "Michael Dornellas was attacked by Carcharhinus obscurus",
                "Henry Kreckman was attacked by Notorynchus cepedianus",
                "Katie Hester was attacked by Rhincodon typus",
                "Mark Adams was attacked by Carcharodon carcharias",
                "Leslie Gano was attacked by Orectolobus hutchinsi",
                "Whitefield Rolle was attacked by Sphyrna lewini",
                "Nixon Pierre was attacked by Carcharhinus brachyurus",
                "Sabrina Garcia was attacked by Sphyrna lewini",
                "Benjamin Brown was attacked by Galeocerdo cuvier",
                "Andrew Hindley was attacked by Ginglymostoma cirratum",
                "Bryan Collins was attacked by Galeocerdo cuvier",
                "male was attacked by Rhincodon typus",
                "Kerry Anderson was attacked by Notorynchus cepedianus",
                "Lacy Webb was attacked by Carcharodon carcharias",
                "male was attacked by Carcharias taurus",
                "male was attacked by Carcharhinus obscurus",
                "Russell Easton was attacked by Ginglymostoma cirratum",
                "Wolfgang Leander was attacked by Negaprion brevirostris",
                "Richard Horton was attacked by Ginglymostoma cirratum",
                "Kent Bonde was attacked by Carcharhinus obscurus",
                "Robert Gunn was attacked by Carcharhinus plumbeus",
                "Jim Abernethy was attacked by Ginglymostoma cirratum",
                "Derek Mitchell was attacked by Carcharodon carcharias",
                "Alton Curtis was attacked by Carcharhinus brachyurus",
                "male was attacked by Carcharhinus plumbeus",
                "Burgess & 2 seamen was attacked by Carcharias taurus",
                "Wilber Wood was attacked by Orectolobus hutchinsi",
                "boy was attacked by Carcharhinus brevipinna",
                "Erik Norrie was attacked by Sphyrna lewini",
                "Scott Curatolo-Wagemann was attacked by Ginglymostoma cirratum",
                "Kenny Isham was attacked by Carcharhinus brachyurus",
                "Lowell Nickerson was attacked by Carcharodon carcharias",
                "Peter Albury was attacked by Notorynchus cepedianus",
                "Wyatt Walker was attacked by Ginglymostoma cirratum",
                "William Barnes was attacked by Carcharhinus brachyurus",
                "Valerie Fortunato was attacked by Lamna ditropis",
                "Ken Austin was attacked by Carcharhinus obscurus",
                "John Fenton was attacked by Carcharodon carcharias",
                "Jose Molla was attacked by Carcharodon carcharias",
                "Jane Engle was attacked by Carcharhinus obscurus",
                "Judson Newton was attacked by Carcharhinus limbatus",
                "John Cooper was attacked by Ginglymostoma cirratum",
                "Herbert J. Mann was attacked by Sphyrna lewini",
                "Bruce Cease was attacked by Isurus oxyrinchus",
                "Judy St. Clair was attacked by Ginglymostoma cirratum",
                "Larry Press was attacked by Carcharias taurus",
                "male from pleasure craft Press On Regardless was attacked by Galeocerdo cuvier",
                "Robert Marx was attacked by Notorynchus cepedianus",
                "Renata Foucre was attacked by Carcharhinus obscurus",
                "Hayward Thomas & Shalton Barr was attacked by Sphyrna lewini",
                "Bill Whitman was attacked by Carcharhinus leucas",
                "Eric Gijsendorfer was attacked by Carcharhinus obscurus",
                "Carl James Harth was attacked by Carcharhinus brachyurus",
                "Carl Starling was attacked by Carcharhinus amblyrhynchos",
                "George Vanderbilt was attacked by Carcharias taurus",
                "Kevin Paffrath was attacked by Carcharhinus brachyurus",
                "Erich Ritter was attacked by Rhincodon typus",
                "unknown was attacked by Carcharhinus brevipinna",
                "a pilot was attacked by Carcharhinus leucas",
                "Michael Beach was attacked by Notorynchus cepedianus",
                "Omar Karim Huneidi was attacked by Carcharhinus amblyrhynchos",
                "Richard Pinder was attacked by Carcharhinus brachyurus"
                #endregion
            };

            var infoAboutPeopleThatWasInBahamas =
                Queries.InfoAboutPeopleThatWasInBahamasHeroicModeQuery();

            var res = infoAboutPeopleThatWasInBahamas.OrderBy(x => x)
                .SequenceEqual(expectedResult.OrderBy(x => x));
            AssertBoolEqualsTrue(res);

        }

        [TestMethod]
        public void InfoAboutFinesInEuropeQueryTest_ReturnsCorrectResult()
        {
            var infoAboutFinesInEurope = Queries.InfoAboutFinesInEuropeQuery();

            var states = infoAboutFinesInEurope.Select(state => state.Split(':')[0]);

            var expectedStates = new List<string>
            {
                #region States
                "Italy",
                "Croatia",
                "Greece",
                "France",
                "Ireland",
                "Albania",
                "Andorra",
                "Austria",
                "Belarus",
                "Belgium",
                "Bosnia and Herzegovina",
                "Bulgaria",
                "Czech Republic",
                "Denmark",
                "Estonia",
                "Faroe Islands",
                "Finland",
                "Germany",
                "Gibraltar",
                "Guernsey",
                "Holy See (Vatican City)",
                "Hungary",
                "Iceland",
                "Isle of Man",
                "Jersey",
                "Kosovo",
                "Latvia",
                "Liechtenstein",
                "Lithuania",
                "Luxembourg"
                #endregion
            };

            AssertBoolEqualsTrue(expectedStates.SequenceEqual(states));

            var fines = infoAboutFinesInEurope
                .Select(state => state.Split(':')[1])
                .Take(6);

            var exptectedFines = new List<string>
            {
                " 17900 EUR",
                " 8900 HRK",
                " 6750 EUR",
                " 3150 EUR",
                " 300 EUR",
                " 0 ALL"
            };
            AssertBoolEqualsTrue(exptectedFines.SequenceEqual(fines));
        }

        #region TestHelperEqualityMethods

        private static bool CheckTwoCollections<T1, T2>(IDictionary<T1, T2> first, IDictionary<T1, T2> second)
        {
            return first != null && second != null && first.Count == second.Count;
        }

        private static bool SimpleDictionaryEquals<T1, T2>(IDictionary<T1, T2> first, IDictionary<T1, T2> second)
        {
            return CheckTwoCollections(first, second) &&
                first.All(keyValuePair => second.ContainsKey(keyValuePair.Key) && keyValuePair.Value.Equals(second[keyValuePair.Key]));
        }

        private static bool ComplexDictionaryEquals<T1, T2>(Dictionary<T1, List<T2>> first, IDictionary<T1, List<T2>> second) where T2 : class
        {
            return CheckTwoCollections(first, second) &&
                first.All(keyValuePair => second.ContainsKey(keyValuePair.Key) && keyValuePair.Value.SequenceEqual(second[keyValuePair.Key]));
        }

        private static bool ComplexDictionaryEquals<T1, T2, T3>(IDictionary<T1, Dictionary<T2, List<T3>>> first,
            IDictionary<T1, Dictionary<T2, List<T3>>> second) where T3 : class
        {
            if (!CheckTwoCollections(first, second))
            {
                return false;
            }
            for (var i = 0; i < first.Count; i++)
            {
                var firstInnerDictionary = first.ElementAt(i).Value;
                if (!second.Select(item => item.Key).Contains(first.ElementAt(i).Key))
                {
                    return false;
                }
                var secondInnerDictionary = second.First(item => item.Key.Equals(first.ElementAt(i).Key)).Value;


                if (firstInnerDictionary.Count != secondInnerDictionary.Count)
                {
                    return false;
                }
                for (var j = 0; j < firstInnerDictionary.Count; j++)
                {
                    var firstInnerList = firstInnerDictionary.ElementAt(j).Value;
                    if (!second.Select(item => item.Key).Contains(first.ElementAt(j).Key))
                    {
                        return false;
                    }
                    var secondInnerList = secondInnerDictionary.First(item => item.Key.Equals(firstInnerDictionary.ElementAt(j).Key)).Value;
                    if (firstInnerList.Count != secondInnerList.Count)
                    {
                        return false;
                    }
                    if (firstInnerList.Where((t, k) => !secondInnerList.Contains(firstInnerList.ElementAt(k))).Any())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static void AssertBoolEqualsTrue(bool res)
        {
            Assert.AreEqual(true, res, "Actual result and the expected one are not equal.");
        }

        #endregion
    }
}
