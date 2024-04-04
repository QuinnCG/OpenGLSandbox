using OpenGLSandbox.Applications;
using OpenGLSandbox.Applications.BatchApp;

namespace OpenGLSandbox;

static class Program
{
	private static void Main()
	{
#if !DEBUG
		var builder = new StringBuilder();
		builder.AppendLine("Applications:");

		var appTypes = new Type[]
		{
			typeof(TriangleApp),
			typeof(QuadApp),
			typeof(BatchApp)
		};

		for (int i = 0; i < appTypes.Length; i++)
		{
			builder.AppendLine($"\t({i + 1}) {appTypes[i].Name}");
		}

		Console.WriteLine(builder);
		Console.WriteLine("Select application by number:");

		while (true)
		{
			var key = Console.ReadKey(true).KeyChar;
			int index = key - '1';

			if (index < appTypes.Length)
			{
				Console.Clear();

				var app = Activator.CreateInstance(appTypes[index]) as Application;
				app!.Run();

				break;
			}
		}
#else
		var app = new BatchApp();
		app.Run();
#endif
	}
}
