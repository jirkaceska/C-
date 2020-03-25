namespace PV178.Homeworks.HW03.Tones
{
    public interface IPiano
    {
        /// <summary>
        /// Try play tone with desired key, if not found, do nothing.
        /// </summary>
        /// <param name="key">Key of tone.</param>
        void TryPlayTone(char key);
    }
}