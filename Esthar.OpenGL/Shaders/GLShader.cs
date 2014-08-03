using System;
using System.IO;
using Esthar.Core;
using OpenTK.Graphics.OpenGL;

namespace Esthar.OpenGL
{
    public sealed class GLShader : IDisposable
    {
        public static GLShader CompileShaderFromFile(string fileName, ShaderType type)
        {
            using (GLService.AcquireContext())
            {
                int id = GL.CreateShader(type);
                using (StreamReader sr = new StreamReader(fileName))
                    GL.ShaderSource(id, sr.ReadToEnd());

                string logInfo;
                GL.CompileShader(id);
                GL.GetShaderInfoLog(id, out logInfo);

                if (logInfo.Length > 0 && !logInfo.Contains("hardware"))
                    throw Exceptions.CreateException("Compile shader failed! Log:{0}", logInfo);


                return new GLShader(id, type);
            }
        }

        public static GLShader CompileShaderFromSource(string source, ShaderType type)
        {
            using (GLService.AcquireContext())
            {
                int id = GL.CreateShader(type);
                GL.ShaderSource(id, source);

                string logInfo;
                GL.CompileShader(id);
                GL.GetShaderInfoLog(id, out logInfo);

                if (logInfo.Length > 0 && !logInfo.Contains("hardware"))
                    throw Exceptions.CreateException("Compile shader failed! Log:{0}", logInfo);

                return new GLShader(id, type);
            }
        }

        public readonly int Id;
        public readonly ShaderType Type;

        private GLShader(int id, ShaderType type)
        {
            Id = id;
            Type = type;
        }

        public void Dispose()
        {
            using (GLService.AcquireContext())
                GL.DeleteShader(Id);
        }
    }
}