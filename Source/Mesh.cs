namespace OpenGLSandbox;

record Mesh(float[] Vertices, uint[] Indices)
{
	public float[] Vertices { get; } = Vertices;
	public uint[] Indices { get; } = Indices;
}
