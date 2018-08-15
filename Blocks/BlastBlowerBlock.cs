using System;
using Engine;

namespace Game
{
	public class BlastBlowerBlock : CubeBlock
	{
		public override int GetFaceTextureSlot(int face, int value)
		{
			return 220;
		}
		
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float x = Vector3.Dot(forward, -Vector3.UnitX);
			float x2 = Vector3.Dot(forward, Vector3.UnitY);
			float x3 = Vector3.Dot(forward, -Vector3.UnitY);
			float num4 = MathUtils.Min(MathUtils.Min(num, num2, num3), MathUtils.Min(x, x2, x3));
			if ((double)num != (double)num4 && (double)num2 != (double)num4)
			{
				double num5 = (double)num3;
				double num6 = (double)num4;
			}
			return new BlockPlacementData
			{
				Value = Terrain.MakeBlockValue(540, 0, 0),
				CellFace = raycastResult.CellFace
			};
		}
		
		public static int GetPower(int data)
		{
			return data & 99;
		}
		
		public static int SetPower(int data, int direction)
		{
			return (data & -100) | (direction & 99);
		}
		
		public const int Index = 540;
	}
}
