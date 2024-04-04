using OpenTK.Mathematics;

namespace OpenGLSandbox.Applications.BatchApp;

class RenderObject(Shape shape, Vector2 position, Texture? texture = null)
{
	public Texture? Texture { get; set; } = texture;
	public Shape Shape { get; set; } = shape;
	public Vector2 Position { get; set; } = position;

	public float[] GetVertexData()
	{
		var vertices = new Vertex[Shape.Vertices.Length];
		Array.Copy(Shape.Vertices, vertices, vertices.Length);

		for (int i = 0; i < vertices.Length; i++)
		{
			vertices[i].Position += Position;
		}


		var data = new float[Vertex.FloatCount * vertices.Length];
		int j = 0;

		for (int i = 0; i < data.Length; i += Vertex.FloatCount)
		{
			var vertex = vertices[j];

			data[i + 0] = vertex.Position.X;
			data[i + 1] = vertex.Position.Y;
			data[i + 2] = vertex.UV.X;
			data[i + 3] = vertex.UV.Y;

			j++;
		}

		return data;
	}
}
