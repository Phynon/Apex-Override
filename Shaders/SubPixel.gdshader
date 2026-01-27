shader_type canvas_item;
render_mode unshaded;

uniform vec2 CamOffset = vec2(0.0, 0.0);

void vertex() {
    VERTEX -= CamOffset;
}

void fragment() {
    vec2 tex_size = clamp(fwidth(UV) / TEXTURE_PIXEL_SIZE, 1e-5, 1);
    vec2 tx = UV / TEXTURE_PIXEL_SIZE - 0.5 * tex_size;
    vec2 tx_offset = smoothstep(vec2(1) - tex_size, vec2(1), fract(tx));
    vec2 uv = (floor(tx) + 0.5 + tx_offset) * TEXTURE_PIXEL_SIZE;
    COLOR = textureGrad(TEXTURE, uv, dFdx(uv), dFdy(uv));
}