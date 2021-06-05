#version 440 core

out vec4 Frag_Color;

uniform vec3 cameraPosition;
uniform float width;
uniform float height;

uniform float screenWidth;
uniform float screenHeight;

uniform float dx;
uniform float dy;

uniform vec3 Color;

void main()
{
	gl_FragDepth = 0.9;

	float pixelGlobalWidth = width / screenWidth;
	float pixelGlobalHeight = height / screenHeight;

	vec2 coords = vec2((2.0 * gl_FragCoord.x / screenWidth - 1.0) * width + cameraPosition.x, (2.0 * gl_FragCoord.y / screenHeight - 1.0) * height + cameraPosition.y);

	if((dx / 2.0 - abs(mod(coords.x, dx) - dx / 2.0)) <= pixelGlobalWidth || (dy / 2.0 - abs(mod(coords.y, dy) - dy / 2.0)) <= pixelGlobalHeight || 
		abs(coords.x) <= pixelGlobalWidth * 3.0 || abs(coords.y) <= pixelGlobalHeight * 3.0)
		Frag_Color = vec4(Color, 1.0);
	else discard;

	//Frag_Color *= vec4(gl_FragCoord.x / screenWidth, gl_FragCoord.y / screenHeight, 0.0, 1.0);
}