using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw01
{
    enum Weapon
    {
        Rock,
        Paper,
        Scissors
    }
    interface Strategy
    {
        Weapon Weapon { get; }
    }
    interface IFighter
    {
        int RockDamage { get; }
        int PaperDamage { get; }
        int ScissorsDamage { get; }
        int Hitpoints { get; }
        Strategy Strategy { get; }
        int Level { get; }
        int MaxHP { get; }
        Weapon SelectWeapon();
        int GetDamage(Weapon weapon);
        void TakeDamage(int damage);
    }

    abstract class AbstractFighter : IFighter
    {
        public int RockDamage { get; protected set; }

        public int PaperDamage { get; protected set; }

        public int ScissorsDamage { get; protected set; }

        public int Hitpoints { get; protected set; }

        public Strategy Strategy { get; protected set; }

        public int Level { get; protected set; }

        public int MaxHP { get; protected set; }

        public int GetDamage(Weapon weapon)
        {
            switch (weapon)
            {
                case Weapon.Rock:
                    return RockDamage;
                case Weapon.Paper:
                    return PaperDamage;
                case Weapon.Scissors:
                    return ScissorsDamage;
                default:
                    throw new ArgumentException(String.Format("Invalid weapon {0} for method \'GetDamage\' in instance {1}", weapon, this.GetType()));
            }
        }

        public Weapon SelectWeapon()
        {
            Weapon selectedWeapon = Strategy.Weapon;
            Console.WriteLine("{0} has selected {1}", ToString(), selectedWeapon);
            return selectedWeapon;
        }

        public void TakeDamage(int damage)
        {
            if (damage <= 0)
            {
                throw new ArgumentException(String.Format("Damage must be positive integer. Damage value is {0} in method \'TakeDamage\' in instance {1}", damage, this.GetType()));
            }
            Console.WriteLine("{0} has been damaged by {1} hitpoints", ToString(), damage);
            Console.WriteLine();
            int newValue = Hitpoints - damage;
            Hitpoints = Math.Max(newValue, 0);
        }

        public bool IsDead()
        {
            return Hitpoints <= 0;
        }
    }

    
}
