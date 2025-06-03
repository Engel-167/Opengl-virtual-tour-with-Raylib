#version 330

// Input vertex attributes (from vertex shader)
in vec3 fragPosition;    // Fragment position in world space
in vec2 fragTexCoord;    // Fragment texture coordinates
in vec3 fragNormal;      // Fragment normal in world space

// Input uniform values
uniform sampler2D texture0; // Base color texture (albedo)
uniform vec4 colDiffuse;    // Diffuse color tint from material (often white)

uniform vec3 lightDir;      // Direction *towards* the light source
uniform vec4 lightColor;    // Color and intensity of the directional light
uniform vec4 ambient;       // Ambient light color and intensity
uniform vec3 viewPos;       // Camera position in world space

// Shadow mapping uniforms
uniform mat4 lightVP;           // Light's combined view-projection matrix
uniform sampler2D shadowMap;    // Depth texture rendered from light's perspective
uniform int shadowMapResolution; // Resolution of the shadow map texture

// Output fragment color
out vec4 finalColor;

// --- Simple Pseudo-Random Function ---
// Used for jittering shadow samples
vec2 randomOffsetGenerator(vec2 seed) {
    float n = sin(dot(seed, vec2(12.9898, 78.233)));
    return fract(vec2(n, n * 43758.5453) * 43758.5453);
}

// --- Predefined Disk Sample Offsets (12 samples) ---
const int diskSamples = 12;
const vec2 diskKernel[diskSamples] = vec2[](
vec2( 0.326247f,  0.40582f ), vec2( -0.840154f, -0.07358f ),
vec2( -0.695914f,  0.457137f ), vec2( -0.203345f, -0.620716f ),
vec2( 0.96234f,  -0.194983f ), vec2(  0.473434f, -0.480026f ),
vec2( -0.519456f, -0.768961f ), vec2( -0.094184f,  0.929389f ),
vec2( 0.79552f,  0.597705f ), vec2( -0.347336f,  0.296979f ),
vec2( 0.05311f, -0.091586f ), vec2(  0.11397f, -0.993465f )
);

// --- Shadow Calculation Function (Randomized PCF with 12 Disk Samples) ---
float CalculateShadowFactor(vec4 fragPosLightSpace, vec3 normal, vec3 lightDirection)
{
    // Perspective divide
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    // Transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;
    float currentDepth = projCoords.z;

    // Check if the fragment is outside the light's valid depth range
    if (projCoords.z <= 0.0 || projCoords.z >= 1.0) {
        return 1.0; // Not shadowed
    }

    // Calculate bias to avoid shadow acne
    float bias = 0.000185; //max(0.00008 * (1.0 - dot(normal, lightDirection)), 0.000032); // Adjust bias as needed

    float shadow = 0.0; // Accumulator for shadow contribution
    vec2 texelSize = 1.5 / vec2(shadowMapResolution);
    // Radius of the PCF disk kernel in texels. Adjust for softness.
    float diskRadius = 1.8; // Start with this, tune if needed

    for(int i = 0; i < diskSamples; ++i) // Loop 12 times
    {
        // Get the offset from the reduced disk kernel
        vec2 kernelOffset = diskKernel[i] * diskRadius * texelSize;

        // Generate a unique random offset per sample for jittering
        vec2 sampleSeed = gl_FragCoord.xy + vec2(i, i * 0.77); // Use screen coord + index as seed
        vec2 randomJitter = (randomOffsetGenerator(sampleSeed) * 2.0 - 1.0) * texelSize * 0.75; // Scale jitter

        // Calculate final sample coordinate
        vec2 sampleCoord = projCoords.xy + kernelOffset + randomJitter;

        // Sample depth from shadow map
        float pcfDepth = texture(shadowMap, sampleCoord).r;

        // Compare depths (fragment is shadowed if it's further than the occluder)
        shadow += (currentDepth - bias) > pcfDepth ? 1.0 : 0.0;
    }
    // Average the results over the reduced number of disk samples
    shadow /= float(diskSamples); // Divide by 12

    // Return shadow factor (1.0 = fully lit, 0.0 = fully shadowed)
    return 1.0 - shadow;
}

// --- Main Function ---
void main()
{
    // --- Basic Properties ---
    vec4 texelColor = texture(texture0, fragTexCoord);
    vec4 materialColor = colDiffuse;
    vec3 normal = normalize(fragNormal);
    vec3 viewDir = normalize(viewPos - fragPosition);
    vec3 lightDirection = normalize(-lightDir);

    // --- Ambient Lighting ---
    vec3 ambientColor = texelColor.rgb * ambient.rgb;

    // --- Diffuse Lighting ---
    vec3 diffuseBase = texelColor.rgb * materialColor.rgb;
    float NdotL = max(dot(normal, lightDirection), 0.0);
    vec3 diffuseContrib = diffuseBase * lightColor.rgb * NdotL;

    // --- Specular Lighting ---
    vec3 specularColor = vec3(0.0);
    if (NdotL > 0.0)
    {
        vec3 halfwayDir = normalize(lightDirection + viewDir);
        float specAngle = max(dot(normal, halfwayDir), 0.0);
        float shininess = 32.0;
        specularColor = lightColor.rgb * pow(specAngle, shininess);
    }

    // --- Shadow Calculation ---
    vec4 fragPosLightSpace = lightVP * vec4(fragPosition, 1.0);
    float shadowFactor = CalculateShadowFactor(fragPosLightSpace, normal, lightDirection);

    // --- Combine Lighting ---
    vec3 directLight = (diffuseContrib + specularColor) * shadowFactor;
    vec3 litColor = ambientColor + directLight;

    // --- Saturation Boost ---
    float saturationBoost = 1.5;
    vec3 luminanceWeights = vec3(0.299, 0.587, 0.114);
    float luminance = dot(litColor, luminanceWeights);
    vec3 grayScaleColor = vec3(luminance);
    vec3 saturatedColor = mix(grayScaleColor, litColor, saturationBoost);

    finalColor.rgb = saturatedColor;
    finalColor.a = texelColor.a * materialColor.a;

    // --- Gamma Correction ---
    finalColor = pow(finalColor, vec4(1.0 / 2.2));
}