namespace DestinationRolodex.Rendering;

/// <summary>
/// SkSL port of the approved GLSL renderer (handoff: uno-reference/shaders/reference.frag.glsl).
/// The effect equations are unchanged. Two deliberate port edits:
///   1. pow(x, 2.) became x*x. GLSL pow() with a negative base is undefined; the square is the same number.
///   2. main() converts Skia's top-left local coordinates to the GL bottom-left UV space the equations assume,
///      and image sampling converts back to top-left pixel coordinates for the child image shaders.
/// uResolution is the overscanned media-layer size in the same local units as main(coord).
/// </summary>
internal static class TransitionShaderSource
{
    public const string Sksl = """
uniform shader uFrom;
uniform shader uTo;
uniform float uProgress;
uniform float uStrength;
uniform float uDirection;
uniform float uAspect;
uniform float uAspectA;
uniform float uAspectB;
uniform float2 uFocalA;
uniform float2 uFocalB;
uniform int uMode;
uniform float2 uResolution;
uniform float2 uImageSizeA;
uniform float2 uImageSizeB;
const float PI = 3.141592653589793;

float hash(float2 p) { return fract(sin(dot(p, float2(127.1, 311.7))) * 43758.5453); }

float noise(float2 p) {
  float2 i = floor(p);
  float2 f = fract(p);
  f = f * f * (3. - 2. * f);
  return mix(mix(hash(i), hash(i + float2(1, 0)), f.x), mix(hash(i + float2(0, 1)), hash(i + float2(1, 1)), f.x), f.y);
}

float fbm(float2 p) { return .58 * noise(p) + .28 * noise(p * 2.03 + 5.7) + .14 * noise(p * 4.11 + 11.2); }

float2 crop(float2 uv, float aspect, float2 focal) {
  float2 scale = aspect > uAspect ? float2(uAspect / aspect, 1.) : float2(1., aspect / uAspect);
  return clamp(uv * scale + (1. - scale) * float2(focal.x, 1. - focal.y), float2(.001), float2(.999));
}

float4 A(float2 uv) { float2 p = crop(uv, uAspectA, uFocalA); return uFrom.eval(float2(p.x, 1. - p.y) * uImageSizeA); }
float4 B(float2 uv) { float2 p = crop(uv, uAspectB, uFocalB); return uTo.eval(float2(p.x, 1. - p.y) * uImageSizeB); }

float ease(float t) { return t * t * (3. - 2. * t); }
float luma(float4 c) { return dot(c.rgb, float3(.2126, .7152, .0722)); }

float4 morph(float2 uv, float p) {
  float bell = sin(PI * p);
  float2 q = uv * float2(2.7 * uAspect, 2.7);
  // Both images are advected through one low-frequency field in opposing directions.
  float n = fbm(q + float2(p * .32, -p * .38 * uDirection));
  float m = fbm(q + float2(7.1, 3.8) + float2(-p * .26, p * .4));
  float a = luma(A(uv));
  float b = luma(B(uv));
  float2 field = float2(n - .5, m - .5) + float2((a - b) * .19, (a + b - 1.) * .10);
  float power = (.18 + uStrength * .48) * bell;
  float2 drift = float2(sin(uv.y * PI * 2. + p * PI) * .028, uDirection * .045) * bell;
  float2 va = uv + field * power * p + drift * p;
  float2 vb = uv - field * power * (1. - p) - drift * (1. - p);
  float mask = clamp(p + (n - .5) * bell * (.55 + .3 * uStrength), 0., 1.);
  return mix(A(va), B(vb), smoothstep(0., 1., mask));
}

float4 liquid(float2 uv, float p) {
  float2 origin = float2(.48, uDirection > 0. ? .38 : .62);
  float2 v = (uv - origin) * float2(uAspect, 1.);
  float r = length(v);
  float2 normal = v / max(r, .0001) / float2(uAspect, 1.);
  float radius = p * 1.35 - .13;
  float wobble = (noise(uv * 4.8) - .5) * .035 * sin(PI * p);
  float sd = r - radius + wobble;
  float sdn = sd / (.095 + .045 * uStrength);
  float ring = exp(-(sdn * sdn));
  float wave = sin(sd * 25. - p * 5.5) * ring;
  float displacement = wave * (.025 + .085 * uStrength) * sin(PI * p);
  float2 va = uv + normal * displacement;
  float2 vb = origin + (uv - origin) * (1. - ring * .07 * uStrength) - normal * displacement * .78;
  float mask = 1. - smoothstep(-.065, .065, sd);
  float4 ca = A(va);
  float4 cb = B(vb);
  // Very small spectral separation at the moving liquid edge, not a full-screen blur.
  float fringe = ring * .0035 * uStrength * sin(PI * p);
  ca.r = A(va + normal * fringe).r;
  ca.b = A(va - normal * fringe).b;
  cb.r = B(vb - normal * fringe).r;
  cb.b = B(vb + normal * fringe).b;
  float4 c = mix(ca, cb, mask);
  c.rgb += ring * .035 * uStrength * sin(PI * p);
  return c;
}

half4 main(float2 coord) {
  float2 vUv = coord / uResolution;
  vUv.y = 1. - vUv.y;
  float p = clamp(uProgress, 0., 1.);
  if (p <= .00001) { return half4(A(vUv).rgb, 1.); }
  if (p >= .99999) { return half4(B(vUv).rgb, 1.); }
  float4 color;
  if (uMode == 0) { color = morph(vUv, p); }
  else if (uMode == 1) { color = liquid(vUv, p); }
  else { color = mix(A(vUv), B(vUv), ease(p)); }
  return half4(color.rgb, 1.);
}
""";
}
