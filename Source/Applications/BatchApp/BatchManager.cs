namespace OpenGLSandbox.Applications.BatchApp;

using BatchData = (Batch batch, float lifespan);

class BatchManager
{
	private readonly HashSet<RenderObject> _renderObjects = [];
	private readonly HashSet<BatchData> _batches = [];

	public void Submit(RenderObject renderObject)
	{
		_renderObjects.Add(renderObject);
	}

	public void GenerateBatches()
	{
		// TODO: Paginate render objects into batches based on textures (circa. 32 unique textures per batch).
		// Cancel destruction timers for batches that recieve render objects.
		throw new NotImplementedException();
	}

	public void CleanBatches()
	{
		// TODO: Start desruction timer on empy batches.
		// Empty all batches.
		// Destroy batches with destruction timer's that are complete.
		throw new NotImplementedException();
	}
}
