# JACAMENOVILLE - Technical Art Direction Guide

## Visual Target Reference
**Aesthetic:** "Hybrid High-Fidelity"  
**References:**
- **Vehicles/Environment:** Need for Speed, Forza Motorsport
- **UI/Interaction:** The Sims 4, Inzoi
- **World Feel:** GTA V (urban density, life simulation)

---

## I. Lighting Configuration (NFS Look)

### Directional Light Setup
```yaml
Main Sun Light:
  Type: Directional
  Color: 
    Day: RGB(255, 245, 235)  # Warm sunlight
    Sunset: RGB(255, 180, 100)  # Golden hour
    Night: RGB(100, 120, 180)  # Cool moonlight
  Intensity:
    Day: 1.2 - 1.5
    Overcast: 0.6 - 0.8
    Night: 0.1 - 0.3
  Shadow:
    Type: Soft Shadows
    Resolution: 2048 (PC), 1024 (Mobile)
    Distance: 150m
    Cascades: 4
```

### Ambient Lighting
```yaml
Environment Lighting:
  Source: Skybox
  Intensity Multiplier: 1.0
  
Ambient Mode: Gradient
  Sky Color: RGB(135, 190, 235)
  Equator Color: RGB(180, 180, 180)
  Ground Color: RGB(80, 70, 60)
```

### Time-of-Day Color Grades

| Time | Sky Color | Light Temp | Fog Color |
|------|-----------|------------|-----------|
| 6:00 (Dawn) | #FFE4B5 | 4500K | #FFD4AA |
| 12:00 (Noon) | #87CEEB | 6500K | #E0E0E0 |
| 18:00 (Sunset) | #FF6B35 | 3200K | #FF8C69 |
| 21:00 (Night) | #1A1A3A | 8000K | #2A2A4A |

---

## II. Post-Processing Stack (URP)

### Volume Profile Settings

```yaml
Global Volume:
  Mode: Global
  Priority: 0
  Weight: 1.0
```

### Bloom Configuration (NFS Signature)
```yaml
Bloom:
  Enabled: true
  Threshold: 0.9
  Intensity: 0.8
  Scatter: 0.7
  Clamp: 65472
  Tint: RGB(255, 255, 255)
  
  # For neon signs / night scenes:
  High Intensity Mode:
    Threshold: 0.7
    Intensity: 1.5
```

### Color Grading (Cinematic Look)
```yaml
Color Grading:
  Mode: High Definition Range
  
  # Tone Mapping
  Tonemapping Mode: ACES  # Film-like response curve
  
  # White Balance
  Temperature: 0 (neutral, adjust per time of day)
  Tint: 0
  
  # Color Adjustments
  Post Exposure: 0.5
  Contrast: 10
  Color Filter: RGB(255, 252, 248)  # Slight warm tint
  Saturation: 10
  
  # Lift/Gamma/Gain (NFS Style)
  Lift:
    Trackball: RGB(0.95, 0.95, 1.0)  # Slight blue in shadows
  Gamma:
    Trackball: RGB(1.0, 1.0, 1.0)
  Gain:
    Trackball: RGB(1.05, 1.02, 1.0)  # Warm highlights
```

### Ambient Occlusion
```yaml
Ambient Occlusion:
  Enabled: true
  Method: Multi Scale Volumetric Obscurance
  Intensity: 0.5
  Radius: 0.3
  Direct Lighting Strength: 0.25
```

### Depth of Field (Cinematic Shots Only)
```yaml
Depth of Field:
  Mode: Bokeh
  Focus Distance: Variable (gameplay: off, cutscenes: on)
  Aperture: f/2.8
  Blade Count: 5
```

### Motion Blur (Optional - NFS Feel)
```yaml
Motion Blur:
  Enabled: true (PC only)
  Mode: Camera and Objects
  Quality: High
  Intensity: 0.5
  Clamp: 0.05
```

### Film Grain (Subtle)
```yaml
Film Grain:
  Type: Medium
  Intensity: 0.1
  Response: 0.8
```

---

## III. Reflection Setup (Car Shine)

### Reflection Probes
```yaml
Probe Distribution:
  Spacing: 20-30 meters in urban areas
  
Probe Settings:
  Type: Baked (static) + Realtime (hero vehicles)
  Resolution: 256 (PC), 128 (Mobile)
  HDR: Enabled
  Box Projection: Enabled
  Shadow Distance: 100
  
Realtime Probes (for player vehicle):
  Refresh Mode: Every Frame
  Time Slicing: Individual Faces
```

### Screen Space Reflections
```yaml
SSR (PC Only):
  Algorithm: PBR Accumulation
  Preset: High
  Max Ray Steps: 64
  Object Thickness: 0.1
```

---

## IV. Texture Budgets

### Resolution Guidelines

