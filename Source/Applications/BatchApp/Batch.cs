using OpenTK.Graphics.OpenGL4;

namespace OpenGLSandbox.Applications.BatchApp;
class Batch
{
	public const int DefaultVBufferSize = 4 * 32;
	public const int DefaultIBufferSize = 6 * 32;

	public int Handle { get; }

	private readonly HashSet<Texture> _textures = [];

	public Batch()
	{
		int vao = GL.GenVertexArray();
		GL.BindVertexArray(vao);

		int vbo = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
		GL.BufferData(BufferTarget.ArrayBuffer, DefaultVBufferSize, 0, BufferUsageHint.DynamicDraw);

		int ibo = GL.GenBuffer();
		GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
		GL.BufferData(BufferTarget.ElementArrayBuffer, DefaultIBufferSize, 0, BufferUsageHint.DynamicDraw);

		int stride = 4 * 4;

		GL.EnableVertexAttribArray(0);
		GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);

		GL.EnableVertexAttribArray(1);
		GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 4 * 2);
	}

	~Batch()
	{
		GL.DeleteVertexArray(Handle);
	}

	public bool ContainsTexture(Texture texture)
	{
		return _textures.Contains(texture);
	}

	public void Clear()
	{
		_textures.Clear();
	}

	public void UpdateBuffer(params RenderObject[] renderObjects)
	{
		// TODO: Update dynamic buffer with data gathered from render objects.
		throw new NotImplementedException();
	}

	private void GenerateBufferData(RenderObject[] renderObjects, out float[] vertices, out uint[] indices)
	{
		throw new NotImplementedException();
	}
}
