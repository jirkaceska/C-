using PV178.Homeworks.HW03.Utils;

namespace PV178.Homeworks.HW03.Tones
{
    internal class CoolPiano : IPiano
    {
        private ITonesDictionary<string> tones;

        public CoolPiano()
        {
            tones = new TonesDictionary<string>();
            var tonesToAdd = new[]
            {
                ('a', "piano-a", 261),
                ('s', "piano-s", 293),
                ('d', "piano-d", 330),
                ('f', "piano-f", 349),
                ('g', "piano-g", 392),
                ('h', "piano-h", 440),
                ('j', "piano-j", 494),
            };
            tones.AddRange(tonesToAdd);
        }

        public void TryPlayTone(char key)
        {
            if (tones.ContainsKey(key))
            {
                string fileName = tones.GetInfo(key).name;
                Sounder.MakeCoolSound(fileName);
            }
        }
    }
}