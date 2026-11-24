using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

/////MÜKÖDIK

namespace KonzolosRajzolo
{


    class Program
    {

        // =========================================================================
        // RAJZOLÓ ALKALMAZÁS - IKT PÁROS PROJEKT (11.F)
        // Tanult C# ismeretek: Console vezérlés, 2D tömbök, switch, ciklusok,
        // String/StringBuilder kezelés, File I/O, Hibakezelés és Aszinkron Beep hangjelzések.
        // =========================================================================

        // Rajzlap konstans méretei (50 oszlop x 20 sor)
        const int VASZON_SZELESSEG = 50;
        const int VASZON_MAGASSAG = 20;

        // Pozicionálás a konzolablakban
        const int PANEL_SZELESSEG = 24;  // Bal oldali panel szélessége
        const int CANVAS_OFFSET_X = 26;  // A rajzlap bal felső sarka (kerettel együtt a 25. oszloptól)
        const int CANVAS_OFFSET_Y = 1;   // A kereten belüli rajzlap első sora

        // Alkalmazás állapota
        static char[,] vaszonKarakter = new char[VASZON_MAGASSAG, VASZON_SZELESSEG];
        static ConsoleColor[,] vaszonSzin = new ConsoleColor[VASZON_MAGASSAG, VASZON_SZELESSEG];

        static int kurzorX = 0; // 0 .. 49
        static int kurzorY = 0; // 0 .. 19
        static int elozoKurzorX = 0;
        static int elozoKurzorY = 0;

        static char rajzKarakter = '#';
        static ConsoleColor rajzSzin = ConsoleColor.Cyan;
        static bool eszkozLeteve = false; // false: Felemelve, true: Letéve

        static bool menuMod = false;
        static int kijeloltMenuIndex = 0;
        static string statuszUzenet = "Üdvözöl a Rajzoló! Nyilak: Mozgás | Szóköz: Toll le/fel";
        static bool fut = true;

        // Beep hangjelzés időzítő (cooldown a beragadás ellen)
        static DateTime utolsoBeepIdopont = DateTime.MinValue;

        [DllImport("user32.dll")]
        static extern bool MessageBeep(uint uType);


        // Használható karakterek és színek listája (a 11.F tananyag alapján)
        static readonly char[] valaszthatoKarakterek = new char[] { '#', '*', '@', 'O', '+', 'X', '█', '▒', '░' };
        static readonly ConsoleColor[] valaszthatoSzinek = new ConsoleColor[] {
            ConsoleColor.Cyan, ConsoleColor.Yellow, ConsoleColor.Green,
            ConsoleColor.Red, ConsoleColor.Magenta, ConsoleColor.White, ConsoleColor.DarkYellow
        };
        static int szinIndex = 0;
        static int karakterIndex = 0;

        // Menüpontok megnevezései (szigorúan 22 karakterre korlátozva)
        static readonly string[] menuPontok = new string[] {
            "1. Karakter váltása",
            "2. Szín váltása",
            "3. Minta (minta.txt)",
            "4. [Saját 1] Középre",
            "5. [Saját 2] Törlés",
            "6. [Saját 3] Tükrözés",
            "7. Vissza rajzoláshoz",
            "8. Kilépés"
        };

        static void Main(string[] args)
        {

            // UTF-8 kódolás beállítása a magyar karakterek és Unicode keretek megjelenítéséhez
            Console.OutputEncoding = Encoding.UTF8;

            // Konzol ablak méretének ellenőrzése (legalább 78 oszlop x 24 sor)
            if (Console.WindowWidth < 78 || Console.WindowHeight < 24)
            {
                try
                {
                    Console.SetWindowSize(80, 25);
                }
                catch
                {
                    Console.WriteLine("Hiba: Nagyítsd a konzolablakot legalább 78 oszloposra és 24 sorosra!");
                    return;
                }
            }

            Console.CursorVisible = false;
            VaszonAlaphelyzetbe();
            KépernyőKirajzolása(); // Kezdeti teljes képernyő kirajzolás

            // Fő vezérlési ciklus
            while (fut)
            {
                // Egy billentyű beolvasása (várakozik a gombnyomásra)
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (menuMod)
                {
                    MenüGombKezelés(keyInfo);
                    KépernyőKirajzolása(); // Menüben a kijelölés váltásakor frissítünk
                }
                else
                {
                    RajzolóGombKezelés(keyInfo);

                    // LAG MEGSZÜNTETÉSE: Ha felhalmozódtak billentyűk a pufferben (pl. nyomva tartott nyíl),
                    // mindegyiket azonnal feldolgozzuk újrarajzolás nélkül, és csak a legutolsó pozíciónál frissítünk!
                    while (Console.KeyAvailable && !menuMod && fut)
                    {
                        ConsoleKeyInfo extraKey = Console.ReadKey(true);
                        RajzolóGombKezelés(extraKey);
                    }
                }
            }

            Console.Clear();
            Console.ResetColor();
            Console.CursorVisible = true;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("A rajzoló program bezárult. Szép napot!");
        }

