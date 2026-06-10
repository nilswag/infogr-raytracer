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

        public Raytracer(Surface surf, Camera camera, RTScene scene)
        {
            Surf = surf;
            Camera = camera;
            Scene = scene;
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

            for (int x = 0; x < Surf.width; x++)
            {
                for (int y = 0; y < Surf.height; y++)
                {
                    
                    //Now loop through grid of 9 rays and average the color
                    Color3 color = new Color3(0,0,0);

                    for(float i = 0f; i <= 1f; i=i+0.5f)
                        for(float j = 0f; j <= 1f; j = j + 0.5f)
                        {
                            float a = (x+i)/Surf.width;
                            float b = (y+j)/Surf.height;
                            
                            Vector3 planePoint = Camera.ImagePlane[0] + a * u + b * v;
                            Vector3 direction = Vector3.Normalize(planePoint - Camera.Pos);

                            color += TraceRay(direction, Camera.Pos, 1);
                        }

                    
                    Surf.Plot(x, y, color/9);
                }
            }
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

                //If the intersection object is a mirror, shoot the reflected ray with recursion
                if(!km.Equals(new Color3(0f, 0f, 0f)) && depth < depth_max)
                {
                    Vector3 n = closestInter.Norm;
                    Vector3 reflection = direction - 2 * n * Vector3.Dot(direction, n);
                    color += km * TraceRay(reflection, closestInter.Pos, depth + 1);                    
                }

                color += AmbientLight * kd; //Add ambient lighting 

                //Go through all lights in the scene, and add their effects.
                foreach (var light in Scene.Lights)
                {  
                    Vector3 shadowRay = Vector3.Normalize(light.Pos-closestInter.Pos);

                    //If it is a spotlight, check whether intersection is within the cone
                    if(light.Direction != new Vector3(0,0,0) && Vector3.Dot(light.Direction, -1 * shadowRay) < light.CutOff)
                    {
                        continue;
                    }

                    bool hits = false;

                    //Check if the shadowray hits obstacle on the way to light
                    foreach (var obstacle in Scene.Primitives){
                        Intersection hit = obstacle.Intersect(shadowRay, closestInter.Pos);
                        float totalLength = (light.Pos - closestInter.Pos).Length;
                        if (hit != null && epsilon < hit.Dist && hit.Dist < totalLength - epsilon && hit.Prim != closestInter.Prim)
                        {
                            hits = true;
                        }
                    
                    }
                    //If no obstacle was hit by the shadowray, calculate the color of the pixel with the lights effects
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