# Clear Outside Furniture Mod for PlateUp!

This mod allows you to quickly delete furnitures placed in your trash area outside your restaurant during the planning phase.

## Features
- Press **DELETE key** during planning phase to **instantly remove** all outside appliances and decorations
- **Removes user-placed decorations** (rugs, plants, ornaments, etc.)
- **Does NOT remove walls or map terrain** (pebbles/ground that came with the map)
- **Fixes camera shifting issue** - items are destroyed immediately, not moved far away
- **Note:** Direct deletion means TrashToCash won't give you money back (trade-off for fixing camera issue)
- Automatically detects movable furniture outside the restaurant boundaries
- Safe to use - only works during planning phase, ignores permanent map structures

## Installation

### Prerequisites
1. Visual Studio 2022 (with C# workload)
2. PlateUp! installed via Steam
3. **KitchenLib** mod installed (subscribe on Steam Workshop)
4. **TrashToCash** mod installed (you already have this)

### Build Instructions

1. **Update the project file**:
   - Open `ClearOutsideFurniture.csproj`
   - Change the `<PlateUpDir>` path to match YOUR PlateUp installation
   - Default: `C:\Program Files (x86)\Steam\steamapps\common\PlateUp\PlateUp`

2. **Build the mod**:
   - Open the project in Visual Studio 2022
   - Build in **Release** mode
   - The DLL will automatically copy to `PlateUp/Mods/ClearOutsideFurniture/`

3. **Manual installation** (if auto-copy doesn't work):
   - Navigate to your PlateUp folder
   - Create folder: `PlateUp/Mods/ClearOutsideFurniture/`
   - Copy `ClearOutsideFurniture.dll` into that folder

## Usage

1. Launch PlateUp!
2. During **Planning Phase**, press the **DELETE** key
3. All **user-placed appliances and decorations** outside are **instantly destroyed**
4. **Decoration markers and visual effects are automatically cleaned up**
5. **Map walls and terrain stay** - only items YOU placed are removed
6. **Camera won't shift** - items are deleted immediately, not moved away

## Troubleshooting

### "Not all furniture is being deleted"
- The mod uses coordinate bounds to detect "outside" furniture
- You may need to adjust the bounds in `IsOutsideRestaurant()` method
- Default bounds: furniture beyond X/Z coordinates of ±15

### How to adjust the detection area:
Open `Main.cs` and find this function:
```csharp
private bool IsOutsideRestaurant(Unity.Mathematics.float3 position)
{
    float x = position.x;
    float z = position.z;
    
    // CHANGE THESE VALUES to match your restaurant size
    bool isOutside = x < -15f || x > 15f || z < -15f || z > 15f;
    
    return isOutside;
}
```

### Change the keybind:
In `OnUpdate()`, change `KeyCode.Delete` to any other key:
- `KeyCode.F9` - F9 key
- `KeyCode.K` - K key  
- `KeyCode.Backspace` - Backspace key

## Compatibility
- Works with TrashToCash mod (you get money back)
- Works with NoClip mods
- Should work with most other mods

## Notes
- **Only works during Planning Phase** - prevents accidental deletions
- **Only removes items with CAppliance component** - this is furniture/equipment you place
- **Direct deletion** - items are destroyed immediately to prevent camera shifting
- **TrashToCash incompatibility:** You won't get money back because items are deleted instantly instead of being moved far away. This is the trade-off for fixing the camera issue.
- **About decoration markers:** If you still see leftover markers after deletion, you may need to inspect Kitchen.dll to find the correct component names for linked views/child entities

**To get money back (but camera will shift):**
If you prefer getting money back over fixing the camera, change line in `DeleteOutsideFurniture()`:
```csharp
// Replace this line:
EntityManager.DestroyEntity(entity);

// With these lines:
position.Position = new Unity.Mathematics.float3(100f, 0f, 100f);
Set(entity, position);
```

**To get money back (but camera will shift):**
If you prefer getting money back over fixing the camera, change line in `DeleteOutsideFurniture()`:
```csharp
// Replace this line:
EntityManager.DestroyEntity(entity);

// With these lines:
position.Position = new Unity.Mathematics.float3(100f, 0f, 100f);
Set(entity, position);
```

**To verify component names:**
- Use dnSpy or ILSpy to inspect `Kitchen.dll` in `PlateUp_Data\Managed\`
- Look for components starting with `C` (like CWall, CTile, etc.)
- You can add more component exclusions if needed

**If decoration markers are still left behind after deletion:**

1. **Download and install dnSpy**: https://github.com/dnSpy/dnSpy/releases
2. **Open Kitchen.dll**: `Steam\steamapps\common\PlateUp\PlateUp\PlateUp_Data\Managed\Kitchen.dll`
3. **Search for decoration-related components**:
   - Press Ctrl+Shift+K to open search
   - Search for: "decoration", "marker", "linked", "view", "child"
   - Look for components (structs) starting with `C`
4. **Common patterns to look for**:
   - `CLinkedView` or `CViewData` - visual markers
   - `CDecorationBonus` or `CDecoration` - decoration-specific data
   - `CLinkedEntity` or `CChildEntity` - child/linked entities
   - Look for any `IBufferElementData` that might store linked entities

5. **Once you find the correct component names**, add them back to the `CleanupLinkedEntities()` function in the code

Example of what to add if you find the real component names:
```csharp
private void CleanupLinkedEntities(Entity entity)
{
    // Replace these with REAL component names from Kitchen.dll
    if (Has<CActualComponentName>(entity))
    {
        EntityManager.RemoveComponent<CActualComponentName>(entity);
    }
}
```

## Support
If you have issues:
1. Check the PlateUp logs in `PlateUp_Data/`
2. Verify KitchenLib is installed
3. Make sure you're pressing DELETE during planning phase (not during the day)

## License
Free to use and modify
