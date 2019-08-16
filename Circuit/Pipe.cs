using Engine;
using Engine.Graphics;
using System.Collections.Generic;

namespace Game
{
	public class Pipe : CubeDevice
	{
		public BlockMesh[] Meshes = new BlockMesh[63];
		public Pipe(int id) : base("Pipe" + id.ToString())
		{
			var model = ContentManager.Get<Model>("Models/Battery");
			var meshes = new BlockMesh[6];
			int i;
			ModelMeshPart meshPart = model.FindMesh("Battery").MeshParts[0];
			Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Battery").ParentBone);
			for (i = 0; i < 6; i++)
			{
				var blockMesh = new BlockMesh();
				var m = Matrix.CreateTranslation(0f, (i & 1) != 0 ? 0.3f : -0.3f, 0f);
				//m = i >= 4 ? (i != 4 ? (Matrix.CreateRotationX(MathUtils.PI)) : Matrix.CreateTranslation(0.5f, 0f, 0.5f)) : (Matrix.CreateRotationX(MathUtils.PI / 2f) * Matrix.CreateTranslation(0f, 0f, -0.5f) * Matrix.CreateRotationY(i * MathUtils.PI / 2f) * Matrix.CreateTranslation(0.5f, 0.5f, 0.5f));
				switch (i)
				{
					case 0:
						m *= Matrix.CreateRotationX(MathUtils.PI / 2); break;
					case 1:
						m *= Matrix.CreateRotationX(MathUtils.PI / 2) * Matrix.CreateTranslation(0.3f, 0f, 0f); break;
					case 4:
						m *= Matrix.CreateRotationX(MathUtils.PI * 3 / 2) * Matrix.CreateTranslation(0.3f, 0f, 0f); break;
					case 5:
						m *= Matrix.CreateRotationZ(MathUtils.PI * 3 / 2); break;
				}
				blockMesh.AppendModelMeshPart(meshPart, boneAbsoluteTransform * m * Matrix.CreateScale(.66f) * Matrix.CreateTranslation(new Vector3(0.5f)), false, false, false, false, Color.White);
				blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation(-2 / 16f, 4 / 16f, 0f));
				meshes[i] = blockMesh;
			}
			for (i = 0; i < 63; i++)
			{
				Meshes[i] = new BlockMesh();
				for (int j = 0; j < 6; j++)
					if (((i + 1) & 1 << j) != 0)
						Meshes[i].AppendBlockMesh(meshes[j]);
			}
		}
		public override Device Create(Point3 p, int value)
		{
			Type = ElementType.Supply | (ElementType)(GetType(Terrain.ExtractData(value)) << 20);
			return this;
		}
		public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
		{
			return new BlockPlacementData { Value = Terrain.ReplaceLight(value, 0), CellFace = raycastResult.CellFace };
		}
		public override string GetCraftingId() => DefaultDisplayName;
		public override void GenerateTerrainVertices(Block block, BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
		{
			int data = Terrain.ExtractData(value);
			generator.GenerateMeshVertices(block, x, y, z, Meshes[GetType(data)], SubsystemPalette.GetColor(generator, PaintableItemBlock.GetColor(data)), null, geometry.SubsetOpaque);
		}
		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			value = Terrain.ExtractData(value);
			color *= SubsystemPalette.GetColor(environmentData, PaintableItemBlock.GetColor(value));
			BlocksManager.DrawMeshBlock(primitivesRenderer, Meshes[GetType(value)], color, size, ref matrix, environmentData);
		}
		public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			dropValues.Add(new BlockDropValue { Value = Terrain.ReplaceLight(oldValue, 0), Count = 1 });
		}
		public static int GetType(int data) => ((data >> (15 - 3)) | ((data & 1023) - 13)) + 1;
		public override bool IsFaceTransparent(SubsystemTerrain subsystemTerrain, int face, int value) => true;
	}
	/*public class Diode : Device
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
	}*/
}