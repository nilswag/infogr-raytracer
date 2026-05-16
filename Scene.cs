using System;
using System.Collections.Generic;
using System.Text;

namespace Template
{
    public class Scene
    {
        public List<Light> Lights { get; }

        public List<Primitive> Primitives { get; }

        public Scene(List<Light> lights, List<Primitive> primitives)
        {
            Lights = lights;
            Primitives = primitives;
        }
    }
}
