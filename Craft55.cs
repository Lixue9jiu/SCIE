using System;

namespace Game
{
	// Token: 0x02000139 RID: 313
	public class CraftingRecipe
	{
		// Token: 0x04000792 RID: 1938
		public const int MaxSize = 6;

		// Token: 0x04000793 RID: 1939
		public string Description;

		// Token: 0x04000794 RID: 1940
		public string[] Ingredients = new string[36];

		// Token: 0x04000795 RID: 1941
		public int RemainsCount;

		// Token: 0x04000796 RID: 1942
		public int RemainsValue;

		// Token: 0x04000797 RID: 1943
		public float RequiredHeatLevel;

		// Token: 0x04000798 RID: 1944
		public int ResultCount;

		// Token: 0x04000799 RID: 1945
		public int ResultValue;
	}
}
