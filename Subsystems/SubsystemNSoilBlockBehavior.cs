namespace Game
{
	public class SubsystemNSoilBlockBehavior : SubsystemSoilBlockBehavior, IUpdateable
	{
		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (cellFace.Y < 0 || cellFace.Y > 127 || worldItem.Velocity.Length() < 10f)
				return;
			int value = Terrain.ExtractContents(worldItem.Value);
			if (value == 102)
			{
				for (int i = -1; i < 2; i++)
				{
					for (int j = -1; j < 2; j++)
					{
						int cellValueFast = SubsystemTerrain.Terrain.GetCellValueFast(cellFace.X + i, cellFace.Y, cellFace.Z + j);
						if (Terrain.ExtractContents(cellValueFast) == 168)
						{
							SubsystemTerrain.ChangeCell(cellFace.X + i, cellFace.Y, cellFace.Z + j, 168 | SoilBlock.SetNitrogen(Terrain.ExtractData(cellValueFast), 3) << 14, true);
							worldItem.ToRemove = true;
						}
					}
				}
			}
			else if (value == SeedsBlock.Index && SubsystemTerrain.Terrain.GetCellContentsFast(cellFace.X, cellFace.Y + 1, cellFace.Z) == 0)
			{
				value = 0;
				switch (worldItem.Value)
				{
				case 16557:
					value = 20 | FlowerBlock.SetIsSmall(0, true) << 14;
					break;
				case 173:
					value = 19 | TallGrassBlock.SetIsSmall(0, true) << 14;
					break;
				case 49325:
					value = 25 | FlowerBlock.SetIsSmall(0, true) << 14;
					break;
				case 32941:
					value = 24 | FlowerBlock.SetIsSmall(0, true) << 14;
					break;
				case 82093:
					value = 174 | RyeBlock.SetSize(RyeBlock.SetIsWild(0, false), 0) << 14;
					break;
				case 65709:
					value = 174 | RyeBlock.SetSize(RyeBlock.SetIsWild(0, false), 0) << 14;
					break;
				case 114861:
					value = 131 | BasePumpkinBlock.SetSize(BasePumpkinBlock.SetIsDead(0, false), 0) << 14;
					break;
				case 98477:
					value = 204 | CottonBlock.SetSize(CottonBlock.SetIsWild(0, false), 0) << 14;
					break;
				}
				if (value != 0)
				{
					SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y + 1, cellFace.Z, value, true);
					worldItem.ToRemove = true;
				}
			}
		}
	}
}