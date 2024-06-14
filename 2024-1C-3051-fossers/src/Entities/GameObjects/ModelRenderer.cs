using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Scenes;

namespace WarSteel.Common;

public abstract class GameObjectRenderer
{
    protected Effect _effect;

    public GameObjectRenderer(Effect effect){
        _effect = effect;
    }

    public abstract void Draw(GameObject gameObject, Scene scene);

    
}
