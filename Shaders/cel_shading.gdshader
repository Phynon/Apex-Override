shader_type spatial;
uniform vec4 color : source_color = vec4(1.0);
uniform float roughness = 1.0;
uniform float lightSteps = 2.0;

void fragment() {
ALBEDO = color.rgb;
ROUGHNESS = roughness;
}

void light() {
//Diffuse lighting
	float NdotL = clamp(dot(NORMAL, LIGHT), 0.0, 1.0);
	float NdotLRound = (round(NdotL * lightSteps)/ lightSteps);
	
	DIFFUSE_LIGHT = NdotLRound * ALBEDO * (LIGHT_COLOR / PI) * ATTENUATION;
}

