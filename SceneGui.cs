using ImGuiNET;
using OpenTK.Mathematics;

namespace Graphics
{
    public class SceneGui
    {
        public ImGuiController controller;
        private int selectedEntityId = -1;
        private readonly Dictionary<int, string> _nameBuffers = new();
        private int _lastSelectedEntityId = -1;
        private readonly Scene owner;
        private FilePicker albedoPicker;
        private FilePicker specularPicker;
        private FilePicker normalPicker;
        private FilePicker lightpicker;
        private FilePicker projectorPicker;
        private FilePicker modelPicker;
        private FilePicker skyboxPickerPosX;
        private FilePicker skyboxPickerNegX;
        private FilePicker skyboxPickerPosY;
        private FilePicker skyboxPickerNegY;
        private FilePicker skyboxPickerPosZ;
        private FilePicker skyboxPickerNegZ;
        private FilePicker terrainPicker;

        public SceneGui(ImGuiController controller, Scene owner)
        {
            this.controller = controller;
            this.owner = owner;

            albedoPicker = new FilePicker(".tga");
            albedoPicker.OnFileSelected = (filePath) =>
            {
                if (selectedEntityId >= 0)
                {
                    var entity = owner.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                    if (entity != null)
                    {
                        var textureComp = entity.GetComponent<AlbedoMapComponenet>();
                        if (textureComp == null)
                        {
                            textureComp = new AlbedoMapComponenet(entity);
                            entity.AddComponent(textureComp);
                        }

                        textureComp.LoadTexture(filePath);
                    }
                }
            };

            specularPicker = new FilePicker(".tga");
            specularPicker.OnFileSelected += (filePath) =>
            { 
                if (selectedEntityId >= 0)
                {
                    var entity = owner.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                    if (entity != null)
                    {
                        var speularComp = entity.GetComponent<SpecularMapComponent>();
                        if (speularComp == null)
                        {
                            speularComp = new SpecularMapComponent(entity);
                            entity.AddComponent(speularComp);
                        }

                        speularComp.LoadTexture(filePath);
                    }
                }
            };

            normalPicker = new FilePicker(".tga");
            normalPicker.OnFileSelected += (filePath) =>
            {
                if (selectedEntityId >= 0)
                {
                    var entity = owner.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                    if (entity != null)
                    {
                        var normalComp = entity.GetComponent<NormalMapComponent>();
                        if (normalComp == null)
                        {
                            normalComp = new NormalMapComponent(entity);
                            entity.AddComponent(normalComp);
                        }

                        normalComp.LoadTexture(filePath);
                    }
                }
            };

            lightpicker = new FilePicker(".tga");
            lightpicker.OnFileSelected += (filePath) =>
            {
                if (selectedEntityId >= 0)
                {
                    var entity = owner.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                    if (entity != null)
                    {
                        var lightmapComp = entity.GetComponent<LightMapComponent>();
                        if (lightmapComp == null)
                        {
                            lightmapComp = new LightMapComponent(entity);
                            entity.AddComponent(lightmapComp);
                        }

                        lightmapComp.LoadTexture(filePath);
                    }
                }
            };

            projectorPicker = new FilePicker(".tga");
            projectorPicker.OnFileSelected += (filePath) =>
            {
                if (selectedEntityId >= 0)
                {
                    var entity = owner.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                    if (entity != null)
                    {
                        var projectorcomp = entity.GetComponent<TextureProjectorComponent>();
                        if (projectorcomp == null)
                        {
                            projectorcomp = new TextureProjectorComponent(entity);
                            entity.AddComponent(projectorcomp);
                        }

                        projectorcomp.LoadTexture(filePath);
                    }
                }
            };

            modelPicker = new FilePicker(".obj");
            modelPicker.OnFileSelected = (filePath) =>
            {
                if (selectedEntityId >= 0)
                {
                    var entity = owner.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                    if (entity != null)
                    {
                        var meshComp = entity.GetComponent<CustomeMeshComponent>();
                        if (meshComp == null)
                        {
                            meshComp = new CustomeMeshComponent(entity);
                            entity.AddComponent(meshComp);
                        }

                        meshComp.LoadModel(filePath);
                        Console.WriteLine($"Loaded model {filePath} into entity {entity.Id}");
                    }
                }
            };

            skyboxPickerPosX = new FilePicker(".tga");
            skyboxPickerPosX.OnFileSelected = (filePath) =>
            {
                var entity = owner.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                entity?.GetComponent<SkyboxComponent>()?.SetFace(0, filePath);
            };

            skyboxPickerNegX = new FilePicker(".tga");
            skyboxPickerNegX.OnFileSelected = (filePath) =>
            {
                var entity = owner.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                entity?.GetComponent<SkyboxComponent>()?.SetFace(1, filePath);
            };

            skyboxPickerPosY = new FilePicker(".tga");
            skyboxPickerPosY.OnFileSelected = (filePath) =>
            {
                    var entity = owner.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                    entity?.GetComponent<SkyboxComponent>()?.SetFace(2, filePath);
            };

            skyboxPickerNegY = new FilePicker(".tga");
            skyboxPickerNegY.OnFileSelected = (filePath) =>
            {
                    var entity = owner.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                    entity?.GetComponent<SkyboxComponent>()?.SetFace(3, filePath);
            };


            skyboxPickerPosZ = new FilePicker(".tga");
            skyboxPickerPosZ.OnFileSelected = (filePath) =>
            {
                    var entity = owner.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                    entity?.GetComponent<SkyboxComponent>()?.SetFace(4, filePath);
            };

            skyboxPickerNegZ = new FilePicker(".tga");
            skyboxPickerNegZ.OnFileSelected = (filePath) =>
            {        
                    var entity = owner.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                    entity?.GetComponent<SkyboxComponent>()?.SetFace(5, filePath);    
            };

            terrainPicker = new FilePicker(".tga");
            terrainPicker.OnFileSelected = (filePath) =>
            {
                if (selectedEntityId >= 0)
                {
                    var entity = owner.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                    if (entity != null)
                    {
                        var terrain = entity.GetComponent<TerrainComponent>();
                        if (terrain == null)
                        {
                            terrain = new TerrainComponent(entity);
                            entity.AddComponent(terrain);
                        }

                        terrain.ApplyHeightMap(filePath);
                        Console.WriteLine($"Loaded heightmap {filePath} into terrain component of entity {entity.Id}");
                    }
                }
            };
        }

