using OpenTK;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CG_SplineVisualizer
{
    internal static class InputManager
    {
        public static event EventHandler<MouseButtonEventArgs> MouseDown;
        public static event EventHandler<MouseWheelEventArgs> MouseWheel;

        private static KeyboardState curKeyboardState;
        private static MouseState curMouseState;
        private static Vector2 curMouseDownPosition;

        internal static void Update()
        {
            curKeyboardState = Keyboard.GetState();
            curMouseState = Mouse.GetState();
            curMouseDownPosition = new Vector2();
        }

        public static bool IsKeyDown(Key key)
        {
            return curKeyboardState.IsKeyDown(key);
        }

        public static bool IsMouseButtonDown(MouseButton btn)
        {
            return curMouseState.IsButtonDown(btn);
        }

        public static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //PointAddEventArgs pointAddEventArgs01 = new PointAddEventArgs()
            //{
            //    MouseArgs = e,
            //    Projection = 
            //}
            MouseDown?.Invoke(sender, e);
        }

        public static void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            MouseWheel?.Invoke(sender, e);
        }

        public static Vector2 GetMouseDownPosition()
        {
            return new Vector2(curMouseDownPosition.X, curMouseDownPosition.Y);
        }
    }
}