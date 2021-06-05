#version 440 core

layout(points) in;
layout(triangle_strip, max_vertices = 4) out;

in VS_OUT{
	vec3 color;
} gs_in[];

out vec3 Color;

uniform float radius;

void build_rectangle(vec4 position)
{
	Color = gs_in[0].color;

	gl_Position = position - vec4(radius, radius, 0.0, 0.0); // bottom left
	EmitVertex();

	gl_Position = position + vec4(radius, -radius, 0.0, 0.0); // bottom right
	EmitVertex();

	gl_Position = position + vec4(-radius, radius, 0.0, 0.0); // top left
	EmitVertex();

	gl_Position = position + vec4(radius, radius, 0.0, 0.0); // top right
	EmitVertex();
	
	EndPrimitive();
}

void main()
{
	build_rectangle(gl_in[0].gl_Position);
}