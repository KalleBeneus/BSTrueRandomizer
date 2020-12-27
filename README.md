# BSTrueRandomizer
Tool for augmenting the Randomizer mode experience in "Bloodstained: Ritual of the night"

Generate mod files that randomizes item placements for chests, quest rewards and crafting. Run from the command line, use --help for available options.

Requires the following game files to be provided (--input option from the command line):
* PB_DT_CraftMaster.json
* PB_DT_CraftMaster.uasset
* PB_DT_DropRateMaster.json
* PB_DT_DropRateMaster.uasset
* PB_DT_QuestMaster.json
* PB_DT_QuestMaster.uasset
* PB_DT_ItemMaster.json
* PB_DT_ItemMaster.uasset

In order to package the resulting mod file, the Unreal Engine tool UnrealPak.exe must also be provided (--unrealpak-path option from the command line)
