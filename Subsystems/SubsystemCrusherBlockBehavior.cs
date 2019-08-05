using Engine;
using System.Collections.Generic;

namespace Game
{
	public class SubsystemCrusherBlockBehavior : SubsystemBlockBehavior
	{
		public override int[] HandledBlocks => new[] { CrusherBlock.Index };

		public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
		{
			if (!ComponentEngine.IsPowered(Utils.Terrain, cellFace.X, cellFace.Y, cellFace.Z) || worldItem.Velocity.Length() < 20f)
				return;
			int l;
			Vector3 v = CellFace.FaceToVector3(cellFace.Face);
			var position = new Vector3(cellFace.Point) + new Vector3(0.5f) - 0.75f * v;
			if (Terrain.ExtractContents(worldItem.Value) == 6)
			{
				for (l = 0; l < 5; l++)
					Utils.SubsystemProjectiles.FireProjectile(79, position, -20f * v, Vector3.Zero, null);
				worldItem.ToRemove = true;
			}
			else
			{
				var list = new List<BlockDropValue>(8);
				BlocksManager.Blocks[Terrain.ExtractContents(worldItem.Value)].GetDropValues(SubsystemTerrain, worldItem.Value, 0, 3, list, out bool s);
				for (l = 0; l < list.Count; l++)
				{
					var blockDropValue = list[l];
					for (int i = 0; i <= blockDropValue.Count; i++)
						Utils.SubsystemProjectiles.FireProjectile(blockDropValue.Value, position, -20f * v, Vector3.Zero, null);
				}
				worldItem.ToRemove = true;
			}
		}
	}

    public class SubsystemSpinnerBlockBehavior : SubsystemBlockBehavior
    {
        public override int[] HandledBlocks => new[] { SpinnerBlock.Index };

        
        public override void OnHitByProjectile(CellFace cellFace, WorldItem worldItem)
        {
            if (!ComponentEngine.IsPowered(Utils.Terrain, cellFace.X, cellFace.Y, cellFace.Z))
                return;
            int l;
            int num1 = base.SubsystemTerrain.Terrain.GetCellValue(cellFace.X, cellFace.Y, cellFace.Z);
            int num2 = base.SubsystemTerrain.Terrain.GetCellContents(cellFace.X, cellFace.Y, cellFace.Z);
            Vector3 v = CellFace.FaceToVector3(cellFace.Face);
            var position = new Vector3(cellFace.Point) + new Vector3(0.5f) - 0.75f * v;
            if (Terrain.ExtractContents(worldItem.Value) == CottonWadBlock.Index)
            {
               // if ((num1 + 1) % 10 == 3)
               // {
               //     int value = Terrain.ReplaceData(num2, 0);
               //     base.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y, cellFace.Z, value);
                    Utils.SubsystemProjectiles.FireProjectile(StringBlock.Index, position, -1f * v, Vector3.Zero, null);
                    worldItem.ToRemove = true;
              //  }else
              //  {
               //     int value = Terrain.ReplaceData(num2, num1+1);
              //      base.SubsystemTerrain.ChangeCell(cellFace.X, cellFace.Y, cellFace.Z, value);
              //      Log.Information(num1);
              //      worldItem.ToRemove = true;
              //  }
            }else if (Terrain.ExtractContents(worldItem.Value) == StringBlock.Index)
            {
                
                Utils.SubsystemProjectiles.FireProjectile(CanvasBlock.Index, position, -1f * v, Vector3.Zero, null);
                worldItem.ToRemove = true;
            }
            else if (Terrain.ExtractContents(worldItem.Value) == CanvasBlock.Index)
            {
               
                Utils.SubsystemProjectiles.FireProjectile(CarpetBlock.Index, position, -1f * v, Vector3.Zero, null);
                worldItem.ToRemove = true;
            }
            else
            {
                return;
            }
            
            
        }
    }
}