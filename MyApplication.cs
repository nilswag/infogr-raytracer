using OpenTK.Mathematics;
using SixLabors.ImageSharp.Formats.Pbm;
using System.Diagnostics;
using System.Globalization;

namespace Template;

class MyApplication
{
    // member variables
    public Surface screen;
    private readonly Stopwatch timer = new();

    private Raytracer rayTracer;
    private Camera camera;
    public Dictionary<string, RTScene> scenes { get; }

    private int targetPrimitiveIndex = 1;

    // constructor
    public MyApplication(Surface screen)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        this.screen = screen;
        camera = new Camera(Vector3.Zero, Vector3.UnitZ, fov: 120f, aspectRatio: screen.width / (float)screen.height);
        scenes = [];
        rayTracer = new Raytracer(screen, camera, new RTScene([], [])); //empty scene untill loaded differently
    }

    // initialize
    public void Init()
    {
        //(optional) example of how you can load a triangle mesh in any file format supported by Assimp
        MeshT mesh = Util.ImportMesh("assets/cube.obj");

        scenes["basic"] = new RTScene(
        [
                    //  position                    color/intensity             //direction            //cutoff
            new Light(new Vector3(-8f, 5f, 5f), new Color3(100f, 100f, 100f)),
            new Light(new Vector3(-10f, 3f, 5f), new Color3(100f, 100f, 100f), new Vector3(2f, -2f, 5f), 0.95f)
        ],
        [
                    //  center                   radius     color                 specularcolor     specularity   mirrorcolor
            new Sphere(new Vector3(-5f, 0f, 13f), 2f, new Color3(1f, 0f, 0f), new Color3(1f, 0.5f, 0f), 10, new Color3(0f, 0f, 0f)),
            new Sphere(new Vector3(0f, 0f, 12f), 2f, new Color3(0f, 1f, 0f), new Color3(0.3f, 0.5f, 0f), 10, new Color3(0.5f, 0.5f, 0.5f)),
            new Sphere(new Vector3(5f, 0f, 14f), 2f, new Color3(0f, 0f, 0f), new Color3(0f, 0f, 0f), 1, new Color3(1f, 1f, 1f)),
            new Sphere(new Vector3(5f, 0f, 0f), 2f, new Color3(0f, 0f, 1f), new Color3(0f, 0f, 0f), 1, new Color3(0f, 0f, 0f)),
                    //    normal                    position                    color                       specularcolor           specularity     mirrorcolor
            new Plane(new Vector3(0f, 1f, 0f), new Vector3(0f, -5f, 0f), new Color3(0.5f, 0.5f, 0.5f)),
            new Plane(new Vector3(0f, 0f, -1f), new Vector3(0f, 0f, 25f), new Color3(0.3f, 0.2f, 0.5f), new Color3(0.2f, 0.2f, 0.2f), 10, new Color3(0f, 0f, 0f)),
                    //      
            new Triangle(new Vector3(-10f, -3f, 13f), new Vector3(-15f, 0f, 13f), new Vector3(-8f, 5f, 13f), new Color3(1f, 0f, 0f))
        ]);

        foreach(Triangle tr in mesh.Triangles)
        {
            scenes["basic"].Primitives.Add(tr);
        }

        // TODO: possibly more scene initializations

        //TODO: Maybe allow users to pick scene --> method?
        rayTracer.Scene = scenes["basic"];
        camera.SetPoints();
    }

    public void RotateCamera(float yawDelta, float pitchDelta)
    {
        camera.Rotate(yawDelta, pitchDelta);
    }
    public Vector3 GetForward() => camera.Forward;
    public Vector3 GetRight() => camera.Right;
    public Vector3 GetUp() => camera.Up;
    public void SetTargetPrimitiveIndex(int index)
    {
        targetPrimitiveIndex = index;
        UpdateCameraTarget();
    }

    public void MoveCamera(Vector3 delta)
    {
        camera.Pos += delta;
        camera.SetPoints();
    }

    public void AdjustFOV(float delta)
    {
        camera.FOV = Math.Clamp(camera.FOV + delta, 20f, 150f);
        camera.SetPoints();

        rayTracer.Refresh();
    }

    private void UpdateCameraTarget()
    {
        if (rayTracer.Scene.Primitives.Count == 0) return;

        targetPrimitiveIndex = Math.Clamp(targetPrimitiveIndex, 0, rayTracer.Scene.Primitives.Count - 1);
        Vector3 target = rayTracer.Scene.Primitives[targetPrimitiveIndex].Pos;
        Vector3 dir = Vector3.Normalize(target - camera.Pos);
        camera.Rotate(
            MathHelper.RadiansToDegrees(MathF.Atan2(dir.Z, dir.X)) - camera.Yaw,
            MathHelper.RadiansToDegrees(MathF.Asin(dir.Y)) - camera.Pitch
        );
    }

    // tick: renders one frame
    private TimeSpan deltaTime = new();
    private uint frames = 0;
    private string timeString = "---- ms/frame";

    public void Tick()
    {
        timer.Restart();

        //screen.Clear(0);

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

    // Needed target swap implemented with arrow keys in template.cs (puts a black screen at some point as target idk why and start with green ball)
    public void NextTarget()
    {
        if (rayTracer.Scene.Primitives.Count == 0) return;
        targetPrimitiveIndex = (targetPrimitiveIndex + 1) % rayTracer.Scene.Primitives.Count;
        UpdateCameraTarget();
    }

    public void PreviousTarget()
    {
        if (rayTracer.Scene.Primitives.Count == 0) return;
        targetPrimitiveIndex = (targetPrimitiveIndex - 1 + rayTracer.Scene.Primitives.Count) % rayTracer.Scene.Primitives.Count;
        UpdateCameraTarget();
    }
}