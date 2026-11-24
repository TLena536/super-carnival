using System;

namespace Rajzolo;

internal static class Felulet
{
    public static void Megjelenit()
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine("KONZOLOS RAJZOLÓ          │ RAJZLAP");
        Console.WriteLine(new string('─', 24) + "┼" + new string('─', Allapot.VASZON_SZELESSEG + 1));

        if (Allapot.MenuMod)
            Menu.Megjelenit();
        else
            RajzAllapotMegjelenit();

        // Rajzlap kerete és tartalma
        for (int y = 0; y < Allapot.VASZON_MAGASSAG; y++)
        {
            Console.SetCursorPosition(Allapot.VASZON_BAL, Allapot.VASZON_FELSO + y);
            Console.Write('│');
            for (int x = 0; x < Allapot.VASZON_SZELESSEG; x++)
            {
                if (x == Allapot.KurzorX && y == Allapot.KurzorY && !Allapot.MenuMod)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(Allapot.VaszonKarakter[y, x] == ' ' ? '█' : Allapot.VaszonKarakter[y, x]);
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = Allapot.VaszonSzin[y, x];
                    Console.Write(Allapot.VaszonKarakter[y, x]);
                    Console.ResetColor();
                }
            }
            Console.Write('│');
        }

        Console.SetCursorPosition(0, 21);
        Console.Write(Allapot.Statusz.PadRight(75));
        Console.SetCursorPosition(0, 22);
        Console.Write(Allapot.MenuMod
            ? "Fel/Le: választás | Enter: megnyitás | Esc: vissza".PadRight(75)
            : "Nyilak: mozgás | Szóköz: toll | M: menü | F1-F12: nyomdák | Esc: kilépés".PadRight(75));
    }

    private static void RajzAllapotMegjelenit()
    {
        Console.SetCursorPosition(0, 3); Console.Write($"Pozíció: ({Allapot.KurzorX}, {Allapot.KurzorY})");
        Console.SetCursorPosition(0, 5); Console.Write($"Karakter: {Allapot.RajzKarakter}");
        Console.SetCursorPosition(0, 7); Console.Write($"Szín: {Allapot.RajzSzin}");
        Console.SetCursorPosition(0, 9); Console.Write($"Eszköz: {(Allapot.EszkozLeteve ? "LETÉVE" : "felemelve")}");
        Console.SetCursorPosition(0, 12); Console.Write("M: menü");
        Console.SetCursorPosition(0, 13); Console.Write("F1-F3: X");
        Console.SetCursorPosition(0, 14); Console.Write("F4-F6: +");
        Console.SetCursorPosition(0, 15); Console.Write("F7-F9: négyzet");
        Console.SetCursorPosition(0, 16); Console.Write("F10-F12: rombusz");
    }
}
