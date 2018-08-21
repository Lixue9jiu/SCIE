using Engine;

namespace Game
{
	public class Train : Item
    {
		public override int GetFaceTextureSlot(int face, int value)
		{
			return 2;
		}
		public override Vector3 GetIconViewOffset(int value, DrawBlockEnvironmentData environmentData)
		{
			return Vector3.One;
		}
	}
}
