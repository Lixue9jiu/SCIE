using Engine;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemNSoilBlockBehavior : SubsystemSoilBlockBehavior, IUpdateable
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new int[1]
				{
					168
				};
			}
		}

		public int UpdateOrder
		{
			get
			{
				return 0;
			}
		}

		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			if (!((double)componentBody.Mass <= 20.0) && !componentBody.IsSneaking)
			{
				Vector3 velocity2 = componentBody.Velocity;
				if (!((double)velocity2.Y >= -3.0) || (!((double)velocity2.Y >= 0.0) && !((double)m_random.UniformFloat(0f, 1f) >= 1.5 * (double)m_subsystemTime.GameTimeDelta) && !((double)velocity2.LengthSquared() <= 1.0)))
				{
					m_toDegrade[cellFace.Point] = true;
				}
			}
		}

		public override void OnPoll(int value, int x, int y, int z, int pollPass)
		{
			bool hydration = SoilBlock.GetHydration(Terrain.ExtractData(value));
			if (DetermineHydration(x, y, z, 3))
			{
				if (!hydration)
				{
					m_toHydrate[new Point3(x, y, z)] = true;
				}
			}
			else if (hydration)
			{
				m_toHydrate[new Point3(x, y, z)] = false;
			}
		}

		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			if (DegradesSoilIfOnTopOfIt(base.SubsystemTerrain.Terrain.GetCellValue(x, y + 1, z)))
			{
				int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(x, y, z);
				base.SubsystemTerrain.ChangeCell(x, y, z, Terrain.ReplaceContents(cellValue, 2), true);
			}
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			m_subsystemTime = base.Project.FindSubsystem<SubsystemTime>(true);
		}

		public void Update(float dt)
		{
			if (m_subsystemTime.PeriodicGameTimeEvent(2.5, 0.0))
			{
				foreach (Point3 key2 in m_toDegrade.Keys)
				{
					if (base.SubsystemTerrain.Terrain.GetCellContents(key2.X, key2.Y, key2.Z) == 168)
					{
						int cellValue = base.SubsystemTerrain.Terrain.GetCellValue(key2.X, key2.Y, key2.Z);
						base.SubsystemTerrain.ChangeCell(key2.X, key2.Y, key2.Z, Terrain.ReplaceContents(cellValue, 2), true);
					}
				}
				m_toDegrade.Clear();
			}
			if (m_subsystemTime.PeriodicGameTimeEvent(10.0, 0.0))
			{
				foreach (KeyValuePair<Point3, bool> item in m_toHydrate)
				{
					Point3 key = item.Key;
					bool value = item.Value;
					int cellValue2 = base.SubsystemTerrain.Terrain.GetCellValue(key.X, key.Y, key.Z);
					if (Terrain.ExtractContents(cellValue2) == 168)
					{
						int data = SoilBlock.SetHydration(Terrain.ExtractData(cellValue2), value);
						int value2 = Terrain.ReplaceData(cellValue2, data);
						base.SubsystemTerrain.ChangeCell(key.X, key.Y, key.Z, value2, true);
					}
				}
				m_toHydrate.Clear();
			}
		}

		private bool DegradesSoilIfOnTopOfIt(int value)
		{
			Block block = BlocksManager.Blocks[Terrain.ExtractContents(value)];
			if (!block.IsFaceTransparent(base.SubsystemTerrain, 5, value))
			{
				return block.IsCollidable;
			}
			return false;
		}

		private bool DetermineHydration(int x, int y, int z, int steps)
		{
			if (steps > 0 && y > 0 && y < 126)
			{
				if (!DetermineHydrationHelper(x - 1, y, z, steps - 1) && !DetermineHydrationHelper(x + 1, y, z, steps - 1) && !DetermineHydrationHelper(x, y, z - 1, steps - 1) && !DetermineHydrationHelper(x, y, z + 1, steps - 1))
				{
					if (steps >= 2)
					{
						if (!DetermineHydrationHelper(x, y - 1, z, steps - 2))
						{
							return DetermineHydrationHelper(x, y + 1, z, steps - 2);
						}
						return true;
					}
					return false;
				}
				return true;
			}
			return false;
		}

		private bool DetermineHydrationHelper(int x, int y, int z, int steps)
		{
			int cellValueFast = base.SubsystemTerrain.Terrain.GetCellValueFast(x, y, z);
			int num = Terrain.ExtractContents(cellValueFast);
			int data = Terrain.ExtractData(cellValueFast);
			switch (num)
			{
			case 168:
				if (!SoilBlock.GetHydration(data))
				{
					goto default;
				}
				goto IL_0039;
			default:
				if (num != 2)
				{
					return false;
				}
				goto IL_0039;
			case 18:
				{
					return true;
				}
				IL_0039:
				return DetermineHydration(x, y, z, steps);
			}
		}

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (Terrain.ExtractContents(worldItem.Value) == 102 && worldItem.Velocity.Length() >= 20f)
			{
				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 2; j++)
					{
						if (base.SubsystemTerrain.Terrain.GetCellContents(cellFace.X + i, cellFace.Y, cellFace.Z + j) == 168)
						{
							int cellValueFast = base.SubsystemTerrain.Terrain.GetCellValueFast(cellFace.X + i, cellFace.Y, cellFace.Z + j);
							Terrain.ExtractContents(cellValueFast);
							bool hydration = SoilBlock.GetHydration(Terrain.ExtractData(cellValueFast));
							base.SubsystemTerrain.ChangeCell(cellFace.X + i, cellFace.Y, cellFace.Z + j, 168 + (SoilBlock.SetNitrogen(0, 3) + SoilBlock.SetHydration(0, hydration)) * 16384, true);
							worldItem.ToRemove = true;
						}
					}
				}
			}
			if (BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)] is SeedsBlock && worldItem.Velocity.Length() >= 20f && base.SubsystemTerrain.Terrain.GetCellContents(cellFace.X, cellFace.Y + 1, cellFace.Z) == 0)
			{
				int value = 0;
				switch (worldItem.Value)
				{
				case 16557:
					value = 20 + FlowerBlock.SetIsSmall(0, true) * 16384;
					break;
				case 173:
					value = 19 + TallGrassBlock.SetIsSmall(0, true) * 16384;
					break;
				case 49325:
					value = 25 + FlowerBlock.SetIsSmall(0, true) * 16384;
					break;
				case 32941:
					value = 24 + FlowerBlock.SetIsSmall(0, true) * 16384;
					break;
				case 82093:
					value = 174 + RyeBlock.SetSize(RyeBlock.SetIsWild(0, false), 0) * 16384;
					break;
				case 65709:
					value = 174 + RyeBlock.SetSize(RyeBlock.SetIsWild(0, false), 0) * 16384;
					break;
				case 114861:
					value = 131 + BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, false), 0) * 16384;
					break;
				case 98477:
					value = 204 + CottonBlock.SetSize(CottonBlock.SetIsWild(0, false), 0) * 16384;
					break;
				}
				base.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y + 1, cellFace.Z, value, true);
				worldItem.ToRemove = true;
			}
		}
	}
}