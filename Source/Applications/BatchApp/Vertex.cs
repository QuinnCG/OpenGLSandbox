using OpenTK.Mathematics;

namespace OpenGLSandbox.Applications.BatchApp;

struct Vertex
{
	public const int FloatCount = 2 + 2;

	public Vector2 Position;
	public Vector2 UV;

	public Vertex(Vector2 position, Vector2 uv)
	{
		Position = position;
		UV = uv;
	}
	public Vertex(float x, float y, float u, float v)
	{
		Position = new(x, y);
		UV = new(u, v);
	}
}
