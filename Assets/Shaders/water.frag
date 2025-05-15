#version 330 core  
in vec3 fsWorldPos;
in vec3 fsNormal;

uniform vec3 viewPos;    // camera position  
uniform vec3 lightDir;   // normalized light direction  

out vec4 fragColor;

void main() {
    // diffuse term
    float d = max(dot(fsNormal, -lightDir), 0.0);
    vec3 diffuse = vec3(0.0, 0.3, 0.6) * d;

    // simple specular
    vec3 V = normalize(viewPos - fsWorldPos);
    vec3 R = reflect(lightDir, fsNormal);
    float spec = pow(max(dot(V,R),0.0), 32);
    vec3 specular = vec3(0.8) * spec;

    // final color, with alpha for transparency
    fragColor = vec4(diffuse + specular, 0.6);
}