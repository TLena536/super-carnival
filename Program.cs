using System;

namespace Rajzolo;

internal class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Title = "Konzolos rajzoló";
        if (Console.WindowWidth < 76 || Console.WindowHeight < 23)
        {
            Console.WriteLine("Nagyítsd a konzolablakot legalább 76 oszloposra és 23 sorosra.");
            Console.WriteLine("Nyomj meg egy billentyűt a kilépéshez.");
            Console.ReadKey(true);
            return;
        }
        Allapot.Inicializalas();

        bool fut = true;
        while (fut)
        {
            Felulet.Megjelenit();
            ConsoleKeyInfo bill = Console.ReadKey(true);

            if (Allapot.MenuMod)
            {
                Menu.Kezel(bill, ref fut);
            }
            else
            {
                switch (bill.Key)
                {
                    case ConsoleKey.M:
                    case ConsoleKey.Tab:
                        Allapot.MenuMod = true;
                        Allapot.Statusz = "Menü: fel/le, Enter, Esc";
                        break;
                    case ConsoleKey.Escape:
                        fut = false;
                        break;
                    default:
                        Nyomdak.RajzoloGombKezeles(bill);
                        break;
                }
            }
        }

        Console.ResetColor();
        Console.Clear();
        Console.WriteLine("A rajzoló bezárult. Viszlát!");
    }
}
