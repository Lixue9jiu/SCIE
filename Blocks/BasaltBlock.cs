using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class BasaltBlock : PaintedCubeBlock
	{
		public const int Index = 67;

		public BasaltBlock() : base(40)
		{
		}
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			int data = Terrain.ExtractData(oldValue);
			if (toolLevel > 3 && (data & 98304) == 32768 && (Random.Int() & 3) == 0)
			{
				dropValues.Add(new BlockDropValue { Value = oldValue, Count = 1 });
				showDebris = true;
				return;
			}
			if (!IsColored(data) && toolLevel > 2 && (data = data >> 1 & 16383) > 0 && data < 11)
			{
				if ((Random.Int() & 7) == 0)
					dropValues.Add(new BlockDropValue { Value = ItemBlock.IdTable["ScrapIron"], Count = 1 });
				else if ((Random.Int() & 7) == data && (data == 4 || data == 6))
					dropValues.Add(new BlockDropValue
					{
						Value = Terrain.ReplaceData(ItemBlock.Index, data == 4 ? 6 + 14 : 4 + 14),
						Count = 1
					});
				dropValues.Add(new BlockDropValue
				{
					Value = Terrain.ReplaceData(ItemBlock.Index, data + 14),
					Count = 1
				});
				for (data = (Random.Int() & 1) + (2 | data & 1); data-- != 0;)
					dropValues.Add(new BlockDropValue { Value = ExperienceBlock.Index, Count = 1 });
				showDebris = true;
			}
			else
				base.GetDropValues(subsystemTerrain, oldValue, newValue, toolLevel, dropValues, out showDebris);
		}
		public override BlockPlacementData GetDigValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, int toolValue, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = (Terrain.ExtractData(value) & 65536) != 0 ? Terrain.ReplaceData(MagmaBlock.Index
					, FluidBlock.SetIsTop(FluidBlock.SetLevel(0, 4), toolValue == MagmaBucketBlock.Index)) : 0,
				CellFace = raycastResult.CellFace
			};
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			int data = Terrain.ExtractData(value);
			if (IsColored(data))
				return m_coloredTextureSlot;
			data = data >> 1 & 16383;
			return data > 0 && data < 13 ? (data + 243) : DefaultTextureSlot;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			int data = Terrain.ExtractData(value);
			string name = (data & 65536) != 0 ? Utils.Get("不稳定") : string.Empty;
			if ((data & 32768) != 0)
				name += Utils.Get("易爆");
			data &= 16383;
			if (IsColored(data) || data == 0 || data > 20)
				return name + (data == 0 ? base.GetDisplayName(subsystemTerrain, value) : data == 22 ? "铀矿石" : "磷矿石");
			data = (data >> 1) + 14;
			name += ItemBlock.Items[data].GetDisplayName(subsystemTerrain, Terrain.ReplaceData(ItemBlock.Index, data));
			return name.Substring(0, name.Length - (Utils.TR.Count == 0 ? 2 : 5));
		}
		public override float GetExplosionPressure(int value)
		{
			return (Terrain.ExtractData(value) & 32768) != 0 ? 10f : base.GetExplosionPressure(value);
		}
		public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateCubeVertices(this, value, x, y, z, IsColored(Terrain.ExtractData(value) & 16383) ? SubsystemPalette.GetColor(generator, GetColor(Terrain.ExtractData(value))) : Color.White, geometry.OpaqueSubsetsByFace);
		}
		public override IEnumerable<int> GetCreativeValues()
		{
			var list = new List<int>(base.GetCreativeValues());
			int count = list.Count;
			int i = 0;
			for (; i < count; i++)
				list.Add(list[i] | 65536 << 14);
			for (i = 0; i < count; i++)
				list.Add(list[i] | 32768 << 14);
			for (i = 0; i < count; i++)
				list.Add(list[i] | (65536 | 32768) << 14);
			const int M = 13;
			for (i = 1; i < M; i++)
				list.Add(BlockIndex | i << 15);
			for (i = 1; i < M; i++)
				list.Add(BlockIndex | i << 15 | 65536 << 14);
			for (i = 1; i < M; i++)
				list.Add(BlockIndex | i << 15 | 32768 << 14);
			for (i = 1; i < M; i++)
				list.Add(BlockIndex | i << 15 | (65536 | 32768) << 14);
			return list;
		}
	}
}