        // =========================================================================
        // VÁSZON KEZELÉSE ÉS ALAPHELYZET
        // =========================================================================
        static void VaszonAlaphelyzetbe()
        {
            for (int y = 0; y < VASZON_MAGASSAG; y++)
            {
                for (int x = 0; x < VASZON_SZELESSEG; x++)
                {
                    vaszonKarakter[y, x] = ' ';
                    vaszonSzin[y, x] = ConsoleColor.White;
                }
            }
        }

        // =========================================================================
        // KÉPERNYŐ KIRAJZOLÁSA (Teljes redraw szükség esetén)
        // =========================================================================
        static void KépernyőKirajzolása()
        {
            // 1. Bal oldali panel (Menü / Állapot)
            PanelKirajzolása();

            // 2. Rajzlap keretének kirajzolása (Unicode Box Drawing)
            // Felső keret
            Console.SetCursorPosition(CANVAS_OFFSET_X - 1, 0);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write('┌' + new string('─', VASZON_SZELESSEG) + '┐');

            // Oldalsó keretek és vászon cellák
            for (int y = 0; y < VASZON_MAGASSAG; y++)
            {
                // Bal oldali keret
                Console.SetCursorPosition(CANVAS_OFFSET_X - 1, CANVAS_OFFSET_Y + y);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write('│');

                // Vászon sora
                for (int x = 0; x < VASZON_SZELESSEG; x++)
                {
                    EgyCellaKirajzolása(x, y);
                }

                // Jobb oldali keret
                Console.SetCursorPosition(CANVAS_OFFSET_X + VASZON_SZELESSEG, CANVAS_OFFSET_Y + y);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write('│');
            }

            // Alsó keret
            Console.SetCursorPosition(CANVAS_OFFSET_X - 1, CANVAS_OFFSET_Y + VASZON_MAGASSAG);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write('└' + new string('─', VASZON_SZELESSEG) + '┘');

            // 3. Státuszsáv frissítése
            StatuszSavKirajzolasa();
        }

        // =========================================================================
        // CÉLZOTT CELLA KIRAJZOLÁSA (Részleges, villámgyors frissítés 0 lag-gal)
        // =========================================================================
        static void EgyCellaKirajzolása(int x, int y)
        {
            Console.SetCursorPosition(CANVAS_OFFSET_X + x, CANVAS_OFFSET_Y + y);

            if (x == kurzorX && y == kurzorY)
            {
                // Kurzor kiemelése (Piros ha toll letéve, Kék ha felemelve)
                if (eszkozLeteve)
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }

                char kijeloltCh = vaszonKarakter[y, x] == ' ' ? '▉' : vaszonKarakter[y, x];
                Console.Write(kijeloltCh);
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = vaszonSzin[y, x];
                Console.BackgroundColor = ConsoleColor.Black;
                Console.Write(vaszonKarakter[y, x]);
                Console.ResetColor();
            }
        }

        // =========================================================================
        // STÁTUSZSÁV ÉS PANEL RÉSZLEGES FRISSÍTÉSE
        // =========================================================================
        static void StatuszSavKirajzolasa()
        {
            Console.SetCursorPosition(0, VASZON_MAGASSAG + 1);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('─', 77));

            Console.SetCursorPosition(0, VASZON_MAGASSAG + 2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            string uzenetFormazott = FormatalSor(" INFÓ: " + statuszUzenet, 76);
            Console.Write(uzenetFormazott);
            Console.ResetColor();
        }

