using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;
using WarSteel.Scenes.SceneProcessors;


public class Shadow : GameObjectRenderer
{

    public int _size;
    private static readonly string DEPTH = "DepthPass";

    public Shadow(int size) : base(ContentRepoManager.Instance().GetEffect("Default"))
    {
        _size = size;

    }


    public override void Draw(GameObject gameObject, Scene scene)
    {
        _effect.CurrentTechnique = _effect.Techniques[DEPTH];
        LightProcessor processor = scene.GetSceneProcessor<LightProcessor>();

        foreach (var modelMesh in gameObject.Model.Meshes)
        {
            foreach (var part in modelMesh.MeshParts)
            {
                part.Effect = _effect;

                _effect.Parameters["WorldViewProjection"].SetValue(gameObject.Transform.LocalToWorldMatrix(modelMesh.ParentBone.Transform)  * processor.GetLightViewProjection());
            }

            modelMesh.Draw();
        }
    }
}