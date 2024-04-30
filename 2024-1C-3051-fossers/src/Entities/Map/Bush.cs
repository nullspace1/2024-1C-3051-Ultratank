using System;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Managers;

namespace WarSteel.Entities;


class Bush : Entity
{
    public Bush() : base("bush", Array.Empty<string>(), new Transform(), Array.Empty<Component>())
    {
    }

    public override void LoadContent()
    {
        Model model = ContentRepoManager.Instance().GetModel("Map/Bush");
        Renderable = new Renderable(model);

        base.LoadContent();
    }
}