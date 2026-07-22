# Desktop OS Simulation Game 🖥️⚡

A custom desktop environment simulation and idle game built in Unity, featuring an in-game terminal, web browser, crypto mining system, and modular window management.

---

## 🛠️ Core Systems Architecture

*   **Window Management (`WindowManager` & `WindowController`):** Handles spawning, dragging (`WindowDragHandler`), focusing (`WindowFocusHandler`), and taskbar tracking for desktop applications and popups.
*   **App Database (`IconManager` & `AppData`):** Manages installed applications, pricing, and dynamic taskbar widget mapping.
*   **Terminal System (`TerminalController`):** Uses reflection (`Assembly`) to automatically register classes implementing `ITerminalCommand`, supporting command-line execution and dynamic property overrides (like `IsHidden`).
*   **Economy & Mining (`ResourceManager` & `MiningManager`):** Tracks USD and Bitcoin balances, handles background crypto generation, and powers the crypto exchange slider.
*   **Upgrade System (`UpgradeManager`):** Manages scaling costs, levels, and effects for stats like internet speed, mining efficiency, and UI unlocks.

---

## 💻 Terminal Commands & Cheats

To test features rapidly, the terminal includes integrated developer commands:

*   **`help`** - Displays all available (unhidden) commands, sorted alphabetically.
*   **`clear`** - Clears the terminal output screen.
*   **`echo <text>`** - Prints text back to the terminal.
*   **`3 hidden cheat commands`**

## In Prototype Phase (Unfinished)
*   Gameplay is still lacking or absent.
