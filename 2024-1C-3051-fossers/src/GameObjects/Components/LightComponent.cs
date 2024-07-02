using Microsoft.Xna.Framework;
using WarSteel.Scenes;
using WarSteel.Scenes.SceneProcessors;
using Color = Microsoft.Xna.Framework.Color;

namespace WarSteel.Entities;

class LightComponent : IComponent
{

    private Color _color;

    public float DecayFactor;


    public Light CurrentLightSource;

    public LightComponent(Color light, float decayFactor = 0)
    {
        _color = light;
        DecayFactor = decayFactor;
    }

    public void OnStart(GameObject self, Scene scene)
    {
        CurrentLightSource = new Light(self.Transform, _color);
        scene.GetSceneProcessor<LightProcessor>().AddLight(CurrentLightSource);
    }

    public void OnUpdate(GameObject self, GameTime gameTime, Scene scene)
    {
        _color *= 1 - DecayFactor * (float)gameTime.ElapsedGameTime.TotalSeconds;
        CurrentLightSource.Color = _color;

    }

    public void Destroy(GameObject self, Scene scene)
    {
        scene.GetSceneProcessor<LightProcessor>().RemoveLight(CurrentLightSource);
    }

}
