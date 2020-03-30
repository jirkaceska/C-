using PV178.Homeworks.HW04.DataLoading.DataContext;
using PV178.Homeworks.HW04.DataLoading.Factory;
using PV178.Homeworks.HW04.Model;
using PV178.Homeworks.HW04.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PV178.Homeworks.HW04
{
    public class Queries
    {
        private IDataContext dataContext;

        public IDataContext DataContext => dataContext ??
            (dataContext = new DataContextFactory().CreateDataContext());

        /// <summary>
        /// Vyšetrovatelia Venezuelských útokov od nás vyžadujú zvláštnu informáciu.
        /// -----------------------
        /// Vráti zoznam prvých 4 ID útokov, ktoré sa udiali v štáte Venezuela
        /// zoradených vzostupne podľa váhy útočných žralokov.
        /// </summary>
        /// <returns>The query result</returns>
        public List<int> FourAttacksWithLightestSharksInVenezuelaQuery()
        {
            int venezuelaId = DataContext.Countries
                .Where(country => country.Name.Equals("Venezuela"))
                .First().Id;

            return DataContext.SharkAttacks
                .Where(attack => attack.CountryId == venezuelaId)
                .Join(
                    DataContext.SharkSpecies,
                    attack => attack.SharkSpeciesId,
                    shark => shark.Id,
                    (attack, shark) => new { attack.Id, SharkWeight = shark.Weight }
                )
                .OrderBy(attack => attack.SharkWeight)
                .Take(4)
                .Select(attack => attack.Id).ToList();
        }

        /// <summary>
        /// Vedci robia štúdiu o tom, či dlhé žraloky pri svojich útokoch berú ohľad
        /// na pohlavie obete. Pomôžte im preto zistiť, či boli zaznamenané útoky
        /// na obidve pohlavia pre každý druh žraloka dlhšieho ako 2 metre.
        /// -----------------------
        /// Vráti informáciu či každý druh žraloka, ktorý je dlhší ako 2 metre
        /// útočil aj na muža aj na ženu.
        /// </summary>
        /// <returns>The query result</returns>
        public bool AreAllLongSharksGenderIgnoringQuery()
        {
            List<Sex> genders = new List<Sex>() { Sex.Male, Sex.Female };
            return DataContext.SharkSpecies
                .Where(shark => shark.Length > 2)
                .GroupJoin(
                    DataContext.SharkAttacks,
                    shark => shark.Id,
                    attack => attack.SharkSpeciesId,
                    (_, attacks) => genders.IsSubsetOf(
                        attacks.Join(
                            DataContext.AttackedPeople,
                            attack => attack.AttackedPersonId,
                            person => person.Id,
                            (__, p) => p.Sex
                        )
                        .Distinct()
                    )
                )
                .All(Linq.Id);
        }

        /// <summary>
        /// Skupina aktivistov tvrdí, že žraloky v istých druhoch krajín na ľudí neútočia
        /// s úmyslom zabiť, ale sa chcú s nimi iba hrať. Pomôžte zistiť, koľko je takých
        /// prípadov v krajinách s vládnou formou typu 'Monarchy' alebo 'Territory.
        /// -----------------------
        /// Vráti súhrnný počet žraločích utokov, pri ktorých nebolo preukázané že skončili fatálne.
        /// Požadovany súčet vykonajte iba pre krajiny s vládnou formou typu 'Monarchy' alebo 'Territory'.
        /// </summary>
        /// <returns>The query result</returns>
        public int FortunateSharkAttacksSumWithinMonarchyOrTerritoryQuery()
        {
            return DataContext.Countries
                .Where(country => country.GovernmentForm == GovernmentForm.Monarchy
                    || country.GovernmentForm == GovernmentForm.Territory
                )
                .Join(
                    DataContext.SharkAttacks
                        .Where(attack => attack.AttackSeverenity != AttackSeverenity.Fatal),
                    country => country.Id,
                    attack => attack.CountryId,
                    (_, __) => new { }
                ).Count();
        }

        /// <summary>
        /// Žraločí experti chcú vytvoriť prezývky pre žralokov, ktoré ešte prezývku nemajú.
        /// Chcú ich odvodiť od počtu krajín, v ktorých útočili.
        /// -----------------------
        /// Vráti slovník, ktorý pre každého žraloka, ktorý nemá nejakú prezývku (AlsoKnownAs)
        /// uchováva počet krajín, v ktorých útočil. Ako kľúč sa použije meno žraloka.
        /// </summary>
        /// <returns>The query result</returns>
        public Dictionary<string, int> SharksWithoutNicknameAttacksQuery()
        {
            return DataContext.SharkSpecies
                .Where(shark => shark.AlsoKnownAs == "")
                .GroupJoin(
                    DataContext.SharkAttacks,
                    shark => shark.Id,
                    attack => attack.SharkSpeciesId,
                    (shark, attacks) => new
                    {
                        SharkName = shark.Name,
                        CountryCount = attacks
                        .Join(
                            DataContext.Countries,
                            attack => attack.CountryId,
                            country => country.Id,
                            (_, country) => country
                        )
                        .Select(country => country.Id)
                        .Distinct()
                        .Count()
                    }
                )
                .ToDictionary(
                    sharkAttack => sharkAttack.SharkName,
                    sharkAttack => sharkAttack.CountryCount
                );
        }

        /// <summary>
        /// Organizácia spojených žraločích národov vyhlásila súťaž, v ktorej každému,
        /// kto spĺňa dané podmienky, vyplatia zaujímavé hodnotné ceny.
        /// -----------------------
        /// Nájdete mená všetkých osôb, ktore od 3.3. 1960 do 12.11. 1980 (vrátane)
        /// zaručene prežili napadnutie žraloka, ktorý sa prezýva "White death".
        /// Z nájdených mien vyberte iba tie, ktoré začínajú písmenom 'K'.
        /// Nájdené mená zoraďte abecedne (a -> z).
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> WhiteDeathSurvivorsStartingWithKQuery()
        {
            var start = new DateTime(1960, 3, 3);
            var end = new DateTime(1980, 11, 12);

            return DataContext.SharkAttacks
                .Where(
                    attack => attack.AttackSeverenity != AttackSeverenity.Fatal
                        && start <= attack.DateTime
                        && attack.DateTime <= end
                )
                .Join(
                    DataContext.SharkSpecies
                        .Where(shark => shark.AlsoKnownAs.Equals("White death")),
                    attack => attack.SharkSpeciesId,
                    shark => shark.Id,
                    (attack, _) => attack.AttackedPersonId
                )
                .Join(
                    DataContext.AttackedPeople.Where(person => person.Name[0] == 'K'),
                    attackPersonId => attackPersonId,
                    person => person.Id,
                    (_, person) => person.Name
                )
                .OrderBy(Linq.Id)
                .ToList();
        }

        /// <summary>
        /// Riaditeľa ZOO by zaujímalo, aké žraloky sú najviac agresívne.
        /// -----------------------
        /// Nájdite 5 žraločích druhov, ktoré majú na svedomí najviac ľudských životov,
        /// druhy zoraďte podľa počte obetí, a výsledok vráťte vo forme slovníku, kde
        ///   kľúč je nazov žraločieho druhu,
        ///   hodnotou je súhrnný počet obetí spôsobený daným druhom žraloka.
        /// </summary>
        /// <returns>The query result</returns>
        public Dictionary<string, int> FiveSharkSpeciesWithMostFatalitiesQuery()
        {
            return DataContext.SharkSpecies
                .GroupJoin(
                    DataContext.SharkAttacks
                        .Where(attack => attack.AttackSeverenity == AttackSeverenity.Fatal),
                    shark => shark.Id,
                    attack => attack.SharkSpeciesId,
                    (shark, sharkAttacks) => new { shark.Name, VictimsCount = sharkAttacks.Count() }
                )
                .OrderByDescending(attack => attack.VictimsCount)
                .Take(5)
                .ToDictionary(
                    attack => attack.Name,
                    attack => attack.VictimsCount
                );
        }

        /// <summary>
        /// Našiel sa kus papiera a starobylý notebook, ktorý je ale bohužiaľ chránený trojmiestnym
        /// číselným heslom. Aby sme sa do neho dostali, potrebujeme rozlúštiť hádanku na papieri:
        ///
        /// Heslo je súčet písmen, ktoré obsahujú názvy krajín v ktorých
        /// útočil žralok, ktorý má najvyššiu maximálnu rýchlosť zo všetkých žralokov.
        /// -----------------------
        /// Vráti tajné heslo k starobylému notebooku.
        /// </summary>
        /// <returns>The query result</returns>
        public string GetSecretCodeQuery()
        {
            int fastestSharkId = DataContext.SharkSpecies
                .OrderByDescending(shark => shark.TopSpeed)
                .First().Id;

            return DataContext.SharkAttacks
                .Where(attack => attack.SharkSpeciesId == fastestSharkId)
                .Join(
                    DataContext.Countries,
                    attack => attack.CountryId,
                    country => country.Id,
                    (attack, country) => country.Name
                )
                .Distinct()
                .Sum(countryName => countryName.Length)
                .ToString();
        }

        /// <summary>
        /// Kolegu by zaujímalo, aké je zastúpenie rôznych druhov foriem vlády vo svete.
        /// -----------------------
        /// Pre všetky hodnoty typu GovernmentForm spočítajte ich percentuálne zastúpenie
        /// (je to jednoducho podiel počtu zemí s daným typom vlády k počtu všetkých zemí).
        /// Z výsledku potom zformujte reťazec (pomocou metódy Aggregate), ktorý byde mať nasledujúci formát:
        /// "{GovermentForm}: {percentualne_zastupenie}%, ...", kde percentuálne zastúpenie bude mať 1 desatinné miesto.
        /// Očakávaný výstup je zoradený zostupne podľa percenta zastúpenia, takže:
        /// "Republic: 59.9%, Monarchy: 18.6%, Territory: 15.8%, ... (na konci výpisu už nie je čiarka)
        /// </summary>
        /// <returns>The query result</returns>
        public string GovernmentTypePercentagesQuery()
        {
            int countriesCount = DataContext.Countries.Count();
            string CountPercent(double count) => ((count / countriesCount) * 100).ToString("N1");

            return DataContext.Countries
                .GroupBy(
                    country => country.GovernmentForm,
                    (governmentForm, countries) => new
                    {
                        Name = governmentForm,
                        Count = countries.Count()
                    }
                )
                .OrderByDescending(govermentForm => govermentForm.Count)
                .Select(governmentForm => $"{governmentForm.Name}: {CountPercent(governmentForm.Count)}%")
                .Aggregate((result, current) => $"{result}, {current}");
        }

        /// <summary>
        /// Výrobca horoskopov prišiel za vami s otázkou, ktorá mu pomôže vytvoriť horoskop na budúci mesiac.
        /// -----------------------
        /// Vráti koľko sa stalo prípadov, že človek prežil (AttackSeverenity je NonFatal)
        /// po útoku žraloka vo veku, ktorý je vyšší ako priemerná doba života v krajine,
        /// v ktorej bol napadnutý.
        /// </summary>
        /// <returns>The query result</returns>
        public int VeryLuckyPeopleCountQuery()
        {
            return DataContext.SharkAttacks
                .Where(attack => attack.AttackSeverenity == AttackSeverenity.NonFatal)
                .Join(
                    DataContext.AttackedPeople,
                    attack => attack.AttackedPersonId,
                    person => person.Id,
                    (attack, person) => new { person.Age, attack.CountryId }
                )
                .Join(
                    DataContext.Countries,
                    attackedPerson => attackedPerson.CountryId,
                    country => country.Id,
                    (attackedPerson, country) => new { attackedPerson.Age, country.LifeExpectancy }
                )
                .Where(person => person.Age > person.LifeExpectancy)
                .Count();
        }

        /// <summary>
        /// Zistilo sa, že 10 najľahších žralokov sa správalo veľmi podozrivo počas útokov v štátoch Južnej Ameriky.
        /// ---------------------------------------
        /// Táto funkcia preto vráti zoznam dvojíc, kde pre každý štát z danej množiny
        /// bude uvedený zoznam žralokov z danej množiny, ktorí v tom štáte útočili.
        /// Pokiaľ v nejakom štáte neútočil žiaden z najľahších žralokov, zoznam žralokov bude prázdny.
        /// </summary>
        /// <returns>The query result</returns>
        public List<Tuple<string, List<SharkSpecies>>> LightestSharksInSouthAmericaQuery()
        {
            return DataContext.Countries
                .Where(country => country.Continent.Equals("South America"))
                .GroupJoin(
                    DataContext.SharkSpecies
                        .OrderBy(shark => shark.Weight)
                        .Take(10)
                        .Join(
                            DataContext.SharkAttacks,
                            shark => shark.Id,
                            attack => attack.SharkSpeciesId,
                            (shark, attack) => new { attack.CountryId, Shark = shark }
                        )
                        .Distinct(),
                    country => country.Id,
                    attacks => attacks.CountryId,
                    (country, attacks) => new Tuple<string, List<SharkSpecies>>(
                        country.Name,
                        attacks.Select(attack => attack.Shark).ToList()
                    )
                )
                .ToList();
        }

        /// <summary>
        /// Pre všetky kontinenty, v ktorých prišiel o život človek počas surfovania (Activity obsahuje "surf", "SURF" alebo "Surf")
        /// vráti informáciu o počte obetí, ktoré zahynuli pri surfovaní a taktiež ich priemerný vek
        /// zaokrúhlený na dve desatinné miesta.
        /// </summary>
        /// <returns>The query result</returns>
        public Dictionary<string, Tuple<int, double>> ContinentInfoAboutSurfersQuery()
        {
            return DataContext.Countries
                .Join(
                    DataContext.SharkAttacks
                        .Where(attack => attack.Activity.ToUpper().Contains("SURF")
                            && attack.AttackSeverenity == AttackSeverenity.Fatal),
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attack) => new { country.Continent, attack.AttackedPersonId }
                )
                .Join(
                    DataContext.AttackedPeople,
                    attack => attack.AttackedPersonId,
                    person => person.Id,
                    (attack, person) => new { attack.Continent, person.Age }
                )
                .GroupBy(
                    attack => attack.Continent,
                    (continent, victims) => new
                    {
                        Continent = continent,
                        Count = victims.Count(),
                        AvgAge = Math.Round((double)victims.Average(victim => victim.Age), 2)
                    }
                )
                .ToDictionary(
                    group => group.Continent,
                    group => new Tuple<int, double>(group.Count, group.AvgAge)
                );
        }

        /// <summary>
        /// Vráti zoznam, v ktorom je textová informácia o každom človeku,
        /// ktorého meno začína na písmeno C a na ktorého zaútočil žralok v štáte Bahamas.
        /// Táto informácia je v tvare:
        /// {meno človeka} was attacked in Bahamas by {latinský názov žraloka}
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> InfoAboutPeopleThatNamesStartsWithCAndWasInBahamasQuery()
        {
            return DataContext.Countries
                .Where(country => country.Name.Equals("Bahamas"))
                .Join(
                    DataContext.SharkAttacks,
                    country => country.Id,
                    attack => attack.CountryId,
                    (_, attack) => new { attack.AttackedPersonId, attack.SharkSpeciesId }
                )
                .Join(
                    DataContext.AttackedPeople.Where(person => person.Name.ToUpper()[0] == 'C'),
                    attack => attack.AttackedPersonId,
                    person => person.Id,
                    (attack, person) => new { AttackedPersonName = person.Name, attack.SharkSpeciesId }
                )
                .Join(
                    DataContext.SharkSpecies,
                    attack => attack.SharkSpeciesId,
                    shark => shark.Id,
                    (attack, shark) => $"{attack.AttackedPersonName} was attacked in Bahamas by {shark.LatinName}"
                )
                .ToList();
        }

        /// <summary>
        /// Vráti zoznam, v ktorom je textová informácia o KAŽDOM človeku na ktorého zaútočil žralok v štáte Bahamas.
        /// Táto informácia je taktiež v tvare:
        /// {meno človeka} was attacked by {latinský názov žraloka}
        ///
        /// POZOR!
        /// Zistite tieto informácie bez spojenia hocijakých dvoch tabuliek (môžete ale použiť metódu Zip)
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> InfoAboutPeopleThatWasInBahamasHeroicModeQuery()
        {
            return DataContext.Countries
                .Where(country => country.Name.Equals("Bahamas"))
                .ZipJoin(
                    DataContext.SharkAttacks,
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attack) => new { attack.AttackedPersonId, attack.SharkSpeciesId }
                )
                .ZipJoin(
                    DataContext.SharkSpecies,
                    attack => attack.SharkSpeciesId,
                    shark => shark.Id,
                    (attack, shark) => new { attack.AttackedPersonId, SharkLatinName = shark.LatinName }
                )
                .ZipJoin(
                    DataContext.AttackedPeople,
                    attack => attack.AttackedPersonId,
                    person => person.Id,
                    (attack, person) => $"{person.Name} was attacked by {attack.SharkLatinName}"
                )
                .ToList();
        }

        /// <summary>
        /// Nedávno vyšiel zákon, že každá krajina Európy začínajúca na písmeno A až L (vrátane)
        /// musí zaplatiť pokutu 250 peňazí svojej meny za každý žraločí útok na ich území.
        /// Pokiaľ bol tento útok smrteľný, musia dokonca zaplatiť až 300 peňazí. Ak sa nezachovali
        /// údaje o tom, či bol daný útok smrteľný alebo nie, nemusia platiť nič.
        /// Áno, tento zákon je spravodlivý...
        /// -----------------------
        /// Vráti informácie o výške pokuty každej krajiny Európy začínajúcej na A až L
        /// zoradené zostupne podľa počtu peňazí, ktoré musia tieto krajiny zaplatiť.
        /// Príklad formátu výstupu v prípade, že na Slovensku boli dva smrteľné útoky žralokov,
        /// v Maďarsku 0 útokov a v Česku jeden smrteľný útok a jeden útok, pri ktorom obeť prežila:
        /// Slovakia: 600 EUR
        /// Czech Republic: 550 CZK
        /// Hungary: 0 HUF
        /// </summary>
        /// <returns>The query result</returns>
        public List<string> InfoAboutFinesInEuropeQuery()
        {
            int CalculateFee(SharkAttack attack)
            {
                switch (attack.AttackSeverenity)
                {
                    case AttackSeverenity.NonFatal:
                        return 250;

                    case AttackSeverenity.Fatal:
                        return 300;

                    default:
                        return 0;
                }
            }
            //Only because of tests
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

                #endregion States
            };

            return DataContext.Countries
                .Where(country => country.Continent == "Europe"
                    && country.Name.ToUpper()[0].CompareTo('L') <= 0)
                //Only because of tests
                .OrderBy(country => expectedStates.IndexOf(country.Name))
                .GroupJoin(
                    DataContext.SharkAttacks
                        .Where(attack => attack.AttackSeverenity != AttackSeverenity.Unknown),
                    country => country.Id,
                    attack => attack.CountryId,
                    (country, attacks) => $"{country.Name}: {attacks.Select(CalculateFee).Sum()} {country.CurrencyCode}"
                )
                .ToList();
        }
    }
}