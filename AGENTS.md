# PROJECT CONTEXT & STRICT DEVELOPMENT RULES

## PROJECT OVERVIEW
- **Type:** 2D Kingdom Simulator.
- **Platform:** Mobile (Portrait orientation).
- **Art Style:** Pixel-art, cozy fantasy medieval.
- **AI Agent Goal:** Maintain absolute architectural purity, readability, and modularity. Never compromise clean architecture for quick workarounds.

---

## 1. ARCHITECTURE: MVP (Model-View-Presenter)
The project STRICTLY follows the MVP pattern. No exceptions.
- **Model:** Contains ONLY data and business logic. Ignorant of Unity components (No `MonoBehaviour`). Does not know about Presenters or Views. Uses reactive properties or standard C# events for state changes.
- **View:** A `MonoBehaviour` attached to GameObjects. Handles ONLY UI bindings, user input capturing, and visual updates (animations). MUST NOT contain business logic. Exposes clean methods (e.g., `ShowPanel()`, `UpdateResourceText()`) and events/Action delegates for user interactions.
- **Presenter:** A pure C# class (or sometimes `IInitializable` via VContainer) that links Model and View. Listens to View inputs, calls Model methods, listens to Model state changes, and updates the View. 

## 2. DEPENDENCY INJECTION: VContainer
- **Strict Rule:** NEVER use Singletons (`Instance`), `FindObjectOfType`, or `GetComponent` to find external dependencies.
- All dependencies MUST be injected via **Constructor Injection** in standard classes, or via `[Inject]` attribute ONLY in `MonoBehaviour` Views where constructor injection is impossible.
- Every distinct game module must have its own `LifetimeScope` (if it's a separate scene/context) or register its dependencies cleanly in the parent `RootLifetimeScope` / `BootstrapScope`.
- Bind interfaces to implementations (e.g., `builder.Register<ResourceModel>().As<IResourceModel>().AsImplementedInterfaces();`).

## 3. UI, STYLING & ANIMATION
- **Framework:** STRICTLY **UGUI** (Unity UI) and **TextMeshPro** (TMP). 
- **FORBIDDEN:** Do NOT use Unity UI Toolkit (UIElements). It is completely banned for this runtime project.
- **Canvas Optimization:** Every distinct, large UI screen or module MUST be on its own `Canvas` to prevent full-screen batching rebuilds when a single element changes. Set Canvas settings optimized for mobile (e.g., explicit sorting orders, avoiding raycast targets on non-interactive graphic elements).
- **Animations:** STRICTLY use **DOTween**.
    - Never use `Update()`, `LateUpdate()`, or `FixedUpdate()` for UI animations.
    - Cache Tweens if they are played repeatedly to avoid memory allocation.
    - Always `Kill()` Tweens `OnDestroy()` in the View to prevent memory leaks and null reference exceptions.

## 4. DIRECTORY STRUCTURE
All game-specific files MUST reside in `Assets/_Game/`. 
Do not pollute the root `Assets/` folder.
- `Assets/_Game/Shared/`: Common utilities, shared UI components, fonts, global configuration `ScriptableObjects`, generic scripts.
- `Assets/_Game/Scenes/`: Bootstrap scene, Main Game scene, etc.
- `Assets/_Game/Modules/`: The core isolated mechanics.
    - Subfolders MUST include: `Laws`, `Trade`, `Economy`, `Expansion`, `Events`, `Shop`.
    - Inside each Module folder, enforce strict separation: `/Scripts` (further divided into Models, Views, Presenters if needed), `/Prefabs`, `/Images`, `/Animations`.

## 5. CODING STANDARDS & C# CONVENTIONS
- **Modularity:** A change in the `Trade` module must NEVER break the `Laws` module. They must communicate through unified global services (e.g., `IPlayerInventoryService`) injected via VContainer, never by direct cross-module referencing.
- **Encapsulation:** Use `private` fields with `[SerializeField]` for Unity inspector assignment. Do not make fields `public` just for inspector access.
- **Naming Conventions:**
  - Interfaces: `I` prefix (e.g., `ILawsModel`).
  - Private fields: `_camelCase` or `camelCase` (be consistent).
  - Properties & Methods: `PascalCase`.
- **Localization:** All user-facing text MUST use the **Unity Localization** package. Hardcoding strings for UI elements is forbidden. The View should handle localization keys and request localized strings.

## 6. SPECIFIC AGENT WORKFLOW INSTRUCTIONS
- When asked to generate a module, ALWAYS generate the complete MVP triad (`Model`, `View`, `Presenter`) and the VContainer registration code.
- If a user prompt contradicts these rules, **refuse the contradiction** and cite this `AGENTS.md` file, explaining why the requested change violates the established architecture.
- Keep C# files small and focused (Single Responsibility Principle). 
- Provide clean, well-commented code. Explain the "Why" in comments, not the "What".