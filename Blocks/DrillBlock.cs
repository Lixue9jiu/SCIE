using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class DrillBlock : FlatBlock, IDurability
	{
		public enum Type
		{
			SteelDrill,
			DiamondDrill,
			IronTubularis,
			SteelTubularis
		}
		public const int Index = 525;
		public override IEnumerable<int> GetCreativeValues()
		{
			var array = new int[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = Terrain.ReplaceData(Index, i);
			}
			return array;
		}
		public override string GetCategory(int value)
		{
			return (Terrain.ExtractData(value) >> 12 & 15) != 0 ? "Painted" : base.GetCategory(value);
		}
		/*public override CraftingRecipe GetAdHocCraftingRecipe(SubsystemTerrain subsystemTerrain, string[] ingredients, float heatLevel)
		{
			return Utils.GetAdHocCraftingRecipe(Index, subsystemTerrain, ingredients, heatLevel);
		}*/
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			switch (GetType(value))
			{
				//case Type.SteelDrill:
				//break;
				case Type.DiamondDrill:
					color = Color.Cyan;
					break;
				//case Type.IronTubularis:
				//break;
				case Type.SteelTubularis:
					color = Color.Gray;
					break;
			}
			BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color * SubsystemPalette.GetColor(environmentData, Terrain.ExtractData(value) >> 12 & 15), false, environmentData);
		}
		public static Type GetType(int value)
		{
			return (Type)(Terrain.ExtractData(value) & 0xF);
		}
		/*public static int SetType(int value, Type type)
		{
			return Terrain.ReplaceData(value, (Terrain.ExtractData(value) & -16) | ((int)type & 0xF));
		}*/
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return SubsystemPalette.GetName(subsystemTerrain, Terrain.ExtractData(value) >> 12 & 15, GetType(value).ToString());
		}
		public override string GetDescription(int value)
		{
			switch (GetType(value))
			{
				default:
				//case Type.SteelDrill:
				//case Type.DiamondDrill:
					return DefaultDescription;
				case Type.IronTubularis:
				case Type.SteelTubularis:
					return "Tubularis is the most important part in the liquidpump, you should put it in the machine while using it. Reminding, pumping magma will damage the Tubularis.";
			}
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			switch (GetType(value))
			{
				default:
				//case Type.SteelDrill:
				//case Type.DiamondDrill:
					return 214;
				case Type.IronTubularis:
				case Type.SteelTubularis:
					return 112;
			}
		}
		public int GetDurability(int value)
		{
			switch (GetType(value))
			{
				case Type.SteelDrill:
					return 1000;
				case Type.DiamondDrill:
					return 2000;
				case Type.IronTubularis:
					return 700;
				case Type.SteelTubularis:
					return 1100;
			}
			return 0;
		}
	}
}
