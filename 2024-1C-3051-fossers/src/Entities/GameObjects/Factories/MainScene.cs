using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using WarSteel.Common;
using WarSteel.Common.Shaders;
using WarSteel.Entities;
using WarSteel.Managers;
using WarSteel.Scenes;
using WarSteel.Scenes.Main;
using Vector3 = Microsoft.Xna.Framework.Vector3;

class MainSceneFactory
{

    private Scene _scene;
    private string PLAYER = "player";
    private string GROUND = "ground";

    public MainSceneFactory(Scene scene)
    {
        _scene = scene;
    }

    public GameObject PlayerTank(Vector3 position)
    {
        Transform tankTransform = new();
        Transform turretTransform = new(tankTransform, Vector3.Zero);
        Transform cannonTransform = new(turretTransform, Vector3.Zero);
        GameObject tank = new(new string[] { PLAYER }, tankTransform, ContentRepoManager.Instance().GetModel("Tanks/Panzer/Panzer"), new TankRenderable(turretTransform, cannonTransform));
        tank.AddComponent(new DynamicBody(new Collider(new BoxShape(200, 325, 450), (c) => { }), new Vector3(0, 100, 0), 200, 0.9f, 2f));
        tank.AddComponent(new PlayerControls(cannonTransform));
        tank.AddComponent(new TurretController(turretTransform, _scene.GetCamera(), 3f));
        tank.AddComponent(new CannonController(cannonTransform, _scene.GetCamera()));
        tank.Transform.Position = position;
        return tank;
    }

    public GameObject Tree(Vector3 position)
    {
        Model model = ContentRepoManager.Instance().GetModel("Map/SimpleTree");
        GameObjectRenderer renderer = new Default(0.5f, 0.5f, Color.Brown);
        GameObject tree = new(new string[] { GROUND }, new Transform(), model, renderer);
        tree.AddComponent(new StaticBody(new Collider(new BoxShape(1000, 200, 200), (c) => { }), new Vector3(0, 500, 0)));
        tree.Transform.Position = position;
        return tree;
    }

    public GameObject Rock(Vector3 position, string size)
    {
        Model model = ContentRepoManager.Instance().GetModel("Map/" + size + "Stone");
        GameObjectRenderer renderable = new Default(0.5f,0.5f,Color.DarkGray);
        GameObject rock = new(new string[] { GROUND }, new Transform(), model,renderable);
        rock.AddComponent(new StaticBody(new Collider(new ConvexShape(model, rock.Transform), (c) => { }), Vector3.Zero));
        rock.Transform.Position = position;
        return rock;
    }

    public GameObject Bush(Vector3 position)
    {
        Model model = ContentRepoManager.Instance().GetModel("Map/Bush");
        GameObjectRenderer renderer = new Default(0.5f, 0.5f, Color.Red);
        GameObject bush = new(new string[] { GROUND }, new Transform(), model,renderer);
        bush.Transform.Position = position;
        return bush;
    }

    public GameObject Ground(Vector3 position)
    {
        Model model = ContentRepoManager.Instance().GetModel("Map/Ground");
        GameObjectRenderer renderer = new Default(0.5f, 0.5f, Color.Gray);
        GameObject ground = new(new string[] { GROUND }, new Transform(),model, renderer);
        ground.Transform.Position = position;
        ground.AddComponent(new StaticBody(new Collider(new BoxShape(100, 100000, 100000), (e) => { }), Vector3.Up * 70));
        return ground;
    }



}