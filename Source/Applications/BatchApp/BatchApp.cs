using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenGLSandbox.Applications.BatchApp;

class BatchApp : Application
{
	private int _shader;
	private readonly BatchManager _batchManager = new();

	protected override void OnInitialize()
	{
		var renderObject = new RenderObject(Shape.Quad, Vector2.Zero);
		float[] vertices = renderObject.GetVertexData();
		uint[] indices = renderObject.Shape.Indices;

		int vao = GL.GenVertexArray();
		GL.BindVertexArray(vao);

		int vbo = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
		GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * 4, vertices, BufferUsageHint.StaticDraw);

		int ibo = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
		GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * 4, indices, BufferUsageHint.StaticDraw);

		int stride = 4 * 4;

		GL.EnableVertexAttribArray(0);
		GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);

		GL.EnableVertexAttribArray(1);
		GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 4 * 2);

		_shader = CreateShader("Batch.glsl");
		GL.UseProgram(_shader);
	}

	protected override void OnRender()
	{
		float orthoScale = 5f;

		var mvp = Matrix4.Identity;
		mvp *= Matrix4.CreateOrthographic(Size.X / Size.Y * orthoScale, orthoScale, 0.1f, 1f);

		GL.UniformMatrix4(GL.GetUniformLocation(_shader, "u_mvp"), true, ref mvp);
		GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
	}

	protected override void OnCleanUp()
	{
		GL.DeleteProgram(_shader);
	}

	private void Submit(RenderObject renderObject)
	{
		_batchManager.Submit(renderObject);
	}
}
