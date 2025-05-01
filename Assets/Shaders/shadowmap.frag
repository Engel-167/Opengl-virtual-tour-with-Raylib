#version 330

// Input vertex attributes (from vertex shader)
in vec3 fragPosition;    // Fragment position in world space
in vec2 fragTexCoord;    // Fragment texture coordinates
in vec3 fragNormal;      // Fragment normal in world space

// Input uniform values
uniform sampler2D texture0; // Base color texture (albedo)
uniform vec4 colDiffuse;    // Diffuse color tint from material

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

// --- Shadow Calculation Function (PCF) ---
float CalculateShadowFactor(vec4 fragPosLightSpace, vec3 normal, vec3 lightDirection)
{
    // Perspective divide
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;

    // Transform to [0,1] range
    projCoords = projCoords * 0.5 + 0.5;

    // Get the fragment's current depth from the light's perspective
    float currentDepth = projCoords.z;

    // Check if the fragment is outside the light's frustum (avoids sampling outside the map)
    // Important: If outside near/far plane, it shouldn't be shadowed by definition *within* the frustum
    // Return 1.0 (fully lit) if outside the valid depth range [0, 1]
    if (projCoords.z <= 0.0 || projCoords.z >= 1.0) {
        return 1.0; // Not shadowed if outside frustum depth
    }

    // Calculate bias to avoid shadow acne (self-shadowing)
    float bias = max(0.0005 * (1.0 - dot(normal, lightDirection)), 0.0001);

    // PCF - Percentage-Closer Filtering
    float shadow = 0.0; // Accumulator for shadow contribution (fraction of samples that are shadowed)
    vec2 texelSize = 1.0 / vec2(shadowMapResolution); // Size of one texel
    int pcfSamples = 3; // Kernel size (3x3 = 9 samples). Increase for smoother but slower shadows.
    float halfSamples = float(pcfSamples -1) / 2.0;

    for(int x = 0; x < pcfSamples; ++x)
    {
        for(int y = 0; y < pcfSamples; ++y)
        {
            vec2 offset = vec2(x - halfSamples, y - halfSamples) * texelSize;
            // Sample depth from shadow map at offset coordinate
            float pcfDepth = texture(shadowMap, projCoords.xy + offset).r;
            // Check if the current fragment is further away (in shadow) compared to the stored depth
            shadow += (currentDepth - bias) > pcfDepth ? 1.0 : 0.0;
        }
    }
    shadow /= float(pcfSamples * pcfSamples); // Average the results

    // shadow = 0.0 means fully lit, shadow = 1.0 means fully shadowed
    // We return (1.0 - shadow) because we want a factor to multiply the light by (1 = lit, 0 = shadow)
    return 1.0 - shadow;
}

// --- Main Function ---
void main()
{
    // --- Basic Properties ---
    vec4 texelColor = texture(texture0, fragTexCoord);
    // Use colDiffuse as a tint for the material
    vec4 materialColor = colDiffuse;
    vec3 normal = normalize(fragNormal);
    vec3 viewDir = normalize(viewPos - fragPosition);
    // Direction *from* fragment *to* light source
    vec3 lightDirection = normalize(-lightDir);

    // --- Ambient Lighting (Fix Applied) ---
    // Calculate ambient based on texture color and ambient uniform ONLY.
    vec3 ambientColor = texelColor.rgb * ambient.rgb;

    // --- Diffuse Lighting ---
    // Use texture color tinted by material color for diffuse base
    vec3 diffuseBase = texelColor.rgb * materialColor.rgb;
    float NdotL = max(dot(normal, lightDirection), 0.0);
    vec3 diffuseContrib = diffuseBase * lightColor.rgb * NdotL;

    // --- Specular Lighting ---
    vec3 specularColor = vec3(0.0);
    if (NdotL > 0.0) // Only calculate specular if light hits the surface
    {
        vec3 reflectDir = reflect(-lightDirection, normal);
        float specAngle = max(dot(viewDir, reflectDir), 0.0);
        // Shininess factor (adjust for desired highlight size)
        float shininess = 32.0;
        // Specular usually uses light color directly, maybe tinted by a specular material color if available
        specularColor = lightColor.rgb * pow(specAngle, shininess);
    }

    // --- Shadow Calculation ---
    // Transform fragment position to light space
    vec4 fragPosLightSpace = lightVP * vec4(fragPosition, 1.0);
    // Get shadow factor (0.0 = shadowed, 1.0 = lit) using the PCF function
    float shadowFactor = CalculateShadowFactor(fragPosLightSpace, normal, lightDirection);

    // --- Combine Lighting (Fix Applied) ---
    // Apply shadow factor ONLY to diffuse and specular components
    vec3 directLight = (diffuseContrib + specularColor) * shadowFactor;

    // Final color before saturation/gamma = Ambient + Shadowed Direct Light
    vec3 litColor = ambientColor + directLight;

    // --- Saturation Boost ---
    // Increase saturation for vibrancy
    float saturationBoost = 1.5; // Value > 1 increases saturation. Adjust as needed (e.g., 1.1 to 1.5)
    vec3 luminanceWeights = vec3(0.299, 0.587, 0.114); // Standard weights for luminance
    float luminance = dot(litColor, luminanceWeights);
    vec3 grayScaleColor = vec3(luminance);
    vec3 saturatedColor = mix(grayScaleColor, litColor, saturationBoost); // Interpolate towards original color

    // Assign the saturated color
    finalColor.rgb = saturatedColor;
    // Combine alpha from texture and material tint
    finalColor.a = texelColor.a * materialColor.a;

    // --- Gamma Correction ---
    // Apply gamma correction for more realistic brightness perception
    finalColor = pow(finalColor, vec4(1.0 / 2.2));
}
