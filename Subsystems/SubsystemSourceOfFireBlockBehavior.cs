using System.Collections.Generic;
using Engine;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemSourceOfFireBlockBehavior : SubsystemTorchBlockBehavior
	{
		public override int[] HandledBlocks
		{
			get
			{
				return new []
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
		public void TryExplode(int x, int y, int z)
		{
			if (Utils.SubsystemGameInfo.WorldSettings.EnvironmentBehaviorMode != EnvironmentBehaviorMode.Living)
			{
				return;
			}
			var terrain = SubsystemTerrain.Terrain;
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
			int originX = chunk.Origin.X, originY = chunk.Origin.Y;
			var list = new List<Point3>();
			for (var i = m_particleSystemsByCell.Keys.GetEnumerator(); i.MoveNext();)
			{
				var key = i.Current;
				if (key.X >= originX && key.X < originX + 16 && key.Z >= originY && key.Z < originY + 16)
				{
					list.Add(key);
				}
			}
			for (originX = 0; originX < list.Count; originX++)
			{
				RemoveTorch(list[originX]);
			}
		}
		public override void Load(ValuesDictionary valuesDictionary)
		{
			base.Load(valuesDictionary);
			Utils.Load(Project);
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