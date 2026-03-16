# Command Pattern Game — Battaglia Navale

Progetto Unity didattico che implementa il **Command Pattern** attraverso un prototipo di Battaglia Navale a turni.

---

## Indice

- [Panoramica](#panoramica)
- [Design Pattern](#design-pattern)
- [Struttura del Progetto](#struttura-del-progetto)
- [Script](#script)
- [Architettura](#architettura)
- [Funzionalità](#funzionalità)
- [Requisiti](#requisiti)

---

## Panoramica

Il progetto dimostra l'applicazione del **Command Pattern** in un contesto di gioco a turni. Due giocatori si alternano eseguendo attacchi su una griglia 10×10. Ogni azione viene incapsulata in un oggetto comando, che può essere eseguito e annullato.

---

## Design Pattern

### Command Pattern

Il pattern è strutturato come segue:

```
ICommand (Interface)
    └── AttackCommand (Concrete Command)
            ├── Execute()  →  esegue l'attacco sulla griglia
            └── Undo()     →  annulla l'attacco (ripristina la cella)
```

- **Invoker** — `TurnManager`: esegue i comandi e mantiene la cronologia in uno `Stack<ICommand>`.
- **Receiver** — `BattleGrid` / `Cell`: riceve ed elabora i comandi.
- **Command** — `AttackCommand`: incapsula l'azione con i parametri necessari.

### Altri pattern applicati

| Pattern | Dove |
|---|---|
| State | `CellState` (Intact / Hit) |
| Manager | `GameManager`, `TurnManager` |
| Dependency Injection | `[SerializeField]` su MonoBehaviour |

---

## Struttura del Progetto

```
Command_Pattern_Game/
├── Assets/
│   ├── Project/
│   │   └── CommandPattern/
│   │       └── Scripts/
│   │           ├── Commands/
│   │           │   ├── ICommand.cs
│   │           │   └── AttackCommand.cs
│   │           ├── Core/
│   │           │   ├── BattleGrid.cs
│   │           │   └── Cell.cs
│   │           ├── Entities/
│   │           │   ├── IPlayer.cs
│   │           │   └── Player.cs
│   │           └── Managers/
│   │               ├── GameManager.cs
│   │               └── TurnManager.cs
│   └── Scenes/
│       └── SampleScene.unity
├── Packages/
│   └── manifest.json
└── ProjectSettings/
    └── ProjectVersion.txt
```

---

## Script

### `ICommand.cs` — Namespace: `BattleNavale.Commands`

Interfaccia base del Command Pattern.

| Metodo | Descrizione |
|---|---|
| `Execute()` | Esegue il comando |
| `Undo()` | Annulla il comando |

---

### `AttackCommand.cs` — Namespace: `BattleNavale.Commands`

Comando concreto che rappresenta un attacco a una cella della griglia nemica.

| Membro | Tipo | Descrizione |
|---|---|---|
| `_x`, `_y` | `int` | Coordinate dell'attacco |
| `_enemyGrid` | `BattleGrid` | Riferimento alla griglia del nemico |
| `Execute()` | metodo | Chiama `BattleGrid.ReceiveAttack(x, y)` |
| `Undo()` | metodo | Chiama `BattleGrid.UndoAttack(x, y)` |

---

### `Cell.cs` — Namespace: `BattleNavale.Core`

Rappresenta una singola cella della griglia.

| Membro | Descrizione |
|---|---|
| `CellState` (enum) | `Intact` o `Hit` |
| `ReceiveAttack()` | Imposta lo stato su `Hit` |
| `UndoAttack()` | Ripristina lo stato su `Intact` |

---

### `BattleGrid.cs` — Namespace: `BattleNavale.Core`

`MonoBehaviour` che gestisce la griglia 10×10.

| Membro | Descrizione |
|---|---|
| `_cells[10,10]` | Matrice di celle |
| `Awake()` | Inizializza la griglia |
| `ReceiveAttack(x, y)` | Delega l'attacco alla cella `[x, y]` |
| `UndoAttack(x, y)` | Delega l'undo alla cella `[x, y]` |

---

### `IPlayer.cs` — Namespace: `BattleNavale.Entities`

Interfaccia per i giocatori.

| Metodo | Descrizione |
|---|---|
| `TakeTurn()` | Attiva il turno del giocatore |

---

### `Player.cs` — Namespace: `BattleNavale.Entities`

`MonoBehaviour` che implementa `IPlayer`. Gestisce l'input del giocatore durante il suo turno.

| Membro | Descrizione |
|---|---|
| `_isMyTurn` | Flag per abilitare l'input |
| `_turnManager` | Riferimento serializzato al `TurnManager` |
| `_battleGrid` | Riferimento serializzato alla `BattleGrid` nemica |
| `TakeTurn()` | Abilita l'input |
| `Update()` | Legge il click sinistro del mouse e invia un `AttackCommand` al `TurnManager` |

> **Nota:** Le coordinate dell'attacco sono attualmente hardcoded a `(0, 0)` — placeholder da completare con la logica di selezione cella.

---

### `TurnManager.cs` — Namespace: `BattleNavale.Managers`

`MonoBehaviour` che orchestra i turni e la cronologia dei comandi.

| Membro | Tipo | Descrizione |
|---|---|---|
| `_turnQueue` | `Queue<IPlayer>` | Coda circolare dei giocatori |
| `_commandHistory` | `Stack<ICommand>` | Cronologia comandi per l'undo |
| `StartGame(p1, p2)` | metodo | Inizializza la coda con i due giocatori |
| `NextTurn()` | metodo | Passa il turno al giocatore successivo |
| `ExecuteCommand(cmd)` | metodo | Esegue il comando e lo aggiunge allo stack |
| `UndoLastCommand()` | metodo | Preleva e annulla l'ultimo comando dello stack |

---

### `GameManager.cs` — Namespace: `BattleNavale.Managers`

`MonoBehaviour` responsabile dell'inizializzazione della partita.

| Membro | Descrizione |
|---|---|
| `_turnManager` | Riferimento serializzato |
| `_playerOne`, `_playerTwo` | Riferimenti serializzati ai due giocatori |
| `Start()` | Chiama `TurnManager.StartGame(playerOne, playerTwo)` |

---

## Architettura

```
GameManager
    └── TurnManager.StartGame(playerOne, playerTwo)
            │
            ├── NextTurn() → Player.TakeTurn()
            │
            └── ExecuteCommand(ICommand)
                    │
                    ├── command.Execute() → BattleGrid.ReceiveAttack(x, y) → Cell.ReceiveAttack()
                    └── command.Undo()   → BattleGrid.UndoAttack(x, y)   → Cell.UndoAttack()

Stack<ICommand> _commandHistory  ←  cronologia per Undo
Queue<IPlayer>  _turnQueue       ←  rotazione turni
```

---

## Funzionalità

- **Turni a rotazione** — i giocatori si alternano tramite una `Queue`.
- **Esecuzione comandi** — ogni attacco è un oggetto `ICommand` autonomo.
- **Undo** — l'ultimo attacco può essere annullato tramite `TurnManager.UndoLastCommand()`.
- **Griglia 10×10** — ogni cella tiene traccia del proprio stato (`Intact` / `Hit`).
- **Estensibilità** — nuovi comandi si aggiungono implementando `ICommand`, senza modificare il codice esistente.

---

## Requisiti

- **Unity** 2022.3.62f3 (LTS)
- **.NET** Standard 2.1
- Nessuna dipendenza esterna

---

## Namespace

| Namespace | Contenuto |
|---|---|
| `BattleNavale.Commands` | `ICommand`, `AttackCommand` |
| `BattleNavale.Core` | `BattleGrid`, `Cell`, `CellState` |
| `BattleNavale.Entities` | `IPlayer`, `Player` |
| `BattleNavale.Managers` | `GameManager`, `TurnManager` |
