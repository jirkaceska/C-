using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw01
{
    
    class Program
    {
        static IGame game = new Game();
        public static void processCommand(Command command)
        {
            switch (command)
            {
                case Command.NewGame:
                    game.NewGame();
                    break;
                case Command.Fight:
                    if (!game.IsInFight)
                    {
                        game.Fight();
                    }
                    else
                    {
                        Console.WriteLine("You are already in fight!");
                        Console.WriteLine();
                    }
                    break;
                case Command.Healer:
                    if (!game.IsInFight)
                    {
                        game.Healer();
                    }
                    else
                    {
                        Console.WriteLine("You cannot heal in fight!");
                        Console.WriteLine();
                    }
                    break;
                case Command.LvlupRock:
                case Command.LvlupPaper:
                case Command.LvlupScissors:
                    if (!game.IsInFight)
                    {
                        game.LvlUp(command);
                    }
                    else
                    {
                        Console.WriteLine("You cannot level up in fight!");
                        Console.WriteLine();
                    }
                    break;
                case Command.Rock:
                case Command.Paper:
                case Command.Scissors:
                    if (game.IsInFight)
                    {
                        game.FightWith(command);
                    }
                    else
                    {
                        Console.WriteLine("You cannot pick weapon. You are not fighting!");
                        Console.WriteLine();
                    }
                    break;
                case Command.Help:
                    ConsoleManager.WriteHelp();
                    break;
            }
        }
        static void Main(string[] args)
        {
            ConsoleManager.WriteHelp();
            string commandStr;

            do
            {
                
                commandStr = Console.ReadLine();
                if (ConsoleManager.ParseCommand(commandStr, out Command command))
                {
                    if (game.IsInitialized || command == Command.NewGame || command == Command.Help)
                    {
                        processCommand(command);
                    }
                    else
                    {
                        Console.WriteLine("You must start new game");
                        Console.WriteLine();
                    }
                }
                else
                {
                    Console.WriteLine("Unknown command! Try again.");
                    Console.WriteLine();
                }
            } while (commandStr != null && !(game.IsInitialized && game.IsEnded()));
            Console.WriteLine("Hope you enjoyed the game. See you soon!");
        }
    }
}
