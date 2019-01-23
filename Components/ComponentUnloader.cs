using Engine;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
	public class ComponentUnloader : ComponentInventoryBase
	{
		protected ComponentBlockEntity m_componentBlockEntity;
		public SubsystemPlayers SubsystemPlayers;
		public bool DispenseItem = true;

		public bool Place()
		{
			Point3 coordinates = m_componentBlockEntity.Coordinates;
			int cellValue = Utils.Terrain.GetCellValue(coordinates.X, coordinates.Y, coordinates.Z);
			if (Terrain.ExtractContents(cellValue) != Bullet2Block.Index || (Terrain.ExtractData(cellValue) >> 10) == 0)
				return false;
			int num = 0;
			int slotValue;
			while (true)
			{
				if (num >= SlotsCount - 1)
					return false;
				slotValue = GetSlotValue(num);
				int slotCount = GetSlotCount(num);
				if (slotValue != 0 && slotCount > 0)
					break;
				num++;
			}
			int face = FourDirectionalBlock.GetDirection(cellValue);
			for (num = RemoveSlotItems(num, 1); num-- > 0;)
			{
				var position = new Vector3(coordinates) + new Vector3(0.5f);
				Vector3 vector = CellFace.FaceToVector3(face);
				if (!Place(position + vector, face, slotValue) && DispenseItem)
				{
					Vector3 vector2 = position + 0.6f * vector;
					Utils.SubsystemPickables.AddPickable(slotValue, 1, vector2, 1.8f * (vector + m_random.Vector3(0.2f, false)), null);
					Utils.SubsystemAudio.PlaySound("Audio/DispenserDispense", 1f, 0f, new Vector3(vector2.X, vector2.Y, vector2.Z), 3f, true);
				}
			}
			return true;
		}

		public bool Place(Vector3 position, int face, int value)
		{
			var result = Utils.SubsystemTerrain.Raycast(position, position + CellFace.FaceToVector3(face) * 8f, true, true, null);
			var componentMiner = SubsystemPlayers.FindNearestPlayer(position).ComponentMiner;
			if (result.HasValue && componentMiner.Place(result.Value, value))
			{
				if (componentMiner.ComponentCreature.PlayerStats != null)
					componentMiner.ComponentCreature.PlayerStats.BlocksPlaced--;
				return true;
			}
			return false;
		}

		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
			SubsystemPlayers = Project.FindSubsystem<SubsystemPlayers>(true);
		}
	}
	/*public class ComponentVariant : Component
	{
		public struct Gene
		{
			public float[] DominantGenes;
			public float[] RecessiveGenes;
			public float LastTime;
		}
		public static Gene Mutate(Gene father, Gene mother)
		{
			var child = new Gene();
			int i = 0, len = father.DominantGenes.Length;
			for (; i < len; i++)
				child.DominantGenes[i] = ((Utils.Random.Int() & 1) != 0 ? father.DominantGenes : father.RecessiveGenes)[i];
			for (i = 0; i < len; i++)
			{
				var val = ((Utils.Random.Int() & 1) != 0 ? mother.DominantGenes : mother.RecessiveGenes)[i];
				if (val > child.DominantGenes[i])
				{
					child.RecessiveGenes[i] = child.DominantGenes[i];
					child.DominantGenes[i] = val;
				}
				else
					child.RecessiveGenes[i] = val;
			}
			return child;
		}
		public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
		{
			base.Load(valuesDictionary, idToEntityMap);
			//HeatLevel = valuesDictionary.GetValue<float>("HeatLevel");
			//ComponentEatPickableBehavior
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			base.Save(valuesDictionary, entityToIdMap);
			//valuesDictionary.SetValue("FireTimeRemaining", m_fireTimeRemaining);
			//valuesDictionary.SetValue("HeatLevel", HeatLevel);
		}
	}*/
}