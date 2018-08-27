using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;


namespace Game
{
	public class SteelBlock : CubeBlock
	{
		public const int Index = 510;
        [Serializable]
        public enum Type
        {
            BasicMachineCase,
            SteelBlock,
            SecondryMachineCase,
            FireBrickWall
        }
        public override IEnumerable<int> GetCreativeValues()
        {
            if (DefaultCreativeData < 0)
            {
                return base.GetCreativeValues();
            }
            var list = new List<int>(4);
            for (int i = 0; i < list.Capacity; i++)
            {
                list.Add(Terrain.ReplaceData(Index, i));
            }
            return list;
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            switch (GetType(value))
            {
                //case Type.SteelDrill:
                //break;
                case Type.BasicMachineCase:
                    color = Color.White;
                    break;
                case Type.SecondryMachineCase:
                    color = Color.LightGray;
                    break;
                case Type.SteelBlock:
                    color = Color.LightGray;
                    break;
                case Type.FireBrickWall:
                    color = new Color(255,153,18);
                    break;
            }
            BlocksManager.DrawCubeBlock(primitivesRenderer, value, new Vector3(size), ref matrix, color, color, environmentData);
        }

        public override BlockPlacementData GetPlacementValue(SubsystemTerrain subsystemTerrain, ComponentMiner componentMiner, int value, TerrainRaycastResult raycastResult)
        {

            return new BlockPlacementData
            {
                //Value = GetType(value) == Type.SteelBlock ? Terrain.ReplaceData(Index, 1) : 0,
                Value = Terrain.ReplaceData(Index, Terrain.ExtractData(value) & 0xF),
                CellFace = raycastResult.CellFace
            };
        }
        public override void GenerateTerrainVertices(BlockGeometryGenerator generator, TerrainGeometrySubsets geometry, int value, int x, int y, int z)
        {
            Color color = Color.White;
            switch (GetType(value))
            {
                //case Type.SteelDrill:
                //break;
                case Type.BasicMachineCase:
                    color = Color.White;
                    break;
                case Type.SecondryMachineCase:
                    color = Color.LightGray;
                    break;
                case Type.SteelBlock:
                    color = Color.LightGray;
                    break;
                case Type.FireBrickWall:
                    color = new Color(255, 153, 18);
                    break;
            }
            generator.GenerateCubeVertices(this, value, x, y, z, color, geometry.OpaqueSubsetsByFace);
        }
        public static Type GetType(int value)
        {
            return (Type)(Terrain.ExtractData(value) & 0xF);
        }
        public static int SetType(int value, Type type)
        {
            return Terrain.ReplaceData(value, (Terrain.ExtractData(value) & -16) | ((int)type & 0xF));
        }
        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
        {
            return GetType(value).ToString();
        }
        public override string GetDescription(int value)
        {
            switch (GetType(value))
            {
                case Type.BasicMachineCase:
                    return "The basic case of some machine. Very heavy and durable. Extremely resilient to digging and explosions. Can be crafted from multiple ironplate or steel ingots.";
                case Type.SecondryMachineCase:
                    return "The Secondry case of some machine or device. Very heavy and durable. Extremely resilient to digging and explosions. Can be crafted from multiple steelplate.";
                case Type.SteelBlock:
                    return "The block of steel. Very heavy and durable. Extremely resilient to digging and explosions. Can be crafted from multiple steel ingots.";
                case Type.FireBrickWall:
                    return "Fire Brick wall can be made by combining several fire bricks together and binding them with mortar. It is a versatile, strong and good looking industrial material.";
            }
            return string.Empty;
        }
        public override void GetDropValues(SubsystemTerrain subsystemTerrain, int oldValue, int newValue, int toolLevel, List<BlockDropValue> dropValues, out bool showDebris)
		{
			showDebris = true;
			dropValues.Add(new BlockDropValue
			{
				Value = oldValue,
				Count = 1
			});
		}
        public override int GetFaceTextureSlot(int face, int value)
        {
            switch (GetType(value))
            {
                case Type.BasicMachineCase:
                    return 107;
                case Type.SecondryMachineCase:
                    return 107;
                case Type.SteelBlock:
                    return 180;
                case Type.FireBrickWall:
                    return 39;
            }
            return 0;
        }
	}
}
