using System.Collections.Generic;

namespace PV178.Homeworks.HW03.Tones
{
    public interface ITonesDictionary<T> : IDictionary<char, (T, int)>
    {
        /// <summary>
        /// Add collection of tones to dictionary.
        /// </summary>
        /// <param name="tones">Collection to add.</param>
        void AddRange(ICollection<(char key, T name, int frequence)> tones);

        /// <summary>
        /// Get info about tone by its key.
        /// </summary>
        /// <param name="key">Key of tone.</param>
        /// <returns>Info about tone.</returns>
        (T name, int frequence) GetInfo(char key);
    }
}