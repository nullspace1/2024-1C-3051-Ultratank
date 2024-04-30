using Microsoft.Xna.Framework;
using WarSteel.Scenes;

namespace WarSteel.Entities;

public interface Component
{
    public string Id();

    void UpdateEntity(Entity self, GameTime gameTime, Scene scene);
}