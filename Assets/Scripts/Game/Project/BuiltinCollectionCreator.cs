using System.Collections.Generic;
using System.Linq;
using DLS.Description;

namespace DLS.Game
{
	public static class BuiltinCollectionCreator
	{
		public static StarredItem[] GetDefaultStarredList()
		{
			return new StarredItem[]
			{
				new("IN/OUT", true),
				new(ChipTypeHelper.GetName(ChipType.Nand), false)
			};
		}

		public static ChipCollection[] CreateDefaultChipCollections()
		{
			return new[]
			{
				CreateChipCollection("BASIC",
					ChipType.Nand,
					ChipType.Clock,
					ChipType.Pulse,
					ChipType.Key,
					ChipType.TriStateBuffer
				),
				CreateChipCollection("IN/OUT",
					ChipType.In_1Bit,
					ChipType.In_4Bit,
					ChipType.In_8Bit,
					ChipType.In_16Bit,
					ChipType.Out_1Bit,
					ChipType.Out_4Bit,
					ChipType.Out_8Bit,
					ChipType.Out_16Bit
				),
				CreateChipCollection("MERGE/SPLIT",
					ChipType.Merge_1To4Bit,
					ChipType.Merge_1To8Bit,
					ChipType.Merge_1To16Bit,
					ChipType.Merge_4To8Bit,
					ChipType.Merge_8To16Bit,
					ChipType.Split_4To1Bit,
					ChipType.Split_8To4Bit,
					ChipType.Split_8To1Bit,
					ChipType.Split_16To8Bit,
					ChipType.Split_16To1Bit
				),
				CreateChipCollection("BUS",
					ChipType.Bus_1Bit,
					ChipType.Bus_4Bit,
					ChipType.Bus_8Bit,
					ChipType.Bus_16Bit
				),
				CreateChipCollection("DISPLAY",
					ChipType.SevenSegmentDisplay,
					ChipType.DisplayDot,
					ChipType.DisplayRGB,
					ChipType.DisplayLED
				),
				CreateChipCollection("MEMORY",
					ChipType.Rom_256x16,
					ChipType.Ram_256x8,
					ChipType.Ram_64Kx16
				)
			};
		}

		public static void AddMissingBuiltinChipsToCollections(List<ChipCollection> collections)
		{
			if (collections == null) return;

			InsertAfter(collections, "IN/OUT", ChipType.In_8Bit, ChipType.In_16Bit);
			InsertAfter(collections, "IN/OUT", ChipType.Out_8Bit, ChipType.Out_16Bit);
			InsertAfter(collections, "MERGE/SPLIT", ChipType.Merge_1To8Bit, ChipType.Merge_1To16Bit);
			InsertAfter(collections, "MERGE/SPLIT", ChipType.Merge_4To8Bit, ChipType.Merge_8To16Bit);
			InsertAfter(collections, "MERGE/SPLIT", ChipType.Split_8To1Bit, ChipType.Split_16To8Bit);
			InsertAfter(collections, "MERGE/SPLIT", ChipType.Split_16To8Bit, ChipType.Split_16To1Bit);
			InsertAfter(collections, "MEMORY", ChipType.Rom_256x16, ChipType.Ram_256x8);
			InsertAfter(collections, "MEMORY", ChipType.Ram_256x8, ChipType.Ram_64Kx16);
			InsertAfter(collections, "BUS", ChipType.Bus_8Bit, ChipType.Bus_16Bit);
		}

		static void InsertAfter(List<ChipCollection> collections, string collectionName, ChipType afterType, ChipType newType)
		{
			ChipCollection collection = collections.FirstOrDefault(c => ChipDescription.NameMatch(c.Name, collectionName));
			if (collection == null) return;

			string newName = ChipTypeHelper.GetName(newType);
			if (collection.Chips.Any(c => ChipDescription.NameMatch(c, newName))) return;

			string afterName = ChipTypeHelper.GetName(afterType);
			int idx = collection.Chips.FindIndex(c => ChipDescription.NameMatch(c, afterName));
			if (idx >= 0) collection.Chips.Insert(idx + 1, newName);
			else collection.Chips.Add(newName);
		}

		static ChipCollection CreateChipCollection(string name, params ChipType[] chipTypes)
		{
			return new ChipCollection(name, chipTypes.Select(t => ChipTypeHelper.GetName(t)).ToArray());
		}
	}
}
