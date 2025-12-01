using ImGuiNET;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Graphics
{
    internal class FilePicker
    {
        public bool IsOpen = false;
        private string currentPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private string selectedFile = null;

        private readonly string[] allowedExtensions;

        public Action<string> OnFileSelected;

        public FilePicker(params string[] extensions)
        {
            allowedExtensions = extensions != null && extensions.Length > 0
                ? extensions.Select(e => e.ToLowerInvariant()).ToArray()
                : new[] { ".*" };
        }

        public void Open() => IsOpen = true;

        public void Draw()
        {
            if (!IsOpen) return;

            ImGui.Begin("Choose a file", ref IsOpen, ImGuiWindowFlags.AlwaysAutoResize);

            if (ImGui.Button("Back"))
            {
                var parent = Directory.GetParent(currentPath);
                if (parent != null) currentPath = parent.FullName;
            }

            ImGui.Text("Path: " + currentPath);
            ImGui.Separator();

            ImGui.BeginChild("FileList", new Vector2(500, 300), ImGuiChildFlags.Borders);

            foreach (var dir in Directory.GetDirectories(currentPath))
            {
                if (ImGui.Selectable("[Folder] " + Path.GetFileName(dir)))
                    currentPath = dir;
            }

            foreach (var file in Directory.GetFiles(currentPath))
            {
                string ext = Path.GetExtension(file).ToLowerInvariant();
                if (allowedExtensions[0] != ".*" && !allowedExtensions.Contains(ext))
                    continue;

                if (ImGui.Selectable(Path.GetFileName(file)))
                    selectedFile = file;
            }

            ImGui.EndChild();
            ImGui.Separator();

            if (!string.IsNullOrEmpty(selectedFile))
            {
                ImGui.TextWrapped("Chosen: " + selectedFile);

                if (ImGui.Button("Load"))
                {
                    IsOpen = false;
                    OnFileSelected?.Invoke(selectedFile);
                    selectedFile = null;
                }

                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    IsOpen = false;
                    selectedFile = null;
                }
            }

            ImGui.End();
        }
    }
}
