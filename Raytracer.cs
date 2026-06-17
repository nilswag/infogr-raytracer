using Assimp;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Linq;

namespace Template
{
    public class Raytracer
    {
        // TODO: Laat het de debug output maken bij middelste y waarde
        // TODO: adapt the normal! (voor als je in een sphere zit bijv)

        // Surface waar de raytracer op gaat werken
        public Surface Surf { get; set; }

        // Camera
        public Camera Camera { get; set; }

        //Scene
        public RTScene Scene {get; set;}


        Color3 AmbientLight = new Color3(0.08f, 0.08f, 0.08f);

        float epsilon = 0.0001f;

        int depth_max = 8;

        int[] pixelOrder;
        int currentPixel = 0;
        int pixelsPerFrame = 21954;

        public Raytracer(Surface surf, Camera camera, RTScene scene)
        {
            Surf = surf;
            Camera = camera;
            Scene = scene;

            int renderWidth = surf.width / 2;
            pixelOrder = Enumerable.Range(0, renderWidth * surf.height).ToArray();
            var rng = new Random();
            for (int i = pixelOrder.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (pixelOrder[i], pixelOrder[j]) = (pixelOrder[j], pixelOrder[i]);
            }
        }

        public void Refresh()
        {
            currentPixel = 0;
            Surf.Clear(0);

            int renderWidth = Surf.width / 2;
            pixelOrder = Enumerable.Range(0, renderWidth * Surf.height).ToArray();

            var rng = new Random();
            for (int i = pixelOrder.Length - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (pixelOrder[i], pixelOrder[j]) = (pixelOrder[j], pixelOrder[i]);
            }
        }

        public void Render()
        {
            // Reserve the left half of the screen for the raytraced image
            // Right half will be used for the debug visualization
            int renderWidth = Surf.width / 2;
            int renderHeight = Surf.height;

            // The image plane is defined by 4 corner points
            // u goes from top-left to top-right.
            // v goes from top-left to bottom-left.
            // two vectors, we can map a screen pixel to a 3D point on the camera plane.
            Vector3 u = Camera.ImagePlane[1] - Camera.ImagePlane[0];
            Vector3 v = Camera.ImagePlane[2] - Camera.ImagePlane[0];

            // Progressive rendering:
            // we do not render all pixels every frame,
            // only a chunk of them, so camera movement stays responsive.
            int start = currentPixel;
            int end = Math.Min(currentPixel + pixelsPerFrame, pixelOrder.Length);

            Parallel.For(start, end, n =>
            {
                // Convert shuffled 1D pixel index back to 2D coordinates.
                // Important: x is in [0, renderWidth), not the full screen width.
                int idx = pixelOrder[n];
                int x = idx % renderWidth;
                int y = idx / renderWidth;

                // Final accumulated pixel color.
                Color3 color = new Color3(0, 0, 0);

                // Supersampling: cast 9 rays through each pixel
                // (3x3 grid: 0, 0.5, 1 in both directions)
                // and average them for smoother edges.
                for (float i = 0f; i <= 1f; i += 0.5f)
                for (float j = 0f; j <= 1f; j += 0.5f)
                {
                    // Convert pixel position to normalized image-plane coordinates.
                    // a runs horizontally over the left render area only.
                    // b runs vertically over the full render height.
                    float a = (x + i) / renderWidth;
                    float b = (y + j) / renderHeight;

                    // Find the exact 3D point on the camera image plane
                    // that corresponds to this sample inside the pixel.
                    Vector3 planePoint = Camera.ImagePlane[0] + a * u + b * v;

                    // Primary ray direction:
                    // from camera position through the image plane sample point.
                    Vector3 direction = Vector3.Normalize(planePoint - Camera.Pos);

                    // Trace the ray into the scene and accumulate its returned color.
                    color += TraceRay(direction, Camera.Pos, 1);
                }

                // Average the 9 samples and write the final pixel to the screen buffer.
                Surf.Plot(x, y, color / 9);
            });

            // Move the progressive rendering window forward.
            currentPixel = end;

            // If reached the end of the shuffled pixel list,
            // wrap around so rendering continues.
            if (currentPixel >= pixelOrder.Length) currentPixel = 0;

            // Draw the debug panel in the right half of the screen.
            DrawDebug();
        }

