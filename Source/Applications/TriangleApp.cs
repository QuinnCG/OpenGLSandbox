using OpenTK.Graphics.OpenGL4;

namespace OpenGLSandbox.Applications;

class TriangleApp : Application
{
	private int _vao, _vbo, _ibo;
	private int _shader;

	protected override void OnInitialize()
	{
		_vao = GL.GenVertexArray();
		GL.BindVertexArray(_vao);

		var mesh = LoadMesh("Triangle.mesh");
		Assert(mesh.Vertices.Length > 0, "Mesh vertices are missing!");
		Assert(mesh.Indices.Length > 0, "Mesh indices are missing!");

		_vbo = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
		GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * mesh.Vertices.Length, mesh.Vertices, BufferUsageHint.StaticDraw);

		_ibo = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
		GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * mesh.Indices.Length, mesh.Indices, BufferUsageHint.StaticDraw);

		GL.EnableVertexAttribArray(0);
		GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, sizeof(float) * 2, 0);

		_shader = CreateShader("Basic.shader");
		GL.UseProgram(_shader);
	}

	protected override void OnRender()
	{
		GL.DrawElements(BeginMode.Triangles, 3, DrawElementsType.UnsignedInt, 0);
	}

	protected override void OnCleanUp()
	{
		GL.DeleteBuffer(_vbo);
		GL.DeleteBuffer(_ibo);
		GL.DeleteVertexArray(_vao);

		GL.DeleteProgram(_shader);
	}
}
