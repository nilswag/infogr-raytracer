using OpenTK.Mathematics;
using SixLabors.ImageSharp.Formats.Pbm;
using System.Diagnostics;
using System.Globalization;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        private readonly Stopwatch timer = new();

        private Raytracer rayTracer;
        private Camera camera;
        public Dictionary<string, RTScene> scenes { get; }

        // constructor
        public MyApplication(Surface screen)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            this.screen = screen;
            camera = new Camera(Vector3.Zero, Vector3.UnitZ, fov: 120f, aspectRatio: screen.width/(float)screen.height);
            scenes = [];
            rayTracer = new Raytracer(screen, camera, new RTScene([], [])); //empty scene untill loaded differently
        }

        // initialize
        public void Init()
        {
            // (optional) example of how you can load a triangle mesh in any file format supported by Assimp
            object? mesh = Util.ImportMesh("assets/cube.obj"); //had to change it to this otherwise it wouldnt run...

            scenes["basic"] = new RTScene(
                [
                    //         x   y   z   r   g   b
                    new Light(-8f, 5f, 5f, 100f, 100f, 100f),
                    new Light(5f, 3f, 5f, 100f, 100f, 100f)
                ],
                [
                    //          x   y   z    rd   r   g   b  rs   gs   bs spec rm  gm  bm
                    new Sphere(-5f, 0f, 13f, 2f, 1f, 0f, 0f, 1f, 0.5f, 0f, 10, 0f, 0f, 0f),
                    new Sphere(0f, 0f, 12f, 2f, 0f, 1f, 0f, 0.3f, 0.5f, 0f, 10, 0.5f, 0.5f, 0.5f),
                    new Sphere(5f, 0f, 14f, 2f, 0f, 0f, 0f, 0f, 0f, 0f, 1, 1f, 1f, 1f), 
                    new Sphere(5f, 0f, 0f, 2f, 0f, 0f, 1f, 0f, 0f, 0f, 1, 0f, 0f, 0f), 
                    //        nx  ny  nz  px  py  pz  red   green blue
                    new Plane(0f, 1f, 0f, 0f, -5f, 0f, 0.5f, 0.5f, 0.5f),
                    new Plane(0f, 0f, -1f, 0f, 0f, 25f, 0.3f, 0.2f, 0.5f, 0.2f, 0.2f, 0.2f, 10, 0f, 0f, 0f)
                ]
            );

            // TODO: possibly more scene initializations

            //TODO: Maybe allow users to pick scene --> method?
            rayTracer.Scene = scenes["basic"];
        }

        // tick: renders one frame
        private TimeSpan deltaTime = new();
        private uint frames = 0;
        private string timeString = "---- ms/frame";
        public void Tick()
        {
            timer.Restart();

            screen.Clear(0);

            rayTracer.Render();

            deltaTime += timer.Elapsed;
            frames++;
            if (deltaTime.TotalSeconds > 1)
            {
                timeString = (deltaTime.TotalMilliseconds / frames).ToString("F1") + " ms/frame";
                frames = 0;
                deltaTime = TimeSpan.Zero;
            }


            screen.PrintOutlined(timeString, 2, 2, Color4.White);
        }
    }
}