        static void PozicioPanelFrissites()
        {
            if (!menuMod)
            {
                Console.SetCursorPosition(0, 2);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" Pozíció: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"({kurzorX:D2}, {kurzorY:D2})".PadRight(12));

                Console.SetCursorPosition(0, 5);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" Eszköz:  ");
                if (eszkozLeteve)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[LETÉVE]".PadRight(12));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[FELEMELVE]".PadRight(12));
                }
                Console.ResetColor();
            }
        }

        static void PanelKirajzolása()
        {
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(FormatalSor("=== RAJZOLÓ 2026 ===", PANEL_SZELESSEG));

            if (menuMod)
            {
                Console.SetCursorPosition(0, 1);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(FormatalSor("--- MENÜ MÓD ---", PANEL_SZELESSEG));
                Console.ResetColor();

                for (int i = 0; i < menuPontok.Length; i++)
                {
                    Console.SetCursorPosition(0, 2 + i);
                    if (i == kijeloltMenuIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write(FormatalSor("> " + menuPontok[i], PANEL_SZELESSEG));
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(FormatalSor("  " + menuPontok[i], PANEL_SZELESSEG));
                    }
                }

                // Súgó a menü alatt
                Console.SetCursorPosition(0, 12);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(FormatalSor(" [Nyilak]: Navigáció", PANEL_SZELESSEG));
                Console.SetCursorPosition(0, 13);
                Console.Write(FormatalSor(" [Enter]: Kiválasztás", PANEL_SZELESSEG));
                Console.SetCursorPosition(0, 14);
                Console.Write(FormatalSor(" [Esc]: Vissza rajzhoz", PANEL_SZELESSEG));

                // Üres sorok törlése a panel alján
                for (int row = 15; row <= 20; row++)
                {
                    Console.SetCursorPosition(0, row);
                    Console.Write(new string(' ', PANEL_SZELESSEG));
                }
            }
            else
            {
                Console.SetCursorPosition(0, 1);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(FormatalSor("--- ÁLLAPOTPANEL ---", PANEL_SZELESSEG));
                Console.ResetColor();

                Console.SetCursorPosition(0, 2);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" Pozíció: ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($"({kurzorX:D2}, {kurzorY:D2})".PadRight(14));

                Console.SetCursorPosition(0, 3);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" Karakter:");
                Console.ForegroundColor = rajzSzin;
                Console.Write($" [{rajzKarakter}]".PadRight(14));

                Console.SetCursorPosition(0, 4);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" Rajzszín:");
                Console.ForegroundColor = rajzSzin;
                Console.Write($" {rajzSzin}".PadRight(14));

                Console.SetCursorPosition(0, 5);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(" Eszköz:  ");
                if (eszkozLeteve)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[LETÉVE]".PadRight(14));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("[FELEMELVE]".PadRight(14));
                }
                Console.ResetColor();

                // Billentyűsúgó
                Console.SetCursorPosition(0, 7);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(FormatalSor("--- VEZÉRLÉS ---", PANEL_SZELESSEG));
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.SetCursorPosition(0, 8);
                Console.Write(FormatalSor(" Nyilak : Mozgatás", PANEL_SZELESSEG));
                Console.SetCursorPosition(0, 9);
                Console.Write(FormatalSor(" Szóköz : Toll le/fel", PANEL_SZELESSEG));
                Console.SetCursorPosition(0, 10);
                Console.Write(FormatalSor(" F1-F12 : Nyomdák", PANEL_SZELESSEG));
                Console.SetCursorPosition(0, 11);
                Console.Write(FormatalSor(" M / Tab: Menü megnyitás", PANEL_SZELESSEG));
                Console.SetCursorPosition(0, 12);
                Console.Write(FormatalSor(" Esc    : Kilépés", PANEL_SZELESSEG));

                for (int row = 13; row <= 20; row++)
                {
                    Console.SetCursorPosition(0, row);
                    Console.Write(new string(' ', PANEL_SZELESSEG));
                }
            }
        }

        static string FormatalSor(string szoveg, int maxHossz)
        {
            if (szoveg == null) szoveg = "";
            if (szoveg.Length > maxHossz)
            {
                return szoveg.Substring(0, maxHossz);
            }
            return szoveg.PadRight(maxHossz);
        }

        // =========================================================================
        // VEZÉRLÉS: RAJZOLÓ MÓD
        // =========================================================================
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
                    EgyCellaKirajzolása(kurzorX, kurzorY);
                    PozicioPanelFrissites();
                    StatuszSavKirajzolasa();
                    break;

                case ConsoleKey.M:
                case ConsoleKey.Tab:
                    menuMod = true;
                    statuszUzenet = "Menü mód aktív. Nyilak: Navigáció | Enter: Kiválasztás";
                    KépernyőKirajzolása();
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
                StatuszSavKirajzolasa();
                return;
            }

            elozoKurzorX = kurzorX;
            elozoKurzorY = kurzorY;

            kurzorX = ujX;
            kurzorY = ujY;

            if (eszkozLeteve)
            {
                CellaRajzolása(kurzorX, kurzorY);
            }

            statuszUzenet = $"Pozíció: ({kurzorX}, {kurzorY})";

            // Célzott (részleges) villámgyors frissítés - Nincs képernyővillogás, Nincs lag!
            EgyCellaKirajzolása(elozoKurzorX, elozoKurzorY);
            EgyCellaKirajzolása(kurzorX, kurzorY);
            PozicioPanelFrissites();
            StatuszSavKirajzolasa();
        }

        static void CellaRajzolása(int x, int y)
        {
            vaszonKarakter[y, x] = rajzKarakter;
            vaszonSzin[y, x] = rajzSzin;
        }

        // =========================================================================
        // NYOMDÁK KEZELÉSE ÉS HATÁRELLENŐRZÉS (F1 - F12)
        // =========================================================================
        static void NyomdaRányomása(char tipus, int meret)
        {
            int r = meret / 2; // Sugár: 3->1, 5->2, 7->3

            // ELŐZETES HATÁRELLENŐRZÉS: Ha akárcsak 1 cella is kilóg, nem rajzolunk semmit!
            if (kurzorX - r < 0 || kurzorX + r >= VASZON_SZELESSEG ||
                kurzorY - r < 0 || kurzorY + r >= VASZON_MAGASSAG)
            {
                HangJelzésHiba();
                statuszUzenet = $"HIBA: A(z) {meret}x{meret} nyomda nem fér el a szélén!";
                StatuszSavKirajzolasa();
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

            statuszUzenet = $"Nyomda elhelyezve: {tipus} ({meret}x{meret}) a ({kurzorX},{kurzorY}) ponton";
            KépernyőKirajzolása(); // Nyomda után teljes frissítés
        }

        // =========================================================================
        // VEZÉRLÉS: MENÜ MÓD
        // =========================================================================
        static void MenüGombKezelés(ConsoleKeyInfo keyInfo)
        {
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    kijeloltMenuIndex--;
                    if (kijeloltMenuIndex < 0) kijeloltMenuIndex = menuPontok.Length - 1;
                    break;
                case ConsoleKey.DownArrow:
                    kijeloltMenuIndex++;
                    if (kijeloltMenuIndex >= menuPontok.Length) kijeloltMenuIndex = 0;
                    break;
                case ConsoleKey.Enter:
                    MenüPontVégrehajtása(kijeloltMenuIndex);
                    break;
                case ConsoleKey.Escape:
                case ConsoleKey.M:
                case ConsoleKey.Tab:
                    menuMod = false;
                    statuszUzenet = "Visszatértél a rajzoláshoz.";
                    break;
                default:
                    HangJelzésHiba();
                    statuszUzenet = "Érvénytelen billentyű a menüben!";
                    break;
            }
        }

        static void MenüPontVégrehajtása(int index)
        {
            switch (index)
            {
                case 0: // Karakter váltása
                    karakterIndex = (karakterIndex + 1) % valaszthatoKarakterek.Length;
                    rajzKarakter = valaszthatoKarakterek[karakterIndex];
                    statuszUzenet = $"Új rajzkarakter: '{rajzKarakter}'";
                    break;

                case 1: // Szín váltása
                    szinIndex = (szinIndex + 1) % valaszthatoSzinek.Length;
                    rajzSzin = valaszthatoSzinek[szinIndex];
                    statuszUzenet = $"Új rajzszín: {rajzSzin}";
                    break;

                case 2: // Minta betöltése szövegfájlból
                    MintaBetöltéseFájlból("minta.txt");
                    menuMod = false;
                    break;

                case 3: // Saját 1: Ugrás a középpontba
                    elozoKurzorX = kurzorX;
                    elozoKurzorY = kurzorY;
                    kurzorX = VASZON_SZELESSEG / 2;
                    kurzorY = VASZON_MAGASSAG / 2;
                    statuszUzenet = $"Kurzor a középpontban: ({kurzorX}, {kurzorY})";
                    menuMod = false;
                    break;

                case 4: // Saját 2: Vászon törlése
                    VaszonAlaphelyzetbe();
                    statuszUzenet = "A rajzlap törölve lett!";
                    menuMod = false;
                    break;

                case 5: // Saját 3: Vízszintes tükrözés
                    VászonTükrözéseVízszintesen();
                    statuszUzenet = "A rajzlap vízszintesen meg lett tükrözve!";
                    menuMod = false;
                    break;

                case 6: // Vissza rajzhoz
                    menuMod = false;
                    statuszUzenet = "Rajzolási mód aktív.";
                    break;

                case 7: // Kilépés
                    fut = false;
                    break;
            }
        }

        // =========================================================================
        // MINTA BETÖLTÉSE SZÖVEGFÁJLBÓL ÉS ELLENŐRZÉS
        // =========================================================================
        static void MintaBetöltéseFájlból(string fajlNev)
        {
            // Több lehetséges elérést megvizsgálunk (projektnél és futtatásnál)
            string[] probaUtvonalak = new string[] {
                Path.Combine(AppContext.BaseDirectory, fajlNev),
                Path.Combine(Directory.GetCurrentDirectory(), fajlNev),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", fajlNev)
            };

            string eleresiUt = null;
            foreach (string ut in probaUtvonalak)
            {
                if (File.Exists(ut))
                {
                    eleresiUt = ut;
                    break;
                }
            }

            // Ha sehol sem található, automatikusan létrehozzuk a mintafájlt egy kedves rajzzal!
            if (eleresiUt == null)
            {
                eleresiUt = Path.Combine(AppContext.BaseDirectory, fajlNev);
                try
                {
                    string[] alapMinta = new string[] {
                        "  /\\_/\\  ",
                        " ( o.o ) ",
                        "  > ^ <  ",
                        "========="
                    };
                    File.WriteAllLines(eleresiUt, alapMinta);
                }
                catch { }
            }

            try
            {
                string[] sorok = File.ReadAllLines(eleresiUt);

                if (sorok.Length > VASZON_MAGASSAG)
                {
                    HangJelzésHiba();
                    statuszUzenet = $"HIBA: Túl sok sor ({sorok.Length} > {VASZON_MAGASSAG})!";
                    return;
                }

                for (int i = 0; i < sorok.Length; i++)
                {
                    if (sorok[i].Length > VASZON_SZELESSEG)
                    {
                        HangJelzésHiba();
                        statuszUzenet = $"HIBA: A(z) {i + 1}. sor túl széles!";
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

                statuszUzenet = $"Minta ('{fajlNev}') sikeresen betöltve!";
            }
            catch (Exception ex)
            {
                HangJelzésHiba();
                statuszUzenet = "HIBA: Fájlbeolvasás: " + ex.Message;
            }
        }

        // =========================================================================
        // SAJÁT EXTRA FUNKCIÓ: VÍZSZINTES TÜKRÖZÉS
        // =========================================================================
        static void VászonTükrözéseVízszintesen()
        {
            for (int y = 0; y < VASZON_MAGASSAG; y++)
            {
                for (int x = 0; x < VASZON_SZELESSEG / 2; x++)
                {
                    int jobbX = VASZON_SZELESSEG - 1 - x;

                    char tempChar = vaszonKarakter[y, x];
                    vaszonKarakter[y, x] = vaszonKarakter[y, jobbX];
                    vaszonKarakter[y, jobbX] = tempChar;

                    ConsoleColor tempSzin = vaszonSzin[y, x];
                    vaszonSzin[y, x] = vaszonSzin[y, jobbX];
                    vaszonSzin[y, jobbX] = tempSzin;
                }
            }
        }

        // =========================================================================
        // HANGJELZÉS HIBA ESETÉN (ASZINKRON + COOLDOWN GARANCIA A LAGGOLÁS ELLEN)
        // =========================================================================


        static void HangJelzésHiba()
        {
            // Cooldown, hogy nyomva tartott nyílnál ne torlódjon
            if ((DateTime.Now - utolsoBeepIdopont).TotalMilliseconds < 200)
                return;
            utolsoBeepIdopont = DateTime.Now;

            try
            {
                Console.Beep(800, 120);   // szinkron, pont mint a tesztben
            }
            catch
            {
                try { Console.Write("\a"); } catch { }
            }
        }


    }
}
