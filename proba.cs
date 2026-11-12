// Ezt a metódust tedd be a Main metódusod alá az osztályon belül:
static void RajzolasHaKell(int x, int y, ref bool eszkozLetett, char aktivKarakter, char[,] rajzlap)
{
    // Ha az eszköz letett állapotban van, beírjuk a karaktert a rajzlapra
    if (eszkozLetett)
    {
        rajzlap[y, x] = aktivKarakter;
    }
}



// Változók a kurzor pozíciójához és az eszköz állapotához
int kurzorX = 10;
int kurzorY = 5;
bool eszkozLetett = false; // Kezdetben felemelt[cite: 1]

// A rajzlap határai (példa méretek)
int minX = 0, maxX = 40;
int minY = 0, maxY = 20;

// Fő vezérlő ciklus (pl. egy while ciklus[cite: 6])
bool fut = true;
while (fut)
{
    ConsoleKeyInfo gomb = Console.ReadKey(true);

    switch (gomb.Key)
    {
        case ConsoleKey.UpArrow:
            if (kurzorY > minY) { kurzorY--; RajzolasHaKell(kurzorX, kurzorY, ref eszkozLetett); }
            else { Console.Beep(); } // Ha túllépi a határt, Beep[cite: 1, 2]
            break;

        case ConsoleKey.DownArrow:
            if (kurzorY < maxY) { kurzorY++; RajzolasHaKell(kurzorX, kurzorY, ref eszkozLetett); }
            else { Console.Beep(); }
            break;

        case ConsoleKey.LeftArrow:
            if (kurzorX > minX) { kurzorX--; RajzolasHaKell(kurzorX, kurzorY, ref eszkozLetett); }
            else { Console.Beep(); }
            break;

        case ConsoleKey.RightArrow:
            if (kurzorX < maxX) { kurzorX++; RajzolasHaKell(kurzorX, kurzorY, ref eszkozLetett); }
            else { Console.Beep(); }
            break;

        case ConsoleKey.Spacebar:
            // Eszköz állapotának váltása (felemelt <-> letett)[cite: 1]
            eszkozLetett = !eszkozLetett;
            break;



        // Példa F1 billentyűre (3x3-as X alakzat)
        case ConsoleKey.F1:
            int mintMeret = 3;
            int felsoFél = mintMeret / 2; // 3 / 2 = 1 (így a középpont a kurzor lesz)

            // 1. LÉPÉS: Teljes befoglaló négyzet ellenőrzése a határok ellen
            if (kurzorX - felsoFél < minX || kurzorX + felsoFél > maxX ||
                kurzorY - felsoFél < minY || kurzorY + felsoFél > maxY)
            {
                Console.Beep(); // Kilógna, hiba![cite: 2]
                break;
            }

            // 2. LÉPÉS: A 3x3-as minta definíciója (string tömbként vagy stringekként)
            string[] minta3x3 = {
        "#.#",
        ".#.",
        "#.#"
    };

            // 3. LÉPÉS: Kiírás a rajzlapra, ha mindenhol elfér
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (minta3x3[i][j] == '#')
                    {
                        int célX = kurzorX - felsoFél + j;
                        int célY = kurzorY - felsoFél + i;

                        // Ide jön a rajzlap adott cellájának frissítése a kiválasztott karakterrel
                        // Rajzlap[célY][célX] = aktivKarakter;
                    }
                    // Ha '.', akkor nem csinálunk semmit (érintetlen marad)[cite: 2]
                }
            }
            break;
            // Itt jönnek majd az F1-F12 pecsétek is...
    }
}







//// Konzolos próbaprogram. Windows Terminalban vagy konzolablakban futtasd.
//Console.OutputEncoding = System.Text.Encoding.UTF8;

//if (Console.IsInputRedirected || Console.IsOutputRedirected)
//{
//    Console.WriteLine("A próbához nyiss interaktív konzolablakot.");
//    return;
//}

//if (Console.WindowWidth < 76 || Console.WindowHeight < 23)
//{
//    Console.WriteLine("Nagyítsd a konzolablakot legalább 76 oszloposra és 23 sorosra.");
//    return;
//}

//Console.Clear();
//Console.WriteLine("Konzolpróba | F1: színes jel | B: hang | L: fájl | Esc: kilépés");
//Console.WriteLine("Az alsó sor az utolsó leütött billentyűt mutatja.");
//bool fut = true;
//while (fut)
//{
//    ConsoleKey billentyu = Console.ReadKey(true).Key;
//    Console.SetCursorPosition(0, 20);
//    Console.Write(("Billentyű: " + billentyu).PadRight(75));

//    switch (billentyu)
//    {
//        case ConsoleKey.F1:

//            //Console.SetCursorPosition(26, 4); // oszlop, majd sor
//            //Console.ForegroundColor = ConsoleColor.Cyan;
//            //Console.Write('#');
//            //Console.ResetColor();
//            //break;
//        case ConsoleKey.B:
//            if (OperatingSystem.IsWindows())
//                Console.Beep(800, 120);
//            else
//                Console.Write("\a");
//            break;
//        case ConsoleKey.L:
//            string utvonal = Path.Combine(AppContext.BaseDirectory, "minta.txt");
//            if (File.Exists(utvonal))
//            {
//                string[] sorok = File.ReadAllLines(utvonal);
//                for (int i = 0; i < sorok.Length && i < 5; i++)
//                {
//                    Console.SetCursorPosition(26, 7 + i);
//                    Console.Write(sorok[i]);
//                }
//            }
//            else
//            {
//                Console.SetCursorPosition(26, 7);
//                Console.Write("A mintafájl nem található.");
//            }
//            break;
//        case ConsoleKey.Escape:
//            fut = false;
//            break;
//    }
//}

//Console.SetCursorPosition(0, 21);
//Console.WriteLine("A próba véget ért.");
