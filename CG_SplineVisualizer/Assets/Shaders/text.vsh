﻿#version 440 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec2 aTexCoords;
out vec2 TexCoords;

void main()
{
	gl_Position = vec4(aPosition, 1.0f);
	TexCoords = aTexCoords;
}