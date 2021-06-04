#version 330 core
layout(location = 0) in vec2 aPosition;
out vec4 aColor;

uniform mat4 proj;
uniform mat4 view;

void main()
{
	gl_Position = proj * view * vec4(aPosition, 0.0f, 1.0f);
	aColor = vec4(vec3(0.0f), 1.0f);
}
