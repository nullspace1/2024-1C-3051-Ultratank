using Microsoft.Xna.Framework;
using WarSteel.Scenes;
using WarSteel.Scenes.SceneProcessors;
using Color = Microsoft.Xna.Framework.Color;

namespace WarSteel.Entities;

class LightComponent : IComponent {

    private Color _color;


    public Light CurrentLightSource;

    public LightComponent(Color light){
        _color = light;
    
    }

    public void OnStart(GameObject self, Scene scene){

        CurrentLightSource = new Light(self.Transform,_color);
        self.Renderer = new Common.Shaders.Renderer(Color.White);
        scene.GetSceneProcessor<LightProcessor>().AddLight(CurrentLightSource);
    }

    public void OnUpdate(GameObject self, GameTime gameTime, Scene scene){
    }

    public void Destroy(GameObject self, Scene scene){
        scene.GetSceneProcessor<LightProcessor>().RemoveLight(CurrentLightSource);
    }

}
