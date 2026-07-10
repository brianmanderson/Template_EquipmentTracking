# Template_EquipmentTracking (SterillizationTracking)

Windows desktop app for tracking sterilization use counts of reusable brachytherapy
applicator kits. Each kit part has a total-use limit and a warning threshold; the app
counts uses per kit, color-codes status, and flags kits that need to be reordered, so
the brachy team knows when equipment is approaching its sterilization/use limit.

## How it works

- On first launch you pick a shared folder for equipment data (path remembered in
  `Applicator_Path.txt`). All state lives as plain text/JSON files in that folder --
  no database, so a shared network drive works for the whole team.
- Kit *templates* (name plus parts, each with total uses, warning uses, and a
  description) are defined in a template editor window and stored in `Templates.json`.
- Individual kits are instantiated from a template with a kit number. Each kit gets a
  `<KitName>/Kit <n>/Uses.txt` file recording per-part use counts and usage dates.
- Buttons on each kit row add/remove a use, reset counts, or record a reorder
  (reorders are logged with timestamps under a `Reorders/` subfolder).
- Kits turn a warning/alert color as they approach or hit their use limits; the main
  window can filter the kit list by applicator name.

## Components

- `SterillizationTracking/` -- the WPF application (main window, template editor,
  kit classes with one- and two-part variants, kit-number service).
- `UseKit/` -- a minimal stub project, not functional on its own.

## Requirements

- Windows, .NET 7 (`net7.0-windows`), WPF + WinForms (folder picker).
- NuGet: `System.Text.Json`, `DocumentFormat.OpenXml`.

Build and run `SterillizationTracking.sln` in Visual Studio; the executable is the
entry point. This is a template/utility repo for a clinical team workflow, not a
maintained library (last updated 2024).
