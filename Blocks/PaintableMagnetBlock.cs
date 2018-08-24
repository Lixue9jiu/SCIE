using System.Collections.Generic;
using Engine;
using Engine.Graphics;
using Game;

public class MagnetBlock : Game.MagnetBlock, IPaintableBlock
{
	public new const int Index = 167;

	public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
	{
		return m_collisionBoxesByData[Terrain.ExtractData(value) & 1];
	}

	public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
	{
		int data = Terrain.ExtractData(value);
		if (GetIsCross(data))
		{
			generator.GenerateMeshVertices(this, x, y, z, m_meshesByData[(data ^ 1) & 1], SubsystemPalette.GetColor(generator, GetPaintColor(value)), null, geometry.SubsetOpaque);
		}
		generator.GenerateMeshVertices(this, x, y, z, m_meshesByData[data & 1], SubsystemPalette.GetColor(generator, GetPaintColor(value)), null, geometry.SubsetOpaque);
	}

	public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
	{
		color *= SubsystemPalette.GetColor(environmentData, GetPaintColor(value));
		int data = Terrain.ExtractData(value);
		if (GetIsCross(data))
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_meshesByData[0], color, size, ref matrix, environmentData);
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_meshesByData[1], color, size, ref matrix, environmentData);
			return;
		}
		BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneMesh, color, size, ref matrix, environmentData);
	}

	public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
	{
		BlockPlacementData result;
		if (componentMiner.Project.FindSubsystem<SubsystemMagnetBlockBehavior>(true).MagnetsCount < 8)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			result = default(BlockPlacementData);
			result.CellFace = raycastResult.CellFace;
			result.Value = Terrain.ReplaceData(value, SetColor(SetIsCross(MathUtils.Abs(forward.X) <= MathUtils.Abs(forward.Z) ? 1 : 0, GetIsCross(Terrain.ExtractData(value))), GetPaintColor(value)));
			return result;
		}
		var componentPlayer = componentMiner.ComponentPlayer;
		if (componentPlayer != null)
		{
			componentPlayer.ComponentGui.DisplaySmallMessage("Too many magnets", true, false);
		}
		result = default(BlockPlacementData);
		return result;
	}

	public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
	{
		int data = Terrain.ExtractData(oldValue);
		if (!GetColor(data).HasValue)
		{
			base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
			return;
		}
		showDebris = true;
		if (toolLevel >= RequiredToolLevel)
		{
			dropValues.Add(new BlockDropValue
			{
				Value = Terrain.MakeBlockValue(DefaultDropContent, 0, data & -2),
				Count = (int)DefaultDropCount
			});
		}
	}
	public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData)
	{
		return GetIsCross(Terrain.ExtractData(value)) ? new Vector3(-1.2f, -0.3f, -0.5f) : DefaultIconBlockOffset;
	}
	public override float GetIconViewScale(int value, DrawBlockEnvironmentData environmentData)
	{
		return GetIsCross(Terrain.ExtractData(value)) ? 1.8f : DefaultIconViewScale;
	}

	public override IEnumerable<int> GetCreativeValues()
	{
		var array = new int[32];
		for (int i = 0; i < 32; i++)
		{
			array[i] = Terrain.MakeBlockValue(Index, 0, i << 1);
		}
		return array;
	}

	public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
	{
		return SubsystemPalette.GetName(subsystemTerrain, GetPaintColor(value), GetIsCross(Terrain.ExtractData(value)) ? "Cross " + DefaultDisplayName : DefaultDisplayName);
	}

	public int? GetPaintColor(int value)
	{
		return GetColor(Terrain.ExtractData(value));
	}

	public int Paint(SubsystemTerrain subsystemTerrain, int value, int? color)
	{
		return Terrain.ReplaceData(value, SetColor(Terrain.ExtractData(value), color));
	}

	public static int? GetColor(int data)
	{
		if ((data & 30) != 0)
		{
			return data >> 1 & 15;
		}
		return null;
	}

	public static int SetColor(int data, int? color)
	{
		data &= -31;
		return color.HasValue ? color == 0 ? data : data | color.Value << 1 : data;
	}
	public static bool GetIsCross(int data)
	{
		return (data & 0x20) != 0;
	}

	public static int SetIsCross(int data, bool isCross)
	{
		if (!isCross)
		{
			return data & 0x1F;
		}
		return data | 0x20;
	}
}
