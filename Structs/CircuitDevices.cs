using Engine;
using Engine.Graphics;
using GameEntitySystem;
using System;
using System.Collections.Generic;
using TemplatesDatabase;

namespace Game
{
	public abstract class DeviceBlock : Device, IComparable<DeviceBlock>, IEquatable<DeviceBlock>
	{
		public string DefaultDisplayName;
		public string DefaultDescription;
		public int Voltage;

		protected DeviceBlock(int voltage, ElementType type = ElementType.Device | ElementType.Connector) : base(type)
		{
			Voltage = voltage;
		}
		public override string GetCraftingId()
		{
			return DefaultDisplayName;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return DefaultDisplayName;
		}
		public override string GetDescription(int value)
		{
			return DefaultDescription;
		}

		public int CompareTo(DeviceBlock other)
		{
			return Voltage.CompareTo(other.Voltage);
		}
		public bool Equals(DeviceBlock other)
		{
			return base.Equals(other) && Voltage == other.Voltage;
		}
	}
	public class Battery : DeviceBlock, IEquatable<Battery>
	{
		public readonly float Size;
		protected BoundingBox[] m_collisionBoxes;
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public int RemainCount = 10000;
		public Battery(int voltage, string modelName, string meshName, Matrix boneTransform, Matrix tcTransform, string description = "", string name = "", float size = 1f) : base(voltage, ElementType.Connector | ElementType.Container)
		{
			DefaultDisplayName = name;
			DefaultDescription = description;
			Size = size;
			Model model = ContentManager.Get<Model>(modelName);
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh(meshName, true).ParentBone);
			BlockMesh blockMesh = new BlockMesh();
			blockMesh.AppendModelMeshPart(model.FindMesh(meshName, true).MeshParts[0], boneAbsoluteTransform * boneTransform, false, false, false, false, Color.LightGray);
			blockMesh.TransformTextureCoordinates(tcTransform * Matrix.CreateScale(0.05f), -1);
			m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
			m_collisionBoxes = new BoundingBox[]
			{
				blockMesh.CalculateBoundingBox()
			};
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			generator.GenerateMeshVertices(block, x, y, z, m_standaloneBlockMesh, Color.White, null, geometry.SubsetOpaque);
			WireDevice.GenerateWireVertices(generator, value, x, y, z, 4, 0f, Vector2.Zero, geometry.SubsetOpaque);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, size * Size, ref matrix, environmentData);
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = value,
				CellFace = raycastResult.CellFace
			};
		}
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			return m_collisionBoxes;
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage == 0 && RemainCount > 0)
			{
				voltage = Voltage;
				RemainCount--;
			}
			else if(voltage >= Voltage)
			{
				voltage -= Voltage;
				RemainCount++;
			}
		}
		public bool Equals(Battery other)
		{
			return base.Equals(other) && RemainCount == other.RemainCount;
		}
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
		{
			return true;
		}
	}
	/*public class QCBattery : Battery, IEquatable<QCBattery>
	{
		public int Factor;
		protected QCBattery(int voltage) : base(voltage)
		{
			Factor = 2;
		}
		protected QCBattery(int voltage, int factor) : base(voltage)
		{
			Factor = factor;
		}
		public bool Equals(QCBattery other)
		{
			return base.Equals(other) && Factor == other.Factor;
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage == 0)
			{
				voltage = Voltage;
				RemainCount--;
			}
			else if (voltage >= Voltage)
				RemainCount += Factor;
		}
	}
	public class QCBattery12V : QCBattery
	{
		public QCBattery12V() : base(12)
		{
		}
	}
	public class ElectricFurnace : FixedDevice
	{
		public ElectricFurnace() : base(5)
		{
		}
	}*/
	public class EntityDevice<T> : FixedDevice, IBlockBehavior where T : Component
	{
		public T Component;
		public string Name;
		public EntityDevice(string name, int resistance) : base(resistance)
		{
			Name = name;
		}
		public void OnBlockAdded(SubsystemTerrain subsystemTerrain, int value, int oldValue)
		{
			if (oldValue == -1)
			{
				return;
			}
			var valuesDictionary = new ValuesDictionary();
			valuesDictionary.PopulateFromDatabaseObject(subsystemTerrain.Project.GameDatabase.Database.FindDatabaseObject(Name, subsystemTerrain.Project.GameDatabase.EntityTemplateType, true));
			valuesDictionary.GetValue<ValuesDictionary>("BlockEntity").SetValue("Coordinates", Point);
			Entity entity = subsystemTerrain.Project.CreateEntity(valuesDictionary);
			Component = entity.FindComponent<T>(true);
			subsystemTerrain.Project.AddEntity(entity);
		}
		public void OnBlockRemoved(SubsystemTerrain subsystemTerrain, int value, int newValue)
		{
			ComponentBlockEntity blockEntity = subsystemTerrain.Project.FindSubsystem<SubsystemBlockEntities>(true).GetBlockEntity(Point.X, Point.Y, Point.Z);
			if (blockEntity != null)
			{
				Vector3 position = new Vector3(Point) + new Vector3(0.5f);
				foreach (IInventory item in blockEntity.Entity.FindComponents<IInventory>())
				{
					item.DropAllItems(position);
				}
				subsystemTerrain.Project.RemoveEntity(blockEntity.Entity, true);
			}
		}
	}
	public class Fridge : EntityDevice<ComponentChestNew>
	{
		public Fridge() : base("ChestNew", 2000)
		{
		}
		public override void Simulate(ref int voltage)
		{
			Component.Powered = voltage > 110;
		}
		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4 || face == 5)
				return 107;
			switch (Terrain.ExtractData(value) >> 15)
			{
				case 0:
					if (face == 0)
					{
						return 106;
					}
					return 107;
				case 1:
					if (face == 1)
					{
						return 106;
					}
					return 107;
				case 2:
					if (face == 2)
					{
						return 106;
					}
					return 107;
				default:
					if (face == 3)
					{
						return 106;
					}
					return 107;
			}
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			CellFace cellFace = raycastResult.CellFace;
			Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
			float num = Vector3.Dot(forward, Vector3.UnitZ);
			float num2 = Vector3.Dot(forward, Vector3.UnitX);
			float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
			float num4 = Vector3.Dot(forward, -Vector3.UnitX);
			int data = 0;
			float max = MathUtils.Max(num, num2, num3, num4);
			if (num == max)
			{
				data = 2;
			}
			else if (num2 == max)
			{
				data = 3;
			}
			else if (num3 == max)
			{
				data = 0;
			}
			else if (num4 == max)
			{
				data = 1;
			}
			BlockPlacementData result = default(BlockPlacementData);
			result.Value = Terrain.ReplaceData(ElementBlock.Index, data << 15);
			result.CellFace = raycastResult.CellFace;
			return result;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return "Freezer";
		}
		public override string GetDescription(int value)
		{
			return "A Freezer is a good place to protect food that can delay the decay of it. It will hold up to 16 stacks of items.";
		}
	}

    public class Magnetizer : FixedDevice
    {
        public Magnetizer() : base(1000)
        {
        }
        public override int GetFaceTextureSlot(int face, int value)
        {
            if (face == 4 || face == 5)
                return 107;
            switch (Terrain.ExtractData(value) >> 15)
            {
                case 0:
                    if (face == 0)
                    {
                        return 239;
                    }
                    return 107;
                case 1:
                    if (face == 1)
                    {
                        return 239;
                    }
                    return 107;
                case 2:
                    if (face == 2)
                    {
                        return 239;
                    }
                    return 107;
                default:
                    if (face == 3)
                    {
                        return 239;
                    }
                    return 107;
            }
        }
        public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
        {
            return false;
        }
    

        public override void GenerateTerrainVertices(Block block,BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
        {
            generator.GenerateCubeVertices(block, value, x, y, z, Color.LightGray, geometry.OpaqueSubsetsByFace);
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, Color.LightGray, Color.LightGray, environmentData);
        }
        public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
        {
            Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
            float num = Vector3.Dot(forward, Vector3.UnitZ);
            float num2 = Vector3.Dot(forward, Vector3.UnitX);
            float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
            float num4 = Vector3.Dot(forward, -Vector3.UnitX);
            int data = 0;
            float max = MathUtils.Max(num, num2, num3, num4);
            if (num == max)
            {
                data = 2;
            }
            else if (num2 == max)
            {
                data = 3;
            }
            else if (num3 == max)
            {
                data = 0;
            }
            else if (num4 == max)
            {
                data = 1;
            }
            BlockPlacementData result = default(BlockPlacementData);
			result.Value = Terrain.ReplaceData(ElementBlock.Index, data << 15 | 2);
            result.CellFace = raycastResult.CellFace;
            return result;
        }
        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
        {
            return "Magnetizer";
        }
        public override string GetDescription(int value)
        {
            return "A Magnetizer is a device to create industrial magnet by melting steel ingot in a stronge magnetic field provided by wire.";
        }
    }

    public class Separator : FixedDevice
    {
        public Separator() : base(3000)
        {
        }
        public override int GetFaceTextureSlot(int face, int value)
        {
            if (face == 4 || face == 5)
                return 107;
            switch (Terrain.ExtractData(value) >> 15)
            {
                case 0:
                    if (face == 0)
                    {
                        return 240;
                    }
                    return 107;
                case 1:
                    if (face == 1)
                    {
                        return 240;
                    }
                    return 107;
                case 2:
                    if (face == 2)
                    {
                        return 240;
                    }
                    return 107;
                default:
                    if (face == 3)
                    {
                        return 240;
                    }
                    return 107;
            }
        }
        public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
        {
            return true;
        }
        public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
        {
            generator.GenerateCubeVertices(block, value, x, y, z, Color.LightGray, geometry.OpaqueSubsetsByFace);
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, Color.LightGray, Color.LightGray, environmentData);
        }
        public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
        {
            Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
            float num = Vector3.Dot(forward, Vector3.UnitZ);
            float num2 = Vector3.Dot(forward, Vector3.UnitX);
            float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
            float num4 = Vector3.Dot(forward, -Vector3.UnitX);
            int data = 0;
            float max = MathUtils.Max(num, num2, num3, num4);
            if (num == max)
            {
                data = 2;
            }
            else if (num2 == max)
            {
                data = 3;
            }
            else if (num3 == max)
            {
                data = 0;
            }
            else if (num4 == max)
            {
                data = 1;
            }
            BlockPlacementData result = default(BlockPlacementData);
            result.Value = Terrain.ReplaceData(ElementBlock.Index, data << 15|3);
            result.CellFace = raycastResult.CellFace;
            return result;
        }
        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
        {
            return "Separator";
        }
        public override string GetDescription(int value)
        {
            return "A Separator is a device to separate matarial by high frequency rotation, it is a shrinking version of centrifuge.";
        }
       
    }


    


    public class AirBlower : FixedDevice
    {
        public AirBlower() : base(3000)
        {
        }
        public override int GetFaceTextureSlot(int face, int value)
        {
            if (face == 4 || face == 5)
            { 
                return 107;
            
            }else
            {
                return 220;
            }
        }
        public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
        {
            Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
            float num = Vector3.Dot(forward, Vector3.UnitZ);
            float num2 = Vector3.Dot(forward, Vector3.UnitX);
            float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
            float num4 = Vector3.Dot(forward, -Vector3.UnitX);
            int data = 0;
            float max = MathUtils.Max(num, num2, num3, num4);
            if (num == max)
            {
                data = 2;
            }
            else if (num2 == max)
            {
                data = 3;
            }
            else if (num3 == max)
            {
                data = 0;
            }
            else if (num4 == max)
            {
                data = 1;
            }
            BlockPlacementData result = default(BlockPlacementData);
            result.Value = Terrain.ReplaceData(ElementBlock.Index, data << 15 | 4);
            result.CellFace = raycastResult.CellFace;
            return result;
        }
        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
        {
            return "AirBlower";
        }
        public override string GetDescription(int value)
        {
            return "AirBlower is a device to transfer air into some big machine that need a large amount of hot air.";
        }
        public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
        {
            generator.GenerateCubeVertices(block, value, x, y, z, Color.LightGray, geometry.OpaqueSubsetsByFace);
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, Color.LightGray, Color.LightGray, environmentData);
        }
        public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
        {
            return true;
        }

    }


    public class EFurnace : FixedDevice
    {
        public EFurnace() : base(8000)
        {
        }
        public override int GetFaceTextureSlot(int face, int value)
        {
            if (face == 4 || face == 5)
                return 107;
            switch (Terrain.ExtractData(value) >> 15)
            {
                case 0:
                    if (face == 0)
                    {
                        return 164;
                    }
                    return 107;
                case 1:
                    if (face == 1)
                    {
                        return 164;
                    }
                    return 107;
                case 2:
                    if (face == 2)
                    {
                        return 164;
                    }
                    return 107;
                default:
                    if (face == 3)
                    {
                        return 164;
                    }
                    return 107;
            }
        }
        public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value)
        {
            return true;
        }
        public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
        {
            generator.GenerateCubeVertices(block, value, x, y, z, Color.LightGray, geometry.OpaqueSubsetsByFace);
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, Color.LightGray, Color.LightGray, environmentData);
        }
        public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
        {
            Vector3 forward = Matrix.CreateFromQuaternion(componentMiner.ComponentCreature.ComponentCreatureModel.EyeRotation).Forward;
            float num = Vector3.Dot(forward, Vector3.UnitZ);
            float num2 = Vector3.Dot(forward, Vector3.UnitX);
            float num3 = Vector3.Dot(forward, -Vector3.UnitZ);
            float num4 = Vector3.Dot(forward, -Vector3.UnitX);
            int data = 0;
            float max = MathUtils.Max(num, num2, num3, num4);
            if (num == max)
            {
                data = 2;
            }
            else if (num2 == max)
            {
                data = 3;
            }
            else if (num3 == max)
            {
                data = 0;
            }
            else if (num4 == max)
            {
                data = 1;
            }
            BlockPlacementData result = default(BlockPlacementData);
            result.Value = Terrain.ReplaceData(ElementBlock.Index, data << 15 | 6);
            result.CellFace = raycastResult.CellFace;
            return result;
        }
        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
        {
            return "Electric Resistance Furnace";
        }
        public override string GetDescription(int value)
        {
            return "Electric Resistance Furnace is a device that can heat item by heating resistor, it can reach a high temperature but a large amount heat is needed";
        }
    }
    /*public abstract class Diode : Element
	{
		public int MaxVoltage;
		protected Diode() : base(ElementType.Connector)
		{
		}
		public override void Simulate(ref int voltage)
		{
			if (voltage < 0)
			{
				if (voltage > -MaxVoltage)
					voltage = 0;
				else MaxVoltage = 0;
			}
		}
	}
	public class DiodeDevice : Diode
	{
	}*/
}
