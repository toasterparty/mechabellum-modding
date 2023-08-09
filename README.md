# Mechabellum Modding

An educational expedition to learn more about the game with no intention of providing unfair advantages. Should you feel something in this repository gives an unfair advantage, please reach out and I will take it down immediately.

# How to Install

## Requirements

- Windows 10 or above

## Instructions

1. Download and extract the [Latest Release](https://github.com/toasterparty/mechabellum-modding/releases)

2. Double click `mechabellum-modding-install.bat` and use the file picker window to select your game's .exe file

3. Run the game once, wait until you reach the main menu, and then close it

4. Open `<path-to-game>\Mechabellum\BepInEx\config\mechabellum-modding.cfg` with your favorite text editor to configure mods

# How to Build

1. Install the latest [.NET sdk (6.0)](https://dotnet.microsoft.com/en-us/)

1. Open `/dist/mechabellum-modding-install.bat` with a text editor and note the value of `BEPINEX_VER=`

1. Download that exact version of the BepInEx and extract to `/dist/`

1. Also extract the contents next to game's exe file. Run the game once to generate dlls which can be referenced during dotnet compilation

1. Copy the following from `<patch-to-game>/Mechabellum/BepInEx/interop/` to `/referencedlls`:

- Assembly-CSharp.dll

- Assembly-CSharp-firstpass.dll

1. Run `/tools/build.bat` to build

If that succeeded, you can now zip and distribute the `dist` folder as a standalone release
