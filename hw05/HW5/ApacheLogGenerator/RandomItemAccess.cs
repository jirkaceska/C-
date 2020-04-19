using System;
using System.Collections.Generic;

namespace HW5.ApacheLogGenerator
{
    public class RandomItemAccess
    {
        private readonly Random _random = new Random();
        
        public T RetrieveRandomItem<T>(IList<T> collection)
        {
            return collection[_random.Next(collection.Count)];
        }
    }
}
