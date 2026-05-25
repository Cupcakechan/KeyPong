# KEY PONG — Game Design Document

**Project:** Game 1 of the 20 Games Challenge
**Engine:** Unity 6 (exclusively)
**Language:** C#
**Input:** New Input System (Unity 6 best practice)
**Document Version:** 1.0 — FINAL (locked May 24, 2026)
**Author:** [You] · Creative vision by [You] + Benjamin (Grok) · Technical authoring by Claude

---

## 1. Overview / Concept

**Key Pong** is a single-player reskin of the classic 1972 *Pong*, themed entirely around a **mechanical keyboard / typing-chaos** aesthetic in pixel art. The player controls a paddle made from a **vertical Spacebar key**, defending against a simple AI opponent.

The signature twist: **the ball is a keyboard key**, and it **morphs into a different random key sprite every time it strikes a paddle or a wall** — like a keystroke firing off on every bounce. Combined with neon-glow accents and a dark retro-arcade backdrop, the result is the familiar Pong loop wearing a fresh, satisfying "mechanical keyboard" coat.

---

## 2. Purpose

- **Primary goal:** Complete the foundational game of the 20 Games Challenge (Pong) and ship a polished, public build.
- **Learning targets:**
  - Unity 6 project setup, folder structure, and Git/GitHub workflow.
  - The **New Input System** end-to-end.
  - 2D physics, collision response, and reflection math for ball/paddle bounce.
  - **Dynamic runtime sprite swapping** from a sliced sprite sheet (the key-morph + number-key score system).
  - Clean **scene management** (Main Menu → Gameplay → Game Over → restart).
- **Deliverable:** A WebGL build (and Windows build if feasible) ready for **itch.io** and the portfolio.

---

## 3. Target Audience

- **Primary:** The developer's portfolio reviewers and the 20 Games Challenge community.
- **Secondary:** Casual web players on itch.io who enjoy quick, nostalgic arcade games.
- **Skill assumption:** Anyone who has touched a keyboard can play instantly — zero learning curve.

---

## 4. Core Game Loop

```
        ┌─────────────────────────────────────────────┐
        │  Ball launches from center (random key)      │
        │              ↓                               │
        │  Player moves paddle (W/S or ↑/↓)            │
        │  AI moves right paddle toward ball           │
        │              ↓                               │
        │  Ball hits paddle/wall →                     │
        │     • bounces                                │
        │     • SWAPS to a new random key sprite       │
        │     • speed nudges up slightly               │
        │              ↓                               │
        │  Ball passes a paddle → opponent scores      │
        │              ↓                               │
        │  Score updates (number-key sprites)          │
        │  Ball resets to center w/ new random key     │
        └──────────────┬──────────────────────────────┘
                       ↓
            First to 11 points → Game Over / Victory
```

---

## 5. Game Structure & Scene Flow

Every scene in the build, in order:

```
[ Boot ] → MainMenu ──START GAME──▶ Gameplay ──(score = 11)──▶ GameOver
              │                         ▲                          │
        HOW TO PLAY                     │                    PLAY AGAIN
              │                         └──────────────────────────┘
             QUIT                                                  │
                                                          MAIN MENU ▼
                                                            MainMenu
```

### 5.1 Main Menu Scene (entry point — required)
- **Title:** "KEY PONG" — styled using keyboard **key sprites** where possible (each letter rendered on a key cap).
- **Buttons** styled as big keyboard keys:
  - `START GAME` → loads Gameplay scene
  - `HOW TO PLAY` → shows a controls/rules panel (overlay or sub-panel)
  - `QUIT` → quits application (disabled/hidden on WebGL, where Quit is a no-op)
- Clean, centered, uncluttered layout over the provided background.

### 5.2 Gameplay Scene
- Center court with top/bottom walls (bounce) and left/right scoring zones.
- Left paddle = **player**. Right paddle = **AI**.
- Live score at the top using **number-key sprites**.
- Ball morphs key sprite on every paddle/wall hit.

### 5.3 Game Over / Victory (overlay panel inside Gameplay scene)
- Displays **"YOU WIN"** or **"YOU LOSE"** (key-styled where possible) + final score.
- Buttons (key-styled): `PLAY AGAIN` (reload Gameplay) and `MAIN MENU` (load Main Menu).

---

## 6. Gameplay Flow (a single match)

1. Match starts; both scores at **0**. Ball spawns at center with a **random key sprite**.
2. Short countdown/launch delay (e.g. **1.0s**), then ball launches toward a random side at a random shallow angle.
3. Player and AI rally. Each paddle/wall contact: bounce + **key sprite swap** + **speed increase**.
4. Ball exits left edge → **AI scores +1**. Ball exits right edge → **Player scores +1**.
5. After any point: brief pause (**0.5s**), ball resets to center with a **new random key**, then relaunches.
6. First side to reach **11** ends the match → Game Over / Victory screen.