| Asset Type | PC Resolution | Mobile Resolution | Format |
|------------|---------------|-------------------|--------|
| Hero Vehicle | 4096x4096 | 2048x2048 | BC7 |
| NPC Characters | 2048x2048 | 1024x1024 | BC7 |
| Buildings (Hero) | 4096x4096 | 2048x2048 | BC7 |
| Buildings (Background) | 2048x2048 | 1024x1024 | BC7 |
| Props | 1024x1024 | 512x512 | BC7 |
| UI Elements | 2048x2048 atlas | 1024x1024 atlas | RGBA32 |
| Terrain | 2048x2048 | 1024x1024 | BC7 |
| Skybox | 4096 cube | 2048 cube | BC6H |

### Texture Maps Required per Material

```yaml
Standard PBR Material:
  - Albedo (RGB + Alpha for transparency)
  - Normal Map (RG compressed)
  - Metallic (R) + Smoothness (A) combined
  - Ambient Occlusion (R channel)
  - Optional: Emission map

Car Paint Material (Custom Shader):
  - Base Color
  - Clear Coat Mask
  - Metallic Flakes normal
  - Reflection Mask
```

### VRAM Budget

| Platform | Total VRAM Budget | Textures | Render Targets |
|----------|------------------|----------|----------------|
| PC (Mid) | 4GB | 2.5GB | 1.5GB |
| PC (High) | 8GB | 5GB | 3GB |
| Mobile | 1GB | 600MB | 400MB |

---

## V. Draw Call Limits

### Target Performance

| Platform | Target FPS | Max Draw Calls | Max Batches |
|----------|------------|----------------|-------------|
| PC (60fps) | 60 | 2000 | 1500 |
| PC (30fps) | 30 | 3500 | 2500 |
| Mobile | 30 | 500 | 350 |

### Optimization Strategies

```yaml
Static Batching:
  - All non-moving environment objects
  - Street props, buildings, rails
  
GPU Instancing:
  - Trees, light poles, traffic cones
  - Crowd NPCs (same material)
  
SRP Batcher:
  - Enable for all URP shaders
  - Use MaterialPropertyBlocks sparingly
  
LOD Groups:
  LOD0: Full detail (0-20m)
  LOD1: 50% triangles (20-50m)
  LOD2: 25% triangles (50-100m)
  LOD3: Billboard or cull (100m+)
```

---

## VI. Material Setup Guidelines

### Car Paint Shader (Custom Shader Graph)

```yaml
# Unity Shader Graph Properties (Create in Shader Graph editor)
Properties:
  Base Color:
    Type: Color
    Default: (1, 0, 0, 1)  # Red
  Metallic:
    Type: Float (Slider 0-1)
    Default: 0.9
  Smoothness:
    Type: Float (Slider 0-1)
    Default: 0.95
  Clear Coat:
    Type: Float (Slider 0-1)
    Default: 1.0
  Clear Coat Smoothness:
    Type: Float (Slider 0-1)
    Default: 0.98
  Metallic Flakes Normal Map:
    Type: Texture2D
    Default: "bump"
  Flake Scale:
    Type: Float
    Default: 50
  Fresnel Power:
    Type: Float
    Default: 5
```

### Standard Environment Material

```yaml
Building Material Template:
  Shader: Universal Render Pipeline/Lit
  Surface Type: Opaque
  Render Face: Front
  Alpha Clipping: Off
  
  Base Map: [Albedo texture]
  Normal Map: [Normal texture] Scale: 1.0
  Metallic: 0.0 - 0.2 (most buildings)
  Smoothness: 0.2 - 0.5
  Ambient Occlusion: [AO texture]
```

### Glass Material (Windows)

```yaml
Glass Material:
  Shader: Universal Render Pipeline/Lit
  Surface Type: Transparent
  Blending Mode: Alpha
  
  Base Map: Solid color
  Alpha: 0.2 - 0.4
  Smoothness: 0.95
  
  # Add reflection via Reflection Probe
```

### Neon/Emission Material

```yaml
Neon Sign Material:
  Shader: Universal Render Pipeline/Lit
  Surface Type: Opaque
  
  Emission: Enabled
  Emission Map: [Sign texture]
  Emission Color: HDR value (intensity 2-5)
  
  # Pair with Bloom post-processing
```

---

## VII. Quality Settings (Unity)

### PC High

```yaml
Quality Level: PC_High
  Pixel Light Count: 4
  Texture Quality: Full Res
  Anisotropic Textures: Forced On (16x)
  Anti Aliasing: 4x MSAA
  Soft Particles: Yes
  Realtime Reflection Probes: Yes
  Shadows:
    Distance: 150
    Cascades: 4
    Resolution: Very High (4096)
  LOD Bias: 2.0
  Maximum LOD Level: 0
```

### PC Medium

