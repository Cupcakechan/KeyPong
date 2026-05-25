# KEY PONG — Game Design Document

**Project:** Game 1 of the 20 Games Challenge
**Engine:** Unity 6 (exclusively)
**Language:** C#
**Input:** New Input System (Unity 6 best practice)
**Document Version:** 1.2 — Polish plan locked (May 25, 2026)
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

### 8.1 Classic Mode
- **Win:** Player (left paddle) reaches **11 points** first.
- **Lose:** AI (right paddle) reaches **11 points** first.
- No draws possible (first-to-11). No timer.

### 8.2 Time Attack Mode (added in Polish — implemented)
- **Goal:** score as many **player points** as possible within a **5:00 countdown**. The metric is *your points scored* (AI points are shown for feedback but don't end anything).
- **The AI plays defense.** When the AI scores, the ball simply re-serves — no penalty beyond the time lost, so missing costs you scoring opportunities.
- **No win/lose.** The mode ends only when the timer hits **0:00**.
- **Timer:** top-center `5:00 → 0:00`, cyan `#34E5DC`, **pulses red `#EA3838` in the final 10 seconds**. Freezes while paused.
- **High score:** persisted via **`PlayerPrefs`** (`KeyPong_TimeAttack_Best`); a new record shows **NEW BEST!** (otherwise **TIME'S UP**) and plays the Achievement stinger only on a new best.
- **Results reuse the existing Game Over panel** (mode-aware text + `Score X    Best Y`). The best score is shown on the Main Menu (`TIME ATTACK BEST: N`).
- **Mode selection:** carried from menu to Gameplay via a static `GameSession.Mode`; **Play Again** restarts the same mode.

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

### 10.2 Sprite Sheet — `Keyboard_UI.png` (verified — WORKING RESOLUTION 1×)
- **In-project file size:** **368 × 240 px**, PNG (the 1× authoring resolution; confirmed in Unity's Inspector). *All slicing values below are in this 1× space.*
- **Transparency:** clean — background alpha = 0; keys alpha = 255.
- **Two complete key sets:**
  - **Dark set:** left half, x = **0–183**
  - **Tan/Rose set:** right half, x = **184–367**
- **Standard key cell:** **13 × 14 px**, with a **16 px horizontal pitch** (left edge of key N at `x = 18 + 16·N` for the dark number row).
- **Wide keys (ALT/CTRL/BACK/TAB/SPACE/SHIFT/ENTER):** **45 × 14 px** each (SPACE, SHIFT, ENTER share this width).
- **Detected row bands (Y ranges, top-origin, 1×):**

  | Group        | Contents                          | Y bands (top→bottom)              |
  |--------------|-----------------------------------|-----------------------------------|
  | Numbers/Ops  | `1234567890`, then operators      | 17–30, 33–46                      |
  | Letters      | QWERTYUIOP / ASDFGHJKL... / ZXC...| 65–78, 81–94, 97–110              |
  | Symbols      | `\ / { } [ ] ( ) < >` etc.        | 129–142, 145–158, 161–174         |
  | Wide keys    | ALT CTRL BACK TAB / SPACE SHIFT ENTER | 193–206, 209–222              |

- **Number keys for score:** dark top row, key "1" begins at top-origin **(x=18, y=17)**, each **13 × 14**, pitch **16** → keys 1,2,3,4,5,6,7,8,9,0. *(Unity's Sprite Editor uses a bottom-left origin, so we convert during slicing.)*

### 10.3 Asset Roles in Game

| Game Element     | Source Sprite                                   |
|------------------|-------------------------------------------------|
| **Ball**         | Any single standard key (26×28); swaps to a different random key each hit |
| **Player paddle**| **SPACE** key (45×14 px), **rotated 90°** to stand tall (≈3.2 units at native PPU; transform-scaled to taste in court setup) |
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
- **Rendering (LOCKED):** **Built-in 2D** render pipeline (simplest for Pong + WebGL-friendly). Pixel-perfect handling via a fixed **Pixels Per Unit = 14** (1 standard key row = 1 world unit), filter mode **Point**.
- **Reference resolution (UI):** **1920 × 1080**, Canvas Scaler = *Scale With Screen Size*, **Match = 0.5**.
- **Camera (LOCKED):** Orthographic, **Size = 5** (10 world units of visible height, ~17.8 wide at 16:9). At PPU 14 the 358×240 background = 25.6×17.1 units, so it fully covers the view at native scale (no fractional scaling). Court height ~10 units; paddle/ball world sizes tuned in playtest.
- **Sprite import (both textures):** **Sprite (2D and UI)**, **Point (no filter)**, **Compression: None**, **Pixels Per Unit = 14**. `Keyboard_UI.png` → **Multiple** sprite mode (sliced); `Background.png` → **Single** sprite mode.
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

## 13. Polish Phase — MUST be completed BEFORE the final WebGL build

The vertical slice is complete and playable. These polish items are implemented **in this exact order, ONE at a time, fully tested in the Editor after each** before moving to the next. All new UI uses the existing keyboard-key style and exact prior values (e.g., TMP body font size 44–50, key-styled buttons 460×110). Keep everything **WebGL-performant** (object pooling for trails/particles, no heavy per-frame allocations).

1. **Pause Menu** ✅ **DONE** — toggle via **Spacebar / Esc**; **no on-screen button** (avoids mid-rally misclicks; Spacebar is WebGL-reliable). Options: **Resume**, **Restart**, **Main Menu**. Overlay in keycap style; `Time.timeScale = 0`.
2. **Sound Effects** ✅ **DONE** — `AudioManager` (DontDestroyOnLoad singleton prefab): **6 random typing clacks** on every bounce (pitch-varied; the clack *is* the morph sound), **random button clicks** via a reusable `UIClickSound` component, **looping streamed music** (`Lofi_Loops_-_Retro_Rewind`, vol 0.4), a **score chime**, and **win/lose stingers** (`Achievement` / `Error`). Music **pauses on match end** so the stinger is audible and **resumes on scene transition**.
3. **Ball Trail** ✅ **DONE** — pooled (20) fading **ghost key fragments** that shrink + fade; emitted only while moving; built once at Start (no runtime instantiation) for WebGL.
4. **Light Screen Shake** ✅ **DONE** — `CameraShake` on Main Camera; subtle decaying offset on **paddle hits** (mag ~0.06) and **scores** (mag ~0.15); unscaled-time so it's smooth; snaps back to rest exactly.
5. **Time Attack Mode** ✅ **DONE** — separate **"TIME ATTACK"** menu button; **5:00** countdown, no win/lose, score = your points; **PlayerPrefs** high score on menu + results; timer flashes red in the final 10s. (See §8.2.)
6. **Win/Lose Celebration VFX** ✅ **DONE** — `EndGameVFX` (world-space, pooled, built once): **win** = fountain burst of ~24 key sprites from center; **lose** = a dark dim fades in over the court + a few keys drift down. Reused for Time Attack **NEW BEST!**. Renders below the Overlay UI so result text stays crisp.
7. **General VFX / Feedback** ✅ **DONE** — reusable `JuicePop` component: **ball** pops uniformly (~1.25×) on every bounce (emphasizing the morph); **paddles** squash-stretch (length ×1.15, thickness ×0.8) on impact, springing back. Unscaled-time so they always settle.

**Branding (added):** Cocolito Collective logo (`cocolito-dark.png`, transparent, 877×877) placed **bottom-left of the Main Menu** (square, Preserve Aspect); used as the **Unity Splash Screen** logo (3s, background `#14141E`) and the **Default app icon / WebGL favicon**.

> Only **after all seven items are complete and tested** do we proceed to the WebGL build (and a Windows build if feasible).

### Deferred / true stretch (post-build, optional)
- Difficulty levels (AI speed) and adjustable win score.
- Menu button keycap "press-down" animation.
- Custom pixel/monospace font; key-cap-letter title for "KEY PONG".

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
| 6 | Pixels Per Unit | **PPU = 14** (assets are 1× / 368×240; 1 standard key row ≈ 1 world unit); camera ortho Size = 5 |
| 7 | Background format | **PNG, 358×240, 1×** (converted from JPG; lossless, matches key pixel density) |
| 8 | Time Attack metric | **Player points scored in 5:00** (not differential); AI scoring just re-serves |
| 9 | Time Attack duration | **300 s** (serialized; tunable) |
| 10 | High score storage | **PlayerPrefs** key `KeyPong_TimeAttack_Best` (persists across WebGL reloads in-browser) |
| 11 | Mode plumbing | static **`GameSession.Mode`** set on menu, read by `GameManager` (single Gameplay scene for both modes) |

*All physics/tuning values in §9 remain serialized and tunable during playtesting.*

---

## 16. Development Roadmap

**Phase A — Vertical Slice (COMPLETE ✅)**
- M0 Project Foundation · M1 Asset Pipeline · M2 Main Menu · M3 Gameplay (court, paddles, player input, ball + key-morph, AI, scoring, number-key display, win + Game Over).

**Phase B — Polish (COMPLETE ✅ — all 7 items done + branding, each tested in-Editor):**
1. Pause Menu ✅ → 2. Sound Effects ✅ → 3. Ball Trail ✅ → 4. Light Screen Shake ✅ → 5. Time Attack Mode (+ PlayerPrefs high score) ✅ → 6. Win/Lose Celebration VFX ✅ → 7. General VFX / feedback ✅. Plus Cocolito branding (menu logo, splash screen, app icon).

**Phase C — Ship (NEXT)**
- WebGL build → test → publish to itch.io. Windows standalone build if feasible. Portfolio writeup.

---

*End of Game Design Document v1.4 — Phases A & B COMPLETE. Cleared for Phase C: WebGL build.*
