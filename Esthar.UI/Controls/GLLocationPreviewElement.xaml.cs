using System;
using System.ComponentModel;
using System.Windows;
using Esthar.Core;
using Esthar.OpenGL;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Esthar.UI
{
    /// <summary>
    /// Логика взаимодействия для GLElement.xaml
    /// </summary>
    public partial class GLLocationPreviewElement
    {
        public GLLocationPreviewElement()
        {
            InitializeComponent();
            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            GLControlElement.VSync = false;
            GLControlElement.Load += OnGLControlElementLoaded;
            GLControlElement.Resize += OnGLControlElementResize;
        }

        private void OnGLControlElementLoaded(object sender, EventArgs e)
        {
            Window window = (Window)this.GetRootElement();
            window.Activated += OnWindowActivated;
            window.Deactivated += OnWindowDeactivated;
        }

        private void OnWindowActivated(object sender, EventArgs eventArgs)
        {
            GLService.SubscribeControl(GLControlHost);
            GLLocationRenderer.DrawEvent = GLService.RegisterDrawMethod(DrawPreview);
            ConfigViewport();
        }

        private void OnWindowDeactivated(object sender, EventArgs eventArgs)
        {
            GLService.UnregisterDrawMethod(DrawPreview);
            GLService.UnsubscribeControl(GLControlHost);
        }

        private void OnGLControlElementResize(object sender, EventArgs e)
        {
            ConfigViewport();
        }

        private void ConfigViewport()
        {
            using (GLService.AcquireContext())
            {
                GL.ClearColor(Color4.DimGray);

                int w = GLControlElement.Width;
                int h = GLControlElement.Height;
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, w, h, 0, -1, 1);
                GL.Viewport(0, 0, w, h);

                GLLocationRenderer.DrawEvent.NullSafeSet();
            }
        }

        private void DrawPreview()
        {
            using (GLService.AcquireContext(GLControlElement.WindowInfo))
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                GLLocationRenderer.Draw();
            }
        }
    }
}