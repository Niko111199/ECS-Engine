using Graphics;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graphics
{
    public class Window : GameWindow
    {
        public List<Scene> scenes = new List<Scene>();
        public ImGuiController controller;
        public int sceneIndex = 0;
        public int sceneNumber = 0;
        public ScenePickerGui scenePickerGui;
        public bool hideGui = false;
        public int width;
        public int height;

        public Window(int width, int height, string title)
            : base(GameWindowSettings.Default,
                   new NativeWindowSettings() { Size = new Vector2i(width, height), Title = title })
        {
            this.width = width;
            this.height = height;
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            controller = new ImGuiController(FramebufferSize.X, FramebufferSize.Y);

            scenes.Add(new Scene(controller,this));

            scenePickerGui = new ScenePickerGui(controller);

            GL.Viewport(0, 0, FramebufferSize.X, FramebufferSize.Y);
            ImGuiNET.ImGui.GetIO().FontGlobalScale = 1.0f;
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            controller.WindowResized(e.Width, e.Height);

            width = e.Width;
            height = e.Height;

            scenes[sceneIndex].activeCamera.UpdateAspectRatio(e.Width, e.Height);
            scenes[sceneIndex].frameBuffer.Resize(e.Width, e.Height);
            scenes[sceneIndex].postProcessor.Resize(e.Width, e.Height);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.ClearColor(scenes[sceneIndex].backGroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            scenes[sceneIndex].Render((float)e.Time);

            controller.Update(this, (float)e.Time);

            if (!hideGui)
            {
                scenes[sceneIndex].gui.Render(this, (float)e.Time, ref scenes[sceneIndex].backGroundColor);
                scenePickerGui.Render(this, (float)e.Time);
            }

            controller.Render();
            SwapBuffers();

            if (KeyboardState.IsKeyPressed(Keys.H))
            {
                if(hideGui)
                {
                    hideGui = false;
                }
                else
                {
                    hideGui = true;
                }
            }

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            var io = ImGuiNET.ImGui.GetIO();
            if (e.AsString.Length > 0)
            {
                foreach (char c in e.AsString)
                    io.AddInputCharacter(c);
            }
        }

        public void addScene(Scene scene)
        {
            sceneNumber++;
            scene.sceneId = sceneNumber;
            scenes.Add(scene);
            Console.WriteLine($"Scene {sceneNumber} added Total scenes:  {scenes.Count}");
        }

        public void RemoveScene(Scene scene)
        {
            scenes.Remove(scene);
            sceneNumber--;
            Console.WriteLine($"Scene {sceneNumber} removed. Total scenes: {scenes.Count}");
        }
    }
}
