#version 440 core

out vec4 Color;

uniform vec3 aColor;

void main()
{
	Color = vec4(aColor, 1.0);
}