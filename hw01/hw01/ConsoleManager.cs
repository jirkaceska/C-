using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw01
{
    enum Command
    {
        Invalid,
        NewGame,
        Fight,
        Healer,
        LvlupRock,
        LvlupPaper,
        LvlupScissors,
        Rock,
        Paper,
        Scissors,
        Help
    }

    static class ConsoleManager
    {
        public static bool ParseCommand(string commandStr, out Command command)
        {
            switch (commandStr)
            {
                case "new game":
                    command = Command.NewGame;
                    break;
                case "fight":
                    command = Command.Fight;
                    break;
                case "healer":
                    command = Command.Healer;
                    break;
                case "lvlup rock":
                    command = Command.LvlupRock;
                    break;
                case "lvlup paper":
                    command = Command.LvlupPaper;
                    break;
                case "lvlup scissors":
                    command = Command.LvlupScissors;
                    break;
                case "rock":
                    command = Command.Rock;
                    break;
                case "paper":
                    command = Command.Paper;
                    break;
                case "scissors":
                    command = Command.Scissors;
                    break;
                case "help":
                    command = Command.Help;
                    break;
                default:
                    command = Command.Invalid;
                    return false;
            }
            return true;
        }
        public static void WriteHelp()
        {
            Console.WriteLine("You can display this help by typing \"help\"");
            Console.WriteLine("Start new game by typing \"new game\"");
            Console.WriteLine();
            Console.WriteLine("Here is the list of available commands (case sensitive):");
            Console.WriteLine("\tnew game");
            Console.WriteLine("\tfight");
            Console.WriteLine("\thealer");
            Console.WriteLine("\tlvlup rock");
            Console.WriteLine("\tlvlup paper");
            Console.WriteLine("\tlvlup scissors");
            Console.WriteLine("\tpaper");
            Console.WriteLine("\trock");
            Console.WriteLine("\tscissors");
            Console.WriteLine("\thelp");
            Console.WriteLine();
            Console.WriteLine("End game by typing Ctrl+Z");
            Console.WriteLine();
        }

    }
}
