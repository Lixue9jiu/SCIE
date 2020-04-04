using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class Bullet2Block : SixDirectionalBlock, IElectricElementBlock
	{
		public enum BulletType
		{
			IronBullet,
			HandBullet,
			Shell,
			UShell,
			BucketBullet
		}

		public const int Index = 521;
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		protected static readonly string[] m_displayNames = { "LeadBullet", "RifleBullet", "Shell", "UShell" , "BucketBullet" };
		protected static readonly float[] m_sizes = { 0.5f, 0.5f, 1f, 1f ,0.6f};
		protected static readonly int[] m_textureSlots = { 177, 193, 193, 193 ,196};

		public override void Initialize()
		{
			m_standaloneBlockMesh.AppendMesh("Models/Rods", "SteelRod", Matrix.CreateScale(10f,5f,10f) * Matrix.CreateRotationX(1.6f)*Matrix.CreateTranslation(0, -0.5f, 0), Matrix.Identity, Color.Red);
			base.Initialize();

		}

		public override IEnumerable<int> GetCreativeValues() => new[] { Terrain.MakeBlockValue(521, 0, SetBulletType(0, BulletType.IronBullet)), Terrain.MakeBlockValue(521, 0, SetBulletType(0, BulletType.HandBullet)), Terrain.MakeBlockValue(521, 0, SetBulletType(0, BulletType.Shell)), Terrain.MakeBlockValue(521, 0, SetBulletType(0, BulletType.UShell)), Terrain.MakeBlockValue(521, 0, SetBulletType(0, BulletType.BucketBullet)), Index | 1 << 10 << 14, Index | 2 << 10 << 14 , Index | 3 << 10 << 14 };

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			if ((Terrain.ExtractData(value) >> 10) > 0)
			{
				base.DrawBlock(primitivesRenderer, value, color, size, ref matrix, environmentData);
				return;
			}
			int bulletType = (int)GetBulletType(Terrain.ExtractData(value));
			float size2 = (bulletType >= 0 && bulletType < m_sizes.Length) ? (size * m_sizes[bulletType]) : size;
			if (bulletType == 3)
				color = Color.LightGreen;
			ItemBlock.DrawFlatBlock(primitivesRenderer, value, size2, ref matrix, ItemBlock.Texture, color, false, environmentData);
		}

		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			if ((Terrain.ExtractData(value) >> 10) > 0)
				return base.GetPlacementValue(subsystemTerrain, componentMiner, value, raycastResult);
			return default;
		}

		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			if ((Terrain.ExtractData(value) >> 10) == 1)
				return Utils.Get("放置机");
			if ((Terrain.ExtractData(value) >> 10) == 2)
				return Utils.Get("Inserter");
			if ((Terrain.ExtractData(value) >> 10) == 3)
				return Utils.Get("SmartInserter");
			return m_displayNames[Terrain.ExtractData(value)];
		}

		public override string GetDescription(int value)
		{
			if ((Terrain.ExtractData(value) >> 10) == 1)
				return Utils.Get("放置机");
			if ((Terrain.ExtractData(value) >> 10) == 2)
				return Utils.Get("Inserter");
			if ((Terrain.ExtractData(value) >> 10) == 3)
				return Utils.Get("SmartInserter");
			return DefaultDescription;
		}

		public override string GetCategory(int value)
		{
			return (Terrain.ExtractData(value) >> 10) != 0 ? Utils.Get("机器") : DefaultCategory;
		}

		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return (Terrain.ExtractData(value) >> 10) != 0 ? Vector3.One : DefaultIconViewOffset;
		}

		public override int GetFaceTextureSlot(int face, int value)
		{
			if ((Terrain.ExtractData(value) >> 10) == 1)
				return face == GetDirection(value) ? 111 : 170;
			if ((Terrain.ExtractData(value) >> 10) == 2)
				return face == GetDirection(value) ? 131 : 170;
			if ((Terrain.ExtractData(value) >> 10) == 3)
				return face == GetDirection(value) ? 167 : 170;
			int bulletType = (int)GetBulletType(Terrain.ExtractData(value));
			return bulletType < 0 || bulletType >= m_textureSlots.Length ? 177 : m_textureSlots[bulletType];
		}

		public static BulletType GetBulletType(int data)
		{
			return (BulletType)(data & 0xF);
		}

		public static int SetBulletType(int data, BulletType bulletType)
		{
			return (data & -16) | (int)(bulletType & (BulletType)15);
		}

		public new ElectricElement CreateElectricElement(SubsystemElectricity subsystemElectricity, int value, int x, int y, int z)
		{
			if ((Terrain.ExtractData(value) >> 10) == 1)
				return new UnloaderElectricElement(subsystemElectricity, new Point3(x, y, z));
			if ((Terrain.ExtractData(value) >> 10) == 2)
				return new InserterElectricElement(subsystemElectricity, new Point3(x, y, z));
			if ((Terrain.ExtractData(value) >> 10) == 3)
				return new SInserterElectricElement(subsystemElectricity, new Point3(x, y, z));
			return null;
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
					n = MathUtils.Max(n, (int)MathUtils.Round(connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace) * 15f));
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
						placed = componentUnloader.Place();
				}
				if (placed)
					m_lastDispenseTime = SubsystemElectricity.SubsystemTime.GameTime;
			}
			return false;
		}
	}

	public class InserterElectricElement : MachineElectricElement
	{
		public InserterElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, point)
		{
		}

		public override bool Simulate()
		{
			int n = 0;
			for (int i = 0; i < Connections.Count; i++)
			{
				var connection = Connections[i];
				if (connection.ConnectorType != ElectricConnectorType.Output && connection.NeighborConnectorType != 0)
					n = MathUtils.Max(n, (int)MathUtils.Round(connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace) * 15f));
			}
			int y = Point.Y;
			//if ((n & 7) != 0 && (n & 7) != 7 && y >= 0 && y < 128)
			//	{
			//		int value = Utils.Terrain.GetCellValueFast(Point.X, y, Point.Z);
			//		Utils.SubsystemTerrain.ChangeCell(Point.X, y, Point.Z, Terrain.ReplaceData(value, FourDirectionalBlock.SetDirection(Terrain.ExtractData(value), (n & 7) - 1)));
			//	}
			if (n > 7 && SubsystemElectricity.SubsystemTime.GameTime - m_lastDispenseTime > 0.1)
			{
				ComponentBlockEntity blockEntity = Utils.SubsystemBlockEntities.GetBlockEntity(Point.X, y, Point.Z);
				//bool placed = false;
				if (blockEntity != null)
				{
					var componentUnloader = blockEntity.Entity.FindComponent<ComponentInserter>();
					if (componentUnloader != null)
						componentUnloader.Place();
				}
				//if (placed)
				m_lastDispenseTime = SubsystemElectricity.SubsystemTime.GameTime;
			}
			return false;
		}
	}

	public class SInserterElectricElement : MachineElectricElement
	{
		public SInserterElectricElement(SubsystemElectricity subsystemElectricity, Point3 point) : base(subsystemElectricity, point)
		{
		}

		public override bool Simulate()
		{
			int n = 0;
			for (int i = 0; i < Connections.Count; i++)
			{
				var connection = Connections[i];
				if (connection.ConnectorType != ElectricConnectorType.Output && connection.NeighborConnectorType != 0)
					n = MathUtils.Max(n, (int)MathUtils.Round(connection.NeighborElectricElement.GetOutputVoltage(connection.NeighborConnectorFace) * 15f));
			}
			int y = Point.Y;
			//if ((n & 7) != 0 && (n & 7) != 7 && y >= 0 && y < 128)
			//	{
			//		int value = Utils.Terrain.GetCellValueFast(Point.X, y, Point.Z);
			//		Utils.SubsystemTerrain.ChangeCell(Point.X, y, Point.Z, Terrain.ReplaceData(value, FourDirectionalBlock.SetDirection(Terrain.ExtractData(value), (n & 7) - 1)));
			//	}
			if (n > 7 && SubsystemElectricity.SubsystemTime.GameTime - m_lastDispenseTime > 0.1)
			{
				ComponentBlockEntity blockEntity = Utils.SubsystemBlockEntities.GetBlockEntity(Point.X, y, Point.Z);
				//bool placed = false;
				if (blockEntity != null)
				{
					var componentUnloader = blockEntity.Entity.FindComponent<ComponentSInserter>();
					if (componentUnloader != null)
						componentUnloader.Place();
				}
				//if (placed)
				m_lastDispenseTime = SubsystemElectricity.SubsystemTime.GameTime;
			}
			return false;
		}
	}
}