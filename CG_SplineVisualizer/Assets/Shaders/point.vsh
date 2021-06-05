#version 440 core

layout(location = 0) in vec2 aPosition;

uniform mat4 proj;
uniform mat4 view;

uniform vec3 aColor;

out VS_OUT{
	vec3 color;
} vs_out;

void main()
{
	gl_Position = proj * view * vec4(aPosition, 0.0, 1.0);
	vs_out.color = aColor;
}