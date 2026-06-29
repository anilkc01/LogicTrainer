# LogicTrainer 🥊🧮

A fast-paced math-reflex boxing game for **Meta Quest 2**. Numbers fly at you as equations — punch the ones that match the target, dodge the ones that don't.

## Concept

You're standing in front of 5–6 rings hovering at a fixed distance in front of you. A target number **X** is shown above the rings. Each ring randomly fires a ball labeled with an equation (e.g. `4+1`, `7-2`, `10/2`).

- If the equation **equals X** → **punch it** to score.
- If the equation **doesn't equal X** → **dodge it**, don't punch.

Difficulty ramps up over time: ball speed increases, equations get harder (subtraction, multiplication, division), and spawn rate tightens. Build streaks for score multipliers — hit an 8x streak with only 1 life left and you earn a bonus life.

## Requirements

| Requirement | Version |
|---|---|
| Unity | **6000.4.3f1** (Unity 6 LTS) |
| Meta XR All-in-One SDK | Latest compatible with above |
| Headset | Meta Quest 2 (or Quest 2/3 family) |
| Build target | Android |

### Why not the latest Unity version?
Newer Unity versions (post `6000.4.3f1` at time of writing) have known compatibility issues with the **Meta XR All-in-One SDK** — build errors and broken project settings. `6000.4.3f1` is the latest version confirmed to work cleanly with the SDK. Stick to this version (or close to it) unless you've verified a newer combo yourself.

## Getting Started

### Clone

```bash
git clone https://github.com/<your-username>/LogicTrainer.git
cd LogicTrainer
```

### Open in Unity
1. Open the project via **Unity Hub** using Unity `6000.4.3f1`.
2. Let Unity import all packages (first import can take a while).
3. Make sure **Meta XR All-in-One SDK** is installed (via Package Manager / Asset Store) — it's not committed to the repo (see `.gitignore` below).
4. In **Build Profiles**, switch the platform to **Android / Meta**.

### Build & Run
1. Connect your Quest 2 via USB (enable Developer Mode + USB debugging).
2. `File > Build Settings > Build And Run`, or sideload the APK with **SideQuest** / `adb install`.

## How to Play

1. Put on the headset, grab both controllers (rendered as boxing gloves in-game).
2. Watch the target number **X** displayed above the rings.
3. Balls launch from the rings toward you, each showing an equation.
4. **Punch** balls whose equation equals X with a real swing — punch velocity is tracked via controller motion.
5. **Dodge** (don't punch) balls that don't equal X. Letting a *correct* ball pass costs you a hit too — so don't just dodge everything!
6. You have 3 lives (hearts). Wrong punches or correct balls reaching your head cost a life.
7. Chain correct punches for a streak multiplier. Hit a streak of 8 while on your last life to earn a bonus heart back.
8. Survive as long as possible and rack up score as difficulty ramps up.

## Pros

- 🧠 Genuinely trains mental math under physical/time pressure — not just a reflex game.
- 🥊 Real, satisfying physical feedback — controller-velocity-based punch detection feels tactile.
- 📈 Smooth difficulty curve via tiered ball speed/spawn rate scaling.
- 🎯 Simple, readable core loop — easy to pick up, hard to master.
- 🔁 Streak + bonus-life system adds a "just one more round" hook without being unfair.
- 🛠️ Lightweight — built with placeholder-friendly assets, runs comfortably on standalone Quest 2 hardware.

## Tech Stack

- Unity 6000.4.3f1
- Meta XR All-in-One SDK (Building Blocks: Camera Rig, Passthrough, Controller Tracking)
- C# gameplay scripts: `GameManager`, `LivesManager`, `TierManager`, `BallSpawner`, `MathBall`, `RingMover`

---


