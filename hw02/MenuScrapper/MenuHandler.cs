using MenuScrapper.Enums;
using System;
using System.Linq;
using System.Net;

namespace MenuScrapper
{
    /// <summary>
    /// This class display main menu, restaurant menu and process command from stdin.
    /// </summary>
    public class MenuHandler
    {
        private Options selectedOption;
        private Restaurants selectedRestaurant;

        /// <summary>
        /// Set selectedRestaurant bit to one (as only one)
        /// </summary>
        /// <param name="value">Index of bit to set to 1.</param>
        /// <returns>True if set was successful, false otherwise.</returns>
        private bool SetSelectedRestaurant(int value)
        {
            if (1 <= value && value <= Scrapper.restaurantsCount)
            {
                selectedRestaurant = (Restaurants)(1 << (value - 1));
                return true;
            }
            Console.WriteLine("Neplatné číslo restaurace!");
            return false;
        }

        private FlagDayOfWeek selectedDay;

        /// <summary>
        /// Set selectedDay bit to one (as only one)
        /// </summary>
        /// <param name="value">Index of bit to set to 1.</param>
        /// <returns>True if set was successful, false otherwise.</returns>
        private bool SetSelectedDay(int value)
        {
            if (1 <= value && value <= 5)
            {
                selectedDay = (FlagDayOfWeek)(1 << value);
                return true;
            }
            Console.WriteLine("Neplatné číslo dne!");
            return false;
        }

        /// <summary>
        /// Set selected day bit by day of week.
        /// </summary>
        /// <param name="value">Index of bit to set to 1.</param>
        /// <returns>True if set was successful, false otherwise.</returns>
        private bool SetSelectedDay(DayOfWeek value)
        {
            if (value == DayOfWeek.Saturday || value == DayOfWeek.Sunday)
            {
                Console.WriteLine("V menu dnes nejsou žádné položky, vraťe se v pondělí.");
                Init();
                return false;
            }
            return SetSelectedDay((int)value);
        }

        private readonly Scrapper scrapper;
        public bool IsFinished { get; private set; }

        public static void PrintHelp()
        {
            PrintLine();
            Console.WriteLine("1 - Vypiš dnešní menu všech restaurací.");
            Console.WriteLine("2 - Vypiš dnešní menu vybrané restaurace.");
            Console.WriteLine("3 - Vypiš menu všech restaurací pro vybraný den.");
            Console.WriteLine("4 - Vypiš menu vybraného dne a restaurace.");
            Console.WriteLine("5 - Vypiš týdenní menu všech restaurací.");
            Console.WriteLine("6 - Vyhledej řetězec v jídelních lístcích.");
            Console.WriteLine("7 - Ukončit.");
            Console.WriteLine();
            Console.WriteLine("Vyber možnost z intervalu <1,7>:");
        }

        public static void PrintRestaurantOptions()
        {
            PrintLine();
            Console.WriteLine("1 - U Dřeváka Beer & Grill");
            Console.WriteLine("2 - Al Capone - Pizzeria Ristorante");
            Console.WriteLine("3 - Plzeňský dvůr");
            Console.WriteLine();
            Console.WriteLine("Vyber možnost z intervalu <1,{0}>:", Scrapper.restaurantsCount);
        }

        public static void PrintDayOptions()
        {
            PrintLine();
            Console.WriteLine("1 - Pondělí");
            Console.WriteLine("2 - Úterý");
            Console.WriteLine("3 - Středa");
            Console.WriteLine("4 - Čtvrtek");
            Console.WriteLine("5 - Pátek");            Console.WriteLine("Vyber možnost z intervalu <1,5>:");
        }

        public MenuHandler()
        {
            scrapper = Scrapper.getScrapperInstance();
            Init();
        }

        /// <summary>
        /// Restart menu handler to default state - after each completed action.
        /// </summary>
        private void Init()
        {
            selectedOption = Options.None;
            IsFinished = false;
            selectedDay = 0;
            selectedRestaurant = 0;
            PrintHelp();
        }

        public void ParseOption(string option)
        {
            if (selectedOption == Options.Search)
            {
                Search(option);
                return;
            }
            if (!Int32.TryParse(option, out int opt))
            {
                Error();
                return;
            }

            switch (selectedOption)
            {
                case Options.None:
                    SelectOption(opt);
                    break;

                case Options.TodaySelectedRestaurant:
                    SetSelectedDay(DateTime.Now.DayOfWeek);
                    SetSelectedRestaurant(opt);
                    break;

                case Options.SelectedDayAllRestaurants:
                    SetSelectedDay(opt);
                    selectedRestaurant = (Restaurants)~0;
                    break;

                case Options.SelectedDaySelectedRestaurant:
                    if (selectedRestaurant == 0 && SetSelectedRestaurant(opt)) PrintDayOptions();
                    else SetSelectedDay(opt);
                    break;

                default:
                    Error();
                    break;
            }

            Display();
        }

        private void Display()
        {
            Display((_) => true);
        }

        private void Display(Func<DayMenu, bool> customFilter)
        {
            if (selectedDay != 0 && selectedRestaurant != 0)
            {
                for (int i = 0; i < Scrapper.restaurantsCount; i++)
                {
                    if (selectedRestaurant.HasFlag((Restaurants)(1 << i)))
                    {
                        try
                        {
                            Restaurant restaurant = scrapper[i];

                            var dayMenus = restaurant.
                                Where((menu) => selectedDay.HasFlag(menu.Day.FlagDayOfWeek())).
                                Where(customFilter);

                            if (dayMenus.Count() > 0)
                                PrintRestaurant(restaurant);
                            foreach (DayMenu menu in dayMenus)
                                Console.WriteLine(menu);
                        }
                        catch (WebException e)
                        {
                            PrintLine();
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Zkontrolujte své připojení k internetu. Poté zkuste zobrazit menu znovu.");
                        }
                        catch (Exception e)
                        {
                            PrintLine();
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                Init();
            }
        }

        private void PrintRestaurant(Restaurant r)
        {
            PrintLine();
            Console.WriteLine(r);
            PrintLine();
        }

        private void Search(string str)
        {
            selectedDay = (FlagDayOfWeek)~0;
            selectedRestaurant = (Restaurants)~0;

            Display((dayMenu) => dayMenu.Contains(str));
        }

        private void SelectOption(int opt)
        {
            if (Enum.IsDefined(typeof(Options), opt) && opt != 0)
            {
                selectedOption = (Options)opt;
            }
            else
            {
                Error();
                return;
            }
            switch (selectedOption)
            {
                case Options.WeekAllRestaurants:
                    selectedRestaurant = (Restaurants)~0;
                    selectedDay = (FlagDayOfWeek)~0;
                    break;

                case Options.TodayAllRestaurants:
                    selectedRestaurant = (Restaurants)~0;
                    SetSelectedDay(DateTime.Now.DayOfWeek);
                    break;

                case Options.Search:
                    Console.WriteLine("Napište hledaný řetězec:");
                    break;

                case Options.SelectedDayAllRestaurants:
                    PrintDayOptions();
                    break;

                case Options.SelectedDaySelectedRestaurant:
                case Options.TodaySelectedRestaurant:
                    PrintRestaurantOptions();
                    break;

                case Options.Quit:
                    IsFinished = true;
                    break;
            }
        }

        public static void Error() => Console.WriteLine("Neplatná možnost!");

        public static void PrintLine() => Console.Write("".PadRight(Console.WindowWidth, '_'));
    }
}