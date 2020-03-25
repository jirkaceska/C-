using PV178.Homeworks.HW03.GameImpl;

namespace PV178.Homeworks.HW03
{
    class Program
    {
        static void Main(string[] args)
        {
            IGame game = new Game();
            game.Run();
        }
    }
}
