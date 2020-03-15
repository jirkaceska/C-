using HtmlAgilityPack;
using MenuScrapper.Enums;
using MenuScrapper.Exceptions;
using System;
using System.Linq;

namespace MenuScrapper
{
    /// <summary>
    /// This class stores restaurants menu loaded from web.
    /// To prevent multiple loading of websites this class is singleton - see method getScrapperInstance.
    ///
    /// To add restaraunt to this class:
    ///  - add enum to Restaurants
    ///  - add its switch case to LoadRestaurant
    ///  - add method LoadXXX which is run from LoadRestaurant
    ///  - (add line to print help in MenuHandler)
    /// </summary>
    public class Scrapper
    {
        public static readonly int RestaurantsCount = Enum.GetNames(typeof(Restaurants)).Length;
        private readonly Restaurant[] restaurants = new Restaurant[RestaurantsCount];
        private Restaurants loaded = 0;
        private static Scrapper scrapper;

        /// <summary>
        /// Get ith restaraunt menu if it is already loaded.
        /// Otherwise load it first, then return.
        /// </summary>
        /// <param name="i">Index of restaurant to return.</param>
        /// <returns>Restaurant selected by index i.</returns>
        public Restaurant this[int i]
        {
            get
            {
                Restaurants restaurant = GetEnumFromIndex(i);
                if ((loaded & restaurant) != restaurant)
                {
                    LoadRestaurant(restaurant);
                }
                return restaurants[i];
            }
        }

        /// <summary>
        /// Run restaurant load method by enum - if you extend this class, add case to switch.
        /// </summary>
        /// <param name="restaurant"></param>
        private void LoadRestaurant(Restaurants restaurant)
        {
            switch (restaurant)
            {
                case Restaurants.UDrevaka:
                    LoadUDrevaka();
                    break;

                case Restaurants.AlCapone:
                    LoadAlCapone();
                    break;

                case Restaurants.PlzenskyDvur:
                    LoadPlzenskyDvur();
                    break;

                    /*case Restaurants.StopkovaPivnice:
                        LoadStopkovaPivnice();
                        break;*/
            }
        }

        /// <summary>
        /// Get instance of Scrapper
        /// </summary>
        /// <returns>New instance of scrapper</returns>
        public static Scrapper getScrapperInstance()
        {
            if (scrapper == null)
            {
                scrapper = new Scrapper();
            }
            return scrapper;
        }

        private void LoadUDrevaka()
        {
            DayMenu ParseDay(HtmlNode day)
            {
                string dateStr = day.SelectSingleNode("./div[@class='menu-day']").InnerText;
                DateTime date = Utils.ParseDateTime(dateStr);

                HtmlNodeCollection rows = day.SelectNodes("./div[@class='row']");
                int soupIndex = rows[0].SelectSingleNode("./div").InnerText.IndexOf("Polévka:", 0, 8);
                string soup = soupIndex >= 0 ? rows[0].SelectSingleNode("./div").InnerText.Substring(9) : null;
                Food[] foods = rows
                    .Where((_, index) => index > soupIndex)
                    .Select((row) => new Food(
                        HtmlEntity.DeEntitize(Utils.RemoveLeadingNumbers(
                            row.SelectSingleNode("./div[@class='col-sm-10 col-xs-9']").InnerText)
                        ),
                        Utils.ParsePrice(
                            row.SelectSingleNode("./div[@class='col-sm-2 col-xs-3 special-menu-price']").InnerText
                        )
                    )).ToArray();
                return new DayMenu(date, soup, foods);
            }

            HtmlNode doc = Utils.GetHtmlDoc(Constants.udrevakaUrl).DocumentNode;
            HtmlNode menu = doc.SelectSingleNode("//ul[@class='special-menu pb-xlg']");
            HtmlNodeCollection days = menu.SelectNodes("./li[@class='item-day']");

            DayMenu[] dayMenus = days.Select(ParseDay).ToArray();
            string restaurantName = GetRestaurantName(doc);

            SaveRestaurant(restaurantName, dayMenus, Restaurants.UDrevaka);
        }