        public void Render(Window window, float deltaTime, ref Color4 backgroundColor)
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(375, 850), ImGuiCond.Always);
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, 101), ImGuiCond.Always);
            ImGui.Begin($"Scene {window.sceneIndex}", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);

            var postProcessEffekt = owner.postProcessor.effekt;

            string currentLabel = postProcessEffekt.ToString();

            if (ImGui.BeginCombo("Post-processing", currentLabel))
            {
                foreach (PostProcessEffekt effekt in Enum.GetValues(typeof(PostProcessEffekt)))
                {
                    bool isSelected = (postProcessEffekt == effekt);

                    if (ImGui.Selectable(effekt.ToString(), isSelected))
                    {
                        owner.postProcessor.effekt = effekt;

                        owner.postProcessor.SetEffekt(effekt);
                    }

                    if (isSelected)
                        ImGui.SetItemDefaultFocus();
                }

                ImGui.EndCombo();
            }

            if (postProcessEffekt == PostProcessEffekt.brightness)
            {
                var contrast = owner.postProcessor.contrast;

                ImGui.DragFloat("Change brightness", ref owner.postProcessor.brightness,0.1f);
                ImGui.DragFloat("Change contrast", ref owner.postProcessor.contrast,0.1f);
            }

            if (postProcessEffekt == PostProcessEffekt.pixelate)
            {
                ImGui.DragFloat("pixelsize", ref owner.postProcessor.pixelSize, 0.1f, 0.1f, float.MaxValue);
            }

            if ((postProcessEffekt == PostProcessEffekt.chromaticAberration))
            {
                ImGui.DragFloat("shift amount", ref owner.postProcessor.shiftAmount);
            }

            var vec3Color = new System.Numerics.Vector3(backgroundColor.R, backgroundColor.G, backgroundColor.B);
            if (ImGui.ColorEdit3("Background Color", ref vec3Color))
            {
                backgroundColor = new Color4(vec3Color.X, vec3Color.Y, vec3Color.Z, 1.0f);
                Console.WriteLine($"scene {owner.sceneId} Background color changed to " + backgroundColor);
            }

            float ambient = owner.ambientLight;
            if (ImGui.SliderFloat("Ambient Light", ref ambient, 0.0f, 1.0f))
            {
                owner.ambientLight = ambient;
                Console.WriteLine($"Scene {owner.sceneId} ambient light set to {ambient}");
            }

            if (ImGui.Button("Add Entity"))
            {

                var entity = new Entity(owner.aspectRatio,owner);
                window.scenes[window.sceneIndex].AddEntity(entity);
            }

            ImGui.SameLine();

            var scene = window.scenes[window.sceneIndex];
            if (ImGui.Button("Remove Entity") && selectedEntityId >= 0)
            {
                var entityToRemove = scene.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                if (entityToRemove != null)
                {
                    scene.RemoveEntity(entityToRemove);
                    selectedEntityId = -1;
                }
            }

            ImGui.Separator();
            ImGui.Text("Entities:");

            foreach (var entity in scene.Entities)
            {
                bool isSelected = (entity.Id == selectedEntityId);


                if (ImGui.Selectable(entity.name, isSelected))
                {
                    selectedEntityId = entity.Id;
                }
                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }

            ImGui.End(); 

            if (selectedEntityId >= 0)
            {
                var selectedEntity = scene.Entities.FirstOrDefault(e => e.Id == selectedEntityId);
                var componentTypes = typeof(IComponent).Assembly
                                                       .GetTypes()
                                                       .Where(t => typeof(IComponent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
                if (selectedEntity != null)
                {
                    ImGui.SetNextWindowSize(new System.Numerics.Vector2(375, 950), ImGuiCond.Always);
                    ImGui.SetNextWindowPos(new System.Numerics.Vector2(1550,0), ImGuiCond.Always);
                    ImGui.Begin($"Entity {selectedEntity.Id}", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse);

                    if (selectedEntity != null)
                    {
                        if (selectedEntityId != _lastSelectedEntityId)
                        {
                            _nameBuffers[selectedEntityId] = selectedEntity.name;
                            _lastSelectedEntityId = selectedEntityId;
                        }


                        string buffer = _nameBuffers[selectedEntityId];
                        ImGui.InputText("Name", ref buffer, 64);
                        _nameBuffers[selectedEntityId] = buffer;
                        ImGui.SameLine();

                        if (ImGui.Button("Apply"))
                        {
                            if (!string.IsNullOrWhiteSpace(buffer))
                            {
                                selectedEntity.setName(buffer);
                                Console.WriteLine($"Entity {selectedEntity.Id} renamed to {buffer}");
                            }
                            else
                            {
                                Console.WriteLine("Name cannot be empty!");
                            }
                        }

                        if (ImGui.BeginCombo("Parent", selectedEntity.parent?.name ?? "None"))
                        {
                            foreach (var e in scene.Entities)
                            {
                                if (e != selectedEntity)
                                {
                                    bool isSelected = (selectedEntity.parent == e);
                                    if (ImGui.Selectable(e.name, isSelected))
                                    {
                                        selectedEntity.SetParent(e);
                                    }
                                    if (isSelected)
                                        ImGui.SetItemDefaultFocus();
                                }
                            }
                            ImGui.EndCombo();
                        }
                    }

                    if (ImGui.BeginCombo("Add Component", "Select Component"))
                    {
                        foreach (var type in componentTypes)
                        {
                            if (ImGui.Selectable(type.Name))
                            {
                                if (selectedEntity.GetComponent(type) == null)
                                {
                                    var comp = (IComponent)Activator.CreateInstance(type, selectedEntity);
                                    selectedEntity.AddComponent(comp);
                                    Console.WriteLine($"Added {type.Name} to entity {selectedEntity.Id}");
                                }
                            }
                        }
                        ImGui.EndCombo();
                    }

                    foreach (var comp in selectedEntity.Components.ToList())
                    {
                        ImGui.Separator();
                        if (comp is TransformComponent transform)
                        {
                            ImGui.Text("Transform");
                            var pos = new System.Numerics.Vector3(transform.Position.X, transform.Position.Y, transform.Position.Z);
                            if (ImGui.DragFloat3("Position", ref pos,0.1f))
                                transform.Position = new Vector3(pos.X, pos.Y, pos.Z);
                            var rot = new System.Numerics.Vector3(transform.Rotation.X, transform.Rotation.Y, transform.Rotation.Z);
                            if (ImGui.DragFloat3("Rotation", ref rot,0.1f))
                                transform.Rotation = new Vector3(rot.X, rot.Y, rot.Z);
                            var scale = new System.Numerics.Vector3(transform.Scale.X, transform.Scale.Y, transform.Scale.Z);
                            if (ImGui.DragFloat3("Scale", ref scale, 0.1f))
                                transform.Scale = new Vector3(scale.X, scale.Y, scale.Z);
                        }

                        if (comp is MeshComponent mesh )
                        {
                            ImGui.Text($"{mesh.Name} Mesh");
                            var color = new System.Numerics.Vector4(mesh.color.X, mesh.color.Y, mesh.color.Z, mesh.color.W);
                            if (ImGui.ColorEdit4("Color", ref color))
                            {
                                mesh.color = new Vector4(color.X, color.Y, color.Z, color.W);
                            }

                            if (comp is CustomeMeshComponent customeMesh)
                            {
                                if (ImGui.Button("Load Model"))
                                {
                                    modelPicker.Open();
                                }
                            }

                        }
                        if (comp is TerrainComponent terrain)
                        {
                            if (ImGui.Button("Load Heightmap"))
                                terrainPicker.Open();
                        }

                        if (comp is AlbedoMapComponenet texture)
                        {
                            ImGui.Text($"Texture: {Path.GetFileName(texture.Path)}");

                            bool useTex = selectedEntity.useTexture;
                            if (ImGui.Checkbox("Use Texture", ref useTex))
                            {
                                selectedEntity.SetUseTexture(useTex);
                            }

                            if (ImGui.Button("Change Texture"))
                            {
                                albedoPicker.Open();
                            }

                            if (ImGui.BeginCombo("Wrapping Albedo", texture.Wrapping.ToString()))
                            {
                                foreach (TextureWrapping wrap in Enum.GetValues(typeof(TextureWrapping)))
                                {
                                    if (ImGui.Selectable(wrap.ToString(), wrap == texture.Wrapping))
                                        texture.Wrapping = wrap;
                                }
                                ImGui.EndCombo();
                            }

                            if (ImGui.BeginCombo("Sampling Albedo", texture.Sampling.ToString()))
                            {
                                foreach (TextureSampling samp in Enum.GetValues(typeof(TextureSampling)))
                                {
                                    if (ImGui.Selectable(samp.ToString(), samp == texture.Sampling))
                                        texture.SetSampling(samp);
                                }
                                ImGui.EndCombo();
                            }

                            float aniso = texture.AnisotropicLevel;
                            if (ImGui.SliderFloat("Anisotropy Albedo", ref aniso, 1.0f, 16.0f))
                                texture.AnisotropicLevel = aniso;

                            System.Numerics.Vector2 uvScale = new System.Numerics.Vector2(texture.UVScale.X, texture.UVScale.Y);
                            if (ImGui.DragFloat2("UV Scale Albedo", ref uvScale, 0.1f))
                                texture.UVScale = new OpenTK.Mathematics.Vector2(uvScale.X, uvScale.Y);
                        }

                        if (comp is SpecularMapComponent spec)
                        {
                            ImGui.Text($"Specular Map: {Path.GetFileName(spec.Path)}");

                            if (ImGui.Button("Change Specular Map"))
                            {
                                specularPicker.Open();
                            }

                            if (ImGui.BeginCombo("Wrapping Specular", spec.Wrapping.ToString()))
                            {
                                foreach (TextureWrapping wrap in Enum.GetValues(typeof(TextureWrapping)))
                                {
                                    if (ImGui.Selectable(wrap.ToString(), wrap == spec.Wrapping))
                                        spec.Wrapping = wrap;
                                }
                                ImGui.EndCombo();
                            }

                            if (ImGui.BeginCombo("Sampling Specular", spec.Sampling.ToString()))
                            {
                                foreach (TextureSampling samp in Enum.GetValues(typeof(TextureSampling)))
                                {
                                    if (ImGui.Selectable(samp.ToString(), samp == spec.Sampling))
                                        spec.SetSampling(samp);
                                }
                                ImGui.EndCombo();
                            }

                            float aniso = spec.AnisotropicLevel;
                            if (ImGui.SliderFloat("Anisotropy Specular", ref aniso, 1.0f, 16.0f))
                                spec.AnisotropicLevel = aniso;
                        }

                        if (comp is NormalMapComponent norm)
                        {
                            ImGui.Text($"Normal Map: {Path.GetFileName(norm.Path)}");

                            if (ImGui.Button("Change normal Map"))
                            {
                                normalPicker.Open();
                            }

                            if (ImGui.BeginCombo("Wrapping Normal", norm.Wrapping.ToString()))
                            {
                                foreach (TextureWrapping wrap in Enum.GetValues(typeof(TextureWrapping)))
                                {
                                    if (ImGui.Selectable(wrap.ToString(), wrap == norm.Wrapping))
                                        norm.Wrapping = wrap;
                                }
                                ImGui.EndCombo();
                            }

                            if (ImGui.BeginCombo("Sampling Normal", norm.Sampling.ToString()))
                            {
                                foreach (TextureSampling samp in Enum.GetValues(typeof(TextureSampling)))
                                {
                                    if (ImGui.Selectable(samp.ToString(), samp == norm.Sampling))
                                        norm.SetSampling(samp);
                                }
                                ImGui.EndCombo();
                            }

                            float aniso = norm.AnisotropicLevel;
                            if (ImGui.SliderFloat("Anisotropy Normal", ref aniso, 1.0f, 16.0f))
                                norm.AnisotropicLevel = aniso;
                        }

                        if (comp is LightMapComponent lightmap)
                        {
                            ImGui.Text($"Light Map: {Path.GetFileName(lightmap.Path)}");

                            if (ImGui.Button("Change Light Map"))
                            {
                                lightpicker.Open();
                            }

                            if (ImGui.BeginCombo("Wrapping Light", lightmap.Wrapping.ToString()))
                            {
                                foreach (TextureWrapping wrap in Enum.GetValues(typeof(TextureWrapping)))
                                {
                                    if (ImGui.Selectable(wrap.ToString(), wrap == lightmap.Wrapping))
                                        lightmap.Wrapping = wrap;
                                }
                                ImGui.EndCombo();
                            }

                            if (ImGui.BeginCombo("Sampling Light", lightmap.Sampling.ToString()))
                            {
                                foreach (TextureSampling samp in Enum.GetValues(typeof(TextureSampling)))
                                {
                                    if (ImGui.Selectable(samp.ToString(), samp == lightmap.Sampling))
                                        lightmap.SetSampling(samp);
                                }
                                ImGui.EndCombo();
                            }

                            float aniso = lightmap.AnisotropicLevel;
                            if (ImGui.SliderFloat("Anisotropy Light", ref aniso, 1.0f, 16.0f))
                                lightmap.AnisotropicLevel = aniso;
                        }

                        if (comp is TextureProjectorComponent projector)
                        {
                            ImGui.Text("Texture Projector");

                            if (ImGui.Button("Change Projector Texture"))
                                projectorPicker.Open();

                            float fov = projector.Fov;
                            if (ImGui.SliderFloat("FOV", ref fov, 10f, 120f))
                                projector.Fov = fov;

                            float aspect = projector.AspectRatio;
                            if (ImGui.DragFloat("Aspect Ratio", ref aspect, 0.01f, 0.1f, 5.0f))
                                projector.AspectRatio = aspect;

                            float near = projector.Near;
                            float far = projector.Far;
                            if (ImGui.DragFloat("Near", ref near, 0.01f, 0.01f, far - 0.01f)) projector.Near = near;
                            if (ImGui.DragFloat("Far", ref far, 0.1f, near + 0.01f, 500f)) projector.Far = far;
                        }

                        if (comp is ShowShaderComponent showshader)
                        {
                            ImGui.Text("Shader Viewer");

                            if (ImGui.BeginTabBar("ShaderTabs"))
                            {
                                if (ImGui.BeginTabItem("Vertex Shader"))
                                {
                                    ImGui.TextWrapped(showshader.printVertexShader());
                                    ImGui.EndTabItem();
                                }

                                if (ImGui.BeginTabItem("Fragment Shader"))
                                {
                                    ImGui.TextWrapped(showshader.printFragmentShader());
                                    ImGui.EndTabItem();
                                }

                                ImGui.EndTabBar();
                            }
                        }

                        if (comp is LightComponent light)
                        {
                            ImGui.Text("Light");

                            if (ImGui.BeginCombo("Light Type", light.Type.ToString()))
                            {
                                foreach (LightType type in Enum.GetValues(typeof(LightType)))
                                {
                                    bool isSelected = (light.Type == type);
                                    if (ImGui.Selectable(type.ToString(), isSelected))
                                        light.Type = type;
                                    if (isSelected)
                                        ImGui.SetItemDefaultFocus();
                                }
                                ImGui.EndCombo();
                            }

                            var lightcolor = new System.Numerics.Vector3(light.Color.X, light.Color.Y, light.Color.Z);
                            if (ImGui.ColorEdit3("LightColor", ref lightcolor))
                                light.Color = new Vector3(lightcolor.X, lightcolor.Y, lightcolor.Z);

                            float intensity = light.Intensity;
                            if (ImGui.DragFloat("Intensity", ref intensity, 0.1f, 0f, 10f))
                                light.Intensity = intensity;

                            if (light.Type == LightType.Point)
                            {
                                float range = light.Range;
                                if (ImGui.DragFloat("Range", ref range, 0.1f, 0f, 100f))
                                    light.Range = range;
                            }

                            if (light.Type == LightType.Spot)
                            {
                                float inner = MathHelper.RadiansToDegrees(light.InnerCutoff);
                                float outer = MathHelper.RadiansToDegrees(light.OuterCutoff);

                                if (ImGui.DragFloat("Inner Cutoff", ref inner, 0.5f, 0f, 90f))
                                    light.InnerCutoff = MathHelper.DegreesToRadians(inner);

                                if (ImGui.DragFloat("Outer Cutoff", ref outer, 0.5f, 0f, 90f))
                                    light.OuterCutoff = MathHelper.DegreesToRadians(outer);
                            }

                            ImGui.Text("Depth Map");
                            ImGui.Image(
                                new IntPtr(owner.shadowMap.DepthTexture),
                                new System.Numerics.Vector2(256, 256),
                                new System.Numerics.Vector2(0, 1),
                                new System.Numerics.Vector2(1, 0)
                            );

                           ImGui.DragFloat("lightBias",ref owner.shadowMap.bias, 0.0001f);
                        }

                        if (comp is CameraComponent camera)
                        {
                            ImGui.Text("Camera");
                            if (ImGui.Button("Go To Camera postion"))
                            {
                                scene.activeCamera.goToCamera(camera);
                            }

                            if (ImGui.Button("return to First Camera"))
                            {
                                scene.activeCamera = scene.camera;
                                scene.updateCameraMovement();
                            }

                            if (ImGui.Button("Set As Active Camera"))
                            {
                                scene.activeCamera = camera.Camera;
                                scene.updateCameraMovement();
                                Console.WriteLine($"Scene {scene.sceneId}: Active camera set to entity {selectedEntity.Id}");
                            }
                        }

                        if (comp is ShowNormalsComponenet normalsComp)
                        {
                            ImGui.Text("Show Normals");
                            bool drawNormals = normalsComp.draw;
                            if (ImGui.Checkbox("Show Normals", ref drawNormals))
                            {
                                normalsComp.draw = drawNormals;
                            }
                        }

                        if (comp is SkyboxComponent skybox)
                        {
                            ImGui.Text("Skybox");

                            if (ImGui.Button("Load Right Face")) skyboxPickerPosX.Open();
                            if (ImGui.Button("Load Left Face")) skyboxPickerNegX.Open();
                            if (ImGui.Button("Load Bottom Face")) skyboxPickerPosY.Open();
                            if (ImGui.Button("Load Top Face")) skyboxPickerNegY.Open();
                            if (ImGui.Button("Load Front Face")) skyboxPickerPosZ.Open();
                            if (ImGui.Button("Load Back Face")) skyboxPickerNegZ.Open();
                        }

                        if (comp is WireFrameComponent wireFrame)
                        {
                            ImGui.Text("WireFrame");
                            ImGui.Checkbox("Render As WireFrame", ref wireFrame.RenderAsWire);
                        }

                        if (comp is AnimateComponent animate)
                        {
                            ImGui.Text("Animater");
                            ImGui.DragFloat("change speed", ref animate.speed,0.1f);
                            ImGui.DragFloat("change amblitude", ref animate.Amplitude,0.1f);
                        }

                        if (ImGui.Button($"Remove {comp.GetType().Name}"))
                        {
                            selectedEntity.RemoveComponent(comp);
                        }
                    }

                    ImGui.End(); 
                }
            }
            albedoPicker.Draw();
            specularPicker.Draw();
            normalPicker.Draw();
            lightpicker.Draw();
            projectorPicker.Draw();
            modelPicker.Draw();
            skyboxPickerPosX.Draw();
            skyboxPickerNegX.Draw();
            skyboxPickerPosY.Draw();
            skyboxPickerNegY.Draw();
            skyboxPickerPosZ.Draw();
            skyboxPickerNegZ.Draw();
            terrainPicker.Draw();
        }

    }
}
