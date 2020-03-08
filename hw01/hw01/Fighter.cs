using System;

namespace hw01
{
    internal class Player : AbstractFighter
    {
        public class PlayerStrategy : Strategy
        {
            public Weapon Weapon { get; private set; }

            public void SetWeapon(Command command)
            {
                switch (command)
                {
                    case Command.Rock:
                        Weapon = Weapon.Rock;
                        break;

                    case Command.Paper:
                        Weapon = Weapon.Paper;
                        break;

                    case Command.Scissors:
                        Weapon = Weapon.Scissors;
                        break;

                    default:
                        throw new ArgumentException(String.Format("Invalid command {0} for method \'SetWeapon\'", command));
                }
            }
        }

        private const double LevelUpRatio = 2.414213562;
        private const double HPRatio = 2.61803398875;
        
        public int GetLevelUpXP() => (int)(Math.Pow(LevelUpRatio, Level));

        public int EarnedXP { get; set; }

        public Player()
        {
            setStats(1);
            RockDamage = 2;
            PaperDamage = 2;
            ScissorsDamage = 2;
            Strategy = new PlayerStrategy();
        }

        public void Heal()
        {
            Hitpoints = MaxHP;
            Console.WriteLine("Your hero is fully healed.");
            Console.WriteLine();
        }

        private void setStats(int level)
        {
            Level = level;
            MaxHP = (int)(HPRatio * level) + 1;
            Heal();
        }

        public void LvlUp(Command command)
        {
            int levelUpXP = GetLevelUpXP();
            if (EarnedXP < levelUpXP)
            {
                Console.WriteLine("Insufficient experience to level up!");
                Console.WriteLine();
                return;
            }
            switch (command)
            {
                case Command.LvlupRock:
                    RockDamage++;
                    break;

                case Command.LvlupPaper:
                    PaperDamage++;
                    break;

                case Command.LvlupScissors:
                    ScissorsDamage++;
                    break;

                default:
                    throw new ArgumentException(String.Format("Invalid command {0} for method \'Lvlup\'", command));
            }
            EarnedXP -= levelUpXP;
            setStats(Level + 1);
            Console.WriteLine("Level up successful. {0} incerased. Your level is now {1}.", command, Level);
            Console.WriteLine("You have {0} XP remaining. You need {1} XP to next level.", EarnedXP, GetLevelUpXP());
            Console.WriteLine();
        }

        public override string ToString()
        {
            return "Player";
        }
    }

    internal class Creature : AbstractFighter
    {
        private class CreatureStrategy : Strategy
        {
            private static readonly int weaponsCount = Enum.GetValues(typeof(Weapon)).Length;
            private readonly Random random = new Random();

            public Weapon Weapon
            {
                get => (Weapon)random.Next(weaponsCount);
            }
        }

        public int Experience { get; private set; }
        private const double CreatureConst = 1.5;

        public Creature(int playerLevel)
        {
            Strategy = new CreatureStrategy();
            Level = (int)NormalDistribution(playerLevel, CreatureConst);
            MaxHP = (int)NormalDistribution(CreatureConst, CreatureConst) * Level;
            Hitpoints = MaxHP;
            RockDamage = (int)((NormalDistribution(Level / 2, CreatureConst)) * CreatureConst);
            PaperDamage = (int)((NormalDistribution(Level / 2, CreatureConst)) * CreatureConst);
            ScissorsDamage = (int)((NormalDistribution(Level / 2, CreatureConst)) * CreatureConst);

            Experience = Level * MaxHP * Math.Max(Math.Max(RockDamage, ScissorsDamage), PaperDamage);
        }

        // Copied from https://stackoverflow.com/questions/218060/random-gaussian-variables
        public static double NormalDistribution(double mean, double stdDev)
        {
            Random rand = new Random(); //reuse this if you are generating many
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)

            return Math.Max(randNormal, 1);
        }

        public override string ToString()
        {
            return "Creature";
        }
    }
}