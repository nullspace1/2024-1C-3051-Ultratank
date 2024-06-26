<<<<<<< HEAD
using System.Collections.Generic;
=======
>>>>>>> main
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace WarSteel.Managers;

public class ContentRepoManager
{
    private const string ContentFolder3D = "Models/";
    private const string ContentFolderEffects = "Effects/";
    private const string ContentFolderAudio = "Audio/";
    private const string ContentFolderSpriteFonts = "SpriteFonts/";
    private const string ContentFolderTextures = "Textures/";

    private Dictionary<string, Model> modelCache = new();
    private Dictionary<string, Effect> effectCache = new();
    private Dictionary<string, Texture2D> textureCache = new();
    private Dictionary<string, TextureCube> textureCubeCache = new();
    private Dictionary<string, SpriteFont> spriteFontCache = new();
    private Dictionary<string, SoundEffect> soundEffectCache = new();
    private Dictionary<string, Song> songCache = new();

    public RenderTarget2D GlobalRenderTarget;

    private ContentManager _manager;

    private static ContentRepoManager _INSTANCE = null;

    public ContentManager Manager
    {
        get => _manager;
    }

    public static void SetUpInstance(ContentManager manager, GraphicsDevice device)
    {
        _INSTANCE = new ContentRepoManager
        {
            _manager = manager,
            GlobalRenderTarget = new RenderTarget2D(
             device,
             device.Viewport.Width,
             device.Viewport.Height,
             false, // Mipmap
             SurfaceFormat.Color, // Surface format for color rendering
             DepthFormat.Depth24, // Depth buffer format
             0, // Mip levels
             RenderTargetUsage.PlatformContents)
        };
    }

    public static ContentRepoManager Instance() => _INSTANCE;

    public Effect GetEffect(string effect)
    {
        if (!effectCache.TryGetValue(effect, out var loadedEffect))
        {
            loadedEffect = _manager.Load<Effect>(ContentFolderEffects + effect);
            effectCache[effect] = loadedEffect;
        }
        return loadedEffect;
    }

    public Model GetModel(string model)
    {
        if (!modelCache.TryGetValue(model, out var loadedModel))
        {
            loadedModel = _manager.Load<Model>(ContentFolder3D + model);
            modelCache[model] = loadedModel;
        }
        return loadedModel;
    }

    public Texture2D GetTexture(string texture)
    {
        if (!textureCache.TryGetValue(texture, out var loadedTexture))
        {
            loadedTexture = _manager.Load<Texture2D>(ContentFolderTextures + texture);
            textureCache[texture] = loadedTexture;
        }
        return loadedTexture;
    }

    public TextureCube GetTextureCube(string texture)
    {
        if (!textureCubeCache.TryGetValue(texture, out var loadedTextureCube))
        {
            loadedTextureCube = _manager.Load<TextureCube>(ContentFolderTextures + texture);
            textureCubeCache[texture] = loadedTextureCube;
        }
        return loadedTextureCube;
    }

    public SpriteFont GetSpriteFont(string font)
    {
        if (!spriteFontCache.TryGetValue(font, out var loadedSpriteFont))
        {
            loadedSpriteFont = _manager.Load<SpriteFont>(ContentFolderSpriteFonts + font);
            spriteFontCache[font] = loadedSpriteFont;
        }
        return loadedSpriteFont;
    }

    public SoundEffect GetSoundEffect(string audio)
    {
        if (!soundEffectCache.TryGetValue(audio, out var loadedSoundEffect))
        {
            loadedSoundEffect = _manager.Load<SoundEffect>(ContentFolderAudio + audio);
            soundEffectCache[audio] = loadedSoundEffect;
        }
        return loadedSoundEffect;
    }

    public Song GetSong(string song)
    {
        if (!songCache.TryGetValue(song, out var loadedSong))
        {
            loadedSong = _manager.Load<Song>(ContentFolderAudio + song);
            songCache[song] = loadedSong;
        }
        return loadedSong;
    }

    public SoundEffect GetSoundEffect(string audio)
    {
        return _manager.Load<SoundEffect>(ContentFolderAudio + audio);
    }

    public Song GetSong(string song)
    {
        return _manager.Load<Song>(ContentFolderAudio + song);
    }
}
