# FCAIH_CG_Project

Computer Graphics course project consisting of three parts: two Unity 2D platformer games and a set of C# Windows Forms applications demonstrating computer graphics algorithms.

## Project Structure

```
├── Desert Dash/        — Unity 2D platformer game (desert theme)
├── Platformer/         — Unity 2D platformer game (pixel-art style)
├── Package/            — C# CG algorithm demos (Windows Forms)
└── README.md
```

## Desert Dash

A 2D side-scrolling platformer built with **Unity 6000.0.39f1**. The player controls a character running through a desert environment, avoiding enemies and collecting coins across multiple levels.

### Features
- **Player**: Run and jump with sprite flipping and animation (idle, running, jumping)
- **Enemy AI**: Patrol boar that moves between two waypoints, flipping direction
- **Collectibles**: Coins with score tracking via a singleton `scoreManager`
- **Levels**: Level1, Level2, Level3, Level4 with finish points (`FinshPoint`, `EndLevel`)
- **UI**: Main menu, pause menu (resume, restart, quit), level selection screen, volume slider
- **Audio**: Background music and SFX controlled via `AudioMenu` and `VolumeSetting`

### Controls
- **Arrow Keys / A,D** — Move left/right
- **Space / W / Up Arrow** — Jump
- **ESC** — Pause

## Platformer

A 2D pixel-art platformer built with **Unity 2020.3.8f1** using the Pixel Adventure 1 asset pack. The player navigates levels with moving platforms, collects cherries, avoids traps, and reaches the finish flag.

### Features
- **Player**: 4-state animation (idle, running, jumping, falling) with jump sound effect
- **Moving Platforms**: `WaypointFollower` oscillates platforms between two points
- **Sticky Platforms**: `StickyPlatform` attaches the player to moving platforms via `OnTriggerEnter2D`
- **Collectibles**: Cherries tracked by `ItemCollector` (UI counter + sound)
- **Hazards**: Trap objects trigger death animation and level restart via `PlayerLife`
- **UI**: Start screen, end menu, in-game cherry counter

### Controls
- **Arrow Keys / A,D** — Move left/right
- **Space / W / Up Arrow** — Jump

### Scenes
| Scene | Description |
|-------|-------------|
| Start Screen | Title and play button |
| Level 1 | First platforming level |
| Level 2 | Second level with moving platforms |
| End Screen | Victory screen |

## Package

A collection of C# Windows Forms (.NET Framework) applications demonstrating fundamental computer graphics algorithms. Each app features an interactive canvas with a Cartesian grid, coordinate labels, zoom (mouse wheel), and pan (click-and-drag).

| Project | Algorithm | Inputs |
|---------|-----------|--------|
| **DDA** | DDA (Digital Differential Analyzer) line drawing | Two endpoints (X1,Y1 → X2,Y2) |
| **Bresenham** | Bresenham's line drawing with octant classification | Two endpoints (X1,Y1 → X2,Y2) |
| **Circle** | Midpoint circle drawing algorithm | Center (X,Y) and radius (R) |
| **Ellipse** | Midpoint ellipse drawing algorithm | Center (X,Y), radii (Rx, Ry) |
| **TransformationApp** | 2D transformations on a triangle | 3 vertices + operation parameters |

### TransformationApp Operations
- **Translate** — Move by (Tx, Ty)
- **Scale** — Scale by (Sx, Sy)
- **Rotate** — Rotate by angle θ (using custom Sin/Cos via Taylor series)
- **Reflect X / Reflect Y** — Mirror over X or Y axis
- **Shear X / Shear Y** — Shear along X or Y axis

### Interactive Canvas Controls (all apps)
- **Mouse wheel** — Zoom in/out
- **Click + drag** — Pan the coordinate grid
- **Coordinate grid** — Axis lines with tick labels
