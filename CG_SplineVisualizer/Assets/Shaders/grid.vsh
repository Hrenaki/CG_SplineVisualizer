﻿#version 440 core

layout(location = 0) in vec2 aPosition;

void main()
{
	gl_Position = vec4(aPosition, 0.0f, 1.0f);
}