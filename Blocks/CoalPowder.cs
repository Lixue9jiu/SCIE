using Engine;
using Engine.Graphics;
using System;
using System.Collections.Generic;

namespace Game
{
	public class CoalPowderBlock : FlatBlock, IFuel
	{
		public const int Index = 517;
        [Serializable]
        public enum Type
        {
            Coal,
            CokeCoal
        }
        public override IEnumerable<int> GetCreativeValues()
        {
            if (DefaultCreativeData < 0)
            {
                return base.GetCreativeValues();
            }
            var list = new List<int>(2);
            for (int i = 0; i < 2; i++)
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
                case Type.Coal:
                    color = new Color(28,28,28);
                    break;
                //case Type.IronTubularis:
                //break;
                case Type.CokeCoal:
                    color = Color.DarkGray;
                    break;
            }
            BlocksManager.DrawFlatBlock(primitivesRenderer, value, size, ref matrix, null, color, false, environmentData);
        }
        public static Type GetType(int value)
        {
            return (Type)(Terrain.ExtractData(value) & 0xF);
        }
        public static int SetType(int value, Type type)
        {
            return Terrain.ReplaceData(value, (Terrain.ExtractData(value) & -16) | ((int)type & 0xF));
        }
        public override string GetDescription(int value)
        {
            switch (GetType(value))
            {
                case Type.Coal:
                    return "Coalpowder is black powder obtained by crushing coal chunk. It can be used to be fuel.";
                case Type.CokeCoal:
                    return "Coke Coal Powder looks like silver powder obtained by crushing coke coal. It can be used to be fuel or the reductant agent in the industrial field.";
            }
            return string.Empty;
        }
        public override string GetDisplayName(SubsystemTerrain subsystemTerrain, int value)
        {
            return GetType(value).ToString()+"Powder";
        }
        public float GetHeatLevel(int value)
        {
            switch (GetType(value))
            {
                case Type.Coal:
                    return 1700f;
                case Type.CokeCoal:
                    return 2000f;
            }
            return 0f;
        }
        public float GetFuelFireDuration(int value)
        {
            switch (GetType(value))
            {
                case Type.Coal:
                    return 60f;
                case Type.CokeCoal:
                    return 100f;
            }
            return 0f;
        }
	}
}
