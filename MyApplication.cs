using OpenTK.Mathematics;
using System.Diagnostics;
using System.Globalization;

namespace Template
{
    class MyApplication
    {
        // member variables
        public Surface screen;
        private readonly Stopwatch timer = new();

        private Raytracer Raytracer;
        public Dictionary<string, RTScene> Scenes { get; }

        // constructor
        public MyApplication(Surface screen)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            this.screen = screen;

            Raytracer = new Raytracer();
            Scenes = [];
        }

        // initialize
        public void Init()
        {
            // (optional) example of how you can load a triangle mesh in any file format supported by Assimp
            object? mesh = Util.ImportMesh("../../../assets/cube.obj");

            Scenes["basic"] = new RTScene(
                [], // array van lights, is voor nu leeg gezien we er niks mee doen
                [
                    //          x   y   z   r    red  green blue
                    new Sphere(-5f, 0f, 5f, 2f, 255f, 0f, 0f),
                    new Sphere(0f, 0f, 5f, 2f, 0f, 255, 0f),
                    new Sphere(5f, 0f, 5f, 2f, 0f, 0f, 255f)
                ]
            );
        }

        // tick: renders one frame
        private TimeSpan deltaTime = new();
        private uint frames = 0;
        private string timeString = "---- ms/frame";
        public void Tick()
        {
            timer.Restart();

            screen.Clear(0);

            Raytracer.Render(Scenes["basic"], screen);

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