---

## 7. Controls

| Action            | Keyboard (primary)     | Keyboard (alt) |
|-------------------|------------------------|----------------|
| Paddle Up         | `W`                    | `↑` Up Arrow   |
| Paddle Down       | `S`                    | `↓` Down Arrow |
| Confirm / Select  | `Enter` / Mouse Click  | —              |
| Pause *(stretch)* | `Esc`                  | —              |

- All input is routed through the **New Input System** via an Input Actions asset.
- Menus are navigable by **mouse click** (primary) with keyboard navigation as a stretch goal.

---

## 8. Win / Lose Conditions

- **Win:** Player (left paddle) reaches **11 points** first.
- **Lose:** AI (right paddle) reaches **11 points** first.
- No draws possible (first-to-11). No timer.

---

## 9. Rules & Physics Tuning (initial values — tunable)

> These are starting numbers in Unity world units. We will fine-tune during playtesting. All exposed as serialized fields so no recompile is needed to adjust.

| Parameter                  | Initial Value      | Notes |
|----------------------------|--------------------|-------|
| Court playable height      | ~10 units          | Fits 1080p design; walls at top/bottom |
| Paddle move speed (player) | **8 units/sec**    | Snappy but controllable |
| AI paddle max speed        | **6.5 units/sec**  | Slightly slower than player for fairness |
| AI vertical dead zone      | **0.3 units**      | Prevents jitter when aligned with ball |
| Ball launch speed          | **6 units/sec**    | Initial serve |
| Ball speed increase per hit| **+0.25 units/sec**| Applied on paddle hit |
| Ball max speed             | **14 units/sec**   | Hard cap |
| Launch delay (serve)       | **1.0 sec**        | After reset before ball moves |
| Post-point pause           | **0.5 sec**        | Before reset |
| Win score                  | **11**             | Match point |
| Paddle bounce angle range  | ±**45°**           | Based on where ball hits the paddle |

**Bounce behavior:** Walls reflect the ball's vertical velocity. Paddles reflect horizontal velocity and add vertical influence based on **hit offset** (top of paddle = upward angle, bottom = downward) for skill-based aiming — classic Pong feel.

---

## 10. Art & Asset Specification

### 10.1 Style
- **Pixel art**, dark retro-arcade palette, **neon/glow accents** on active keys.
- All gameplay sprites are sourced from the provided key sprite sheet.

### 10.2 Sprite Sheet — `Keyboard_UI.png` (verified)
- **File size:** **736 × 480 px**, PNG, 32-bit RGBA.
- **Transparency:** clean — background alpha = 0; keys alpha = 255.
- **Authoring note:** art was created at **368 × 240** (1×) and exported at **2×** (736 × 480). Treat the **2× pixels** as our working resolution.
- **Two complete key sets:**
  - **Dark set:** left half, x = **0–367**
  - **Tan/Rose set:** right half, x = **368–735**
- **Standard key cell:** **26 × 28 px**, with a **32 px horizontal pitch** (left edge of key N at `x = 36 + 32·N` for the dark number row).
- **Detected row bands (Y ranges):**

  | Group        | Contents                          | Y bands (top→bottom)              |
  |--------------|-----------------------------------|-----------------------------------|
  | Numbers/Ops  | `1234567890`, then operators      | 34–61, 66–93                      |
  | Letters      | QWERTYUIOP / ASDFGHJKL... / ZXC...| 130–157, 162–189, 194–221         |
  | Symbols      | `\ / { } [ ] ( ) < >` etc.        | 258–285, 290–317, 322–349         |
  | Wide keys    | ALT CTRL BACK TAB / SPACE SHIFT ENTER | 386–413, 418–445              |

- **Number keys for score:** dark top row, key "1" begins at **(x=36, y=34)**, each **26 × 28**, pitch **32** → keys 1,2,3,4,5,6,7,8,9,0.

### 10.3 Asset Roles in Game

| Game Element     | Source Sprite                                   |
|------------------|-------------------------------------------------|
| **Ball**         | Any single standard key (26×28); swaps to a different random key each hit |
| **Player paddle**| **SPACE** wide key, **rotated 90°** to stand tall (vertical paddle) |
| **AI paddle**    | Same SPACE key in the **tan/rose** variant (visually distinguishes AI from the dark player paddle) |
| **Score digits** | Number keys `1`–`0` from the sheet              |
| **Title/buttons**| Letter keys composed into words where feasible  |

### 10.4 Background — `Background.jpg` (provided)
- **File size:** **1168 × 784 px** (≈ 3:2), supplied by Benjamin.
- Dark field with scattered glowing keycaps. **We do not create or alter background art.**
- **Framing (LOCKED):** the game targets **16:9**; the 3:2 background is **scaled to fill** the frame (a small top/bottom crop is acceptable for this abstract glowing-key pattern).

