# Konzolos Rajzoló Alkalmazás (C#)

Egy páros projekt C# nyelven, amely lehetővé teszi konzolos felületen a szabadkézi rajzolást, alakzatok (X, Kereszt, Négyzet, Rombusz) nyomdázását és rajzminták betöltését szövegfájlból.

## Indítási útmutató

1. Nyisd meg a `Rajzolo.csproj` projektet Visual Studióban.
2. Futtasd a `Ctrl + F5` billentyűkombinációval, vagy parancssorból:
   ```bash
   dotnet run
   ```
3. Fontos: A konzolablak szélessége legyen legalább **78 oszlop**, magassága legalább **24 sor**!

## Vezérlés billentyűzettel

### Rajzoló mód:
- **Nyíl billentyűk (`↑`, `↓`, `←`, `→`):** Kurzormozgatás a rajzlapon (határellenőrzéssel).
- **Szóköz (`Space`):** Toll felemelése / letétele (Letett állapotban mozgás közben rajzol).
- **F1 – F12:** Nyomdák elhelyezése a kurzor középpontjával:
  - `F1 / F2 / F3`: X nyomda (3x3, 5x5, 7x7)
  - `F4 / F5 / F6`: Kereszt (+) nyomda (3x3, 5x5, 7x7)
  - `F7 / F8 / F9`: Négyzetkeret nyomda (3x3, 5x5, 7x7)
  - `F10 / F11 / F12`: Rombuszkeret nyomda (3x3, 5x5, 7x7)
- **`M` / `Tab`:** Váltás a Menü módba.
- **`Esc`:** Kilépés a programból.

### Menü mód:
- **Nyíl billentyűk (`↑`, `↓`):** Menüpontok közötti navigálás.
- **`Enter`:** Menüpont kiválasztása.
- **`Esc` / `Tab`:** Visszatérés a rajzoláshoz.

## Elérhető Menüfunkciók
1. **Karakter váltása:** Hozzáférhető rajzjelek (`#`, `*`, `@`, `O`, `+`, `X`, `█`, `▒`).
2. **Szín váltása:** Színek beállítása (Cián, Sárga, Zöld, Piros, Magenta, Fehér).
3. **Minta betöltése:** Szöveges minta beolvasása a `minta.txt` fájlból.
4. **[Saját 1] Ugrás a középpontba:** Kurzormozgatás a (25, 10) pozícióra.
5. **[Saját 2] Vászon törlése:** A rajzlap alaphelyzetbe állítása.
6. **[Saját 3] Vízszintes tükrözés:** A rajzlap vízszintes tengely szerinti megfordítása.
