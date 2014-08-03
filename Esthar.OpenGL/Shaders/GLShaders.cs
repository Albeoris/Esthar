using System;

namespace Esthar.OpenGL
{
    public static class GLShaders
    {
        private static readonly Lazy<GLPalettedTextureShaderProgram> PalettedTextureInstance = new Lazy<GLPalettedTextureShaderProgram>(() => new GLPalettedTextureShaderProgram());

        public static GLPalettedTextureShaderProgram PalettedTexture
        {
            get { return PalettedTextureInstance.Value; }
        }
    }
}