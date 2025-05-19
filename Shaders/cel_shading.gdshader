shader_type spatial;
uniform vec4 color : source_color = vec4(1.0);
uniform float roughness = 1.0;
uniform float lightSteps = 2.0;
uniform bool outline = true;
uniform float specular : hint_range(0.0, 1.0, 0.01) = 0.5;
uniform float specularIntensity = 1.0;

varying vec4 camPos;
varying vec3 viewPos;

void vertex(){
	camPos = VIEW_MATRIX * vec4(CAMERA_POSITION_WORLD, 1.0);
	viewPos = NODE_POSITION_VIEW;
}

void fragment() {
ALBEDO = color.rgb;
ROUGHNESS = float(outline);
}

void light() {
//Diffuse lighting
	float NdotL = clamp(dot(NORMAL, LIGHT), 0.0, 1.0);
	float NdotLRound = (round(NdotL * lightSteps)/ lightSteps);

	DIFFUSE_LIGHT = NdotLRound * ALBEDO * (LIGHT_COLOR / PI) * ATTENUATION;
	
//Specular Lighting
	vec3 viewDir = normalize(camPos.rgb - viewPos);
	vec3 reflectDir = reflect(-LIGHT, NORMAL);
	float spec = dot(viewDir, reflectDir);
	
	spec = pow(max(spec, 0.0), 1.0 / specular);
	//spec = step(0.1, spec) * ( 1. - specular);
	float specRound = (round(spec * lightSteps)/ lightSteps);
	
	specRound *= NdotL * specularIntensity;
	
	DIFFUSE_LIGHT += specRound * LIGHT_COLOR * ATTENUATION;
}
