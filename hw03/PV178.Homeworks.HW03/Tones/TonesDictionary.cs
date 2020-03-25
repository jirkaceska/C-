using PV178.Homeworks.HW03.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PV178.Homeworks.HW03.Tones
{
    public class TonesDictionary<T> : Dictionary<char, (T, int)>, ITonesDictionary<T>
    {
        public void AddRange(ICollection<(char key, T name, int frequence)> tones)
        {
            foreach (var (key, name, frequence) in tones)
            {
                Add(key, (name, frequence));
            }
        }

        public (T name, int frequence) GetInfo(char key)
        {
            return this[key];
        }
    }
}
