using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace ChaosEngine
{
    static class ChaosUtils
    {
        public static int LoadShaderProgram(string vertexShaderPath, string fragmentShaderPath)
        {
            int vertexShaderIndex, fragmentShaderIndex;
            vertexShaderIndex = LoadShader(vertexShaderPath, ShaderType.VertexShader);
            fragmentShaderIndex = LoadShader(fragmentShaderPath, ShaderType.FragmentShader);
            if (vertexShaderIndex == 0 || fragmentShaderIndex == 0)
            {
                Logger.Log("Failed to create shader program.");
                return 0;
            }

            int programIndex = GL.CreateProgram();
            if (programIndex == 0)
            {
                Logger.Log("Failed to create shader program object.");
                return 0;
            }

            GL.AttachShader(programIndex, vertexShaderIndex);
            GL.AttachShader(programIndex, fragmentShaderIndex);

            GL.LinkProgram(programIndex);

            int linkingStatus;
            GL.GetProgram(programIndex, GetProgramParameterName.LinkStatus, out linkingStatus);
            if (linkingStatus == 0)
            {
                Logger.Log("Failed to link shader program. Linking error: " + GL.GetProgramInfoLog(programIndex));
                GL.DeleteProgram(programIndex);
                GL.DeleteShader(vertexShaderIndex);
                GL.DeleteShader(fragmentShaderIndex);
                return 0;
            }

            return programIndex;
        }
        private static int LoadShader(string shaderPath, ShaderType shaderType)
        {
            string shaderText = null;
            if (File.Exists(shaderPath))
                shaderText = File.ReadAllText(shaderPath);
            else
                Logger.Log("Shader file " + shaderPath + " not found.");

            int shaderIndex = GL.CreateShader(shaderType);
            if (shaderIndex == 0)
            {
                Logger.Log("Failed to create shader object for " + shaderType.ToString());
                return 0;
            }

            GL.ShaderSource(shaderIndex, shaderText);
            GL.CompileShader(shaderIndex);

            int compileStatus;
            GL.GetShader(shaderIndex, ShaderParameter.CompileStatus, out compileStatus);
            if (compileStatus == 0)
            {
                Logger.Log("Failed to compile " + shaderType.ToString() + " shader. Compilation error: " + GL.GetShaderInfoLog(shaderIndex));
                GL.DeleteShader(shaderIndex);
                return 0;
            }

            return shaderIndex;
        }
    }
}
