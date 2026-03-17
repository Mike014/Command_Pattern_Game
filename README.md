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

Il progetto dimostra l'applicazione del **Command Pattern** in un contesto di gioco a turni. Due giocatori si alternano eseguendo attacchi su una griglia 3D configurabile. Ogni azione viene incapsulata in un oggetto comando, che può essere eseguito e annullato. La selezione della cella avviene tramite raycast dal mouse; il feedback visivo (colore della cella) riflette lo stato corrente.

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

Rappresenta una singola cella della griglia (dati puri, nessun `MonoBehaviour`).

| Membro | Descrizione |
|---|---|
| `CellState` (enum) | `Intact` o `Hit` |
| `State` | Getter pubblico dello stato corrente |
| `ReceiveAttack()` | Imposta lo stato su `Hit` |
| `UndoAttack()` | Ripristina lo stato su `Intact` |

---

### `BattleGrid.cs` — Namespace: `BattleNavale.Core`

`MonoBehaviour` che gestisce la griglia configurabile e la sua rappresentazione visiva 3D.

| Membro | Tipo | Descrizione |
|---|---|---|
| `_cells[,]` | `Cell[,]` | Matrice logica delle celle |
| `_cellObjects[,]` | `GameObject[,]` | Oggetti 3D della griglia nella scena |
| `_cellRenderers[,]` | `Renderer[,]` | Renderer per il feedback visivo |
| `_cellPrefab` | `GameObject` | Prefab usato per istanziare ogni cella |
| `_cellSize` | `float` | Distanza tra le celle (default: `1`) |
| `CellSize` | getter | Espone `_cellSize` agli altri script |
| `_gridWidth`, `_gridHeight` | `int` | Dimensioni configurabili della griglia (default: `5`) |
| `Awake()` | metodo | Inizializza le matrici e chiama `GenerateGrid()` |
| `GenerateGrid()` | metodo | Istanzia i prefab nella scena sotto un `GridHolder` |
| `ReceiveAttack(x, y)` | metodo | Valida la cella (blocca se già colpita), delega a `Cell`, colora la cella in **rosso** |
| `UndoAttack(x, y)` | metodo | Delega l'undo a `Cell`, ripristina il colore in **bianco** |
| `OnDestroy()` | metodo | Distrugge il `GridHolder` |

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
| `_mainCamera` | Riferimento a `Camera.main`, inizializzato in `Awake()` |
| `TakeTurn()` | Abilita l'input impostando `_isMyTurn = true` |
| `Update()` | **Click sinistro** — lancia un raycast dalla camera; calcola le coordinate della cella colpita tramite `hit.point` e `CellSize`; crea un `AttackCommand` e lo passa al `TurnManager` |
| `Update()` | **Tasto Z** — chiama `TurnManager.UndoLastCommand()` per annullare l'ultimo attacco |

---

### `TurnManager.cs` — Namespace: `BattleNavale.Managers`

`MonoBehaviour` che orchestra i turni e la cronologia dei comandi.

| Membro | Tipo | Descrizione |
|---|---|---|
| `_turnQueue` | `Queue<IPlayer>` | Coda circolare dei giocatori |
| `_commandHistory` | `Stack<ICommand>` | Cronologia comandi per l'undo |
| `StartGame(p1, p2)` | metodo | Inizializza la coda con i due giocatori e avvia il primo turno |
| `NextTurn()` | metodo | Passa il turno al giocatore successivo (ruota la coda) |
| `ExecuteCommand(cmd)` | metodo | Esegue il comando, lo aggiunge allo stack e chiama `NextTurn()` |
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
            │       └── Update() → Raycast → coordinate (x, y)
            │
            └── ExecuteCommand(ICommand)
                    │
                    ├── command.Execute() → BattleGrid.ReceiveAttack(x, y)
                    │       ├── valida CellState (blocca se già Hit)
                    │       ├── Cell.ReceiveAttack()  → CellState = Hit
                    │       └── Renderer.color = Color.red
                    │
                    └── command.Undo()   → BattleGrid.UndoAttack(x, y)
                            ├── Cell.UndoAttack()    → CellState = Intact
                            └── Renderer.color = Color.white

Stack<ICommand> _commandHistory  ←  cronologia per Undo
Queue<IPlayer>  _turnQueue       ←  rotazione turni
```

---

## Funzionalità

- **Turni a rotazione** — i giocatori si alternano tramite una `Queue`.
- **Esecuzione comandi** — ogni attacco è un oggetto `ICommand` autonomo.
- **Undo (tasto Z)** — l'ultimo attacco può essere annullato; il colore della cella viene ripristinato.
- **Selezione cella via raycast** — il click del mouse lancia un `Physics.Raycast` dalla camera; le coordinate logiche vengono calcolate da `hit.point` e `CellSize`.
- **Validazione attacchi** — `BattleGrid.ReceiveAttack()` blocca gli attacchi su celle già colpite.
- **Feedback visivo** — le celle colpite diventano **rosse**; l'undo le riporta **bianche**.
- **Griglia configurabile** — `_gridWidth`, `_gridHeight` e `_cellSize` sono `[SerializeField]` impostabili dall'Inspector (default: 5×5).
- **Generazione 3D automatica** — `GenerateGrid()` istanzia i prefab nella scena all'avvio.
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
