using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using StbiSharp;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace OpenGLSandbox;

unsafe abstract class Application
{
	protected Vector2 Size { get; private set; }

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

	protected static byte[] GetResouceBinary(string path)
	{
		return File.ReadAllBytes($"Resources/{path}");
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

	protected static int LoadTexture(string path)
	{
		var data = GetResouceBinary(path);

		Stbi.SetFlipVerticallyOnLoad(true);
		StbiImage image = Stbi.LoadFromMemory(new ReadOnlySpan<byte>(data), 4);

		int handle = GL.GenTexture();
		GL.BindTexture(TextureTarget.Texture2D, handle);

		GL.TextureParameter(handle, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
		GL.TextureParameter(handle, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
		GL.TextureParameter(handle, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToEdge);
		GL.TextureParameter(handle, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToEdge);

		GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data.ToArray());
		GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

		return handle;
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

		GLFW.WindowHint(WindowHintInt.Samples, 4);
		GLFW.WindowHint(WindowHintBool.Resizable, true);
		GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
		GLFW.WindowHint(WindowHintInt.ContextVersionMinor, 4);
		GLFW.WindowHint(WindowHintInt.ContextVersionMajor, 4);

		const int Width = 1200;
		const int Height = 800;
		const string Title = "OpenGL Sandbox";

		_window = GLFW.CreateWindow(Width, Height, Title, null, null);
		GLFW.MakeContextCurrent(_window);

		Size = new Vector2i(Width, Height);
	}

	private void InitializeOpenGL()
	{
		Log("Initializing");

		GL.LoadBindings(new GLFWBindingsContext());
		Log($"OpenGL version {GL.GetString(StringName.Version)}");

		GL.FrontFace(FrontFaceDirection.Cw);
		GL.CullFace(CullFaceMode.Back);

		GL.Enable(EnableCap.Blend);
		GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		GL.Enable(EnableCap.Multisample);

		GLFW.SetFramebufferSizeCallback(_window, (window, width, height) =>
		{
			GL.Viewport(0, 0, width, height);
			Size = new Vector2i(width, height);
		});

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

	/*
	 * #####################
	 * # RENDER BENCHAMRKS #
	 * #####################
	 * One-At-a-Time-Rendering:
	 *		Best: 233 MS, avg: 258 MS, worst: 544 MS
	*/
	private void MainLoop()
	{
		Log("Entering main loop");

		//int frameCount = 0;
		//float msSum = 0f;
		//float lowMS = float.PositiveInfinity, highMS = 0f;

		while (!GLFW.WindowShouldClose(_window))
		{
			if (GLFW.GetTime() >= 10f) break;

			//var sw = Stopwatch.StartNew();

			GL.ClearColor(Color4.Black);
			GL.Clear(ClearBufferMask.ColorBufferBit);

			OnRender();
			GLFW.SwapBuffers(_window);

			GLFW.PollEvents();

			if (GLFW.GetKey(_window, Keys.Escape) == InputAction.Press)
			{
				GLFW.SetWindowShouldClose(_window, true);
			}

			//sw.Stop();
			//float ms = sw.ElapsedMilliseconds;

			//if (ms < lowMS) lowMS = ms;
			//if (ms > highMS) highMS = ms;

			//msSum += ms;
			//frameCount++;
		}

		//Log($"Best: {lowMS}, avg: {msSum / frameCount}, worst: {highMS}");

		//Console.ReadKey(true);
	}

	private void CleanUp()
	{
		OnCleanUp();

		GLFW.DestroyWindow(_window);
		GLFW.Terminate();
	}
}