        private void LoadAlCapone()
        {
            int rowPerDay = 6;
            HtmlNode doc = Utils.GetHtmlDoc(Constants.alCaponeUrl).DocumentNode;
            HtmlNode menu = doc.SelectSingleNode("//table[@class='table table-responsive']/tbody");
            HtmlNodeCollection rows = menu.SelectNodes("./tr");

            if (rows == null)
            {
                throw new WeekendEmptyException("Pizzeria Alcapone - Brno:\nV menu nejsou o víkendu žádné položky, vraťe se v pondělí.");
            }
            int daysCount = rows.Count / rowPerDay;
            DayMenu[] dayMenus = new DayMenu[daysCount];

            for (int i = 0; i < daysCount; i++)
            {
                string dateStr = rows[i * rowPerDay].SelectSingleNode("./td/h3").InnerText;
                DateTime date = Utils.ParseDateTime(dateStr);

                string soup = rows[i * rowPerDay + 1].SelectSingleNode("./td[2]/h3").InnerText;
                Food[] foods = new Food[4];

                for (int j = 2; j < rowPerDay; j++)
                {
                    HtmlNode actRow = rows[i * rowPerDay + j];

                    string description = HtmlEntity.DeEntitize(actRow.SelectSingleNode("./td[2]/h3").InnerText);
                    int price = Utils.ParsePrice(
                        actRow.SelectSingleNode("./td[3]/h3").InnerText);
                    foods[j - 2] = new Food(description, price);
                }
                dayMenus[i] = new DayMenu(date, soup, foods);
            }

            string restaurantName = GetRestaurantName(doc);
            SaveRestaurant(restaurantName, dayMenus, Restaurants.AlCapone);
        }

        private void LoadPlzenskyDvur()
        {
            HtmlNode doc = Utils.GetHtmlDoc(Constants.plzenskyDvur).DocumentNode;
            HtmlNode menu = doc.SelectSingleNode("//div[@class='listek']/div[@class='tyden']");
            HtmlNodeCollection prices = doc.SelectNodes("//div[@class='listek']/div[@class='tyden_ceny']//td");

            DayMenu ParseDay(HtmlNode title, HtmlNode text)
            {
                string dateStr = title.InnerText;
                DateTime date = Utils.ParseDateTime(dateStr.Split(' ')[1]);
                HtmlNodeCollection rows = text.SelectNodes("./p");
                string soup = rows[0].InnerText;
                Food[] foods = rows
                    .Where((_, index) => index > 0 && index % 2 == 0)
                    .Zip(prices, (HtmlNode food, HtmlNode price) => new Food(
                        HtmlEntity.DeEntitize(food.InnerText.Trim()),
                        Utils.ParsePrice(price.InnerText.Split('-')[1].Trim(), ' ')
                    )).ToArray();

                return new DayMenu(date, soup, foods);
            }

            HtmlNodeCollection titles = menu.SelectNodes("./p[@class='title']");
            HtmlNodeCollection texts = menu.SelectNodes("./div[@class='text']");

            titles.Remove(0);
            texts.Remove(0);
            DayMenu[] dayMenus = titles.Zip(texts, ParseDay).ToArray();

            string restaurantName = GetRestaurantName(doc);
            SaveRestaurant(restaurantName, dayMenus, Restaurants.PlzenskyDvur);
        }

        /*private void LoadStopkovaPivnice()
        {
            HtmlNode doc = Utils.GetHtmlDoc(Constants.stopkovaPivnice).DocumentNode;
            HtmlNode menu = doc.SelectSingleNode("//div[@class='dailyMenuWeek']");

            DayMenu ParseDay(HtmlNode day)
            {
                string dateStr = day.SelectSingleNode("./h2").InnerText.Split('-')[1];
                DateTime date = Utils.ParseDateTime(dateStr);
                HtmlNodeCollection rows = day.SelectNodes(".//tr");
                string soup = rows[0].SelectSingleNode("./td[@class='name']").InnerText;
                Food[] foods = rows
                    .Where((n, i) => i > 0)
                    .Select((row) => new Food(
                        HtmlEntity.DeEntitize(row.SelectSingleNode("./td[@class='name']").InnerText.Trim()),
                        Utils.ParsePrice(row.SelectSingleNode("./td[@class='price']").InnerText, ' ')
                    )).ToArray();

                return new DayMenu(date, soup, foods);
            }

            HtmlNodeCollection days = menu.SelectNodes("./section");

            DayMenu[] dayMenus = days.Select(ParseDay).ToArray();

            string restaurantName = GetRestaurantName(doc);
            SaveRestaurant(restaurantName, dayMenus, Restaurants.StopkovaPivnice);
        }*/

        private void SaveRestaurant(string name, DayMenu[] menu, Restaurants id)
        {
            restaurants[GetIndexFromEnum(id)] = new Restaurant(name, menu);
            loaded |= id;
        }

        private string GetRestaurantName(HtmlNode doc) => HtmlEntity.DeEntitize(doc.SelectSingleNode("//title").InnerText);

        private int GetIndexFromEnum(Restaurants restaurant) => (int)Math.Log((int)restaurant, 2);

        private Restaurants GetEnumFromIndex(int i) => (Restaurants)(1 << i);

        private Scrapper()
        {
        }
    }
}