#version 440 core

out vec4 Frag_Color;

uniform vec3 cameraPosition;
uniform float width;
uniform float height;

uniform int screenWidth;
uniform int screenHeight;

uniform float dx;
uniform float dy;

uniform vec3 Color;

void main()
{
	gl_FragDepth = 0.9;

	float pixelGlobalWidth = width / screenWidth;
	float pixelGlobalHeight = height / screenHeight;

	vec2 coords = vec2((gl_FragCoord.x / screenWidth - 0.5) * width + cameraPosition.x, (gl_FragCoord.y / screenHeight - 0.5) * height + cameraPosition.y);

	if((dx / 2.0 - abs(mod(coords.x, dx) - dx / 2.0)) <= pixelGlobalWidth * 0.5 || (dy / 2.0 - abs(mod(coords.y, dy) - dy / 2.0)) <= pixelGlobalHeight * 0.5 || 
		abs(coords.x) <= pixelGlobalWidth || abs(coords.y) <= pixelGlobalHeight)
		Frag_Color = vec4(Color, 1.0);
	else discard;

	//Frag_Color *= vec4(gl_FragCoord.x / screenWidth, gl_FragCoord.y / screenHeight, 0.0, 1.0);
}