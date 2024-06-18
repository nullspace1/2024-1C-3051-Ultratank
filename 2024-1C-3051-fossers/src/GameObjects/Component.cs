using Microsoft.Xna.Framework;
using WarSteel.Scenes;

namespace WarSteel.Entities;

public interface IComponent
{
    void OnUpdate(GameObject self, GameTime gameTime, Scene scene);
    void OnStart(GameObject self, Scene scene);
    void Destroy(GameObject self, Scene scene);
}