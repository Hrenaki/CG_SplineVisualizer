using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_SplineVisualizer
{
    interface ICamera
    {
        Vector3 Position { get; set; }
        Matrix4 Projection { get; }
        Matrix4 View { get; }
        float Speed { get; set; }
        float NearPlane { get; set; }
        float FarPlane { get; set; }
        void Update();
        void OnMouseWheel(object sender, MouseWheelEventArgs e);
    }
}
