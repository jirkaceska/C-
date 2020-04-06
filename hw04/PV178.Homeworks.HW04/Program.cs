using PV178.Homeworks.HW04.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PV178.Homeworks.HW04
{
    public class Program
    {
        static void Main(string[] args)
        {
            var queries = new Queries();
            foreach(var x in queries.InfoAboutFinesInEuropeQuery())
            {
                System.Console.WriteLine(x);
            }
            Console.WriteLine();
        }
    }
}
