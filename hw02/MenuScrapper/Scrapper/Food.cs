using System;

namespace MenuScrapper
{
    /// <summary>
    /// This struct represent one of food in day menu.
    /// This struct is immutable.
    /// </summary>
    public struct Food
    {
        public string Description { get; private set; }
        public int? Price { get; private set; }

        /// <summary>
        /// Initializes new instance of struct Food.
        /// </summary>
        /// <param name="description">Description of food.</param>
        /// <param name="price">Price of food.</param>
        public Food(string description, int? price)
        {
            Description = description.Trim().Replace('\t', ' ');
            Price = price;
        }

        /// <summary>
        /// Creates string representation of food.
        /// Description is aligned to left, price to right. Added trailing dots.
        /// If description is too long to fit on one line, it breaks in more lines. Every nexts is intended by 3 spaces (because of line numbers)
        /// </summary>
        /// <returns>String from this food.</returns>
        public override string ToString()
        {
            string price = Price.HasValue ? Price.ToString() : "-";
            int consoleWidth = Console.WindowWidth - 5;
            int priceWidth = Utils.GetIntLength(Price ?? 1) + 3;
            return String.Format($"{Utils.BreakStringIntoLines(Description, consoleWidth - priceWidth)}{price} Kč");
        }
    }
}