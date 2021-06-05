#version 440 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoords;
out vec2 TexCoords;

uniform mat4 proj;
uniform mat4 view;

void main()
{
	gl_Position = proj * view * vec4(aPosition, 1.0f);
	TexCoords = aTexCoords;
}