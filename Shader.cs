using OpenTK.Graphics.OpenGL4;

namespace Graphics
{
    public class Shader
    {
        public int program;
        private bool disposed = false;
        private Entity owner;

        public Shader(string vertexShader, string fragmentShader, Entity owner)
        {
            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexShader);
            GL.CompileShader(vertex);
            GL.GetShader(vertex, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(vertex);
                Console.WriteLine(infoLog);
            }

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentShader);
            GL.CompileShader(fragment);
            GL.GetShader(fragment, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(fragment);
                Console.WriteLine(infoLog);
            }

            program = GL.CreateProgram();

            GL.AttachShader(program, vertex);
            GL.AttachShader(program, fragment);

            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine(infoLog);
            }

            GL.DetachShader(program, vertex);
            GL.DetachShader(program, fragment);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);

            this.owner = owner;

            if (success != 0)
            {
                Console.WriteLine("Shader created successfully for entity " + owner.Id);
            }
        }

        public Shader(string vertexShader, string fragmentShader)
        {
            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexShader);
            GL.CompileShader(vertex);
            GL.GetShader(vertex, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(vertex);
                Console.WriteLine(infoLog);
            }

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentShader);
            GL.CompileShader(fragment);
            GL.GetShader(fragment, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(fragment);
                Console.WriteLine(infoLog);
            }

            program = GL.CreateProgram();

            GL.AttachShader(program, vertex);
            GL.AttachShader(program, fragment);

            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine(infoLog);
            }

            GL.DetachShader(program, vertex);
            GL.DetachShader(program, fragment);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);

            if (success != 0)
            {
                Console.WriteLine("Shader created successfully");
            }
        }

        public Shader(string vertexShader, string fragmentShader, string GeometryShader, Entity owner)
        {
            int vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertex, vertexShader);
            GL.CompileShader(vertex);
            GL.GetShader(vertex, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(vertex);
                Console.WriteLine(infoLog);
            }

            int fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragment, fragmentShader);
            GL.CompileShader(fragment);
            GL.GetShader(fragment, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(fragment);
                Console.WriteLine(infoLog);
            }

            int geometry = GL.CreateShader(ShaderType.GeometryShader);
            GL.ShaderSource(geometry, GeometryShader);
            GL.CompileShader(geometry);
            GL.GetShader(geometry, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(geometry);
                Console.WriteLine(infoLog);
            }

            program = GL.CreateProgram();

            GL.AttachShader(program, vertex);
            GL.AttachShader(program, fragment);
            GL.AttachShader(program, geometry);

            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(program);
                Console.WriteLine(infoLog);
            }

            GL.DetachShader(program, vertex);
            GL.DetachShader(program, fragment);
            GL.DetachShader(program, geometry);
            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);
            GL.DeleteShader(geometry);

            this.owner = owner;
        }

        public void Use()
        {
            GL.UseProgram(program);
        }

        public virtual void Dispose()
        {
            if (!disposed)
            {
                GL.DeleteProgram(program);
                disposed = true;
            }
        }
        ~Shader()
        {
            if (!disposed)
            {
                Console.WriteLine("GPU Resource leak! Remeber call Dispose()?");
            }
        }

        public void setInt(string name, int value)
        {
            int location = GL.GetUniformLocation(program, name);
            GL.Uniform1(location, value);
        }

        public void setFloat(string name, float value)
        {
            int location = GL.GetUniformLocation(program, name);
            GL.Uniform1(location, value);
        }

        public void setVec2(string name, OpenTK.Mathematics.Vector2 value)
        {
            int location = GL.GetUniformLocation(program, name);
            GL.Uniform2(location, value);
        }

        public void setVec3(string name, OpenTK.Mathematics.Vector3 value)
        {
            int location = GL.GetUniformLocation(program, name);
            GL.Uniform3(location, value);
        }

        public void setVec4(string name, OpenTK.Mathematics.Vector4 value)
        {
            int location = GL.GetUniformLocation(program, name);
            GL.Uniform4(location, value);
        }

        public void setMat3(string name, OpenTK.Mathematics.Matrix3 value)
        {
            int location = GL.GetUniformLocation(program, name);
            GL.UniformMatrix3(location, false, ref value);
        }

        public void setMat4(string name, OpenTK.Mathematics.Matrix4 value)
        {
            int location = GL.GetUniformLocation(program, name);
            GL.UniformMatrix4(location, false, ref value);
        }
    }
}
