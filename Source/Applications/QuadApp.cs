using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenGLSandbox.Applications;

class QuadApp : Application
{
	private int _vao, _vbo, _ibo;
	private int _shader;

	protected override void OnInitialize()
	{
		_vao = GL.GenVertexArray();
		GL.BindVertexArray(_vao);

		var mesh = LoadMesh("Quad.mesh");
		Assert(mesh.Vertices.Length > 0, "Mesh vertices are missing!");
		Assert(mesh.Indices.Length > 0, "Mesh indices are missing!");

		_vbo = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
		GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * mesh.Vertices.Length, mesh.Vertices, BufferUsageHint.StaticDraw);

		_ibo = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ibo);
		GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(uint) * mesh.Indices.Length, mesh.Indices, BufferUsageHint.StaticDraw);

		int stride = sizeof(float) * 4;

		GL.EnableVertexAttribArray(0);
		GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);

		GL.EnableVertexAttribArray(1);
		GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, sizeof(float) * 2);

		_shader = CreateShader("Quad.shader");
		GL.UseProgram(_shader);
	}

	protected override void OnRender()
	{
		const float orthoScale = 2f;

		var mvp = Matrix4.Identity;
		mvp *= Matrix4.CreateOrthographic(Size.X / Size.Y * orthoScale, orthoScale, 0.1f, 1f);
		GL.UniformMatrix4(GL.GetUniformLocation(_shader, "u_mvp"), true, ref mvp);

		GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
	}

	protected override void OnCleanUp()
	{
		GL.DeleteBuffer(_vbo);
		GL.DeleteBuffer(_ibo);
		GL.DeleteVertexArray(_vao);

		GL.DeleteProgram(_shader);
	}
}
