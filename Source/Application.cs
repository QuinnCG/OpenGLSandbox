using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OpenGLSandbox;

unsafe abstract class Application
{
	protected Vector2 Size => _size;

	private Vector2 _size;

	private Window* _window;
	private int _glErrorID = -1;

	protected static void Log(object message)
	{
		var time = DateTime.Now;

		int hour = time.Hour;
		int minute = time.Minute;
		int second = time.Second;

		Console.ForegroundColor = ConsoleColor.White;
		Console.WriteLine($"[{hour}H:{minute}M:{second}S] [Info] {message}");
	}

	protected static void Error(object message, string type)
	{
		var time = DateTime.Now;

		int hour = time.Hour;
		int minute = time.Minute;
		int second = time.Second;

		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine($"[{hour}H:{minute}M:{second}S] [{type}] {message}");

		string trace = new StackTrace().ToString();
		trace = trace.Replace(Environment.NewLine, Environment.NewLine + '\t');

		Console.WriteLine($"\t{trace}");
	}

	[Conditional("DEBUG")]
	protected static void Assert([DoesNotReturnIf(false)] bool condition, string message)
	{
		if (!condition)
		{
			throw new Exception(message);
		}
	}

	protected static string GetResouceText(string path)
	{
		return File.ReadAllText($"Resources/{path}");
	}

	protected static int CreateShader(string path)
	{
		static int CreateModule(ShaderType type, string source)
		{
			int handle = GL.CreateShader(type);
			GL.ShaderSource(handle, source);
			GL.CompileShader(handle);

			string info = GL.GetShaderInfoLog(handle);
			if (!string.IsNullOrWhiteSpace(info))
			{
				Error(info, "Shader");
			}

			return handle;
		}

		string source = GetResouceText(path);

		int splitIndex = source.IndexOf("// Fragment Shader");
		string vSource = source[..splitIndex];
		string fSource = source[splitIndex..];

		int vs = CreateModule(ShaderType.VertexShader, vSource);
		int fs = CreateModule(ShaderType.FragmentShader, fSource);

		int shader = GL.CreateProgram();
		GL.AttachShader(shader, vs);
		GL.AttachShader(shader, fs);
		GL.LinkProgram(shader);
		GL.ValidateProgram(shader);

		string info = GL.GetProgramInfoLog(shader);
		if (!string.IsNullOrWhiteSpace(info))
		{
			Error(info, "Shader");
		}

		GL.DeleteShader(vs);
		GL.DeleteShader(fs);

		return shader;
	}

	protected static Mesh LoadMesh(string path)
	{
		string raw = GetResouceText(path);
		int splitIndex = raw.IndexOf("// Indices");

		string vertSection = raw[..splitIndex].Replace("// Vertices", string.Empty);
		string indexSection = raw[splitIndex..].Replace("// Indices", string.Empty);

		var separators = new string[] { Environment.NewLine, "," };
		var options = StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries;

		string[] split = vertSection.Split(separators, options);
		float[] vertices = split.Select(float.Parse).ToArray();

		split = indexSection.Split(separators, options);
		uint[] indices = split.Select(uint.Parse).ToArray();

		return new Mesh(vertices, indices);
	}

	public void Run()
	{
		try
		{
			CreateWindow();
			InitializeOpenGL();
			MainLoop();
			CleanUp();
		}
		catch (Exception e)
		{
			Error(e.Message, "Exception");
			Console.ReadKey(true);
		}
	}

	protected abstract void OnInitialize();

	protected abstract void OnRender();

	protected abstract void OnCleanUp();

	private void CreateWindow()
	{
		GLFW.Init();

		GLFW.WindowHint(WindowHintBool.Resizable, false);
		GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
		GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 4);
		GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 3);

		const int Width = 1200;
		const int Height = 800;
		const string Title = "OpenGL Sandbox";

		_window = GLFW.CreateWindow(Width, Height, Title, null, null);
		GLFW.MakeContextCurrent(_window);

		_size = new Vector2i(Width, Height);
	}

	private void InitializeOpenGL()
	{
		Log("Initializing");

		GL.LoadBindings(new GLFWBindingsContext());
		Log($"OpenGL version {GL.GetString(StringName.Version)}");

#if DEBUG
		GL.Enable(EnableCap.DebugOutput);
		GL.DebugMessageCallback((source, type, id, severity, length, message, userParam) =>
		{
			if (id == _glErrorID) return;
			_glErrorID = id;

			if (severity is not (DebugSeverity.DontCare or DebugSeverity.DebugSeverityNotification or DebugSeverity.DebugSeverityLow))
			{
				string msg = Encoding.Default.GetString((byte*)message, length);
				Error(msg, "OpenGL");
			}
		}, 0);
#endif

		OnInitialize();
	}

	private void MainLoop()
	{
		Log("Entering main loop");

		while (!GLFW.WindowShouldClose(_window))
		{
			GL.ClearColor(Color4.Black);
			GL.Clear(ClearBufferMask.ColorBufferBit);

			OnRender();
			GLFW.SwapBuffers(_window);

			GLFW.PollEvents();

			if (GLFW.GetKey(_window, Keys.Escape) == InputAction.Press)
			{
				GLFW.SetWindowShouldClose(_window, true);
			}
		}
	}

	private void CleanUp()
	{
		OnCleanUp();

		GLFW.DestroyWindow(_window);
		GLFW.Terminate();
	}
}
