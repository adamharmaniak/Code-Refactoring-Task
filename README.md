## Popis jednotlivých zmien

### 1. Odstránené nepoužité `using` príkazy

Pôvodne tam boli napríklad:

```csharp
using System.Text;
using System.Threading.Tasks;
```

---

### 2. Trieda `pixel` bola premenovaná na `Cell`

Názov `pixel` nebol podľa mňa ideálny a dokonca ani nezačínal veľkým písmenom.

---

### 3. Premenné boli premenované na zmysluplné anglické názvy

Pôvodné názvy boli menej čitateľné a neboli konzistentné.

Príklady zmien:

* `hoofd` → `Head`
* `xposlijf`, `yposlijf` → `snake`
* `berryx`, `berryy` → `Food`
* `randomnummer` → `random`
* `schermkleur` → rieši už renderer, nie dátový objekt

---

### 4. Smer pohybu už nie je `string`, ale `enum Direction`

Pôvodne sa používali textové hodnoty:

```csharp
"UP"
"DOWN"
"LEFT"
"RIGHT"
```

Namiesto toho som vytvoril `enum`:

```csharp
enum Direction
{
    Up,
    Down,
    Left,
    Right
}
```

Za mňa je toto bezpečnejšie a prehľadnejšie.

---

### 5. Herná logika je oddelená od GUI

Trieda `SnakeGame` neobsahuje žiadne konzolové príkazy, napríklad:

```csharp
Console.Write();
Console.Clear();
Console.SetCursorPosition();
```

Táto trieda rieši iba správanie hry, napríklad:

* pohyb hada,
* jedenie jedla,
* kontrolu kolízií,
* ukončenie hry.

---

### 6. Vykresľovanie je presunuté do `ConsoleGameRenderer`

Trieda `ConsoleGameRenderer` má jedinú zodpovednosť: vykresliť hru do konzoly. Teda dodržiavame princíp jednej zodpovednosti.

---

### 7. Načítanie vstupu je presunuté do `ConsoleInputReader`

Čítanie klávesnice už nie je priamo v hlavnom hernom cykle.

Trieda `ConsoleInputReader` rieši iba vstup od používateľa a prevod stlačenej klávesy na smer pohybu.

---

### 8. Hlavný `Main` je výrazne kratší

Metóda `Main` teraz iba:

* nastaví konzolu,
* vytvorí potrebné objekty,
* spustí hlavný herný cyklus,
* zobrazí koniec hry.

Detaily sú schované v samostatných triedach a metódach.

---

### 9. Odstránené paralelné zoznamy `xposlijf` a `yposlijf`

Pôvodne sa pozícia tela hada ukladala do dvoch oddelených zoznamov:

```csharp
List<int> xposlijf;
List<int> yposlijf;
```

To mohlo viesť ku chybám, pretože súradnice `X` a `Y` tvoria pár (pozíciu).

Namiesto toho sa používa jeden zoznam:

```csharp
LinkedList<Cell>
```

Jedno políčko `Cell` obsahuje naraz `X` aj `Y`.

---

### 10. Opakovaný kód na kreslenie bol nahradený metódou `DrawCell`

Pôvodne sa viackrát opakovali príkazy ako:

```csharp
Console.SetCursorPosition(...);
Console.ForegroundColor = ...;
Console.Write("■");
```

Teraz sa používa jedna spoločná metóda:

```csharp
DrawCell(...)
```

Tým sa odstránila zbytočná duplicita.

---

### 11. Kontrola kolízií je rozdelená do malých metód

Namiesto veľkého bloku kódu sú jednotlivé kontroly rozdelené do samostatných metód.

Príklady:

* `HitsWall`
* `HitsItself`
* `TrimSnake`
* `CreateFood`

Každá metóda rieši jednu konkrétnu vec.

---

### 12. Konštantné hodnoty sú v `GameSettings`

Hodnoty ako šírka, výška, rýchlosť hry a počiatočné skóre už nie sú rozhádzané po celom kóde.

Sú uložené v triede:

```csharp
GameSettings
```

Vďaka tomu sa nastavenia hry menia jednoduchšie a prehľadnejšie.

---

### 13. Jedlo sa generuje iba mimo steny a mimo hada

Pôvodný kód mohol na začiatku vytvoriť jedlo aj na okraji obrazovky.

Teraz sa jedlo generuje iba vo vnútri hracej plochy:

```csharp
random.Next(1, settings.Width - 1)
random.Next(1, settings.Height - 1)
```

Zároveň sa kontroluje, aby sa jedlo nevygenerovalo tam, kde je had.

---

### 14. Pridaný `Thread.Sleep(10)` pri čakaní na vstup

Pôvodný vnútorný cyklus stále aktívne kontroloval čas a klávesnicu.

Pridaním krátkeho uspania:

```csharp
Thread.Sleep(10);
```

sa zníži zbytočné preťažovanie procesora.
