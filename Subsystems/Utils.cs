using System;
using Engine;
using static Game.TerrainBrush;

namespace Game
{
	public static class Utils
	{
		public static void PaintFastSelective(this TerrainChunk chunk, Cell[] cells, int x, int y, int z, int onlyInBlock = BasaltBlock.Index)
		{
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			for (int i = 0; i < cells.Length; i++)
			{
				Cell cell = cells[i];
				int y2 = cell.Y + y;
				if (y2 >= 0 && y2 < 128)
				{
					int index = TerrainChunk.CalculateCellIndex(cell.X + x & 15, y2, cell.Z + z & 15);
					if (Terrain.ExtractContents(onlyInBlock) == Terrain.ExtractContents(chunk.GetCellValueFast(index)))
					{
						//SubsystemMineral.StoreItemData(cell.Value);
						chunk.SetCellValueFast(index, onlyInBlock);
					}
				}
			}
		}
		public static void PaintMaskSelective(this TerrainChunk chunk, Cell[] cells, int x, int y, int z, int mask = BasaltBlock.Index)
		{
			x -= chunk.Origin.X;
			z -= chunk.Origin.Y;
			for (int i = 0; i < cells.Length; i++)
			{
				Cell cell = cells[i];
				int y2 = cell.Y + y;
				if (y2 >= 0 && y2 < 128)
				{
					int index = TerrainChunk.CalculateCellIndex(cell.X + x & 15, y2, cell.Z + z & 15);
					y2 = chunk.GetCellValueFast(index);
					if (Terrain.ExtractContents(mask) == Terrain.ExtractContents(y2))
					{
						//SubsystemMineral.StoreItemData(cell.Value);
						chunk.SetCellValueFast(index, y2 | mask);
					}
				}
			}
		}
		public static int GetDirectionXZ(ComponentMiner componentMiner)
		{
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			float max = MathUtils.Max(num, num2, num3, num4);
			if (num == max)
			{
				return 2;
			}
			else if (num2 == max)
			{
				return 3;
			}
			else if (num3 == max)
			{
				return 0;
			}
			else if (num4 == max)
			{
				return 1;
			}
			return 0;
		}
		public static void Push<T>(this DynamicArray<T> array, T item)
		{
			if (array.m_count >= array.Capacity)
			{
				int value = MathUtils.Max(array.Capacity << 1, 4);
				if (value != array.Capacity)
				{
					T[] arr = new T[value];
					if (array.Array != null)
					{
						Array.Copy(array.Array, 0, arr, 0, array.m_count);
					}
					array.Array = arr;
				}
			}
			array.Array[array.m_count++] = item;
		}
	}
}
