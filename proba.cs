using System;

namespace Rajzolo;

// A két saját menüfunkció egy külön fájlban kapott helyet.
internal static class SajátFunkciók
{
    // 1. saját funkció: az aktuális cella törlése.
    public static void CellaTorlese()
    {
        Allapot.VaszonKarakter[Allapot.KurzorY, Allapot.KurzorX] = ' ';
        Allapot.VaszonSzin[Allapot.KurzorY, Allapot.KurzorX] = ConsoleColor.Gray;
        Allapot.Statusz = $"A ({Allapot.KurzorX}, {Allapot.KurzorY}) cella törölve.";
    }

    // 2. saját funkció: kurzor ugrása a rajzlap közepére.
    public static void UgrasKozepre()
    {
        Allapot.KurzorX = Allapot.VASZON_SZELESSEG / 2;
        Allapot.KurzorY = Allapot.VASZON_MAGASSAG / 2;
        Allapot.Statusz = "A kurzor a rajzlap közepére ugrott.";
    }
}
