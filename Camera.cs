using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Template
{
    public class Camera
    {
        // Positie van camera
        private Vector3 Pos { get; set; }

        // Punt waar camera naartoe kijkt
        private Vector3 LookAt { get; set; }

        // Richting wat boven is vanuit de camera
        private Vector3 UpDirection { get; set; }

        // Array van hoeken die het scherm vlak bepalen
        private Vector3[] Screen { get; set; }

        public Camera()
        {
            Pos = new Vector3(0.0f, 0.0f, 0.0f);
            LookAt = new Vector3(0.0f, 0.0f, 1.0f);
            UpDirection = new Vector3(0.0f, 1.0f, 0.0f);
            Screen = [
                // Deze hoeken van het scherm kun je in template.cs vinden (TODO: waardes niet hardcoden)
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(640.0f, 0.0f, 0.0f),
                new Vector3(640.0f, 400.0f, 0.0f),
                new Vector3(0.0f, 400.0f, 0.0f),
                ];
        }
    }
}
