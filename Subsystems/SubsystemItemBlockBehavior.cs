using Engine;
namespace Game
{
	public class SubsystemItemBlockBehavior : SubsystemThrowableBlockBehavior
	{
		public override int[] HandledBlocks => new int[] { ItemBlock.Index };

		public override bool OnAim(Vector3 start, Vector3 direction, ComponentMiner componentMiner, AimState state)
		{
			int value = componentMiner.ActiveBlockValue;
			return BlocksManager.Blocks[Terrain.ExtractContents(value)] is ItemBlock item && item.GetItem(ref value) is OreChunkBlock && base.OnAim(start, direction, componentMiner, state);
		}
	}
}
