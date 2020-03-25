using PV178.Homeworks.HW03.Tones;
using PV178.Homeworks.HW03.Utils;
using System;
using System.IO;

namespace PV178.Homeworks.HW03.GameImpl
{
    public class Game : IGame
    {
        public int Score { get; private set; }
        public int MaxScore { get; private set; }

        private Reader reader;
        private IPiano piano = new Piano();
        private string song;
        private static string path = $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}Songs{Path.DirectorySeparatorChar}";

        public void Run()
        {
            while (ParseCommand())
            {
                using (reader = new Reader(song))
                {
                    reader.piano = piano;
                    reader.KeyEventHandler += EvaluateKeyPressed;
                    Score = reader.Text.Length;
                    MaxScore = Score;
                    reader.ReadKeys();
                    reader.KeyEventHandler -= EvaluateKeyPressed;
                    song = null;
                }
                Console.WriteLine("Your score: {0} / {1}", Score, MaxScore);
                Console.ReadKey();
            }
        }

        private bool ParseCommand()
        {
            while (song == null)
            {
                PrintSongHelp();
                string command = Console.ReadLine();

                switch (command)
                {
                    case "end":
                        return false;

                    default:
                        if (command.Evaluate())
                        {
                            piano = new CoolPiano();
                            Console.WriteLine("You have successfully activated premium version.");
                            continue;
                        }
                        break;
                }
                string songToPlay = $"{path}{command}.txt";
                if (File.Exists(songToPlay))
                {
                    song = command;
                }
                else
                {
                    Console.WriteLine("Song {0} doesn't exist", songToPlay);
                }
            }
            return true;
        }

        private void PrintSongHelp()
        {
            Console.WriteLine("Write song name to play:");
            Console.WriteLine();
            string[] fileNames = Directory.GetFiles(path);

            foreach (string filePath in fileNames)
            {
                Console.WriteLine(StringUtils.ParseFileName(filePath));
            }
            Console.WriteLine();
            Console.WriteLine("End game by typing 'end'.");
            Console.WriteLine();
        }

        private void EvaluateKeyPressed(object sender, EventArgs e)
        {
            GameEventArgs gameEvent = (GameEventArgs)e;
            int i = gameEvent.Position;
            if (reader.Text[i] != gameEvent.Key)
            {
                Score -= 1;
            }
        }
    }
}