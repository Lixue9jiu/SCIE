using Engine;
using Engine.Serialization;
using Game;
using TemplatesDatabase;

public class TorchBlock : Game.TorchBlock
{
	public new const int Index = 31;
	public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
	{
		int num = Terrain.ExtractData(value);
		if (num < m_collisionBoxes.Length)
		{
			return m_collisionBoxes[num];
		}
		return new BoundingBox[1];
	}
}
namespace Game
{
	public class SubsystemSourceOfFireBlockBehavior : SubsystemTorchBlockBehavior, IUpdateable
	{
		protected Point3[] lightingPoints;

		public override int[] HandledBlocks
		{
			get { return new[]
				{
					17,
					31,
					92,
					104,
					132,
					209
				}; }
		}

		public int UpdateOrder => 0;

		public static void TryExplode(int x, int y, int z)
		{
			if (Utils.SubsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living)
				return;
			var terrain = Utils.SubsystemTerrain.Terrain;
			for (int i = x - 2; i <= x + 2; i++)
				for (int j = y - 4; j <= y + 1; j++)
					if (terrain.IsCellValid(i, j, 0))
						for (int k = z - 2; k <= z + 2; k++)
						{
							int value = terrain.GetCellValueFast(i, j, k);
							if (Terrain.ExtractContents(value) == CoalOreBlock.Index)
							{
								float pressure = BlocksManager.Blocks[CoalOreBlock.Index].GetExplosionPressure(value);
								if (terrain.GetCellContentsFast(i + 1, j, k) == 0)
									Utils.SubsystemExplosions.AddExplosion(i + 1, j, k, pressure, false, false);
								if (terrain.GetCellContentsFast(i - 1, j, k) == 0)
									Utils.SubsystemExplosions.AddExplosion(i - 1, j, k, pressure, false, false);
								if (terrain.GetCellContents(i, j + 1, k) == 0)
									Utils.SubsystemExplosions.AddExplosion(i, j + 1, k, pressure, false, false);
								if (terrain.GetCellContents(i, j - 1, k) == 0)
									Utils.SubsystemExplosions.AddExplosion(i, j - 1, k, pressure, false, false);
								if (terrain.GetCellContentsFast(i, j, k + 1) == 0)
									Utils.SubsystemExplosions.AddExplosion(i, j, k + 1, pressure, false, false);
								if (terrain.GetCellContentsFast(i, j, k - 1) == 0)
									Utils.SubsystemExplosions.AddExplosion(i, j, k - 1, pressure, false, false);
							}
						}
		}

		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			TryExplode(x, y, z);
			int content = Terrain.ExtractContents(value);
			if (content == 92 || content == 104 || content == 209 || (content == 31 && Terrain.ExtractData(value) >= 5))
				return;
			AddTorch(value, x, y, z);
		}

		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			RemoveTorch(new Point3(x, y, z));
		}

		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			RemoveTorch(new Point3(x, y, z));
			OnBlockAdded(value, oldValue, x, y, z);
		}

		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			int content = Terrain.ExtractContents(value);
			if (content == 92 || content == 104 || content == 209)
				return;
			AddTorch(value, x, y, z);
		}

		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			Utils.RemoveElementsInChunk(chunk, m_particleSystemsByCell.Keys, RemoveTorch);
		}
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			int value = SubsystemTerrain.Terrain.GetCellValueFast(x, y, z);
			if (Terrain.ExtractContents(value) == 31 && Terrain.ExtractData(value) == 5)
			{
				return;
			}
			base.OnNeighborBlockChanged(x, y, z, neighborX, neighborY, neighborZ);
		}

		public void RemoveTorch(Point3 p)
		{
			if (m_particleSystemsByCell.TryGetValue(p, out FireParticleSystem particleSystem))
			{
				m_subsystemParticles.RemoveParticleSystem(particleSystem);
				m_particleSystemsByCell.Remove(p);
			}
		}

		public void Update(float dt)
		{
			if (!Utils.SubsystemTime.PeriodicGameTimeEvent(0.1, 0.0))
				return;
			var list = Utils.SubsystemPlayers.ComponentPlayers;
			for (int i = 0; i < list.Count; i++)
			{
				var componentPlayer = list[i];
				Point3 point = GetPoint(componentPlayer.ComponentBody.Position);
				IInventory inventory = componentPlayer.ComponentMiner.Inventory;
				ref Point3 point3 = ref lightingPoints[i];
				if (Terrain.ExtractContents(inventory.GetSlotValue(inventory.ActiveSlotIndex)) == 31)
				{
					if (point != point3)
					{
						if (SubsystemTerrain.Terrain.GetCellContents(point.X, point.Y, point.Z) == 0)
						{
							SubsystemTerrain.ChangeCell(point.X, point.Y, point.Z, Terrain.MakeBlockValue(TorchBlock.Index, 0, 5));
							if (point3 != Point3.Zero)
							{
								SubsystemTerrain.ChangeCell(point3.X, point3.Y, point3.Z, 0);
							}
							point3 = point;
						}
					}
				}
				else if (point3 != Point3.Zero)
				{
					SubsystemTerrain.ChangeCell(point3.X, point3.Y, point3.Z, 0);
					point3 = Point3.Zero;
				}
			}
		}

		public static Point3 GetPoint(Vector3 v)
		{
			return new Point3((int)MathUtils.Round(v.X), (int)MathUtils.Round(v.Y) + 1, (int)MathUtils.Round(v.Z));
		}

		public override void Load(ValuesDictionary valuesDictionary)
		{
			var s = valuesDictionary.GetValue<string>("Points", null);
			if (!string.IsNullOrEmpty(s))
			{
				lightingPoints = HumanReadableConverter.ValuesListFromString<Point3>(';', s);
				for (int i = 0; i < lightingPoints.Length; i++)
				{
					Point3 point = lightingPoints[i];
					if (point != Point3.Zero)
						SubsystemTerrain.ChangeCell(point.X, point.Y, point.Z, 0);
				}
			}
			else
				lightingPoints = new Point3[4];
			base.Load(valuesDictionary);
		}

		public override void Save(ValuesDictionary valuesDictionary)
		{
			valuesDictionary.SetValue("Points", HumanReadableConverter.ValuesListToString(';', lightingPoints));
			base.Save(valuesDictionary);
		}
	}
}