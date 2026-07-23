## A simulator with no clear gameplay goals yet.

## Build 4 (B4) Release Notes

### 🕒 Time & Simulation
* **Custom In-Game Clock (`TimeManager`)**: Decoupled the game loop from real time. Runs on its own timescale now (1 real second = 1 game minute by default).
* **Global Pause**: Added proper pause/resume controls so time actually freezes during menus—no more racking up power bills while reading or setting things up.

### 💸 Bills & Grace Periods
* **Automated Utility Cycles (`BillingManager`)**: Power and ISP bills now trigger based on in-game hours rather than real-time tracking.
* **New Player Buffer**: Gave starting players a grace period to figure out the UI and get a basic mining setup running before the first utility bills hit. (Currently disabled)
* **"No Bills" Banner**: Added a dynamic status banner to the Bills page so you instantly know when everything is paid off.

### 🔔 Notifications
* **Modular Pop-Ups (`NotificationManager`)**: Built a reusable stacking alert system for bills, errors, and background events.
* **Welcome Ping**: Added a startup notification reminding players to check out the new Help app right when they boot up.

### 📦 Core Architecture & Refactoring
* **Centralized Economy**: Moved all the core data—mining rates, totals mined, and exchange buy/sell rates—right into the persistent `ResourceManager`.
* **ScriptableObject Upgrades (`UpgradeData`)**: Converted static upgrade stats into proper project assets, keeping identity, math, and visuals cleanly separated.
* **Persistent Player State (`PlayerUpgradeManager`)**: Centralized progression and cost scaling into a background manager, future-proofing things for save/load implementation.
* **Type-Safe App Unlocks**: Swapped out messy hardcoded strings for clean enums (`UnlockableApp`) managed centrally via `UnlockManager`.
* **OS Cleanup**: Stripped the redundant `WindowFocusHandler.cs` right out of the WindowManager.
* **Decoupled UI**: Pulled all the game logic out of webpage scripts, turning things like `UpgradePage` into lightweight, event-driven display panels.

### 🎨 UI, Formatting & Polish
* **Dynamic Hashrate Formatting**: Put together a global helper to turn unreadable decimals into actual satisfying units—introduced the micro-realm (`uH/s` and `mH/s`).
* **Visual Polish**: 
  * Fixed USD balances showing way too many trailing zeros.
  * Stripped unnecessary decimals out of the Network Speed UI and formatted it cleanly in `MB/s`.
  * Removed that redundant "BTC/s" text sitting next to the new Hashrate displays on the CryptoMiner window.
  * Compressed the Close/RedX window sprite to optimize it.
* **OS Apps**: Renamed the "Notepad" app to "Help" and updated the notes inside.
* **WIP Placeholders**: Threw placeholder graphics over unfinished windows so it's obvious what's still a work in progress.

### ⚖️ Balance & Economy
* **Network Speed Caps**: Capped internet upgrades at level 5 (0.5s lowest load time). Tweaked prices and multipliers so it doesn't instantly break the early game.
* **Mining Speed Buff**: Bumped up the max level for mining speed so players can scale their rigs up to a solid 9.0 mH/s cap.

### 🛠️ Dev & Debug Tools
* **Dynamic Debug Panel (`DebugPanelController`)**: Built an inspectable, draggable debug dashboard driven by an enum list.
* **Live Metrics**: Real-time tracking for USD, BTC, unbilled power draw, mining rates, net speed, and clock status.
* **Market Tracking**: Added tracking for the total USD value of held BTC and current exchange rates to prep for dynamic markets later.
* **Debug Window Fixes**: Gave the debug window proper close/minimize buttons and added an invisible raycast panel so it actually focuses when you click it.

### 🐛 Bug Fixes
* **Cross-Wired Upgrades**: Fixed a bug where buying a Network Speed upgrade was accidentally applying stats to Mining Speed, and vice versa.
* **Net Speed Cap Bug**: Fixed an issue where buying past level 5 on internet upgrades broke functionality.
