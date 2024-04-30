using System.Collections.Generic;
using WarSteel.Scenes;

public enum ScenesNames
{
    MAIN
}

public class SceneManager
{
    private Dictionary<ScenesNames, Scene> _scenes = new();
    private ScenesNames _currentSceneName;

    public void AddScene(ScenesNames name, Scene scene)
    {
        _scenes.Add(name, scene);
    }

    public ScenesNames CurrentSceneKey() => _currentSceneName;
    public Scene CurrentScene() => _scenes[_currentSceneName];

    public void SetCurrentScene(ScenesNames name)
    {
        if (_scenes.ContainsKey(name))
        {
            _currentSceneName = name;
        }

    }


}