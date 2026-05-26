using Assimp;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Template
{
    public class Intersection
    {      
        // Position of intersection
        public Vector3 Pos { get; set; }

         // Distance of intersection to start point
        public float Dist { get; set; }
        
        // Primitive being intersected with
        public Primitive Prim { get; set; }

         // Normal at intersection point
        public Vector3 Norm { get; set; }

        public Intersection(Vector3 pos, float dist, Primitive prim, Vector3 norm)
        {
            Pos = pos;
            Dist = dist;
            Prim = prim;
            Norm = norm;
        }
    }
}