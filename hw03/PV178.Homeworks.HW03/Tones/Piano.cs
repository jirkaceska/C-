using PV178.Homeworks.HW03.Utils;

namespace PV178.Homeworks.HW03.Tones
{
    public class Piano : IPiano
    {
        private readonly ITonesDictionary<char> tones;

        public Piano()
        {
            tones = new TonesDictionary<char>();
            var tonesToAdd = new[]
            {
                ('a', 'C', 261),
                ('s', 'D', 293),
                ('d', 'E', 330),
                ('f', 'F', 349),
                ('g', 'G', 392),
                ('h', 'A', 440),
                ('j', 'H', 494),
            };
            tones.AddRange(tonesToAdd);
        }

        public void TryPlayTone(char key)
        {
            if (tones.ContainsKey(key))
            {
                int frequence = tones.GetInfo(key).frequence;
                Sounder.MakeSound(frequence);
            }
        }
    }
}