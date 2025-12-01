using ImGuiNET;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Graphics
{
    public class ScenePickerGui
    {
        public ImGuiController controller;
        public ScenePickerGui(ImGuiController controller)
        {
            this.controller = controller;
        }

        public void Render(Window window, float deltaTime)
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(375, 100), ImGuiCond.Always);
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, 0), ImGuiCond.Always);

            ImGui.Begin("Scene Control", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);

            ImGui.Text("framerate: " + ImGui.GetIO().Framerate);
            ImGui.Separator();

            string current = $"Scene {window.sceneIndex}";
            if (ImGui.BeginCombo("Active Scene", current))
            {
                for (int i = 0; i < window.scenes.Count; i++)
                {
                    bool isSelected = (i == window.sceneIndex);
                    if (ImGui.Selectable($"Scene {i}", isSelected))
                    {
                        window.sceneIndex = i;
                    }
                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }

                }
                ImGui.EndCombo();
            }

            if(ImGui.Button("Add Scene"))
            {
                window.addScene(new Scene(window.controller,window));
            }

            ImGui.SameLine();

            if (ImGui.Button("Remove Scene") && window.scenes.Count > 1)
            {
                window.RemoveScene(window.scenes[window.sceneIndex]);

                if (window.sceneIndex >= window.scenes.Count)
                {
                    window.sceneIndex = window.scenes.Count - 1;
                }
            }

            ImGui.End();
        }
    }
}

