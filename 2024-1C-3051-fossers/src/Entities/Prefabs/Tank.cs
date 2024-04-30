using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Managers;

namespace WarSteel.Entities;

public class Tank : Entity
{
    public Tank(string name) : base(name, Array.Empty<string>(), new Transform(), new Component[]{new RigidBody(
        Vector3.Zero,Vector3.Zero,1,Matrix.Identity
    )})
    {

    }

    public override void LoadContent()
    {
        Model model = ContentRepoManager.Instance().GetModel("Tanks/Panzer/Panzer");
        Shader texture = new TextureShader(ContentRepoManager.Instance().GetTexture("Tanks/T90/textures_mod/hullA"));
        Renderable = new Renderable(model);
        Renderable.AddShader("texture", texture);

        base.LoadContent();
    }
}