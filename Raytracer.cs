using Assimp;
using OpenTK.Audio.OpenAL;
using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;

namespace Template
{
    public class Raytracer
    {

        // Surface waar de raytracer op gaat werken
        public Surface Surf { get; set; }

        // Camera
        public Camera Camera { get; set; }

        //Scene
        public RTScene Scene {get; set;}


        Color3 AmbientLight = new Color3(0.08f, 0.08f, 0.08f);

        float epsilon = 0.0001f;

        int depth_max = 3;

        int[] pixelOrder;
        int currentPixel = 0;
        int pixelsPerFrame = 21954;
        int randomSamplesGloss = 3; 
        int randomSamplesAreaLights = 3;

        //For keeping randomness intact during multithreading, we consulted with ChatGPT. The link of the conversation can
        //be found here: https://chatgpt.com/share/6a314fc4-7968-83eb-b7f2-59cdc5a84713
        static int seed = Environment.TickCount;
        ThreadLocal<Random> rnd = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        public Raytracer(Surface surf, Camera camera, RTScene scene)
        {
            Surf = surf;
            Camera = camera;
            Scene = scene;

            pixelOrder = Enumerable.Range(0, surf.width * surf.height).ToArray();
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
        }

        public void Render()
        { 
            // We use a grid to trace multiple rays through each pixel
            // We map each pixel (x,y) to a real point on the image plane.
            // Any point 𝑃(𝑎,𝑏) on the image plane has 3D location 𝑃0 + a * u + b * v 
            // where a = (x+0,5)/w and b = (y+0,5)/h, and 𝑎,𝑏 ∈ [0,1] inside the image rectangle. 
            // This point corresponds with the middle of pixel (x,y)
            Vector3 u = Camera.ImagePlane[1] - Camera.ImagePlane[0];
            Vector3 v = Camera.ImagePlane[2] - Camera.ImagePlane[0];

            int start = currentPixel;
            int end = Math.Min(currentPixel + pixelsPerFrame, pixelOrder.Length);

            Parallel.For(start, end, n =>
            {
                int idx = pixelOrder[n];
                int x = idx % Surf.width;
                int y = idx / Surf.width;

                //Now loop through grid of 9 rays and average the color
                Color3 color = new Color3(0, 0, 0);

                for (float i = 0f; i <= 1f; i = i + 0.5f)
                    for (float j = 0f; j <= 1f; j = j + 0.5f)
                    {
                        float a = (x + i) / Surf.width;
                        float b = (y + j) / Surf.height;

                        Vector3 planePoint = Camera.ImagePlane[0] + a * u + b * v;
                        Vector3 direction = Vector3.Normalize(planePoint - Camera.Pos);

                        color += TraceRay(direction, Camera.Pos, 1);
                    }

                Surf.Plot(x, y, color / 9);
            });

            currentPixel = end;
            if (currentPixel >= pixelOrder.Length)
                currentPixel = 0;
        }

        public Color3 TraceRay(Vector3 direction, Vector3 origin, int depth)
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

