using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public enum Materials
	{
		Steel,
		Gold,
		Silver,
		Lead,
		Platinum,
		Zinc,
		Stannary,
		Chromium,
		Titanium,
		Nickel,
		Aluminum,
		Uranium,
		Phosphorus,
		Iron,
		Copper,
		Mercury,
		Germanium,
		FeAlCrAlloy,
		Plastic,
	}
	public class MetalBlock : PaintedCubeBlock
	{
		public const int Index = 510;
		public static readonly string[] Names = new[]
		{
			"BasicMachineCase",
			"SecondryMachineCase",
			"FireBrickWall"
		};
		public static readonly Color[] Colors = new[]
		{
			Color.White, Color.LightGray, new Color(255, 153, 18)
		};
		public MetalBlock() : base(0)
		{
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var arr = new int[15 * (16 + 1)];
			int i;
			for (i = 0; i < 15; i++)
			{
				arr[i] = Terrain.ReplaceData(Index, i << 5);
			}
			for (i = 0; i < 15 * 16; i++)
			{
				arr[i + 15] = Terrain.ReplaceData(Index, i << 1 | 1);
			}
			return arr;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int type = GetType(value);
			color = (type < 3 ? Colors[type] : GetColor((Materials)(type - 3))) * SubsystemPalette.GetColor(environmentData, GetPaintColor(value));
			BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = value,
				CellFace = raycastResult.CellFace
			};
		}
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			int type = GetType(value);
			generator.GenerateCubeVertices(this, value, x, y, z, (type < 3 ? Colors[type] : GetColor((Materials)(type - 3))) * SubsystemPalette.GetColor(generator, GetPaintColor(value)), geometry.OpaqueSubsetsByFace);
		}
		public static int GetType(int value)
		{
			return Terrain.ExtractData(value) >> 5 & 0xF;
		}
		/*public static int SetType(int value, Type type)
		{
			return Terrain.ReplaceData(value, (Terrain.ExtractData(value) & -481) | ((int)type & 0xF) << 5);
		}*/
		public static Color GetColor(Materials type)
		{
			switch (type)
			{
				case Materials.Steel: return Color.LightGray;
				case Materials.Gold: return new Color(255, 215, 0);
				case Materials.Lead: return new Color(88, 87, 86);
				case Materials.Platinum: return new Color(253, 253, 253);
				case Materials.Chromium: return new Color(58, 57, 56);
				case Materials.Iron: return Color.White;
				case Materials.Copper: return new Color(255, 127, 80);
				case Materials.FeAlCrAlloy: return new Color(200, 200, 200);
			}
			return new Color(232, 232, 232);
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int type = GetType(value);
			return type < 3 ? Names[type]: "Soild " + ((Materials)(type - 3)).ToString() + " Block";
		}
		public override string GetDescription(int value)
		{
			switch (GetType(value))
			{
				case 0: return "The basic case of some machine. Very heavy and durable. Extremely resilient to digging and explosions. Can be crafted from multiple ironplate or steel ingots.";
				case 1: return "The Secondry case of some machine or device. Very heavy and durable. Extremely resilient to digging and explosions. Can be crafted from multiple steelplate.";
				case 2: return "Fire Brick wall can be made by combining several fire bricks together and binding them with mortar. It is a versatile, strong and good looking industrial material.";
				default:
					var type = (Materials)(GetType(value) - 3);
					return type == Materials.Steel
						? "The block of steel. Very heavy and durable. Extremely resilient to digging and explosions. Can be crafted from multiple steel ingots."
						: "The block of pure " + type.ToString() + " Can be crafted from multiple " + type.ToString() + " ingots.";
			}
		}
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			dropValues.Add(new BlockDropValue
			{
				Value = oldValue,
				Count = 1
			});
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			switch (GetType(value))
			{
				case 0:
				case 1: return 107;
				case 2: return 39;
			}
			return 180;
		}
	}
}