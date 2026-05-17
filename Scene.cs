using System;
using System.Collections.Generic;
using System.Text;

namespace Template
{
    public class RTScene
    {
        public List<Light> Lights { get; }

        public List<Primitive> Primitives { get; }

        public RTScene(List<Light> lights, List<Primitive> primitives)
        {
            Lights = lights;
            Primitives = primitives;
        }
    }
}
