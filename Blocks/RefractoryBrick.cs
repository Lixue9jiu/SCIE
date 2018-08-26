using System;
using Engine;
using Engine.Graphics;

namespace Game
{
	public class RefractoryBrick : MeshItem
	{
        public RefractoryBrick() : base("A refractory brick is a block of refractory ceramic material used in lining furnaces,kilns,advanced firebox, and fireplaces. It is bulit to withstand high temperature, but also have a low thermal conductivity for great energy efficiency.")
        {
            Model model = ContentManager.Get<Model>("Models/Brick");
            Matrix boneAbsoluteTransform = BlockMesh.GetBoneAbsoluteTransform(model.FindMesh("Brick", true).ParentBone);
            BlockMesh blockMesh = new BlockMesh();
            blockMesh.AppendModelMeshPart(model.FindMesh("Brick", true).MeshParts[0], boneAbsoluteTransform * 1.4f * Matrix.CreateTranslation(0f, -0.02f, 0f), false, false, false, false, Color.White);
            blockMesh.TransformTextureCoordinates(Matrix.CreateTranslation((float)(-32 % 16) / 16f, (float)(-32 / 16) / 16f, 0f), -1);
            m_standaloneBlockMesh.AppendBlockMesh(blockMesh);
        }
        public override void DrawBlock(PrimitivesRenderer3D primitivesRenderer, int value, Color color, float size, ref Matrix matrix, DrawBlockEnvironmentData environmentData)
        {
            color = new Color(255,153,18);
            BlocksManager.DrawMeshBlock(primitivesRenderer, m_standaloneBlockMesh, color, 2f * size, ref matrix, environmentData);
        }
	}
}
