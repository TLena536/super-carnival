static void RajzolóGombKezelés(ConsoleKeyInfo keyInfo)
{
    switch (keyInfo.Key)
    {
        case ConsoleKey.UpArrow:
            KurzorMozgatás(0, -1);
            break;
        case ConsoleKey.DownArrow:
            KurzorMozgatás(0, 1);
            break;
        case ConsoleKey.LeftArrow:
            KurzorMozgatás(-1, 0);
            break;
        case ConsoleKey.RightArrow:
            KurzorMozgatás(1, 0);
            break;

        case ConsoleKey.Spacebar:
            eszkozLeteve = !eszkozLeteve;
            if (eszkozLeteve)
            {
                CellaRajzolása(kurzorX, kurzorY);
                statuszUzenet = "Eszköz LETÉVE - Rajzolás aktív.";
            }
            else
            {
                statuszUzenet = "Eszköz FELEMELVE - Mozgás mód.";
            }
            break;

        case ConsoleKey.M:
        case ConsoleKey.Tab:
            menuMod = true;
            statuszUzenet = "Menü mód aktív. Használd a nyilakat és Entert!";
            break;

        case ConsoleKey.Escape:
            fut = false;
            break;

        // Nyomdák F1 - F12
        case ConsoleKey.F1: NyomdaRányomása('X', 3); break;
        case ConsoleKey.F2: NyomdaRányomása('X', 5); break;
        case ConsoleKey.F3: NyomdaRányomása('X', 7); break;

        case ConsoleKey.F4: NyomdaRányomása('+', 3); break;
        case ConsoleKey.F5: NyomdaRányomása('+', 5); break;
        case ConsoleKey.F6: NyomdaRányomása('+', 7); break;

        case ConsoleKey.F7: NyomdaRányomása('N', 3); break; // Négyzet
        case ConsoleKey.F8: NyomdaRányomása('N', 5); break;
        case ConsoleKey.F9: NyomdaRányomása('N', 7); break;

        case ConsoleKey.F10: NyomdaRányomása('R', 3); break; // Rombusz
        case ConsoleKey.F11: NyomdaRányomása('R', 5); break;
        case ConsoleKey.F12: NyomdaRányomása('R', 7); break;

        default:
            break;
    }
}

static void KurzorMozgatás(int dx, int dy)
{
    int ujX = kurzorX + dx;
    int ujY = kurzorY + dy;

    // Határellenőrzés
    if (ujX < 0 || ujX >= VASZON_SZELESSEG || ujY < 0 || ujY >= VASZON_MAGASSAG)
    {
        HangJelzésHiba();
        statuszUzenet = "HIBA: A rajzlap szélét nem lépheted át!";
        return;
    }

    kurzorX = ujX;
    kurzorY = ujY;

    if (eszkozLeteve)
    {
        CellaRajzolása(kurzorX, kurzorY);
    }
    statuszUzenet = $"Pozíció: ({kurzorX}, {kurzorY})";
}

static void CellaRajzolása(int x, int y)
{
    vaszonKarakter[y, x] = rajzKarakter;
    vaszonSzin[y, x] = rajzSzin;
}




static void NyomdaRányomása(char tipus, int meret)
{
    int r = meret / 2; 

    // ha 1 cella is kilóg, nem rajzolunk semmit!
    if (kurzorX - r < 0 || kurzorX + r >= VASZON_SZELESSEG ||
        kurzorY - r < 0 || kurzorY + r >= VASZON_MAGASSAG)
    {
        HangJelzésHiba();
        statuszUzenet = $"HIBA: A(z) {meret}x{meret} nyomda nem fér el a rajzlap szélén!";
        return;
    }

    // Minta kirajzolása a kiválasztott alakzat alapján
    for (int dy = -r; dy <= r; dy++)
    {
        for (int dx = -r; dx <= r; dx++)
        {
            bool kellRajzolni = false;

            switch (tipus)
            {
                case 'X': // Átlók
                    kellRajzolni = (Math.Abs(dx) == Math.Abs(dy));
                    break;
                case '+': // Kereszt
                    kellRajzolni = (dx == 0 || dy == 0);
                    break;
                case 'N': // Négyzetkeret
                    kellRajzolni = (Math.Abs(dx) == r || Math.Abs(dy) == r);
                    break;
                case 'R': // Rombuszkeret
                    kellRajzolni = (Math.Abs(dx) + Math.Abs(dy) == r);
                    break;
            }

            if (kellRajzolni)
            {
                vaszonKarakter[kurzorY + dy, kurzorX + dx] = rajzKarakter;
                vaszonSzin[kurzorY + dy, kurzorX + dx] = rajzSzin;
            }
        }
    }

    statuszUzenet = $"Nyomda elhelyezve: {tipus} ({meret}x{meret}) a(z) ({kurzorX},{kurzorY}) ponton";
}