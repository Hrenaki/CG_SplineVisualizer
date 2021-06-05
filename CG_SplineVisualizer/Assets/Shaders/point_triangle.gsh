#version 440 core

layout(points) in;
layout(triangle_strip, max_vertices = 3) out;

in VS_OUT{
	vec3 color;
} gs_in[];

out vec3 Color;

uniform float radius;

void build_triangle(vec4 position)
{
	Color = gs_in[0].color;

	float new_radius = radius * 1.5;
	float a = new_radius * sqrt(3.0);

	gl_Position = position - vec4(new_radius / 2.0, a / 2.0, 0.0, 0.0); // bottom left
	EmitVertex();

	gl_Position = position + vec4(new_radius / 2.0, -a / 2.0, 0.0, 0.0); // bottom right
	EmitVertex();

	gl_Position = position + vec4(0.0, new_radius, 0.0, 0.0); // top
	EmitVertex();

	EndPrimitive();
}

void main()
{
	build_triangle(gl_in[0].gl_Position);
}