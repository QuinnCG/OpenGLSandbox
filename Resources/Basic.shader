// Vertex Shader
#version 330 core

layout (location = 0) in vec2 a_position;

void main()
{
	gl_Position = vec4(a_position, 0.0, 1.0);
}

// Fragment Shader
#version 330 core

out vec4 f_color;

void main()
{
	f_color = vec4(1.0, 0.0, 0.0, 1.0);
}
