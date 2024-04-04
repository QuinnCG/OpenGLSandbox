using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace OpenGLSandbox.Applications;

class QuadApp : Application
{
	private int _vao, _vbo, _ibo;
	private int _shader, _texture;

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

		_shader = CreateShader("Quad.glsl");
		GL.UseProgram(_shader);

		_texture = LoadTexture("Logo.png");
		GL.ActiveTexture(TextureUnit.Texture0);
		GL.BindTexture(TextureTarget.Texture2D, _texture);
	}

	protected override void OnRender()
	{
		int count = 100;
		int dim = (int)MathF.Round(MathF.Sqrt(count));

		int width = dim;
		int height = dim;

		var quads = new Vector2[width * height];
		float gap = 0.05f;

		int halfWidth = width / 2;
		int halfHeight = height / 2;

		float size = 1f + gap;
		float halfSize = size / 2f;

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				float x = ((float)i - halfWidth + halfSize);
				float y = ((float)j - halfHeight + halfSize);

				quads[(j * width) + i] = new Vector2(x, y) * size;
			}
		}

		float orthoScale = height + (height * 0.2f);

		foreach (var quad in quads)
		{
			var mvp = Matrix4.Identity;
			mvp *= Matrix4.CreateTranslation(quad.X, quad.Y, 0f);
			mvp *= Matrix4.CreateOrthographic(Size.X / Size.Y * orthoScale, orthoScale, 0f, 1f);

			GL.UniformMatrix4(GL.GetUniformLocation(_shader, "u_mvp"), true, ref mvp);
			GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
		}
	}

	protected override void OnCleanUp()
	{
		GL.DeleteBuffer(_vbo);
		GL.DeleteBuffer(_ibo);
		GL.DeleteVertexArray(_vao);

		GL.DeleteProgram(_shader);
		GL.DeleteTexture(_texture);
	}
}
