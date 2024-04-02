// Vertex Shader
#version 440 core

layout (location = 0) in vec2 a_position;
layout (location = 1) in vec2 a_uv;

out vec2 v_uv;

uniform mat4 u_mvp;

void main()
{
	gl_Position = vec4(a_position, 0.0, 1.0) * u_mvp;
	v_uv = a_uv;
}

// Fragment Shader
#version 440 core

in vec2 v_uv;

out vec4 f_color;

void main()
{
	f_color = vec4(1.0, 0.0, 0.0, 1.0);
}
