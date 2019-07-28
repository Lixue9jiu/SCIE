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
			int value;
			while (true)
			{
				if (num >= SlotsCount - 1)
					return false;
				value = GetSlotValue(num);
				int slotCount = GetSlotCount(num);
				if (value != 0 && slotCount > 0)
					break;
				num++;
			}
			int face = FourDirectionalBlock.GetDirection(cellValue);
			for (num = RemoveSlotItems(num, 1); num-- > 0;)
			{
				var position = new Vector3(coordinates) + new Vector3(0.5f);
				Vector3 vector = CellFace.FaceToVector3(face);
				if (!Place(position + vector, face, value) && DispenseItem)
				{
					Vector3 vector2 = position + 0.6f * vector;
					Utils.SubsystemPickables.AddPickable(value, 1, vector2, 1.8f * (vector + m_random.Vector3(0.2f, false)), null);
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
			this.LoadItems(valuesDictionary);
			m_componentBlockEntity = Entity.FindComponent<ComponentBlockEntity>(true);
			SubsystemPlayers = Project.FindSubsystem<SubsystemPlayers>(true);
		}

		public override void Save(ValuesDictionary valuesDictionary, EntityToIdMap entityToIdMap)
		{
			this.SaveItems(valuesDictionary);
		}
	}
}