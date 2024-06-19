using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Entities.Map;
using WarSteel.Managers;
using WarSteel.Scenes;
using WarSteel.Scenes.Main;
using WarSteel.Utils;
using Vector3 = Microsoft.Xna.Framework.Vector3;

class MainSceneFactory
{

    private Scene _scene;
    private string GROUND = "ground";

    public MainSceneFactory(Scene scene)
    {
        _scene = scene;
    }

    public Player PlayerTank(Vector3 position)
    {
        return new Player(_scene, position);
    }

    public GameObject Tree(Vector3 position)
    {
        Model model = ContentRepoManager.Instance().GetModel("Map/SimpleTree");
        GameObjectRenderer renderer = new Default(Color.Brown);
        GameObject tree = new(new string[] { GROUND }, new Transform(), model, renderer);
        Random rand = new();
        tree.Transform.Dimensions = tree.Transform.Dimensions * (float)Crypto.GetRandomNumber(0.9, 1.5);
        tree.Transform.RotateEuler(tree.Transform.Up * (float)Crypto.GetRandomNumber(0, 360));
        tree.AddComponent(new StaticBody(new Collider(new BoxShape(1000, 200, 200), (c) => { }), new Vector3(0, 500, 0)));
        tree.Transform.Position = position;
        return tree;
    }

    public GameObject Rock(Vector3 position, RockSize size)
    {
        return RockFactory.Generate(new string[] { GROUND }, position, size);
    }

    public GameObject Bush(Vector3 position)
    {
        Model model = ContentRepoManager.Instance().GetModel("Map/Bush");
        GameObjectRenderer renderer = new Default(Color.Red);
        GameObject bush = new(new string[] { GROUND }, new Transform(), model, renderer);
        bush.Transform.Position = position;
        return bush;
    }

    public GameObject Ground(Vector3 position)
    {
        Model model = ContentRepoManager.Instance().GetModel("Map/Ground");
        GameObjectRenderer renderer = new Default(Color.Gray);
        GameObject ground = new(new string[] { GROUND }, new Transform(), model, renderer);
        ground.Transform.Position = position;
        ground.AddComponent(new StaticBody(new Collider(new BoxShape(100, 100000, 100000), (e) => { }), Vector3.Up * 70));
        return ground;
    }

}