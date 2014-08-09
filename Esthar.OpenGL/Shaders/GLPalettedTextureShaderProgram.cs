using System;
using Esthar.Core;
using OpenTK.Graphics.OpenGL;

namespace Esthar.OpenGL
{
    public sealed class GLPalettedTextureShaderProgram : GLShaderProgram
    {
        public GLPalettedTextureShaderProgram()
        {
            using (DisposableAction insurance = new DisposableAction(Dispose))
            {
                using (GLService.AcquireContext())
                using (GLShader fs = GLShader.CompileShaderFromSource(FragmentShaderSource, ShaderType.FragmentShader))
                using (GLShader vs = GLShader.CompileShaderFromSource(VertexShaderSource, ShaderType.VertexShader))
                {
                    GL.AttachShader(Id, fs.Id);
                    GL.AttachShader(Id, vs.Id);
                    Link();
                }

                insurance.Cancel();
            }
        }

        public IDisposable Use(GLTexture texture, GLTexture palette, int paletteIndex)
        {
            using (GLService.AcquireContext())
            {
                GL.UseProgram(Id);

                GL.Uniform1(GL.GetUniformLocation(Id, "paletteIndex"), paletteIndex);

                GL.Uniform1(GL.GetUniformLocation(Id, "palette"), 1);
                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, palette.Id);

                GL.Uniform1(GL.GetUniformLocation(Id, "texture"), 0);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, texture.Id);
            }

            return UnuseProgramAction;
        }

        private const string FragmentShaderSource = @"uniform sampler2D texture;
uniform sampler2D palette;
uniform int paletteIndex;

void main()
{
	int colorIndex = int(texture2D(texture, gl_TexCoord[0].xy, 0.0).r * 255);
	gl_FragColor = texelFetch(palette, ivec2(colorIndex, paletteIndex), 0);
}";

        private const string VertexShaderSource = @"void main() {
    gl_Position = ftransform();
    gl_TexCoord[0] = gl_MultiTexCoord0;
}";
    }
}