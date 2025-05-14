#version 330 core  
// --- per-vertex inputs (Raylib uses “vertexPosition” / “vertexNormal” by default)
layout (location = 0) in vec3 vertexPosition;
layout (location = 1) in vec3 vertexNormal;

uniform mat4 mvp;      // built-in Raylib uniform for the combined Model*View*Projection  
uniform float uTime;   // our time accumulator  

out vec3 fsWorldPos;
out vec3 fsNormal;

void main() {
    // simple wave displacement along Y
    float wave1 = sin(vertexPosition.x * 3.0 + uTime * 2.0) * 0.05;
    float wave2 = cos(vertexPosition.z * 2.5 + uTime * 1.5) * 0.05;
    vec3 displacedPos = vertexPosition + vec3(0.0, wave1 + wave2, 0.0);

    fsWorldPos = displacedPos;
    fsNormal   = normalize(vertexNormal);

    gl_Position = mvp * vec4(displacedPos, 1.0);
}