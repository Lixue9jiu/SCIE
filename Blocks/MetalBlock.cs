using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;


namespace Game
{
	public class IronBlock : PaintedCubeBlock
	{
		public const int Index = 46;
		public IronBlock() : base(180)
		{
		}
	}
	public class CopperBlock : PaintedCubeBlock
	{
		public const int Index = 47;
		public CopperBlock() : base(181)
		{
		}
	}
	public class MetalBlock : PaintedCubeBlock
	{
		public const int Index = 510;
		[Serializable]
        public enum Type
        {
            BasicMachineCase,
            SecondryMachineCase,
            FireBrickWall
        }
		public MetalBlock() : base(0)
		{
		}
        public override IEnumerable<int> GetCreativeValues()
        {
            if (DefaultCreativeData < 0)
            {
                return base.GetCreativeValues();
            }
            var list = new List<int>(14);
			int i;
            for (i = 0; i < list.Capacity; i++)
            {
                list.Add(Terrain.ReplaceData(Index, i << 5));
            }
			for (i = 0; i < 14 * 16; i++)
			{
				list.Add(Terrain.ReplaceData(Index, i << 1 | 1));
			}
			return list;
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			switch (GetType(value))
            {
                //case Type.SteelDrill:
                //break;
                case Type.BasicMachineCase:
                    color = Color.White;
                    break;
                case Type.SecondryMachineCase:
                    color = Color.LightGray;
                    break;
                case Type.FireBrickWall:
                    color = new Color(255,153,18);
                    break;
				default:
					color = GetColor((MetalType)(GetType(value) - 3));
					break;
            }
			color *= SubsystemPalette.GetColor(environmentData, GetPaintColor(value));
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
            Color color;
            switch (GetType(value))
            {
                //case Type.SteelDrill:
                //break;
                case Type.BasicMachineCase:
                    color = Color.White;
                    break;
                case Type.SecondryMachineCase:
                    color = Color.LightGray;
                    break;
                case Type.FireBrickWall:
                    color = new Color(255, 153, 18);
                    break;
				default:
					color = GetColor((MetalType)(GetType(value) - 3));
					break;
			}
			generator.GenerateCubeVertices(this, value, x, y, z, color * SubsystemPalette.GetColor(generator, GetPaintColor(value)), geometry.OpaqueSubsetsByFace);
        }
        public static Type GetType(int value)
        {
            return (Type)(Terrain.ExtractData(value) >> 5 & 0xF);
        }
		/*public static int SetType(int value, Type type)
        {
            return Terrain.ReplaceData(value, (Terrain.ExtractData(value) & -481) | ((int)type & 0xF) << 5);
        }*/
		public static Color GetColor(MetalType type)
		{
			switch (type)
			{
				case MetalType.Steel:
					return Color.LightGray;
				case MetalType.Gold:
					return new Color(255, 215, 0);
				case MetalType.Lead:
					return new Color(88, 87, 86);
				case MetalType.Platinum:
					return new Color(253, 253, 253);
				case MetalType.Chromium:
					return new Color(58, 57, 56);
				case MetalType.Iron:
					return Color.White;
				case MetalType.Copper:
					return new Color(255, 127, 80);
				default:
					return new Color(232, 232, 232);
			}
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
        {
			Type type = GetType(value);
			return type < (Type)3 ? type.ToString() : "Soild " + ((MetalType)(type - 3)).ToString() + " Block";
        }
        public override string GetDescription(int value)
        {
            switch (GetType(value))
            {
                case Type.BasicMachineCase:
                    return "The basic case of some machine. Very heavy and durable. Extremely resilient to digging and explosions. Can be crafted from multiple ironplate or steel ingots.";
                case Type.SecondryMachineCase:
                    return "The Secondry case of some machine or device. Very heavy and durable. Extremely resilient to digging and explosions. Can be crafted from multiple steelplate.";
                case Type.FireBrickWall:
                    return "Fire Brick wall can be made by combining several fire bricks together and binding them with mortar. It is a versatile, strong and good looking industrial material.";
				default:
					MetalType metalType = (MetalType)(GetType(value) - 3);
					return metalType == MetalType.Steel
						? "The block of steel. Very heavy and durable. Extremely resilient to digging and explosions. Can be crafted from multiple steel ingots."
						: "The block of pure " + metalType.ToString() + " Can be crafted from multiple " + metalType.ToString() + " ingots.";
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
                case Type.BasicMachineCase:
                    return 107;
                case Type.SecondryMachineCase:
                    return 107;
                case Type.FireBrickWall:
                    return 39;
			}
            return 180;
        }
	}
}
