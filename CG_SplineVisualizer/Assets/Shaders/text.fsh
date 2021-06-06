#version 440 core
in vec2 TexCoords;
out vec4 vColor;

uniform sampler2D text;
uniform vec3 textColor;

void main()
{
	gl_FragDepth = 0.9;

	vec4 sampled = vec4(1.0, 1.0, 1.0, texture(text, TexCoords).r);
    vColor = vec4(textColor, 1.0) * sampled;
}