            //Shading this intersection correctly, but only if one was found.
            Color3 color = new Color3(0f, 0f, 0f); //black as basis
            if(closestInter.Dist < float.PositiveInfinity)
            {

                Color3 kd = closestInter.Prim.Color;
                Color3 ks = closestInter.Prim.SpecularColor;
                Color3 km = closestInter.Prim.MirrorColor;
                int spec = closestInter.Prim.Specularity;

                //If the intersection object is a non-glosssy mirror, shoot a perfectly reflected ray with recursion
                if(!km.Equals(new Color3(0f, 0f, 0f)) && depth < depth_max && ks.Equals(new Color3(0f, 0f, 0f)))
                {
                    Vector3 n = closestInter.Norm;
                    Vector3 reflection = direction - 2 * n * Vector3.Dot(direction, n);
                    color += km * TraceRay(reflection, closestInter.Pos, depth + 1);                    
                }

                //If the intersection object is a glossy mirror, shoot a blurry reflected ray with random sampling & recursion
                if(!km.Equals(new Color3(0f, 0f, 0f)) && depth < depth_max && !ks.Equals(new Color3(0f, 0f, 0f)))
                {
                    Vector3 n = closestInter.Norm;
                    Vector3 reflection = direction - 2 * n * Vector3.Dot(direction, n);
                    Color3 blurryColor = new Color3(0f,0f,0f);
                    float blurriness = Math.Clamp(1.0f/spec, 0.01f, 0.5f);

                    //Randomly sample near directions and average the result
                    for(int i = 0; i<randomSamplesGloss; i++)
                    {
                        //Create offset vector with each of 3 inputs in range -1, 1
                        Vector3 offset = new Vector3(-1.0f + 2.0f*(float)rnd.Value.NextDouble(), -1.0f + 2.0f*(float)rnd.Value.NextDouble(), -1.0f + 2.0f*(float)rnd.Value.NextDouble());
                        Vector3 glossyReflection = Vector3.Normalize(reflection + blurriness*offset);
                        blurryColor += TraceRay(glossyReflection, closestInter.Pos, depth + 1);
                    }
                    blurryColor /= randomSamplesGloss;

                    color += km * blurryColor;                    
                }

                color += AmbientLight * kd; //Add ambient lighting 

                //Go through all lights in the scene, and add their effects.
                foreach (var light in Scene.Lights)
                {   
                    Color3 colorLight = new Color3(0f,0f,0f);
                    bool isAreaLight = light.VerDir != new Vector3(0f,0f,0f) || light.HorDir != new Vector3(0f,0f,0f);
                    int counter = 0;

                    //For area lights we do random point sampling, for point or spotlights we use its position (once)
                    if (isAreaLight) counter = randomSamplesAreaLights; else counter = 1;

                    for (int i = 0; i<counter;i++){
                        Vector3 lightPoint = new Vector3(0f,0f,0f);
                        //For area lights we do random point sampling, for point or spotlights we use its position (once)
                        if (isAreaLight) lightPoint = RandomAreaLightPoint(light); else lightPoint = light.Pos;
                        Vector3 shadowRay = Vector3.Normalize(lightPoint-closestInter.Pos);

                        //If it is a spotlight, check whether intersection is within the cone, otherwise move on
                        if(light.Direction != new Vector3(0,0,0) && Vector3.Dot(light.Direction, -1 * shadowRay) < light.CutOff)
                        {
                            continue;
                        }

                        bool hits = false;

                        //Check if the shadowray hits obstacle on the way to light
                        foreach (var obstacle in Scene.Primitives){
                            Intersection hit = obstacle.Intersect(shadowRay, closestInter.Pos);
                            float totalLength = (lightPoint - closestInter.Pos).Length;
                            if (hit != null && epsilon < hit.Dist && hit.Dist < totalLength - epsilon && hit.Prim != closestInter.Prim)
                            {
                                hits = true;
                                break;
                            }
                        
                        }
                        //If no obstacle was hit by the shadowray, calculate the color of the pixel with the lights effects
                        if (!hits)
                        {
                            float radiusSquared = (lightPoint-closestInter.Pos).Length * (lightPoint-closestInter.Pos).Length;
                            Vector3 n = closestInter.Norm;
                            Vector3 reflection = shadowRay - 2 * n * Vector3.Dot(shadowRay, n);
                            colorLight += (light.Intensity/radiusSquared) * 
                            (kd * MathF.Max(0, Vector3.Dot(closestInter.Norm, shadowRay)) + 
                            ks * MathF.Pow(MathF.Max(0,Vector3.Dot(direction, reflection)), spec));
                        }
                    }
                    colorLight/=counter;
                    color += colorLight;
                }  
            }
            return color;       
        }

        public Vector3 RandomAreaLightPoint(Light light)
        {   //Get two random floats with ranges -0.5f, 0.5f
            float a = -0.5f + 1.0f*(float)rnd.Value.NextDouble();
            float b = -0.5f + 1.0f*(float)rnd.Value.NextDouble();

            return light.Pos + a * light.HorDir + b * light.VerDir;
        }
    }
}