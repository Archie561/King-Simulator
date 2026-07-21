# GAME DESIGN DOCUMENT (GDD): 2D Kingdom Simulator

## 1. CORE CONCEPT & VISION
- **Genre:** 2D Kingdom Management Simulator / Idle Strategy.
- **Platform:** Mobile (Android/iOS), strictly Portrait mode.
- **Visual Style:** Cozy fantasy medieval, high-quality pixel art.
- **Core Loop:** Pass Laws -> Generate Trade Resources -> Earn Gold via Economy -> Expand Kingdom by buying settlements on the Map -> Repeat.
- **Goal:** Conquer/purchase the entire map (estimated ~80 hours of gameplay).

## 2. TECHNICAL STACK
- **Engine:** Unity
- **UI:** UGUI + TextMeshPro (Strictly NO UI Toolkit). Separate Canvas for each major module.
- **Architecture:** MVP (Model-View-Presenter).
- **Dependency Injection:** VContainer.
- **Animations:** DOTween.
- **Localization:** Unity Localization Package.

---

## 3. GAME MECHANICS

### 3.1. Laws (Progression Module)
- **Stats:** 6 global Kingdom stats (Medicine, Education, Army, Science, Infrastructure, Welfare).
- **Leveling:** Reaching a new level requires points. Once a level is reached, it CANNOT be downgraded, even if negative points are applied. Players can also buy missing points using premium currency (Crystals).
- **Gameplay:** Player receives a deck of Law Cards (max 8). Swipe Right (Accept) / Swipe Left (Reject). Each action adds/subtracts points from specific stats.
- **Refill:** 1 card restores every 2 minutes. Player can pay 2 Crystals to instantly refill a card slot.

### 3.2. Trade (Resource Management Module)
- **Resources:** 6 types (Stone, Wood, Metal, Minerals, Leather, Clay). Value is equal 1:1 across all types.
- **Storage:** Each resource has a cap. Storage gradually fills passively over 24 hours (0% to 100%).
- **Cross-Upgrades:** Expanding a storage requires spending 80% of its capacity in *different* resources (e.g., expanding Wood requires Stone and Metal).
- **Trading Offers:** List of offers from other kingdoms, refreshing every 20 minutes (can be skipped via Crystals). When refreshed, 10 new offers appear.
- **Offer Algorithm:** 
  - 30% Profitable (Player receives more than gives).
  - 40-50% Neutral (1:1 ratio).
  - Rest Unprofitable (Player gives more than receives).
  - Each offer imports 2-3 types and exports 2-3 types (no duplicates in one offer).

### 3.3. Economy (Gold Generation Module)
- **Income:** Passive Gold generated per minute.
- **Businesses:** Player buys businesses (Quarry, Smithy, Tavern, etc.). Infinite amount can be purchased, but costs scale up significantly.
- **Collection:** Businesses have a 24-hour capacity limit. Player MUST manually tap/collect gold from each business type.

### 3.4. Expansion (Core Meta-Game)
- **Map:** Vertical scrollable map with distinct visual Regions (e.g., Forests, Mountains).
- **Settlements:** Map is populated with Villages and Cities.
- **Purchase Cost:** Requires a combination of specific Law Stat levels, Trade Resources, and Gold. Costs scale exponentially.
- **Rewards:** Buying a settlement increases Population (Kingdom Level) and unlocks territory.
- **Region Bonus:** Conquering 100% of the settlements in a specific Region grants a permanent global passive bonus (e.g., +5% stone generation, extra Law card slot).

### 3.5. Random Events (Dynamic Encounters)
- **System:** Occasional "Mail" pop-ups triggering random events.
- **Mechanic:** Player is presented with a narrative problem and a choice. (e.g., "Flood! Pay 5000 Gold to fix shores, or lose 30% of stored Wood?").
- **Dynamic Difficulty:** Event severity and frequency scale based on player progression speed (boosts if slow, hurdles if fast). Cannot affect Crystals.

### 3.6. Shop (Monetization)
- **Features:** Buy Crystals (IAP), buy missing resources, purchase boosters, watch Rewarded Ads for bonuses.

---

## 4. USER EXPERIENCE & JUICE
- **Haptics:** Satisfying vibration feedback on swipes, purchases, and gold collection.
- **Animations:** Smooth UI transitions using DOTween. Bouncy, tactile feedback for buttons.
- **Clarity:** Deeply intuitive UX. If a player lacks resources for an action, the UI must clearly highlight what is missing via dedicated confirmation panels.