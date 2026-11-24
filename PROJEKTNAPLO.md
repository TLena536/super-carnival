# PROJEKTNAPLÓ - Konzolos Rajzoló Alkalmazás

**Projekt neve:** Konzolos Rajzoló (C# Console Application)  
**Tantárgy / Osztály:** IKT projektek / 11.F osztály  
**Fejlesztők / Páros:** [Tanuló 1 Neve] és [Tanuló 2 Neve]  
**Fejlesztési környezet:** Visual Studio 2022 / .NET 8.0 Console Application  

---

## 1. Munkamegosztás és Felelősök

A projektet az útmutatónak megfelelően két fő részre és egyéni funkciókra osztottuk fel:

| Tanuló neve | Elvállalt feladatok és modulok | Saját egyedi funkció |
| :--- | :--- | :--- |
| **Tanuló 1** | Menürendszer megvalósítása, Kurzormozgatás a nyilakkal, Állapotjelző panel frissítése, Toll le/fel státusz kezelése, Célzott cellafrissítési logika | **1. Ugrás a középpontba (25, 10)**<br>**2. Vászon teljes törlése** |
| **Tanuló 2** | Nyomdák (F1–F12) rajzolási logikája, Nyomda határellenőrzése, Minta betöltése `minta.txt` fájlból és pre-validáció, Aszinkron `Beep` kezelés | **3. Vászon vízszintes tükrözése** |

---

## 2. Közös Tervezési Döntések

1. **Ablakméret és Elrendezés:**
   - A konzolablak minimális mérete 78 oszlop széles és 24 sor magas.
   - Bal oldalon (0..24 oszlop) a Menü és Állapotjelző panel található. A panel feliratai szigorúan 24 karakteres szélességre vannak korlátozva (`PadRight` / `Substring`), így nem lógnak át a rajzterületre.
   - Jobb oldalon a 50 oszlop széles × 20 sor magas Rajzlap helyezkedik el, amelyet látható Unicode keret (`┌ ─ ┐`, `│`, `└ ─ ┘`) zár körbe a 25. és 76. oszlopok között.
   - Alsó státuszsáv mutatja az aktuális működési jelzéseket és a hibákat.

2. **Koordinátarendszer Transzformáció:**
   - A rajzlap belső koordinátái `(0, 0)` és `(49, 19)` között mozognak.
   - A konzol képernyőjén a rajzolási pozíció vízszintes értéke: `26 + x`, függőleges értéke: `1 + y`.

3. **Biztonsági Pre-Check és Teljesítmény Szabályok:**
   - **Mozgás:** Ha a kurzor eléri a 0 vagy 49 (X), illetve 0 vagy 19 (Y) határt, aszinkron hangjelzés szólal meg, a kurzor nem lép tovább, a rajz érintetlen marad.
   - **Lag-mentesítés:** A kurzormozgatás során a program csak azt a 2 cellát frissíti a képernyőn, amelyek megváltoztak (az előző pozíciót és az újat). A `Console.KeyAvailable` puffermentesítésnek köszönhetően nyomva tartott nyilak esetén sem halmozódik fel a rajzolás.
   - **Aszinkron Beep:** A hiba-hangjelzés `Task.Run` háttérszálon fut 250ms-os védelmi időzítővel (cooldown), így a fő rajzoló hurok 0 ms-ot sem várakozik.
   - **Nyomdák (F1-F12):** A nyomda elhelyezése előtt a teljes befoglaló négyzeten határellenőrzést végzünk (`kurzorX - r >= 0 && kurzorX + r < 50` stb.). Ha akárcsak 1 cella is kilóg, hiba-hangjelzés adódik és a teljes rajz változatlan marad.
   - **Fájlbetöltés (`minta.txt`):** Beolvasás előtt ellenőrizzük a fájl létezését, a sorok számát (max 20 sor) és az egyes sorok szélességét (max 50 karakter). Hiba esetén hiba-hangjelzés szólal meg, a státuszsáv jelzi a hiányt, és az előző rajz megmarad (nem hoz létre automatikusan mintafájlt).

---

## 3. Git Commit Stratégia és Mérföldkövek

A közös munkát a `main` ágon végeztük megbízható és atomi commitokkal:

1. `commit #1` (Init): `Kezdőprojekt struktúra létrehozása, PROJEKTNAPLO.md inicializálása`
2. `commit #2` (UI): `Képernyő elrendezése: bal panel (0-24 oszlop) és keretezett rajzlap (25-76 oszlop) kirajzolása`
3. `commit #3` (Mozgás): `Kurzormozgatás nyilakkal, határellenőrzés és aszinkron Beep hangjelzés megvalósítása`
4. `commit #4` (Rajzolás): `Toll állapotának váltása Szóközzel, célzott cellafrissítés kiválasztott karakterszínnel`
5. `commit #5` (Menü): `Interaktív menürendszer és státuszpanel frissítése szigorú panel-szélesség korlátozással`
6. `commit #6` (Nyomdák): `F1-F12 alakzat nyomdák (X, Kereszt, Négyzet, Rombusz) 3x3, 5x5, 7x7 méretben`
7. `commit #7` (Nyomda Validáció): `Nyomdák előzetes határellenőrzése - atomic rollback hiba esetén`
8. `commit #8` (Fájl I/O): `Minta betöltése minta.txt fájlból soronkénti méret- és létezés-validációval`
9. `commit #9` (Saját funkciók): `Extra funkciók beépítése: Ugrás középre, Vászon törlése, Vízszintes tükrözés`
10. `commit #10` (Refaktor és Dokumentáció): `Lag-mentesítés véglegesítése (KeyAvailable), Beep cooldown finomhangolása, végleges tesztelés`

---

## 4. Mesterséges Intelligencia (AI) Használati Napló

* **Tanuló 1 AI Tanulási Naplója:**
  > *"A konzolos képernyő-frissítés lag-mentesítéséhez kértem segítséget az AI asszisztenstől. Megtanultam, hogy a teljes képernyő újrarajzolása helyett elegendő csak az elhagyott és az új kurzorpozíció celláját frissíteni `Console.SetCursorPosition(x, y)` segítségével, valamint a `PadRight()` és `Substring()` metódusok együttes használatával megakadályozható a menüszövegek átlógása a rajzvászonra."*

* **Tanuló 2 AI Tanulási Naplója:**
  > *"Az aszinkron hangjelzéshez és a 2D tömbös nyomdák határellenőrzéséhez kértem segítséget. Megértettem, hogy a `Console.Beep()` szinkron hívása megakasztaná a fő ciklust, ezért célszerű azt `Task.Run` háttérszálon futtatni egy rövid védelmi időzítővel (cooldown). Emellett elsajátítottam a `File.ReadAllLines()` tömbjének soronkénti pre-validációját is."*

---

## 5. Tesztelési Bejegyzések és Visszajelzések

A tesztelést a mellékelt Tesztjegyzőkönyv alapján végeztük el a tesztelő párossal:

* **T01-T04 (Alapvezérlés & Határok):** PASS - A kurzor azonnal, akadásmentesen mozog a nyilakkal. A falaknál a hiba-hangjelzés rögtön megszólal és a rajz nem módosul.
* **T05 (Toll le/fel & Cellafrissítés):** PASS - A Szóköz gombbal a toll állapot vált. Letett állapotban a mozgás során folyamatos és lag-mentes a rajzolás.
* **T06-T08 (Nyomdák):** PASS - Az F1-F12 nyomdák pontosan rajzolják az X, Kereszt, Négyzet és Rombusz alakzatokat. Széleken megkísérelt nyomdázás esetén hibaüzenet és hangjelzés adódik, a vászon sértetlen marad.
* **T09-T10 (Fájlbetöltés):** PASS - A létező `minta.txt` helyesen betöltődik. Nem létező fájl esetén a program hiba-hangjelzést ad, jelez a státuszsávon, és megőrzi az előző rajzot anélkül, hogy automatikusan fájlt hozna létre.
* **T11-T12 (Saját funkciók):** PASS - Az Ugrás középre, Vászon törlése és Vízszintes tükrözés hibátlanul működik.
