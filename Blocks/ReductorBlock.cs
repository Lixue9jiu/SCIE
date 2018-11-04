using Engine;
using System.Collections.Generic;

namespace Game
{
	public class ReductorBlock : CubeBlock
	{
		public const int Index = 537;

		
		public override int GetFaceTextureSlot(int face, int value)
		{
			return 144;
		}
	}
}
