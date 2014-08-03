using System;
using System.Threading;
using Esthar.Core;
using OpenTK.Graphics.OpenGL;

namespace Esthar.OpenGL
{
    public static class GLLocationRenderer
    {
        private static readonly object BackgroundLock = new object();
        private static readonly object PagesLock = new object();

        private static CancellationTokenSource _cancelationToken;
        private static GLTexture _background, _cursorTexture, _disabledCursorTexture;
        private static string[] _pages;
        private static GLFontSymbolRenderer _fontRenderer;
        
        public static int WindowX, WindowY, FirstAnswer, LastAnswer, DefaultAnswer, CancelAnswer;
        public static AutoResetEvent DrawEvent;

        public static void Initialize(GLFontSymbolRenderer fontRenderer, GLTexture cursorTexture, GLTexture disabledCursorTexture)
        {
            _fontRenderer = fontRenderer;
            _cursorTexture = cursorTexture;
            _disabledCursorTexture = disabledCursorTexture;
        }

        public static void SetBackground(GLTexture background)
        {
            lock (BackgroundLock)
                _background = background;

            //GLService.InvalidateViewports();
            DrawEvent.Set();
        }

        public static void SetBackground(GLTextureReader reader)
        {
            if (reader == null)
            {
                SetBackground((GLTexture)null);
                return;
            }

            ThreadPool.QueueUserWorkItem(state => SetBackgroundInternal(reader));
        }

        public static void SetText(string[] pages)
        {
            lock (PagesLock)
            {
                _pages = pages;
                int pagesCont = Math.Max(1, _pages == null ? 0 : _pages.Length);
                GLService.SetViewportDesiredHeight(pagesCont * GLLocationPageRenderer.ScreenHeight);
            }

            DrawEvent.Set();
            //GLService.InvalidateViewports();
        }

        private static async void SetBackgroundInternal(GLTextureReader reader)
        {
            using (reader)
            {
                CancellationTokenSource cancelationToken = new CancellationTokenSource();
                lock (BackgroundLock)
                {
                    if (_cancelationToken != null)
                    {
                        _cancelationToken.Cancel();
                        _cancelationToken.Dispose();
                    }
                    _cancelationToken = cancelationToken;
                }

                GLTexture texture = await reader.ReadTextureAsync(cancelationToken.Token);
                lock (BackgroundLock)
                {
                    if (cancelationToken.IsCancellationRequested)
                        texture.SafeDispose();
                    else
                        SetBackground(texture);
                }
            }
        }

        public static void Draw()
        {
            lock (BackgroundLock)
                lock (PagesLock)
                {
                    GLLocationPagesRendererContext context = new GLLocationPagesRendererContext
                    {
                        WindowX = WindowX, 
                        WindowY = WindowY,
                        FirstAnswer = FirstAnswer,
                        LastAnswer = LastAnswer,
                        DefaultAnswer = DefaultAnswer,
                        CancelAnswer = CancelAnswer,
                        SymbolRenderer = _fontRenderer,
                        Background = _background,
                        CursorImage = _cursorTexture,
                        DisabledCursorImage = _disabledCursorTexture,
                    };
                    
                    if (_pages.IsNullOrEmpty())
                    {
                        GLLocationPageRenderer renderer = new GLLocationPageRenderer(0, context, null);
                        renderer.DrawBackground();
                    }
                    else
                    {
                        GLLocationPageRenderer[] renderers = new GLLocationPageRenderer[_pages.Length];
                        for (int i = 0; i < renderers.Length; i++)
                        {
                            renderers[i] = new GLLocationPageRenderer(i, context, _pages[i]);
                            renderers[i].DrawBackground();
                            renderers[i].CalcWindowSize();
                        }
                                      
                        foreach (GLLocationPageRenderer renderer in renderers)
                            renderer.DrawWindow();

                        GL.Enable(EnableCap.Blend);
                        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                        foreach (GLLocationPageRenderer renderer in renderers)
                            renderer.DrawText();
                        GL.Disable(EnableCap.Blend);
                    }
                }
        }
    }
}