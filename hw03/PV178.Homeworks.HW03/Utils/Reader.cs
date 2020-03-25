using PV178.Homeworks.HW03.GameImpl;
using PV178.Homeworks.HW03.Tones;
using System;
using System.IO;
using System.Threading;

namespace PV178.Homeworks.HW03.Utils
{
    /// <summary>
    /// Class responsible for reading songs from textfiles and handling user input.
    /// </summary>
    public class Reader : IDisposable
    {
        public string Text { get; set; }

        public event EventHandler KeyEventHandler;

        public IPiano piano;

        private const int Timeout = 300;
        private readonly Displayer displayer = new Displayer();
        private readonly AutoResetEvent trackDone;
        private readonly Thread checkingThread;
        private readonly Thread gettingThread;
        private readonly string path = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Songs{Path.DirectorySeparatorChar}";
        private char? input;
        private bool end;

        public Reader(string songName)
        {
            if (File.Exists($"{path}{songName}.txt"))
            {
                Text = File.ReadAllText($"{path}{songName}.txt");
                trackDone = new AutoResetEvent(false);
                checkingThread = new Thread(CheckInput) { IsBackground = true };
                gettingThread = new Thread(GetInput) { IsBackground = true };
            }
            else
            {
                throw new ArgumentException("Wrong song path");
            }
        }

        /// <summary>
        /// Starts reading keys and checking whether user pressed some.
        /// </summary>
        public void ReadKeys()
        {
            gettingThread.Start();
            checkingThread.Start();
            trackDone.WaitOne();
        }

        /// <summary>
        /// Performs cleanup.
        /// </summary>
        public void Dispose()
        {
            end = true;
            trackDone.Dispose();
            Console.Clear();
        }

        /// <summary>
        /// Invokes event that says which key was pressed and what is actual reading position.
        /// </summary>
        /// <param name="key">pressed key</param>
        /// <param name="position">actual reading position</param>
        protected virtual void OnKeyPressed(char key, int position)
        {
            KeyEventHandler.Invoke(this, new GameEventArgs(position, key));
        }

        /// <summary>
        /// Invokes event that says no key was pressed and what is actual reading position.
        /// </summary>
        /// <param name="position">actual reading position</param>
        protected virtual void OnKeyNotPressed(int position)
        {
            KeyEventHandler.Invoke(this, new GameEventArgs(position));
        }

        /// <summary>
        /// Periodically checks if some key was pressed.
        /// </summary>
        private void CheckInput()
        {
            for (var i = -6; i < Text.Length; i++)
            {
                displayer.ActualDisplay(Text, i + 6);
                Thread.Sleep(Timeout);
                // First chars just skip (because animation)
                if (i < 0)
                {
                    continue;
                }
                if (input != null)
                {
                    Console.WriteLine(input);
                    OnKeyPressed((char)input, i);
                    input = null;
                }
                else
                {
                    OnKeyNotPressed(i);
                }
            }
            trackDone.Set();
        }

        /// <summary>
        /// Gets input from the user.
        /// </summary>
        private void GetInput()
        {
            while (!end)
            {
                input = Console.ReadKey(true).KeyChar;
                if (input != null && !end)
                {
                    char key = input.Value;
                    piano.TryPlayTone(key);
                }
            }
        }
    }
}