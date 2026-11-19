static void MintaBetöltéseFájlból(string fajlNev)
{
    string eleresiUt = Path.Combine(AppContext.BaseDirectory, fajlNev);

    // Létezik-e a fájl
    if (!File.Exists(eleresiUt))
    {
        HangJelzésHiba();
        statuszUzenet = $"HIBA: A(z) '{fajlNev}' fájl nem található!";
        return;
    }

    try
    {
        string[] sorok = File.ReadAllLines(eleresiUt);

        // Sorok száma nem haladhatja meg a rajzlap magasságát
        if (sorok.Length > VASZON_MAGASSAG)
        {
            HangJelzésHiba();
            statuszUzenet = $"HIBA: A fájl túl sok sort tartalmaz ({sorok.Length} > {VASZON_MAGASSAG})!";
            return;
        }

        // Ellenőrzés: Minden sor hossza el kell férjen a rajzlap szélességén
        for (int i = 0; i < sorok.Length; i++)
        {
            if (sorok[i].Length > VASZON_SZELESSEG)
            {
                HangJelzésHiba();
                statuszUzenet = $"HIBA: A(z) {i + 1}. sor túl széles ({sorok[i].Length} > {VASZON_SZELESSEG})!";
                return;
            }
        }

        
        VaszonAlaphelyzetbe();

        for (int y = 0; y < sorok.Length; y++)
        {
            for (int x = 0; x < sorok[y].Length; x++)
            {
                char c = sorok[y][x];
                if (c != ' ')
                {
                    vaszonKarakter[y, x] = c;
                    vaszonSzin[y, x] = rajzSzin;
                }
            }
        }

        statuszUzenet = $"A(z) '{fajlNev}' minta sikeresen betöltve!";
    }
    catch (Exception ex)
    {
        HangJelzésHiba();
        statuszUzenet = "HIBA: Fájlbeolvasási hiba történt: " + ex.Message;
    }
}