
using System;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Managers;

namespace WarSteel.Entities.Map;

public enum RockSize
{
    SMALL,
    MEDIUM,
    LARGE
}




public class Rock : Entity
{
    private RockSize _rockSize;

    public Rock(RockSize size) : base("rock", Array.Empty<string>(), new Transform(), Array.Empty<Component>())
    {
        _rockSize = size;
    }

    private string GetRockSizeStringValue()
    {
        switch (_rockSize)
        {
            case RockSize.SMALL:
                return "Small";
            case RockSize.MEDIUM:
                return "Medium";
            case RockSize.LARGE:
                return "Large";
            default:
                return "Large";
        }
    }

    public override void LoadContent()
    {
        Model model = ContentRepoManager.Instance().GetModel("Map/" + GetRockSizeStringValue() + "Stone");
        Renderable = new Renderable(model);

        base.LoadContent();
    }
}