# 🕵️‍♂️ A* Pathfinding Showcase

A clean, isolated _implementation of the A* pathfinding algorithm in C#_ with real-time Unity visualization. Built as a technical portfolio piece: **not a game**, but a single well-documented system that demonstrates how to write engine-agnostic algorithmic code with a thin Unity adapter on top.

## Preview

<img width="419" height="280" alt="image" src="https://github.com/user-attachments/assets/fc64a3cc-4ffc-43a2-9590-334b5edddcf4" />
<br>

> **Yellow cells** - found path. <br>
> **Blue cells** - nodes explored by A* (closed set). <br>
> **Dark cells** - walls drawn by the user. <br>
> **Green dot** - start. Red dot - goal. <br>
> **!** Visualization (_Gizmos_) is only visible in **Scene View**, not Game View by default. Enable the **Gizmos** toggle in the Game View toolbar to see it during Play Mode.

## Features

- **Pure C# core**: zero `UnityEngine` dependencies inside `Core/`. The pathfinder runs identically in Unity, a console app, or a server.
- **Binary heap open set**: O(log n) insert and extract instead of O(n) linear scan, with a visible difference in the "nodes evaluated" counter.
- **Four heuristics**: Manhattan, Euclidean, Diagonal, Chebyshev - swappable at runtime via UI dropdown.
- **Diagonal movement** with strict corner-cutting prevention: blocks diagonals when either orthogonal neighbour is a wall (matches Unity NavMesh default behaviour).
- **Weighted terrain**: `GridNode.MovementCost` is factored into path cost, ready to use without changing the algorithm.
- **Live Gizmos visualization**: open/closed sets, path, start/goal rendered in Scene View during Play Mode.
- **19 unit tests**: cover pathfinding correctness, heuristic consistency, corner-cutting rules, and heap integrity. Run in EditMode without a scene.

## Tech Stack

| Layer | Technology |
|---|---|
| Language | C# 9 (.NET Standard 2.1) |
| Engine | Unity 2021.3 LTS or newer |
| Input | Unity Input System (new) |
| UI | Unity UI (uGUI) |
| Tests | Unity Test Framework (NUnit, EditMode) |
| Architecture | Interface-segregated Core / Unity adapter |

## Installation

**Requirements:** Unity 2021.3 LTS+, Input System package installed (`Window → Package Manager → Unity Registry → Input System`).
 
1. Clone or download the repository.
2. Copy the three folders into your Unity project's `Assets/` directory:
```
Assets/
└── AStarShowcase/
    ├── Core/        ← algorithm, no Unity dependency
    ├── Unity/       ← MonoBehaviour adapter + visualization
    └── Tests/       ← NUnit EditMode tests
```
 
3. Unity will compile automatically. If prompted about the Input System backend: click **Yes** and let the editor restart.
4. Open `SampleScene` (or set up manually):
   - Create an empty `GameObject`, name it `GridManager`
   - Add components: `Grid View` and `Pathfinding Demo`
   - In `PathfindingDemo` inspector: drag `GridManager` into the **Grid View** field
5. Set **Main Camera**:
   - `Projection`: Orthographic
   - `Size`: 12
   - Position: `(15, 10, -10)` to centre the default 30×20 grid
6. Press **Play**.

### Running tests
 
`Window → General → Test Runner → EditMode → Run All`
 
No scene required - the Core layer has no Unity runtime dependency.
 
---
 
## Controls
 
| Input | Action |
|---|---|
| `Shift + Left Click` | Place start node (green) |
| `Right Click` | Place goal node (red) |
| `Left Click + Drag` | Draw walls |
| `Left Click + Drag` over wall | Erase walls |
 
Path recalculates instantly after every change. Switch heuristics or toggle diagonal movement via the optional UI panel.

## 👤 Author
🦾 Crafted by Aleksandra Kenig (aka [yourpunk](https://github.com/yourpunk)).<br>
💌 Wanna collab or throw some feedback? You know where to find me.
