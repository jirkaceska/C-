using System;
using System.IO;
using System.Media;
using System.Threading;

namespace PV178.Homeworks.HW03.Utils
{
    /// <summary>
    /// Class for making sound in other thread.
    /// </summary>
    public static class Sounder
    {
        private static string path = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Sounds{Path.DirectorySeparatorChar}";

        /// <summary>
        /// Makes sound with given frequency and duration.
        /// </summary>
        /// <param name="frequency">frequency</param>
        /// <param name="duration">duration</param>
        public static void MakeSound(int frequency, int duration = 300)
        {
            ThreadPool.QueueUserWorkItem(state =>
                Console.Beep(frequency, duration));
        }

        /// <summary>
        /// Play WAV sound with entered file name.
        /// </summary>
        /// <param name="fileName">Name of WAV file (without extension)</param>
        public static void MakeCoolSound(string fileName)
        {
            string soundPath = $"{path}{fileName}.wav";
            SoundPlayer snd = new SoundPlayer(soundPath);
            ThreadPool.QueueUserWorkItem(state => snd.Play());
        }
    }
}