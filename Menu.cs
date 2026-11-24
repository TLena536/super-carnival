using System;

namespace Rajzolo;

internal static class Menu
{
    private static readonly string[] Pontok =
    {
        "Rajzkarakter váltása", "Rajzszín váltása", "Minta betöltése",
        "Cella törlése", "Ugrás középre", "Rajzlap törlése", "Kilépés"
    };
    private static int kivalasztott;

    public static void Megjelenit()
    {
        Console.SetCursorPosition(1, 3); Console.WriteLine("MENÜ");
        for (int i = 0; i < Pontok.Length; i++)
        {
            Console.SetCursorPosition(0, 5 + i);
            Console.Write(i == kivalasztott ? "> " : "  ");
            Console.Write(Pontok[i].PadRight(21));
        }
        Console.SetCursorPosition(0, 14); Console.Write("Karakter: " + Allapot.RajzKarakter);
        Console.SetCursorPosition(0, 15); Console.Write("Szín: " + Allapot.RajzSzin);
    }

    public static void Kezel(ConsoleKeyInfo bill, ref bool fut)
    {
        switch (bill.Key)
        {
            case ConsoleKey.UpArrow: kivalasztott = (kivalasztott + Pontok.Length - 1) % Pontok.Length; break;
            case ConsoleKey.DownArrow: kivalasztott = (kivalasztott + 1) % Pontok.Length; break;
            case ConsoleKey.Escape:
                Allapot.MenuMod = false;
                break;
            case ConsoleKey.Enter:
                Vegrehajt(ref fut);
                break;
        }
    }

    private static void Vegrehajt(ref bool fut)
    {
        switch (kivalasztott)
        {
            case 0:
                char[] karakterek = { '#', '*', '@', '+' };
                int index = Array.IndexOf(karakterek, Allapot.RajzKarakter);
                Allapot.RajzKarakter = karakterek[(index + 1) % karakterek.Length];
                Allapot.Statusz = "Rajzkarakter: " + Allapot.RajzKarakter;
                break;
            case 1:
                ConsoleColor[] szinek = { ConsoleColor.Cyan, ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Yellow, ConsoleColor.Magenta, ConsoleColor.White };
                int si = Array.IndexOf(szinek, Allapot.RajzSzin);
                Allapot.RajzSzin = szinek[(si + 1) % szinek.Length];
                Allapot.Statusz = "Rajzszín: " + Allapot.RajzSzin;
                break;
            case 2:
                Console.CursorVisible = true;
                Console.SetCursorPosition(0, 20);
                Console.Write("Mintafájl neve (pl. minta.txt): ".PadRight(75));
                Console.SetCursorPosition(33, 20);
                string? nev = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(nev)) MintaBetolto.MintaBetolteseFajlbol(nev.Trim());
                break;
            case 3:
                SajátFunkciók.CellaTorlese();
                break;
            case 4:
                SajátFunkciók.UgrasKozepre();
                break;
            case 5:
                Allapot.VaszonAlaphelyzetbe();
                Allapot.Statusz = "A rajzlap törölve.";
                break;
            case 6:
                fut = false;
                break;
        }
        if (fut) Allapot.MenuMod = true;
    }
}