        private void DrawDebug()
        {
            // The debug panel starts at the middle of the screen and occupies the entire right half
            int left = Surf.width / 2;
            int panelWidth = Surf.width - left;
            int panelHeight = Surf.height;

            // Fill the debug panel with a dark background and draw a white border around it
            Surf.Bar(left, 0, Surf.width - 1, Surf.height - 1, new Color3(0.08f, 0.08f, 0.08f));
            Surf.Box(left, 0, Surf.width - 1, Surf.height - 1, Color4.White);

            // Title text for the debug panel
            Surf.Print("DEBUG VIEW", left + 10, 10, Color4.White);

            // Scale factor for projecting world coordinates (x,z)
            // into the 2D top-down debug panel
            float scale = 12f;

            // Center point of the debug panel in screen coordinates
            Vector2 center = new Vector2(left + panelWidth / 2, panelHeight / 2);

            // Helper function:
            // converts a 3D world position into a 2D top down debug position
            // Using X horizontally and Z vertically for the top view
            Vector2 ToDebug(Vector3 p)
            {
                return new Vector2(center.X + p.X * scale, center.Y + p.Z * scale);
            }

            // Draw a small square marker, but only if it is fully inside the screen.
            void DrawMarker(Vector2 p, Color3 color)
            {
                int x = (int)p.X;
                int y = (int)p.Y;

                if (x - 2 < 0 || x + 2 >= Surf.width || y - 2 < 0 || y + 2 >= Surf.height)
                    return;

                Surf.Box(x - 2, y - 2, x + 2, y + 2, color);
            }

            // Camera position in debug space
            Vector2 cam = ToDebug(Camera.Pos);

            // A point a bit in front of the camera, used to draw the viewing direction arrow
            Vector2 camForward = ToDebug(Camera.Pos + Camera.Forward * 2f);

            // Draw the camera as a small yellow square
            DrawMarker(cam, Color4.Yellow);

            // Draw the camera forward direction as a yellow line
            Surf.Line((int)cam.X, (int)cam.Y, (int)camForward.X, (int)camForward.Y, Color4.Yellow);

            // Draw each primitive: spheres as circles, others as markers
            foreach (var obj in Scene.Primitives)
            {
                if (obj is Sphere sphere)
                {
                    int steps = 32;
                    for (int i = 0; i < steps; i++)
                    {
                        float angle1 = 2f * MathF.PI * i / steps;
                        float angle2 = 2f * MathF.PI * (i + 1) / steps;
                        Vector3 p1 = sphere.Pos + new Vector3(MathF.Cos(angle1) * sphere.Radius, 0, MathF.Sin(angle1) * sphere.Radius);
                        Vector3 p2 = sphere.Pos + new Vector3(MathF.Cos(angle2) * sphere.Radius, 0, MathF.Sin(angle2) * sphere.Radius);
                        Vector2 d1 = ToDebug(p1);
                        Vector2 d2 = ToDebug(p2);
                        Surf.Line((int)d1.X, (int)d1.Y, (int)d2.X, (int)d2.Y, Color4.Red);
                    }
                }
                else
                {
                    DrawMarker(ToDebug(obj.Pos), Color4.Red);
                }
            }

            // We choose the center pixel of the left render panel
            // as the debug ray we want to visualize
            int renderWidth = Surf.width / 2;
            int debugPixelX = renderWidth / 2;
            int debugPixelY = Surf.height / 2;

            // Reconstruct the camera image plane basis vectors
            // u = horizontal direction across the image plane
            // v = vertical direction across the image plane
            Vector3 u = Camera.ImagePlane[1] - Camera.ImagePlane[0];
            Vector3 v = Camera.ImagePlane[2] - Camera.ImagePlane[0];

            // Convert chosen pixel to normalized image plane coordinates
            float a = (debugPixelX + 0.5f) / renderWidth;
            float b = (debugPixelY + 0.5f) / Surf.height;

            // Compute exact 3D point on the image plane
            // corresponding to the chosen debug pixel
            Vector3 planePoint = Camera.ImagePlane[0] + a * u + b * v;

            // Generate primary ray direction for that pixel
            Vector3 dir = Vector3.Normalize(planePoint - Camera.Pos);

            // Find the closest intersection for the debug ray
            Intersection debugInter = null;
            float minDist = float.PositiveInfinity;
            foreach (var obj in Scene.Primitives)
            {
                Intersection inter = obj.Intersect(dir, Camera.Pos);
                if (inter != null && inter.Dist < minDist && inter.Dist > epsilon)
                {
                    debugInter = inter;
                    minDist = inter.Dist;
                }
            }

            if (debugInter != null)
            {
                // Primary ray, camera to intersection point
                Vector2 hitDebug = ToDebug(debugInter.Pos);
                Surf.Line((int)cam.X, (int)cam.Y, (int)hitDebug.X, (int)hitDebug.Y, Color4.Lime);

                // Shadow rays, intersection to each light
                foreach (var light in Scene.Lights)
                {
                    Vector3 shadowDir = Vector3.Normalize(light.Pos - debugInter.Pos);
                    float totalLength = (light.Pos - debugInter.Pos).Length;

                    bool blocked = false;
                    foreach (var obstacle in Scene.Primitives)
                    {
                        Intersection hit = obstacle.Intersect(shadowDir, debugInter.Pos);
                        if (hit != null && epsilon < hit.Dist && hit.Dist < totalLength - epsilon && hit.Prim != debugInter.Prim)
                        {
                            blocked = true;
                            break;
                        }
                    }

                    Vector2 lightDebug = ToDebug(light.Pos);
                    Color3 shadowColor = blocked ? Color4.Red : Color4.Yellow;
                    Surf.Line((int)hitDebug.X, (int)hitDebug.Y, (int)lightDebug.X, (int)lightDebug.Y, shadowColor);
                }

                // Reflected ray: only if the hit primitive is a mirror
                if (!debugInter.Prim.MirrorColor.Equals(new Color3(0f, 0f, 0f)))
                {
                    Vector3 n = debugInter.Norm;
                    Vector3 reflection = dir - 2 * n * Vector3.Dot(dir, n);
                    Vector2 reflEnd = ToDebug(debugInter.Pos + reflection * 10f);
                    Surf.Line((int)hitDebug.X, (int)hitDebug.Y, (int)reflEnd.X, (int)reflEnd.Y, Color4.Cyan);
                }
            }
            else
            {
                // No intersection, draw primary ray to fixed distance
                Vector2 rayEnd = ToDebug(Camera.Pos + dir * 20f);
                Surf.Line((int)cam.X, (int)cam.Y, (int)rayEnd.X, (int)rayEnd.Y, Color4.Lime);
            }

            // Legend
            Surf.Print("primary ray", left + 10, panelHeight - 70, Color4.Lime);
            Surf.Print("shadow ray (free)", left + 10, panelHeight - 55, Color4.Yellow);
            Surf.Print("shadow ray (blocked)", left + 10, panelHeight - 40, Color4.Red);
            Surf.Print("reflected ray", left + 10, panelHeight - 25, Color4.Cyan);

            // Print the current camera position for reference
            Surf.Print($"cam x: {Camera.Pos.X:F1}", left + 10, 30, Color4.White);
            Surf.Print($"cam y: {Camera.Pos.Y:F1}", left + 10, 45, Color4.White);
            Surf.Print($"cam z: {Camera.Pos.Z:F1}", left + 10, 60, Color4.White);
        }        public Color3 TraceRay(Vector3 direction, Vector3 origin, int depth)
        {
            //Finding the closest intersection
            Intersection closestInter = new Intersection(Camera.Pos, float.PositiveInfinity, new Plane(Camera.Pos, Camera.Pos, new Color3(0f, 0f, 0f)), Camera.Pos); //random values
            foreach (var obj in Scene.Primitives)
            {
                Intersection inter = obj.Intersect(direction, origin);
                if (inter != null && inter.Dist < closestInter.Dist && inter.Dist > epsilon)
                {
                    closestInter = inter;
                }
            }

            //Shading this intersection correctly but only if one was found
            Color3 color = new Color3(0f, 0f, 0f); //black as basis
            if(closestInter.Dist < float.PositiveInfinity)
            {

                Color3 kd = closestInter.Prim.Color;
                Color3 ks = closestInter.Prim.SpecularColor;
                Color3 km = closestInter.Prim.MirrorColor;
                int spec = closestInter.Prim.Specularity;

                //If the intersection object is a mirror, shoot the reflected ray with recursion
                if(!km.Equals(new Color3(0f, 0f, 0f)) && depth < depth_max)
                {
                    Vector3 n = closestInter.Norm;
                    Vector3 reflection = direction - 2 * n * Vector3.Dot(direction, n);
                    color += km * TraceRay(reflection, closestInter.Pos, depth + 1);                    
                }

                color += AmbientLight * kd; //Add ambient lighting 

                //Go through all lights in the scene, and add their effects
                foreach (var light in Scene.Lights)
                {  
                    Vector3 shadowRay = Vector3.Normalize(light.Pos-closestInter.Pos);

                    //If it is a spotlight, check whether intersection is within the cone.
                    if(light.Direction != new Vector3(0,0,0) && Vector3.Dot(light.Direction, -1 * shadowRay) < light.CutOff)
                    {
                        continue;
                    }

                    bool hits = false;

                    //Check if shadowray hits obstacle on the way to light
                    foreach (var obstacle in Scene.Primitives){
                        Intersection hit = obstacle.Intersect(shadowRay, closestInter.Pos);
                        float totalLength = (light.Pos - closestInter.Pos).Length;
                        if (hit != null && epsilon < hit.Dist && hit.Dist < totalLength - epsilon && hit.Prim != closestInter.Prim)
                        {
                            hits = true;
                        }
                    
                    }
                    //If no obstacl ewas hit by the shadowray, calculate the color of the pixel with the lights effects
                    if (!hits)
                    {
                        float radiusSquared = (light.Pos-closestInter.Pos).Length * (light.Pos-closestInter.Pos).Length;
                        Vector3 n = closestInter.Norm;
                        Vector3 reflection = shadowRay - 2 * n * Vector3.Dot(shadowRay, n);
                        color += (light.Intensity/radiusSquared) * 
                         (kd * MathF.Max(0, Vector3.Dot(closestInter.Norm, shadowRay)) + 
                         ks * MathF.Pow(MathF.Max(0,Vector3.Dot(direction, reflection)), spec));
                    }

                }  
            }
            return color;       
        }
    }
}