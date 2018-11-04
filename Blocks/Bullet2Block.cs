using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class Bullet2Block : SixDirectionalBlock, IElectricElementBlock
	{
		public enum BulletType
		{
			IronBullet
		}

		public const int Index = 521;

		protected static readonly string[] m_displayNames = new string[] { "IronFixedBullet" };
		protected static readonly float[] m_sizes = new float[] { 1f };
		protected static readonly int[] m_textureSlots = new int[] { 209 };
		public override IEnumerable<int> GetCreativeValues()
		{
			return new int[] { Index, Index | 1 << 10 << 14 };
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			if ((Terrain.ExtractData(value) >> 10) != 0)
			{
				base.DrawBlock(primitivesRenderer, value, color, size, ref matrix, environmentData);
				return;
			}
			int bulletType = (int)GetBulletType(Terrain.ExtractData(value));
			float size2 = (bulletType >= 0 && bulletType < m_sizes.Length) ? (size * m_sizes[bulletType]) : size;
			CustomTextureItem.DrawFlatBlock(primitivesRenderer, value, size2, ref matrix, CustomTextureItem.Texture, color, false, environmentData);
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return (Terrain.ExtractData(value) >> 10) != 0 ? base.GetPlacementValue(subsystemTerrain, componentMiner, value, raycastResult) : default;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return (Terrain.ExtractData(value) >> 10) != 0 ? "Unloader" : DefaultDisplayName;
		}
		public override string GetDescription(int value)
		{
			return (Terrain.ExtractData(value) >> 10) != 0 ? "Unloader" : DefaultDescription;
		}
		public override string GetCategory(int value)
		{
			return (Terrain.ExtractData(value) >> 10) != 0 ? "Items" : DefaultCategory;
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return (Terrain.ExtractData(value) >> 10) != 0 ? Vector3.One : DefaultIconViewOffset;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			if ((Terrain.ExtractData(value) >> 10) != 0)
			{
				return face == GetDirection(value) ? 111 : 170;
			}
			int bulletType = (int)GetBulletType(Terrain.ExtractData(value));
			return bulletType < 0 || bulletType >= m_textureSlots.Length ? 209 : m_textureSlots[bulletType];
		}

		public static BulletType GetBulletType(int data)
		{
			return (BulletType)(data & 0xF);
		}

		/*public static int SetBulletType(int data, BulletType bulletType)
		{
			return (data & -16) | (int)(bulletType & (BulletType)15);
		}*/

		public new ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			return new UnloaderElectricElement(subsystemElectricity, new Point3(x, y, z));
		}
		public override float GetProjectilePower(int value)
		{
			return 10f;
		}
	}
	public class UnloaderElectricElement : MachineElectricElement
	{
		public UnloaderElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, point)
		{
		}

		public override bool Simulate()
		{
			int n = 0;
			for (int i = 0; i < Connections.Count; i++)
			{
				var connection = Connections[i];
				if (connection.ConnectorType != ElectricConnectorType.Output && connection.NeighborConnectorType != 0)
				{
					n = MathUtils.Max(n, (int)MathUtils.Round(connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace) * 15f));
				}
			}
			int y = Point.Y;
			if ((n & 7) != 0 && (n & 7) != 7 && y >= 0 && y < 128)
			{
				int value = Utils.Terrain.GetCellValueFast(Point.X, y, Point.Z);
				Utils.SubsystemTerrain.ChangeCell(Point.X, y, Point.Z, Terrain.ReplaceData(value, FourDirectionalBlock.SetDirection(Terrain.ExtractData(value), (n & 7) - 1)));
			}
			if (n > 7 && SubsystemElectricity.SubsystemTime.GameTime - m_lastDispenseTime > 0.1)
			{
				ComponentBlockEntity blockEntity = Utils.SubsystemBlockEntities.GetBlockEntity(Point.X, y, Point.Z);
				bool placed = false;
				if (blockEntity != null)
				{
					var componentUnloader = blockEntity.Entity.FindComponent<ComponentUnloader>();
					if (componentUnloader != null)
					{
						placed = componentUnloader.Place();
					}
				}
				if (placed)
				{
					m_lastDispenseTime = SubsystemElectricity.SubsystemTime.GameTime;
				}
			}
			return false;
		}
	}
}