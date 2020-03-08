using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw01
{
    interface IGame
    {
        bool IsInFight {get; }
        bool IsEnded();
        bool IsInitialized { get; }
        void NewGame();
        void Fight();
        void FightWith(Command command);
        void LvlUp(Command command);
        void Healer();
    }
    
    class Game : IGame
    {
        private Player player;
        private IFighter creature;

        public bool IsInFight { get; private set; }

        public void NewGame()
        {
            Console.WriteLine("New game has started.");
            Console.WriteLine();
            IsInFight = false;
            IsInitialized = true;
            player = new Player();
        }
        public void Fight()
        {
            IsInFight = true;
            creature = new Creature(player.Level);
            Console.WriteLine("You have encountered creature with level {0}! Select weapon.", creature.Level);
            Console.WriteLine();
        }

        public void Healer()
        {
            player.Heal();
        }

        public void LvlUp(Command command)
        {
            player.LvlUp(command);
        }

        public void FightWith(Command command)
        {
            ((Player.PlayerStrategy) player.Strategy).SetWeapon(command);
            Weapon playerWeapon = player.SelectWeapon();
            Weapon creatureWeapon = creature.SelectWeapon();

            int result = CompareWeapon(playerWeapon, creatureWeapon);
            switch (result)
            {
                case 1:
                    creature.TakeDamage(player.GetDamage(playerWeapon));
                    Console.WriteLine("YOU HAVE WON.");
                    Console.WriteLine("Creature have {0}/{1} HP", creature.Hitpoints, creature.MaxHP);
                    break;
                case 0:
                    Console.WriteLine("DRAW.");
                    break;
                case -1:
                    player.TakeDamage(creature.GetDamage(creatureWeapon));
                    Console.WriteLine("YOU HAVE LOST.");
                    Console.WriteLine("You have {0}/{1} HP", player.Hitpoints, player.MaxHP);
                    
                    break;
            }

            IsInFight = !EvaluateFight();
        }

        public bool IsEnded() => player.Hitpoints <= 0 || player.Level >= 10;
        public bool IsInitialized { get; private set; } = false;

        public static int CompareWeapon(Weapon a, Weapon b)
        {
            if (a == b)
            {
                return 0;
            }
            else if (a == Weapon.Rock && b == Weapon.Scissors ||
                a == Weapon.Scissors && b == Weapon.Paper ||
                a == Weapon.Paper && b == Weapon.Rock)
            {
                return 1;
            }
            return -1;
        }

        private bool EvaluateFight()
        {
            if (creature.Hitpoints <= 0)
            {
                int creatureXp = ((Creature)creature).Experience;
                player.EarnedXP += creatureXp;
                Console.WriteLine("Creature is dead. You have gained {0} xp. Now you have level {1} and {2} xp.", creatureXp, player.Level, player.EarnedXP); 
                Console.WriteLine("Xp needed to reach next level: {0}", player.GetLevelUpXP());
                Console.WriteLine();
                return true;
            }
            else if (player.Hitpoints <= 0)
            {
                Console.WriteLine("You are dead.");
                Console.WriteLine();
                return true;
            }
            return false;
        }
    }
}
