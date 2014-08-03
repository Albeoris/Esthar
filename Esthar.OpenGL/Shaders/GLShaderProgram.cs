using System;
using Esthar.Core;
using OpenTK.Graphics.OpenGL;

namespace Esthar.OpenGL
{
    public class GLShaderProgram : IDisposable
    {
        protected static IDisposable UnuseProgramAction = new DisposableAction(() => GL.UseProgram(0));

        public readonly int Id;

        public GLShaderProgram()
        {
            using (GLService.AcquireContext())
                Id = GL.CreateProgram();
        }

        public void Dispose()
        {
            using (GLService.AcquireContext())
                GL.DeleteProgram(Id);
        }

        public void Link()
        {
            using (GLService.AcquireContext())
            {
                GL.LinkProgram(Id);

                int result;
                GL.GetProgram(Id, GetProgramParameterName.LinkStatus, out result);
                if (result != 1)
                {
                    string logInfo;
                    GL.GetProgramInfoLog(Id, out logInfo);
                    throw Exceptions.CreateException("Program {0} log: {1}", Id, logInfo);
                }
            }
        }
    }
}