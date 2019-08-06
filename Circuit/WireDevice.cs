﻿using Engine;
using Engine.Graphics;
using System;

namespace Game
{
	public class WireElement : Device
	{
		public WireElement() : base(ElementType.Connector)
		{
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? paintColor = PaintableItemBlock.GetColor(Terrain.ExtractData(value));
			BlocksManager.DrawMeshBlock(primitivesRenderer, ElementBlock.WireBlock.m_standaloneBlockMesh, (paintColor.HasValue ? SubsystemPalette.GetColor(environmentData, paintColor) : 1.25f * WireBlock.WireColor) * color, 2f * size, ref matrix, environmentData);
		}
		public override int GetWeight(int voltage)
		{
			return 1;
		}
		public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
		{
			return Utils.Get("工业电线");
		}
	}
	public class WireDevice : WireElement
	{
		public BoundingBox[] m_collisionBoxesByFace = new BoundingBox[6];
		public WireDevice()
		{
			for (int i = 0; i < 6; i++)
			{
				Vector3 vector = CellFace.FaceToVector3(i);
				Vector3 v2;
				Vector3 v3;
				if (vector.X != 0f)
				{
					v2 = new Vector3(0f, 1f, 0f);
					v3 = new Vector3(0f, 0f, 1f);
				}
				else if (vector.Y != 0f)
				{
					v2 = new Vector3(1f, 0f, 0f);
					v3 = new Vector3(0f, 0f, 1f);
				}
				else
				{
					v2 = new Vector3(1f, 0f, 0f);
					v3 = new Vector3(0f, 1f, 0f);
				}
				Vector3 v = new Vector3(0.5f, 0.5f, 0.5f) - 0.5f * vector;
				Vector3 v4 = v - 0.5f * v2 - 0.5f * v3;
				Vector3 v5 = v + 0.5f * v2 + 0.5f * v3 + 0.05f * vector;
				m_collisionBoxesByFace[i] = new BoundingBox(Vector3.Min(v4, v5), Vector3.Max(v4, v5));
			}
		}
		public override int GetWeight(int voltage) => 1;
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			GenerateWireVertices(generator, value, x, y, z, 4, 0f, Vector2.Zero, geometry.SubsetOpaque);
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData
			{
				Value = raycastResult.CellFace.Face == 4 ? value : 0,
				CellFace = raycastResult.CellFace
			};
		}
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value)
		{
			var arr = new BoundingBox[6];
			for (int i = 0; i < 6; i++)
				if (i == 4)
					arr[i] = m_collisionBoxesByFace[i];
			return arr;
		}
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => true;
		public static void GenerateWireVertices(BlockGeometryGenerator generator, int value, int x, int y, int z, int mountingFace, float centerBoxSize, Vector2 centerOffset, TerrainGeometrySubset subset)
		{
			var terrain = generator.Terrain;
			Color color = WireBlock.WireColor;
			int num = Terrain.ExtractContents(value);
			if (num == ElementBlock.Index)
			{
				int? color2 = PaintableItemBlock.GetColor(Terrain.ExtractData(value));
				if (color2.HasValue)
					color = SubsystemPalette.GetColor(generator, color2);
			}
			float num3 = LightingManager.LightIntensityByLightValue[Terrain.ExtractLight(value)];
			Vector3 v = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f) - 0.5f * CellFace.FaceToVector3(mountingFace);
			Vector3 vector = CellFace.FaceToVector3(mountingFace);
			var v2 = new Vector2(0.9376f, 0.0001f);
			var v3 = new Vector2(0.03125f, 0.00550781237f);
			Point3 point = CellFace.FaceToPoint3(mountingFace);
			int cellContents = terrain.GetCellContents(x - point.X, y - point.Y, z - point.Z);
			bool flag = cellContents == 2 || cellContents == 7 || cellContents == 8 || cellContents == 6 || cellContents == 62 || cellContents == 72;
			Vector3 v4 = CellFace.FaceToVector3(SubsystemElectricity.GetConnectorFace(mountingFace, ElectricConnectorDirection.Top));
			Vector3 vector2 = CellFace.FaceToVector3(SubsystemElectricity.GetConnectorFace(mountingFace, ElectricConnectorDirection.Left)) * centerOffset.X + v4 * centerOffset.Y;
			int num4 = 0;
			var paths = new DynamicArray<ElectricConnectionPath>();
			ElementBlock.Block.GetAllConnectedNeighbors(terrain, ElementBlock.Block.GetDevice(x, y, z, value), mountingFace, paths);
			foreach (ElectricConnectionPath tmpConnectionPath in paths)
			{
				if ((num4 & (1 << tmpConnectionPath.ConnectorFace)) == 0)
				{
					ElectricConnectorDirection? connectorDirection = SubsystemElectricity.GetConnectorDirection(mountingFace, 0, tmpConnectionPath.ConnectorFace);
					if (centerOffset != Vector2.Zero || connectorDirection != ElectricConnectorDirection.In)
					{
						num4 |= 1 << tmpConnectionPath.ConnectorFace;
						Color color3 = color;
						if (num != ElementBlock.Index)
						{
							int cellValue = terrain.GetCellValue(x + tmpConnectionPath.NeighborOffsetX, y + tmpConnectionPath.NeighborOffsetY, z + tmpConnectionPath.NeighborOffsetZ);
							if (Terrain.ExtractContents(cellValue) == ElementBlock.Index)
							{
								int? color4 = PaintableItemBlock.GetColor(Terrain.ExtractData(cellValue));
								if (color4.HasValue)
									color3 = SubsystemPalette.GetColor(generator, color4);
							}
						}
						Vector3 vector3 = (connectorDirection != ElectricConnectorDirection.In) ? CellFace.FaceToVector3(tmpConnectionPath.ConnectorFace) : (-Vector3.Normalize(vector2));
						var vector4 = Vector3.Cross(vector, vector3);
						float s = (centerBoxSize >= 0f) ? MathUtils.Max(0.03125f, centerBoxSize / 2f) : (centerBoxSize / 2f);
						float num5 = (connectorDirection == ElectricConnectorDirection.In) ? 0.03125f : 0.5f;
						float num6 = (connectorDirection == ElectricConnectorDirection.In) ? 0f : ((tmpConnectionPath.ConnectorFace == tmpConnectionPath.NeighborFace) ? (num5 + 0.03125f) : ((tmpConnectionPath.ConnectorFace != CellFace.OppositeFace(tmpConnectionPath.NeighborFace)) ? num5 : (num5 - 0.03125f)));
						Vector3 vector5 = v - vector4 * 0.03125f + vector3 * s + vector2;
						Vector3 vector6 = v - vector4 * 0.03125f + vector3 * num5;
						Vector3 vector7 = v + vector4 * 0.03125f + vector3 * num5;
						Vector3 vector8 = v + vector4 * 0.03125f + vector3 * s + vector2;
						Vector3 vector9 = v + vector * 0.03125f + vector3 * (centerBoxSize / 2f) + vector2;
						Vector3 vector10 = v + vector * 0.03125f + vector3 * num6;
						if (flag && centerBoxSize == 0f)
						{
							Vector3 v5 = 0.25f * BlockGeometryGenerator.GetRandomWireOffset(0.5f * (vector5 + vector8), vector);
							vector5 += v5;
							vector8 += v5;
							vector9 += v5;
						}
						Vector2 vector11 = v2 + v3 * new Vector2(MathUtils.Max(0.0625f, centerBoxSize), 0f);
						Vector2 vector12 = v2 + v3 * new Vector2(num5 * 2f, 0f);
						Vector2 vector13 = v2 + v3 * new Vector2(num5 * 2f, 1f);
						Vector2 vector14 = v2 + v3 * new Vector2(MathUtils.Max(0.0625f, centerBoxSize), 1f);
						Vector2 vector15 = v2 + v3 * new Vector2(centerBoxSize, 0.5f);
						Vector2 vector16 = v2 + v3 * new Vector2(num6 * 2f, 0.5f);
						float num9 = 0.5f * (num3 + LightingManager.LightIntensityByLightValue[Terrain.ExtractLight(terrain.GetCellValue(x + tmpConnectionPath.NeighborOffsetX, y + tmpConnectionPath.NeighborOffsetY, z + tmpConnectionPath.NeighborOffsetZ))]);
						float num10 = LightingManager.CalculateLighting(-vector4);
						float num11 = LightingManager.CalculateLighting(vector4);
						float num12 = LightingManager.CalculateLighting(vector);
						float num13 = num10 * num3;
						float num14 = num10 * num9;
						float num15 = num11 * num9;
						float num16 = num11 * num3;
						float num17 = num12 * num3;
						float num18 = num12 * num9;
						var color5 = new Color((byte)(color3.R * num13), (byte)(color3.G * num13), (byte)(color3.B * num13));
						var color6 = new Color((byte)(color3.R * num14), (byte)(color3.G * num14), (byte)(color3.B * num14));
						var color7 = new Color((byte)(color3.R * num15), (byte)(color3.G * num15), (byte)(color3.B * num15));
						var color8 = new Color((byte)(color3.R * num16), (byte)(color3.G * num16), (byte)(color3.B * num16));
						var color9 = new Color((byte)(color3.R * num17), (byte)(color3.G * num17), (byte)(color3.B * num17));
						var color10 = new Color((byte)(color3.R * num18), (byte)(color3.G * num18), (byte)(color3.B * num18));
						int count = subset.Vertices.Count;
						subset.Vertices.Count += 6;
						TerrainVertex[] array = subset.Vertices.Array;
						BlockGeometryGenerator.SetupVertex(vector5.X, vector5.Y, vector5.Z, color5, vector11.X, vector11.Y, ref array[count]);
						BlockGeometryGenerator.SetupVertex(vector6.X, vector6.Y, vector6.Z, color6, vector12.X, vector12.Y, ref array[count + 1]);
						BlockGeometryGenerator.SetupVertex(vector7.X, vector7.Y, vector7.Z, color7, vector13.X, vector13.Y, ref array[count + 2]);
						BlockGeometryGenerator.SetupVertex(vector8.X, vector8.Y, vector8.Z, color8, vector14.X, vector14.Y, ref array[count + 3]);
						BlockGeometryGenerator.SetupVertex(vector9.X, vector9.Y, vector9.Z, color9, vector15.X, vector15.Y, ref array[count + 4]);
						BlockGeometryGenerator.SetupVertex(vector10.X, vector10.Y, vector10.Z, color10, vector16.X, vector16.Y, ref array[count + 5]);
						int count2 = subset.Indices.Count;
						subset.Indices.Count += (connectorDirection == ElectricConnectorDirection.In) ? 15 : 12;
						ushort[] array2 = subset.Indices.Array;
						array2[count2] = (ushort)count;
						array2[count2 + 1] = (ushort)(count + 5);
						array2[count2 + 2] = (ushort)(count + 1);
						array2[count2 + 3] = (ushort)(count + 5);
						array2[count2 + 4] = (ushort)count;
						array2[count2 + 5] = (ushort)(count + 4);
						array2[count2 + 6] = (ushort)(count + 4);
						array2[count2 + 7] = (ushort)(count + 2);
						array2[count2 + 8] = (ushort)(count + 5);
						array2[count2 + 9] = (ushort)(count + 2);
						array2[count2 + 10] = (ushort)(count + 4);
						array2[count2 + 11] = (ushort)(count + 3);
						if (connectorDirection == ElectricConnectorDirection.In)
						{
							array2[count2 + 12] = (ushort)(count + 2);
							array2[count2 + 13] = (ushort)(count + 1);
							array2[count2 + 14] = (ushort)(count + 5);
						}
					}
				}
			}
			if (centerBoxSize == 0f && (num4 != 0 || (num == ElementBlock.Index && (Terrain.ExtractData(value) & 1023) == 5)))
			{
				for (int i = 0; i < 6; i++)
				{
					if (i != mountingFace && i != CellFace.OppositeFace(mountingFace) && (num4 & (1 << i)) == 0)
					{
						Vector3 vector17 = CellFace.FaceToVector3(i);
						var v6 = Vector3.Cross(vector, vector17);
						Vector3 vector18 = v - v6 * 0.03125f + vector17 * 0.03125f;
						Vector3 vector19 = v + v6 * 0.03125f + vector17 * 0.03125f;
						Vector3 vector20 = v + vector * 0.03125f;
						if (flag)
						{
							Vector3 v7 = 0.25f * BlockGeometryGenerator.GetRandomWireOffset(0.5f * (vector18 + vector19), vector);
							vector18 += v7;
							vector19 += v7;
							vector20 += v7;
						}
						Vector2 vector21 = v2 + v3 * new Vector2(0.0625f, 0f);
						Vector2 vector22 = v2 + v3 * new Vector2(0.0625f, 1f);
						Vector2 vector23 = v2 + v3 * new Vector2(0f, 0.5f);
						float num19 = LightingManager.CalculateLighting(vector17) * num3;
						float num20 = LightingManager.CalculateLighting(vector) * num3;
						var color11 = new Color((byte)(color.R * num19), (byte)(color.G * num19), (byte)(color.B * num19));
						var color12 = new Color((byte)(color.R * num20), (byte)(color.G * num20), (byte)(color.B * num20));
						int count3 = subset.Vertices.Count;
						subset.Vertices.Count += 3;
						var array3 = subset.Vertices.Array;
						BlockGeometryGenerator.SetupVertex(vector18.X, vector18.Y, vector18.Z, color11, vector21.X, vector21.Y, ref array3[count3]);
						BlockGeometryGenerator.SetupVertex(vector19.X, vector19.Y, vector19.Z, color11, vector22.X, vector22.Y, ref array3[count3 + 1]);
						BlockGeometryGenerator.SetupVertex(vector20.X, vector20.Y, vector20.Z, color12, vector23.X, vector23.Y, ref array3[count3 + 2]);
						int count4 = subset.Indices.Count;
						subset.Indices.Count += 3;
						ushort[] array4 = subset.Indices.Array;
						array4[count4] = (ushort)count3;
						array4[count4 + 1] = (ushort)(count3 + 2);
						array4[count4 + 2] = (ushort)(count3 + 1);
					}
				}
			}
		}
	}
	/*public class SilverWireDevice : WireDevice
	{
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			int? paintColor = PaintableItemBlock.GetColor(Terrain.ExtractData(value));
			BlocksManager.DrawMeshBlock(primitivesRenderer, ElementBlock.WireBlock.m_standaloneBlockMesh, (paintColor.HasValue ? SubsystemPalette.GetColor(environmentData, paintColor) : Color.LightGray) * color, 2f * size, ref matrix, environmentData);
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			GenerateWireVertices(generator, value, x, y, z, 4, 0f, Vector2.Zero, geometry.SubsetOpaque);
		}
	}*/
	public class Switch : FixedDevice, IInteractiveBlock
	{
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public BlockMesh m_standaloneBlockMesh2 = new BlockMesh();
		public BoundingBox[] m_collisionBoxes;
		public Switch() : base("电闸", "电闸")
		{
			Model model = ContentManager.Get<Model>("Models/Switch");
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Body").ParentBone) * Matrix.CreateTranslation(.5f, 0, .5f);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Lever").ParentBone);
			ModelMeshPart meshPart = model.FindMesh("Body").MeshParts[0];
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, boneAbsoluteTransform, false, false, false, false, Color.White);
			m_standaloneBlockMesh2.AppendModelMeshPart(meshPart, boneAbsoluteTransform, false, false, false, false, Color.White);
			meshPart = model.FindMesh("Lever").MeshParts[0];
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, boneAbsoluteTransform2 * Matrix.CreateRotationX(MathUtils.DegToRad(30f)) * Matrix.CreateTranslation(.5f, 0f, .5f), false, false, false, false, Color.White);
			m_standaloneBlockMesh2.AppendModelMeshPart(meshPart, boneAbsoluteTransform2 * Matrix.CreateRotationX(MathUtils.DegToRad(-30f)) * Matrix.CreateTranslation(.5f, 0f, .5f), false, false, false, false, Color.White);
			m_collisionBoxes = new[] { m_standaloneBlockMesh.CalculateBoundingBox() };
		}

		public override void Simulate(ref int voltage)
		{
			if (!Powered) voltage = 0;
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * Color.LightGray, size * 2f, ref matrix, environmentData);
		}
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			Powered = (Terrain.ExtractData(value) & 16384) != 0;
			generator.GenerateMeshVertices(block, x, y, z, Powered ? m_standaloneBlockMesh : m_standaloneBlockMesh2, Color.LightGray, null, geometry.SubsetOpaque);
			WireDevice.GenerateWireVertices(generator, value, x, y, z, 4, 0f, Vector2.Zero, geometry.SubsetOpaque);
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => true;
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;

		public bool OnInteract(TerrainRaycastResult raycastResult, ComponentMiner componentMiner)
		{
			int x = Point.X,
				y = Point.Y,
				z = Point.Z;
			Utils.SubsystemTerrain.ChangeCell(x, y, z, Utils.Terrain.GetCellValueFast(x, y, z) ^ 16384 << 14);
			return true;
		}
		public override Vector3 GetIconBlockOffset(int value, DrawBlockEnvironmentData environmentData) => new Vector3 { Y = .5f };
	}

	public class ElectricFences : FixedDevice
	{
		public BlockMesh m_standaloneBlockMesh = new BlockMesh();
		public BlockMesh m_standaloneBlockMesh2 = new BlockMesh();
		public BoundingBox[] m_collisionBoxes;
		public bool Powered2;
		public SubsystemTime m_subsystemTime;

		public Vector3? m_closestSoundToPlay;

		public Dictionary<ComponentCreature, double> m_lastInjuryTimes = new Dictionary<ComponentCreature, double>();

		public ElectricFences() : base("电栅栏")
		{
			Model model = ContentManager.Get<Model>("Models/IronFence");
			
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Post").ParentBone);
			Matrix boneAbsoluteTransform2 = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Planks").ParentBone);
			ModelMeshPart meshPart = model.FindMesh("Post").MeshParts[0];
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, boneAbsoluteTransform, false, false, false, false, Color.White);
			m_standaloneBlockMesh2.AppendModelMeshPart(meshPart, boneAbsoluteTransform, false, false, false, false, Color.White);
			meshPart = model.FindMesh("Planks").MeshParts[0];
			m_standaloneBlockMesh.AppendModelMeshPart(meshPart, boneAbsoluteTransform2 * Matrix.CreateRotationX(MathUtils.DegToRad(30f)) * Matrix.CreateTranslation(.5f, 0f, .5f), false, false, false, false, Color.White);
			m_standaloneBlockMesh2.AppendModelMeshPart(meshPart, boneAbsoluteTransform2 * Matrix.CreateRotationX(MathUtils.DegToRad(-30f)) * Matrix.CreateTranslation(.5f, 0f, .5f), false, false, false, false, Color.White);
			m_collisionBoxes = new[] { m_standaloneBlockMesh.CalculateBoundingBox() };
		}

		
			public override void Simulate(ref int voltage)
		{
			if (Powered2 = voltage >= 120)
				voltage -= 120;
		}
	
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color * Color.LightGray, size * 2f, ref matrix, environmentData);
		}
		
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = value, CellFace = raycastResult.CellFace };
		}
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => true;
		public override BoundingBox[] GetCustomCollisionBoxes(SubsystemTerrain terrain, int value) => m_collisionBoxes;
		public override void OnCollide(CellFace cellFace, float velocity, ComponentBody componentBody)
		{
			int data = Terrain.ExtractData(base.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z));
			if (!SpikedPlankBlock.GetSpikesState(data))
			{
				return;
			}
			int mountingFace = SpikedPlankBlock.GetMountingFace(data);
			if (cellFace.Face != mountingFace)
			{
				return;
			}
			ComponentCreature componentCreature = componentBody.Entity.FindComponent<ComponentCreature>();
			if (componentCreature != null)
			{
				m_lastInjuryTimes.TryGetValue(componentCreature, out double value);
				if (m_subsystemTime.GameTime - value > 1.0)
				{
					m_lastInjuryTimes[componentCreature] = m_subsystemTime.GameTime;
					componentCreature.ComponentHealth.Injure(0.1f, null, ignoreInvulnerability: false, "Spiked by a trap");
				}
			}
		}


	}
	/*public abstract class DeviceElement : Element, IComparable<DeviceElement>, IEquatable<DeviceElement>
	{
		public int Voltage;

		protected DeviceElement(int voltage) : base(ElementType.Device)
		{
			Voltage = voltage;
		}
		public int CompareTo(DeviceElement other)
		{
			return Voltage.CompareTo(other.Voltage);
		}
		public bool Equals(DeviceElement other)
		{
			return base.Equals(other) && Voltage == other.Voltage;
		}
	}
	public abstract class Diode : Element
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