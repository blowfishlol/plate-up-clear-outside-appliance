# Clear Outside Appliance Mod for PlateUp

This mod allows you to quickly delete furnitures placed in your trash area outside your restaurant during the planning phase.

## Installation

### Prerequisites
1. Visual Studio 2022 (with C# workload)
2. PlateUp! installed via Steam
3. **KitchenLib** mod installed (subscribe on Steam Workshop)

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

