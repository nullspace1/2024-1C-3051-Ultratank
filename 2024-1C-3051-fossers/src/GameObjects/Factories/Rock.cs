using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Managers;
using WarSteel.Scenes;

namespace WarSteel.Entities.Map;

public enum RockSize
{
    SMALL,
    MEDIUM,
    LARGE
}

public class RockFactory 
{
    private static string GetRockSizeStringValue(RockSize rockSize)
    {
        switch (rockSize)
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

    public static GameObject Generate(string[] tags, Vector3 position, RockSize size)
    {
        Model model = ContentRepoManager.Instance().GetModel("Map/" + GetRockSizeStringValue(size) + "Stone");
        GameObjectRenderer renderable = new Default(Color.DarkGray);
        GameObject rock = new(tags, new Transform(), model,renderable);
        rock.AddComponent(new StaticBody(new Collider(new ConvexShape(model, rock.Transform), (c) => { }), Vector3.Zero));
        rock.Transform.Position = position;
        return rock;
    }
}