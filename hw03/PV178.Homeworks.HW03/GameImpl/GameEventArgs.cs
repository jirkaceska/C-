using System;

namespace PV178.Homeworks.HW03.GameImpl
{
    public class GameEventArgs : EventArgs
    {
        public int Position { get; private set; }
        public char Key { get; private set; }

        public GameEventArgs(int position, char key) : this(position)
        {
            Key = key;
        }

        public GameEventArgs(int position)
        {
            Position = position;
            Key = ' ';
        }
    }
}