---

## 11. Technical Specifications (Unity 6)

- **Engine:** Unity 6, 2D project template.
- **Rendering (LOCKED):** **Built-in 2D** render pipeline (simplest for Pong + WebGL-friendly). Pixel-perfect handling via a fixed **Pixels Per Unit = 28** (1 standard key row = 1 world unit), filter mode **Point**.
- **Reference resolution (UI):** **1920 × 1080**, Canvas Scaler = *Scale With Screen Size*, **Match = 0.5**.
- **Camera (LOCKED start):** Orthographic, **Size = 5** (10 world units of visible height) — court fits the 16:9 frame with margins for paddles. Tunable in playtesting.
- **Sprite import:** `Keyboard_UI.png` set to **Sprite (2D and UI)**, **Multiple** sprite mode, **Point (no filter)**, **Compression: None**, **Pixels Per Unit = 28**.
- **Input:** New Input System package; an **Input Actions** asset with a "Gameplay" map (Move) and "UI" map.
- **Architecture:**
  - `GameManager` (match state, scoring, win check) — scene-scoped singleton.
  - `Ball` (movement, bounce, speed cap) + `KeySpriteLibrary` (provides random/indexed key sprites).
  - `PlayerPaddle` (Input System driven) and `AIPaddle` (tracking logic) sharing a `PaddleBase`.
  - `ScoreDisplay` (renders digits from number-key sprites).
  - `SceneLoader` (centralized scene transitions).
  - Consider a **ScriptableObject** (`KeySpriteSet`) holding the sliced key sprites for clean, reusable access.
- **Scenes in Build Settings (LOCKED order):** `MainMenu` (index 0), `Gameplay` (index 1). The **Game Over / Victory screen is an overlay panel inside the Gameplay scene** (not a separate scene) for fewer scenes and instant restart.

---

## 12. Features List (Vertical Slice — Step 1 scope ONLY)

**In scope for the first playable slice:**
1. Unity 6 project + folder structure + Git/GitHub + .gitignore.
2. New Input System set up.
3. Sprite sheet sliced into a usable, swappable key sprite system.
4. Main Menu scene (entry point) with working START / HOW TO PLAY / QUIT.
5. Gameplay scene: court, walls, player paddle (input), AI paddle, ball.
6. Pong physics: bounce off walls/paddles, speed increase, serve/reset.
7. **Key-morph ball** (random key swap on every hit) — the unique feature.
8. Scoring + **number-key score display**, first-to-11.
9. Game Over / Victory with PLAY AGAIN + MAIN MENU.
10. WebGL build that runs the full loop.

**Explicitly OUT of scope for Step 1** (no polish until the slice is fully playable): sound, particles, screen shake, menu animations, options menu, difficulty settings, two-player mode.

---

## 13. Polish & Stretch Goals (only after the slice is playable)

- SFX: mechanical "click/clack" on each bounce; different click per key *(stretch)*.
- Neon glow / trail on the ball; brief flash on the morph.
- Subtle screen shake or paddle squash on hits.
- Menu button hover/press animations (keycap "press down" effect).
- Pause menu (`Esc`).
- Difficulty levels (AI speed) and adjustable win score.
- Windows standalone build alongside WebGL.
- Best-score / quick stats.

---

## 14. Definition of Done (aligns with 20 Games Challenge)

- [ ] Two paddles; player-controlled left, AI-controlled right.
- [ ] Ball bounces off paddles and walls with classic feel.
- [ ] Ball speeds up over the course of a rally.
- [ ] Score tracked and displayed; first to 11 wins.
- [ ] Ball resets to center after each point.
- [ ] Main Menu → Gameplay → Win/Lose → Restart all functional.
- [ ] Unique feature working: ball swaps to a new random key sprite each hit.
- [ ] Public WebGL build on itch.io (Windows build = stretch).

---

## 15. Finalized Decisions (LOCKED — May 24, 2026)

| # | Decision | Locked Choice |
|---|----------|---------------|
| 1 | Render pipeline | **Built-in 2D** (simplest for Pong; WebGL-friendly) |
| 2 | Background framing | **Scale-to-fill** the 16:9 frame (minor top/bottom crop) |
| 3 | Game Over presentation | **Overlay panel** inside the Gameplay scene |
| 4 | AI paddle color | **Tan/Rose** SPACE key (player paddle = dark) |
| 5 | Title styling (Step 1) | **TextMeshPro** title "KEY PONG"; key-cap letters deferred to polish |
| 6 | Pixels Per Unit | **PPU = 28** (1 standard key row = 1 world unit); camera ortho Size = 5 |

*All physics/tuning values in §9 remain serialized and tunable during playtesting.*

---

*End of Game Design Document v1.0 — FINAL. Cleared to begin Milestone 0: Project Foundation.*
