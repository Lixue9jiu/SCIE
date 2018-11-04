using Engine;

namespace Game
{
	public class SubsystemSourceOfFireBlockBehavior : SubsystemTorchBlockBehavior
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new[]
				{
					17,
					31,
					92,
					104,
					132,
					209
				};
			}
		}

		public static void TryExplode(int x, int y, int z)
		{
			if (Utils.SubsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living)
			{
				return;
			}
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
			if (content != 92 && content != 104 && content != 209)
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
			if (content != 92 && content != 104 && content != 209)
				AddTorch(value, x, y, z);
		}

		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			Utils.RemoveElementsInChunk(chunk, m_particleSystemsByCell.Keys, RemoveTorch);
		}

		public void RemoveTorch(Point3 p)
		{
			if (m_particleSystemsByCell.TryGetValue(p, out FireParticleSystem particleSystem))
			{
				m_subsystemParticles.RemoveParticleSystem(particleSystem);
				m_particleSystemsByCell.Remove(p);
			}
		}
	}
}