using Engine;
using Engine.Graphics;

namespace Game
{
	public class Musket3Block : Musket2Block
	{
		public new const int Index = 535;

        public static int SetBulletNum(int data)
        {
			return (data & -241) | ((data & 0xF) << 4);
		}

		public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
		{
			BlocksManager.DrawMeshBlock(primitivesRenderer, GetHammerState(Terrain.ExtractData(value)) ? m_standaloneBlockMeshLoaded : m_standaloneBlockMeshUnloaded, color * Color.LightGray, 2f * size, ref matrix, environmentData);
		}
	}
}