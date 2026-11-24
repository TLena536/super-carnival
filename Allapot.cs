using System;

namespace Rajzolo;

internal static class Allapot
{
    public const int VASZON_SZELESSEG = 50;
    public const int VASZON_MAGASSAG = 18;
    public const int VASZON_BAL = 25;
    public const int VASZON_FELSO = 2;

    public static char[,] VaszonKarakter = new char[VASZON_MAGASSAG, VASZON_SZELESSEG];
    public static ConsoleColor[,] VaszonSzin = new ConsoleColor[VASZON_MAGASSAG, VASZON_SZELESSEG];
    public static int KurzorX = VASZON_SZELESSEG / 2;
    public static int KurzorY = VASZON_MAGASSAG / 2;
    public static bool EszkozLeteve;
    public static bool MenuMod;
    public static char RajzKarakter = '#';
    public static ConsoleColor RajzSzin = ConsoleColor.Cyan;
    public static string Statusz = "Készen áll.";

    public static void Inicializalas() => VaszonAlaphelyzetbe();

    public static void VaszonAlaphelyzetbe()
    {
        for (int y = 0; y < VASZON_MAGASSAG; y++)
            for (int x = 0; x < VASZON_SZELESSEG; x++)
            {
                VaszonKarakter[y, x] = ' ';
                VaszonSzin[y, x] = ConsoleColor.Gray;
            }
    }

    public static void HangJelzesHiba()
    {
        try { Console.Beep(800, 120); }
        catch { Console.Write("\a"); }
    }
}
