Team members: (names and student IDs)
* Furkan 0383570
* Nils 9397388
* Lisa 0819514

Tick the boxes below for the implemented features. Add a brief note only if necessary, e.g., if it's only partially working, or how to turn it on.

Formalities:
[X] This readme.txt
[X] Cleaned (no obj/bin folders)

Minimum requirements implemented:
[X] Camera: position and orientation controls, field of view in degrees
Controls: WASD for front/left/back/right, E for up, Q for down. Hold down right mouse and point to target the camera
[X] Primitives: plane, sphere
[X] Lights: at least 2 point lights, additive contribution, shadows without "acne"
[X] Diffuse shading: (N.L), distance attenuation
[X] Phong shading: (R.V) or (N.H), exponent
[X] Diffuse color texture: only required on the plane primitive, image or procedural, (u,v) texture coordinates
[X] Mirror reflection: recursive
[X] Debug visualization: sphere primitives, rays (primary, shadow, reflected, refracted)

Bonus features implemented:
[X] Triangle primitives: must use the algorithm from the lectures, single triangles or meshes
[X] Interpolated normals: only required on triangle primitives, 3 different vertex normals must be specified
[X] Spot lights: smooth falloff optional
[X] Glossy reflections: not only of light sources but of other objects
[X] Anti-aliasing
[X] Parallelized
Method: Parallel.For 
[X] Textures: on all implemented primitives
[ ] Bump or normal mapping: on all implemented primitives
[ ] Environment mapping: sphere or cube map, without intersecting actual sphere/cube/triangle primitives
[~] Refraction: also requires a reflected ray at every refractive surface, recursive
[X] Area lights: soft shadows
[ ] Scene graph: nodes containing transformations, 3D models or primitives, and child nodes; flattening optional
[ ] Acceleration structure: bounding box or hierarchy, scene with 5000+ primitives
Performance comparison: ... (provide one measurement of speed/time with and without the acceleration structure)
[ ] GPU implementation
Method: ... (for example: fragment shader, compute shader, ILGPU, or CUDA)

Notes:
IMPORTANT: We loaded in the provided mesh, but we didnt move it from its origin position.
So you need to move a few steps forward with W before the scene becomes visible. 
For glossy reflections: right now sample size is quite low to maintain good fps. Can be increased for testing.
For area lights: right now sample size is quite low to maintain good fps. Can be increased for testing.
We added sources as in-code comments
Refraction we only partially managed to do: only in the debug output. But for adding it
to the actual rendering we didnt have enough time