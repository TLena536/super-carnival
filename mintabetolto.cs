using System;
using System.IO;

namespace Rajzolo;

internal static class MintaBetolto
{
    public static void MintaBetolteseFajlbol(string fajlNev)
    {
        string eleresiUt = Path.Combine(AppContext.BaseDirectory, fajlNev);
        if (!File.Exists(eleresiUt))
        {
            Allapot.HangJelzesHiba();
            Allapot.Statusz = $"HIBA: A(z) '{fajlNev}' fájl nem található!";
            return;
        }

        try
        {
            string[] sorok = File.ReadAllLines(eleresiUt);
            if (sorok.Length > Allapot.VASZON_MAGASSAG)
            {
                Allapot.HangJelzesHiba();
                Allapot.Statusz = "HIBA: A fájl túl magas a rajzlaphoz.";
                return;
            }
            for (int i = 0; i < sorok.Length; i++)
            {
                if (sorok[i].Length > Allapot.VASZON_SZELESSEG)
                {
                    Allapot.HangJelzesHiba();
                    Allapot.Statusz = $"HIBA: A(z) {i + 1}. sor túl széles.";
                    return;
                }
            }

            // Csak az összes ellenőrzés után módosítjuk a rajzlapot.
            Allapot.VaszonAlaphelyzetbe();
            for (int y = 0; y < sorok.Length; y++)
                for (int x = 0; x < sorok[y].Length; x++)
                    if (sorok[y][x] != ' ')
                    {
                        Allapot.VaszonKarakter[y, x] = sorok[y][x];
                        Allapot.VaszonSzin[y, x] = Allapot.RajzSzin;
                    }
            Allapot.Statusz = $"A(z) '{fajlNev}' minta sikeresen betöltve.";
        }
        catch (Exception ex)
        {
            Allapot.HangJelzesHiba();
            Allapot.Statusz = "HIBA a fájl olvasásakor: " + ex.Message;
        }
    }
}
