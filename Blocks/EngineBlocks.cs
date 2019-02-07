using System.Collections.Generic;

namespace Game
{
	public class EngineBlock : FurnaceNBlock
	{
		public new const int Index = 504;

		public override IEnumerable<int> GetCreativeValues()
		{
			return new int[] { Index, Index | 1 << 10 << 14 };
		}
		public override float GetHeat(int value)
		{
			return GetHeatLevel(value) != 0 ? 0.5f : 0f;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			int slot = GetHeatLevel(value) > 0 ? 175 : 143;
			if (face != 4 && face != 5)
			{
				switch (GetDirection(value))
				{
				case 0:
					switch (face)
					{
					case 0: return slot;
					default: return 159;
					case 2: return 107;
					}
				case 1:
					switch (face)
					{
					case 1: return slot;
					default: return 159;
					case 3: return 107;
					}
				case 2:
					switch (face)
					{
					case 2: return slot;
					default: return 159;
					case 0: return 107;
					}
				default:
					switch (face)
					{
					case 3: return slot;
					default: return 159;
					case 1: return 107;
					}
				}
			}
			return 107;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return Terrain.ExtractData(value) >> 10 != 0 ? "SteamTurbine" : DefaultDisplayName;
		}
		public override string GetDescription(int value)
		{
			return Terrain.ExtractData(value) >> 10 != 0 ? "SteamTurbine" : DefaultDescription;
		}
	}
	public class EngineHBlock : FurnaceNBlock
	{
		public new const int Index = 523;

		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[34];
			arr[0] = BlockIndex;
			int i;
			for (i = 1; i < 17; i++)
				arr[i] = BlockIndex | SetColor(0, i - 1) << 14;
			arr[17] = BlockIndex | 1 << 10;
			for (i = 18; i < 34; i++)
				arr[i] = BlockIndex | SetColor(1 << 10, i - 1) << 14;
			return arr;
		}
		public override float GetHeat(int value)
		{
			return GetHeatLevel(value) != 0 ? 1f : 0f;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			int slot = GetHeatLevel(value) > 0 ? 187 : 171;
			if (face != 4 && face != 5)
			{
				switch (GetDirection(value))
				{
					case 0:
						switch (face)
						{
							case 0: return slot;
							default: return 159;
							case 2: return 107;
						}
					case 1:
						switch (face)
						{
							case 1: return slot;
							default: return 159;
							case 3: return 107;
						}
					case 2:
						switch (face)
						{
							case 2: return slot;
							default: return 159;
							case 0: return 107;
						}
					default:
						switch (face)
						{
							case 3: return slot;
							default: return 159;
							case 1: return 107;
						}
				}
			}
			return 107;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return (Terrain.ExtractData(value) & 1024) != 0 ? "GasTurbine" : DefaultDisplayName;
		}
		public override string GetDescription(int value)
		{
			return (Terrain.ExtractData(value) & 1024) != 0 ? "GasTurbine" : DefaultDescription;
		}
	}
}