```yaml
Quality Level: PC_Medium
  Pixel Light Count: 2
  Texture Quality: Full Res
  Anisotropic Textures: Per Texture
  Anti Aliasing: 2x MSAA
  Soft Particles: Yes
  Realtime Reflection Probes: Yes
  Shadows:
    Distance: 100
    Cascades: 3
    Resolution: High (2048)
  LOD Bias: 1.5
  Maximum LOD Level: 0
```

### Mobile

```yaml
Quality Level: Mobile
  Pixel Light Count: 1
  Texture Quality: Half Res
  Anisotropic Textures: Disabled
  Anti Aliasing: 2x MSAA
  Soft Particles: No
  Realtime Reflection Probes: No (baked only)
  Shadows:
    Distance: 50
    Cascades: 2
    Resolution: Medium (1024)
  LOD Bias: 0.7
  Maximum LOD Level: 1
```

---

## VIII. Camera Settings

### Gameplay Camera

```yaml
Main Camera:
  Clear Flags: Skybox
  Background: Solid Color (fallback)
  Field of View: 60
  Near Clip: 0.3
  Far Clip: 1000
  
  Post Processing: Enabled
  Anti-aliasing: Post Process (SMAA)
  
  # URP Camera Settings
  Render Type: Base
  Renderer: PC_Renderer / Mobile_Renderer
```

### Cinematic Camera (Cutscenes)

```yaml
Cinematic Camera:
  Field of View: 45 (more telephoto)
  Depth of Field: Enabled
  Motion Blur: Enabled
  Letter Boxing: Optional (21:9 crop)
```

---

## IX. Weather Visual Settings

### Rain Effect
```yaml
Rain Particles:
  Emission: 5000/sec
  Lifetime: 0.5 sec
  Speed: 20-25
  Size: 0.01
  Material: Additive stretched billboard
  
Screen Effects:
  Wet surface shader (increase smoothness)
  Rain drops on camera (post-process)
  Fog density: +50%
```

### Fog/Overcast
```yaml
Fog Settings:
  Mode: Exponential Squared
  Color: Based on time of day
  Density: 
    Clear: 0.005
    Foggy: 0.02
    Industrial (Xero): 0.015
```

---

## X. District Visual Identity

### Fame District (Gold/Glamour)
```yaml
Accent Color: #FFD700 (Gold)
Lighting: Warm, lots of neon
Bloom: High intensity
Materials: Glossy, chrome accents
Atmosphere: Exciting, vibrant
```

### Remi District (Green/Peaceful)
```yaml
Accent Color: #90EE90 (Light Green)
Lighting: Soft, natural
Bloom: Low
Materials: Wood, brick, natural
Atmosphere: Calm, suburban
```

### Kiyo District (Blue/Tech)
```yaml
Accent Color: #00BFFF (Deep Sky Blue)
Lighting: Cool, LED-like
Bloom: Medium with cyan tints
Materials: Glass, aluminum, clean
Atmosphere: Modern, innovative
```

### Zenin District (Silver/Corporate)
```yaml
Accent Color: #C0C0C0 (Silver)
Lighting: Neutral, professional
Bloom: Low
Materials: Marble, glass, steel
Atmosphere: Serious, wealthy
```

### Xero District (Brown/Industrial)
```yaml
Accent Color: #8B4513 (Saddle Brown)
Lighting: Warm industrial
Bloom: Very low
Materials: Rust, concrete, painted metal
Atmosphere: Gritty, working-class
```

---

## XI. Performance Profiling Targets

### Frame Time Budget (60 FPS)

```yaml
Total Frame Time: 16.67ms

Budget Allocation:
  - Render Thread: 8ms
  - Game Logic: 4ms
  - Physics: 2ms
  - Audio: 1ms
  - UI: 1ms
  - Buffer: 0.67ms
```

### Profiler Markers to Monitor

```yaml
Critical Markers:
  - Render.Mesh
  - Shadows.RenderShadowMap
  - PostProcessing
  - Camera.Render
  - PlayerLoop
  
Warning Thresholds:
  - Single draw call > 1ms
  - GC Alloc per frame > 1KB
  - Physics step > 2ms
```

---

## XII. Implementation Checklist

### Phase 1: Basic Setup
- [ ] Configure URP Asset settings
- [ ] Create Global Post-Processing Volume
- [ ] Set up basic lighting rig
- [ ] Configure quality settings tiers

### Phase 2: Advanced Visuals
- [ ] Create car paint shader
- [ ] Place reflection probes
- [ ] Configure time-of-day system
- [ ] Implement weather effects

### Phase 3: Optimization
- [ ] Profile and optimize draw calls
- [ ] Implement LOD system
- [ ] Configure Addressables
- [ ] Test on minimum spec hardware

---

*Art Direction Guide generated by Technical Director audit system*
