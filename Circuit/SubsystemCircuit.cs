using Engine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TemplatesDatabase;

namespace Game
{
	public class SubsystemCircuit : SubsystemBlockBehavior, IUpdateable
	{
		public LockFreeQueue<Element[]> Requests = new LockFreeQueue<Element[]>();
		protected float m_remainingSimulationTime;
		public static ElementBlock Block;
		public int UpdateStep;
		public bool UpdatePath;
		public HashSet<Device> Path;
		public Element[][] CircuitPath;
		public static Dictionary<Point3, Device> Table;
		public override int[] HandledBlocks => new int[] { ElementBlock.Index };
		public int UpdateOrder => 0;
		public override void Load(ValuesDictionary valuesDictionary)
		{
			CircuitPath = new Element[0][];
			base.Load(valuesDictionary);
			Utils.Load(Project);
			Path = new HashSet<Device>();
			Table = new Dictionary<Point3, Device>(valuesDictionary.GetValue("Count", 0));
			Block = BlocksManager.Blocks[ElementBlock.Index] as ElementBlock;
			Task.Run((Action)ThreadFunction);
			BlocksManager.Blocks[IceBlock.Index].HasCollisionBehavior = true;
		}
		public override void OnBlockGenerated(int value, int x, int y, int z, bool isLoaded)
		{
			OnBlockAdded(value, -1, x, y, z);
		}
		public override void OnBlockAdded(int value, int oldValue, int x, int y, int z)
		{
			var device = Block.GetDevice(x, y, z, value);
			if (device == null)
				return;
			if (device is IBlockBehavior behavior)
				behavior.OnBlockAdded(SubsystemTerrain, value, oldValue);
			Table[device.Point] = device;
			UpdatePath = true;
			if ((device.Type & ElementType.Supply) == 0)
			{
				if (oldValue == -1)
					return;
			}
			else
				Path.Add(device);
		}
		public override void OnBlockRemoved(int value, int newValue, int x, int y, int z)
		{
			var device = Block.GetDevice(x, y, z, value);
			if (device == null)
				return;
			UpdatePath = true;
			if (device is IBlockBehavior behavior)
				behavior.OnBlockRemoved(SubsystemTerrain, value, newValue);
			if (device.Next != null)
			{
				if ((device.Type & ElementType.Supply) != 0)
					Path.Remove(device);
			}
			Table.Remove(device.Point);
		}
		public override void OnBlockModified(int value, int oldValue, int x, int y, int z)
		{
			OnBlockRemoved(oldValue, value, x, y, z);
			OnBlockAdded(value, oldValue, x, y, z);
			UpdatePaths();
		}
		public override void OnChunkDiscarding(TerrainChunk chunk)
		{
			int startX = chunk.Origin.X;
			int startY = chunk.Origin.Y;
			var list = new List<Device>();
			for (var i = Path.GetEnumerator(); i.MoveNext();)
			{
				var key = i.Current.Point;
				if (key.X >= startX && key.X < startX + 16 && key.Z >= startY && key.Z < startY + 16)
					list.Add(i.Current);
			}
			for (startX = 0; startX < list.Count; startX++)
				Path.Remove(list[startX]);
		}
		public override void OnNeighborBlockChanged(int x, int y, int z, int neighborX, int neighborY, int neighborZ)
		{
			var device = Block.GetDevice(Utils.Terrain, x, y, z);
			if (device is IUnstableBlock behavior)
				behavior.OnNeighborBlockChanged(SubsystemTerrain, neighborX, neighborY, neighborZ);
		}
		public override bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			var face = raycastResult.CellFace;
			return Block.GetDevice(face.X, face.Y, face.Z, raycastResult.Value) is IInteractiveBlock block && block.OnInteract(raycastResult, componentMiner);
		}
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			var item = Block.GetDevice(Utils.Terrain, cellFace.X, cellFace.Y, cellFace.Z);
			if (item is IItemAcceptableBlock block)
				block.OnHitByProjectile(cellFace, worldItem);
		}
		public override void OnItemHarvested(int x, int y, int z, int blockValue, ref BlockDropValue dropValue, ref int newBlockValue)
		{
			var item = Block.GetDevice(x, y, z, blockValue);
			if (item is IHarvestingItem block)
				block.OnItemHarvested(x, y, z, blockValue, ref dropValue, ref newBlockValue);
		}
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			if (componentBody.Mass < 50f)
				return;
			int x = cellFace.X,
				y = cellFace.Y,
				z = cellFace.Z;
			var item = Block.GetDevice(x, y, z, Utils.Terrain.GetCellValueFast(x, y, z));
			if (velocity < -1f && cellFace.Face == 4 && item is SolarPanel)
			{
				SubsystemTerrain.DestroyCell(0, x, y, z, 0, true, false);
				return;
			}
			if (item is ElectricFences fences && fences.Powered)
			{
				ComponentCreature componentCreature = componentBody.Entity.FindComponent<ComponentCreature>();
				if (componentCreature != null)
				{
					componentCreature.ComponentHealth.Injure(Utils.Random.UniformFloat(4.8f, 5.2f) / componentCreature.ComponentHealth.AttackResilience, null, false, "Electric shock");
				}
			}
		}
		/*public override bool OnEditBlock(int x, int y, int z, int value, ComponentPlayer componentPlayer)
		{
			var item = elementblock.GetDevice(Utils.Terrain, x, y, z);
			if (item is IEditableBlock block)
				block.OnEditBlock(x, y, z, value, componentPlayer);
			return false;
		}*/
		public void Update(float dt)
		{
			UpdatePaths();
			m_remainingSimulationTime = MathUtils.Min(m_remainingSimulationTime + dt, 0.1f);
			while (m_remainingSimulationTime >= 0.02f)
			{
				UpdateStep++;
				m_remainingSimulationTime -= 0.02f;
				for (int i = 0; i < CircuitPath.Length; i++)
					QueueSimulate(CircuitPath[i]);
			}
		}
		public void UpdatePaths()
		{
			if (!UpdatePath) return;
			Requests.Clear();
			int i, j;
			for (i = 0; i < CircuitPath.Length; i++)
			{
				int length = CircuitPath[i].Length;
				for (j = 1; j < length; j++)
					QueueSimulate(CircuitPath[i]);
			}
			CircuitPath = new Element[Path.Count][];
			i = 0;
			for (var enumerator = Path.GetEnumerator(); enumerator.MoveNext(); i++)
			{
				var v = enumerator.Current;
				var visited = new HashSet<Device>
				{
					v
				};
				var neighbors = new DynamicArray<Device>(6);
				var Q = new Queue<Device>();
				int count;
				bool supply = false;
				Q.Enqueue(v);
				while (Q.Count > 0)
				{
					v = Q.Dequeue();
					neighbors.Clear();
					Block.GetAllConnectedNeighbors(Utils.Terrain, v, 4, neighbors);
					v.Next = new Element[neighbors.Count];
					count = 0;
					for (j = 0; j < neighbors.Count+1; j++)
					{
						var w = neighbors.Array[j];
						if (w == null)
							continue;
						if (visited.Add(w))
						{
							if ((w.Type & ElementType.Supply) != 0)
							{
								//if (supply) continue;
								supply = true;
							}
							supply = true;
							v.Next[count++] = w;
							Q.Enqueue(w);
						}
					}
				}
				Element element = enumerator.Current;
				var stack = new DynamicArray<Element>(1)
				{
					element
				};
				var arr = new Element[visited.Count];
				count = 0;
				while (stack.Count > 0)
				{
					if ((element = stack.Array[--stack.Count]) != null)
					{
						arr[count++] = element;
						var next = element.Next;
						for (j = 0; j < next.Length && next[j] != null; j++)
							stack.Add(next[j]);
					}
				}
				CircuitPath[i] = new Element[count];
				Array.Copy(arr, CircuitPath[i], count);
			}
			UpdatePath = false;

		}
		public override void Dispose()
		{
			Requests.Clear();
			Requests.Enqueue(null);
			Utils.LoadedProject = false;
		}
		public void ThreadFunction()
		{
			while (true)
			{
				while (Requests.Count == 0)
					Task.Delay(10).Wait();
				var request = Requests.Dequeue();
				if (request == null)
					return;
				else
				{
					var elements = request;
					int voltage = 0;
					for (int i = 0; i < elements.Length; i++)
						elements[i].Simulate(ref voltage);
				}
				Task.Delay(10).Wait();
			}
		}

		public void QueueSimulate(Element[] elements)
		{
			Requests.Enqueue(elements);
		}
	}
}