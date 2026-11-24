using System;

namespace Rajzolo;

internal static class Nyomdak
{
    public static void RajzoloGombKezeles(ConsoleKeyInfo keyInfo)
    {
        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow: KurzorMozgatas(0, -1); break;
            case ConsoleKey.DownArrow: KurzorMozgatas(0, 1); break;
            case ConsoleKey.LeftArrow: KurzorMozgatas(-1, 0); break;
            case ConsoleKey.RightArrow: KurzorMozgatas(1, 0); break;
            case ConsoleKey.Spacebar:
                Allapot.EszkozLeteve = !Allapot.EszkozLeteve;
                if (Allapot.EszkozLeteve) CellaRajzolasa(Allapot.KurzorX, Allapot.KurzorY);
                Allapot.Statusz = Allapot.EszkozLeteve ? "Eszköz LETÉVE - rajzolás aktív." : "Eszköz FELEMELVE - mozgás mód.";
                break;
            case ConsoleKey.F1: NyomdaRanyomasa('X', 3); break;
            case ConsoleKey.F2: NyomdaRanyomasa('X', 5); break;
            case ConsoleKey.F3: NyomdaRanyomasa('X', 7); break;
            case ConsoleKey.F4: NyomdaRanyomasa('+', 3); break;
            case ConsoleKey.F5: NyomdaRanyomasa('+', 5); break;
            case ConsoleKey.F6: NyomdaRanyomasa('+', 7); break;
            case ConsoleKey.F7: NyomdaRanyomasa('N', 3); break;
            case ConsoleKey.F8: NyomdaRanyomasa('N', 5); break;
            case ConsoleKey.F9: NyomdaRanyomasa('N', 7); break;
            case ConsoleKey.F10: NyomdaRanyomasa('R', 3); break;
            case ConsoleKey.F11: NyomdaRanyomasa('R', 5); break;
            case ConsoleKey.F12: NyomdaRanyomasa('R', 7); break;
        }
    }

    private static void KurzorMozgatas(int dx, int dy)
    {
        int x = Allapot.KurzorX + dx, y = Allapot.KurzorY + dy;
        if (x < 0 || x >= Allapot.VASZON_SZELESSEG || y < 0 || y >= Allapot.VASZON_MAGASSAG)
        {
            Allapot.HangJelzesHiba();
            Allapot.Statusz = "HIBA: A rajzlap szélét nem lépheted át!";
            return;
        }
        Allapot.KurzorX = x;
        Allapot.KurzorY = y;
        if (Allapot.EszkozLeteve) CellaRajzolasa(x, y);
        Allapot.Statusz = $"Pozíció: ({x}, {y})";
    }

    private static void CellaRajzolasa(int x, int y)
    {
        Allapot.VaszonKarakter[y, x] = Allapot.RajzKarakter;
        Allapot.VaszonSzin[y, x] = Allapot.RajzSzin;
    }

    private static void NyomdaRanyomasa(char tipus, int meret)
    {
        int r = meret / 2;
        if (Allapot.KurzorX - r < 0 || Allapot.KurzorX + r >= Allapot.VASZON_SZELESSEG ||
            Allapot.KurzorY - r < 0 || Allapot.KurzorY + r >= Allapot.VASZON_MAGASSAG)
        {
            Allapot.HangJelzesHiba();
            Allapot.Statusz = $"HIBA: A {meret}x{meret} nyomda nem fér el!";
            return;
        }

        for (int dy = -r; dy <= r; dy++)
            for (int dx = -r; dx <= r; dx++)
            {
                bool jelolt = tipus switch
                {
                    'X' => Math.Abs(dx) == Math.Abs(dy),
                    '+' => dx == 0 || dy == 0,
                    'N' => Math.Abs(dx) == r || Math.Abs(dy) == r,
                    'R' => Math.Abs(dx) + Math.Abs(dy) == r,
                    _ => false
                };
                if (jelolt) CellaRajzolasa(Allapot.KurzorX + dx, Allapot.KurzorY + dy);
            }
        Allapot.Statusz = $"Nyomda elhelyezve: {tipus}, {meret}x{meret}.";
    }
}
