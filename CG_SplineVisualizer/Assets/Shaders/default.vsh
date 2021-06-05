#version 440 core

layout(location = 0) in vec2 aPosition;
out vec3 Color;

uniform mat4 proj;
uniform mat4 view;

uniform vec3 aColor;

void main()
{
	gl_Position = proj * view * vec4(aPosition, 0.0f, 1.0f);
	Color = aColor;
}