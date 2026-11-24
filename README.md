# Konzolos Rajzoló Alkalmazás (C#)

## 1. Képernyő-elrendezés és Felület

* **Bal oldali panel (0–24. oszlop):**
  * **Állapotpanel mód:** Folyamatosan mutatja a kurzor aktuális pozícióját `(X, Y)`, a kiválasztott rajzkaraktert, a rajzszínt, az eszköz állapotát (`[LETÉVE]` / `[FELEMELVE]`), valamint a billentyűzet-súgót.
  * **Menü mód:** Interaktív navigációs menüt jelenít meg a beállításokhoz és speciális funkciókhoz.
  * A panel szövegei szigorúan 24 karakteres szélességre vannak korlátozva, így garantáltan nem lógnak át a rajzterületre.
* **Jobb oldali rajzvászon (26–75. oszlop, 1–20. sor):**
  * Egy **50 oszlop széles × 20 sor magas** rajzfelület, amelyet látható Unicode keret (`┌ ─ ┐`, `│`, `└ ─ ┘`) zár körbe.
* **Alsó státuszsáv:**
  * Az aktuális végrehajtott műveletekről és esetleges hibákról ad visszajelzést (pl. határok elérése, nyomda-illenevés).

---

## 2. Vezérlés és Alapfunkciók

**Rajzolási mód:**
* **Nyíl billentyűk (`↑`, `↓`, `←`, `→`):** A kurzor mozgatása a 50×20-as rácson. A léptetés villámgyors és akadásmentes (kizárólag a megváltozott 2 cellát frissíti a konzolon).
* **Szóköz (`Space`):** Toll felemelése és letétele. Letett állapotban a kurzor mozgatása közben folyamatosan beírja a beállított karaktert és színt a mezőkbe.
* **Nyomdák (`F1` – `F12`):** Előre definiált geometrikus alakzatok elhelyezése a kurzor középpontjával:
  * `F1 / F2 / F3`: **X** nyomda (3×3, 5×5, 7×7 méretben)
  * `F4 / F5 / F6`: **Kereszt (+)** nyomda (3×3, 5×5, 7×7 méretben)
  * `F7 / F8 / F9`: **Négyzetkeret** nyomda (3×3, 5×5, 7×7 méretben)
  * `F10 / F11 / F12`: **Rombuszkeret** nyomda (3×3, 5×5, 7×7 méretben)
* **`M` / `Tab`:** Átváltás a Menü módba.
* **`Esc`:** Kilépés a programból.

---

## 3. Menüfunkciók

* **1. Karakter váltása:** Cserélhető rajzjelek (`#`, `*`, `@`, `O`, `+`, `X`, `█`, `▒`, `░`).
* **2. Szín váltása:** Választható rajzszínek (Cián, Sárga, Zöld, Piros, Magenta, Fehér, Sötétsárga).
* **3. Minta betöltése (`minta.txt`):** Beolvassa a rajzot a szövegfájlból. Kizárólag akkor tölti be, ha a fájl létezik, és nem haladja meg a 50×20-as mérethatárt.
* **4. [Saját 1] Ugrás a középpontba:** A kurzor azonnali áthelyezése a vászon közepére `(25, 10)`.
* **5. [Saját 2] Vászon törlése:** A teljes rajzlap alaphelyzetbe állítása (ürítése).
* **6. [Saját 3] Vízszintes tükrözés:** A meglévő rajz megfordítása a vízszintes tengely mentén.

---

## 4. Hibakezelés és Biztonság

* **Határellenőrzés (Pre-check):** Ha a kurzor vagy egy nyomda kilógna a rajzlap széléről, a program hiba-hangjelzést ad, a művelet megszakad, és a rajzlap érintetlen marad.
* **Fájlvalidáció:** Ha a `minta.txt` nem létezik vagy hibás formátumú, hibaüzenet jelenik meg a státuszsávon, és az előző rajz megőrződik.
* **Aszinkron hangjelzés:** A hiba-csipogás külön háttérszálon fut védelmi időzítővel, így nem akasztja meg a rajzoló ciklust.
