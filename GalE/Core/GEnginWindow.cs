using static SDL2.SDL;

using GalE.Util.Math;
using GalE.Renderer;

namespace GalE.Core;

public unsafe class GEnginWindow(
    bool vsync = false, bool show = true, bool fullcreen = false,
    bool vulkan = true, bool openGL = false,
    string title = "", Vector2? position = null, Vector2? size = null) : GEnginObject
{
    public event Action RandererEvent;

    private readonly CheckInit Inited = new();
    private GEnginRenderer? EnginRenderer = null;

    internal IntPtr SDLWindow;
    internal SDL_Event SDLevent;
    internal GEnginBase GEngin { get; private set; }

    public bool Runing { get; private set; } = false;

    public bool Vsync { get; private set; } = vsync;
    public bool Show { get; private set; } = show;
    public bool FullScreen { get; private set; } = fullcreen;

    public bool Vulkan { get; init; } = vulkan;
    public bool OpenGL { get; init; } = openGL;

    public string WindowTitle { get; private set; } = title;
    public Vector2? WindowPostion { get; private set; } = position;
    public Vector2? WindowSize { get; private set; } = size;

    public void Init(GEnginBase Engin)
    {
        Inited.Check(true);

        GEngin = Engin;

        if (SDL_Init(SDL_INIT_EVERYTHING) != 0)
        {
            throw new ArgumentException("Engin Init Error: SDL Init Error");
        }

        //SDL_Vulkan_LoadLibrary();

        WindowPostion ??= new(SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED);
        WindowSize ??= new(0, 0);

        SDLWindow = SDL_CreateWindow(WindowTitle,
                                     WindowPostion.x, WindowPostion.y,
                                     WindowSize.x, WindowSize.y,
                                     SDL_WindowFlags.SDL_WINDOW_SHOWN |
                                     (Vulkan ? SDL_WindowFlags.SDL_WINDOW_VULKAN : (OpenGL ? SDL_WindowFlags.SDL_WINDOW_OPENGL : 0)));
        if (Vulkan)
        {
            EnginRenderer = new GEnginVulkanRenderer();
            GEnginWindow window = this;
            EnginRenderer.Init(window);
        }

        if (OpenGL)
        {
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 4);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 6);
            SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_PROFILE_MASK, SDL_GLprofile.SDL_GL_CONTEXT_PROFILE_CORE);
            SDL_GL_CreateContext(SDLWindow);
        }

        Runing = true;
        Inited.Init();
    }

    public void Tick()
    {
        Inited.Check();
        while (Convert.ToBoolean(SDL_PollEvent(out SDLevent)))
        {
            RandererEvent();
            switch (SDLevent.type)
            {
                case SDL_EventType.SDL_QUIT:
                    Runing = false;
                    break;

                default:
                    break;
                }
        }
    }


    public void SetWindowTitle(string title) => SDL_SetWindowTitle(SDLWindow, title);

    public void SetWindowSize(Vector2 size) => SDL_SetWindowSize(SDLWindow, size.x, size.y);

    public void SetWindowSize(int width, int height) => SDL_SetWindowSize(SDLWindow, width, height);

    public new void Dispose()
    {
        Inited.Check();
        Runing = false;
        EnginRenderer.Dispose();
        SDL_DestroyWindow(SDLWindow);
        SDL_Quit();
        base.Dispose();
    }
}