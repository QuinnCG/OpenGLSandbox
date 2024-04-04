namespace OpenGLSandbox.Applications.BatchApp;

record Shape(Vertex[] Vertices, uint[] Indices)
{
	public static readonly Shape Quad = new(
	[
		new(-0.5f, -0.5f,	0f, 0f),
		new(-0.5f,  0.5f,	0f, 1f),
		new( 0.5f,  0.5f,	1f, 1f),
		new( 0.5f, -0.5f,	1f, 0f)
	],
	[
		0, 1, 2,
		3, 0, 2
	]);

	public Vertex[] Vertices { get; } = Vertices;
	public uint[] Indices { get; } = Indices;
}
