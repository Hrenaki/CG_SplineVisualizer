#version 440 core
in vec3 Color;
out vec4 Frag_Color;

void main()
{
	Frag_Color = vec4(Color, 1.0);
}