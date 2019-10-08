using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace ChaosEngine
{
    public sealed class Model
    {
        public List<Vector3> vertices { get; private set; } = new List<Vector3>();
        public List<Vector3> normals { get; private set; } = new List<Vector3>();
        public List<double[]> texCoords { get; private set; } = new List<double[]>();
        public List<int[]> polygons { get; private set; } = new List<int[]>();
        public List<int[]> normalsPolygons { get; private set; } = new List<int[]>();
        public List<int[]> texPolygons { get; private set; } = new List<int[]>();
        public bool hasNormals { get; private set; } = false;
        public bool hasTexture { get; private set; } = false;
        public static Model parse(string fileName)
        {
            Model model = new Model();

            StreamReader file = new StreamReader(File.OpenRead(fileName));
            string line;
            string[] words;
            while ((line = file.ReadLine()) != null)
            {
                words = line.Split(' ');
                switch (words[0])
                {
                    case "v":
                        model.vertices.Add(new Vector3(double.Parse(words[1]), double.Parse(words[2]), double.Parse(words[3])));
                        break;
                    case "vt":
                        model.texCoords.Add(words.Select<string, double>((word) => { return double.Parse(word); }).ToArray());
                        model.hasTexture = true;
                        break;
                    case "vn":
                        model.normals.Add(new Vector3(double.Parse(words[1]), double.Parse(words[2]), double.Parse(words[3])));
                        model.hasNormals = true;
                        break;
                    case "f":
                        int[] first = new int[words.Length - 1]; // vertices indexes
                        int[] second = null; // texture, if there is texture, normals, if there is normals and no textures
                        int[] third = null; // normals, if there is normals and texture
                        if (model.hasNormals || model.hasTexture)
                            second = new int[words.Length - 1];
                        if (model.hasNormals && model.hasTexture)
                            third = new int[words.Length - 1];
                        int[] curIndexes;
                        for (int i = 0; i < words.Length - 1; i++)
                        {
                            curIndexes = words[i + 1].Split(new string[] { "/", "//", "\\", "\\\\" }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Select<string, int>((index) => { return int.Parse(index); }).ToArray();
                            first[i] = curIndexes[0];
                            if (curIndexes.Length > 1)
                                second[i] = curIndexes[1];
                            if (curIndexes.Length > 2)
                                third[i] = curIndexes[2];
                        }
                        break;
                }
            }

            return model;
        }
    }
    internal static class ChaosUtils
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
            {
                Logger.Log("Shader file " + shaderPath + " not found.");
                return 0;
            }

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
