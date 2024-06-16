using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using WarSteel.Scenes;


public class UI
{
    public Vector2 Position;
    public float Height;
    public float Width;
    private UIRenderer _renderable;
    private ClickAction _action;
    private bool _toDestroy = false;

    public UI(Vector2 position, float width, float height, UIRenderer renderable, ClickAction action)
    {
        _renderable = renderable;
        _action = action;
        Position = position;
        Height = height;
        Width = width;
    }

    public UI(Vector2 position, UIRenderer renderable)
    {
        Position = position;
        _renderable = renderable;
    }

    public UI(Vector2 position, float width, float height, UIRenderer renderable)
    {
        _renderable = renderable;
        Position = position;
        Height = height;
        Width = width;
        _action = (scene, ui) => {};
    }

    public void AddAction(ClickAction action){
        _action = action;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _renderable?.Draw(spriteBatch, this);
    }

    private bool BeingClicked(MouseState state)
    {
        Vector2 position = Position;

        return state.LeftButton == ButtonState.Pressed && state.X <= position.X + Width / 2
               && state.X >= position.X - Width / 2
               && state.Y <= position.Y + Height / 2
               && state.Y >= position.Y - Height / 2;
    }


    public virtual void Update(Scene scene, GameTime time)
    {
        MouseState state = Mouse.GetState();
        if (BeingClicked(state))
        {
            _action.Invoke(scene,this);
        }
    }

    public void Destroy()
    {
        _toDestroy = true;
    }

    public bool IsDestroyed()
    {
        return _toDestroy;
    }

}

public interface UIRenderer
{
    public void Draw(SpriteBatch spriteBatch, UI ui);
}