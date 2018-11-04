using Engine;
using System.Collections.Generic;

namespace Game
{
	public class FractionatingTowerBlock : CubeBlock
	{
		public const int Index = 536;

		

		public override int GetFaceTextureSlot(int face, int value)
		{
			if (face == 4)
			{
				return 107;
			}
			return 112;
		}
	}
}
