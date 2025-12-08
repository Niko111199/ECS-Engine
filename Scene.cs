using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Graphics
{
    public class Scene
    {
        public Color4 backGroundColor = new Color4();
        public SceneGui gui;
        public int sceneId;
        public Camera camera;
        public Camera activeCamera;
        public CameraMovementControls cameraMovement;
        public Window window;
        public float aspectRatio;
        public float ambientLight = 0.1f;

        public FrameBuffer frameBuffer;
        public PostProcessor postProcessor;

        public ShadowMap shadowMap;

        public List<Entity> Entities { get; } = new List<Entity>();

        public Scene(ImGuiController controller, Window window)
        {
            this.window = window;
            aspectRatio = (float)(window.width) / (float)(window.height);
            gui = new SceneGui(controller, this);
            camera = new Camera(new OpenTK.Mathematics.Vector3(0, 0, 3), aspectRatio);
            activeCamera = camera;
            cameraMovement = new CameraMovementControls(activeCamera, window);

            frameBuffer = new FrameBuffer(window.width, window.height);
            postProcessor = new PostProcessor(window.width, window.height,this);

            shadowMap = new ShadowMap(2048, 2048);
        }

        public void updateCameraMovement()
        {
            cameraMovement = new CameraMovementControls(activeCamera, window);
        }

        public void Render(float deltaTime)
        {
            cameraMovement.Update(deltaTime);

            var lightEntity = Entities.FirstOrDefault(e => e.GetComponent<LightComponent>() != null);
            Matrix4 lightSpaceMatrix = Matrix4.Identity;

            if (lightEntity != null)
            {
                var lightTrans = lightEntity.GetComponent<TransformComponent>();
                if (lightTrans == null)
                {
                    lightTrans = new TransformComponent(lightEntity);
                    lightEntity.AddComponent(lightTrans);
                }
                Vector3 lightPos = lightTrans.Position;
                Vector3 lightTarget = lightTrans.Forward;

                Matrix4 lightView = Matrix4.LookAt(lightPos, lightTarget, Vector3.UnitY);

                float orthoSize = 30f;
                float near = 1f;
                float far = 150f;

                Matrix4 lightProj = Matrix4.CreateOrthographicOffCenter(
                    -orthoSize, orthoSize,
                    -orthoSize, orthoSize,
                    near, far
                );

                lightSpaceMatrix = lightProj * lightView;
            }

            shadowMap.Bind();
            GL.Clear(ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(TriangleFace.Front);

            shadowMap.shadowShader.Use();
            shadowMap.shadowShader.setMat4("lightSpaceMatrix", lightSpaceMatrix);

            foreach (var entity in Entities)
            {
                var mesh = entity.GetComponent<MeshComponent>();
                if (mesh == null) continue;

                var transform = entity.GetComponent<TransformComponent>();
                Matrix4 model = transform != null ? transform.GetWorldMatrix() : Matrix4.Identity;

                shadowMap.shadowShader.setMat4("model", model);
                mesh.Draw();
            }

            shadowMap.Unbind(window.width, window.height);

            frameBuffer.Bind();

            GL.ClearColor(backGroundColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(TriangleFace.Back);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Matrix4 view = activeCamera.GetViewMatrix();
            Matrix4 projection = activeCamera.GetProjectionMatrix();

            GL.ActiveTexture(TextureUnit.Texture5);
            GL.BindTexture(TextureTarget.Texture2D, shadowMap.DepthTexture);

            var skyboxEntity = Entities.FirstOrDefault(e => e.GetComponent<SkyboxComponent>() != null);
            if (skyboxEntity != null)
            {
                var skyboxComp = skyboxEntity.GetComponent<SkyboxComponent>();
                skyboxComp.Draw(skyboxEntity.shader, view, projection);
            }

            foreach (var entity in Entities)
            {

                entity.shader.Use();

                var transform = entity.GetComponent<TransformComponent>();
                Matrix4 model = transform != null ? transform.GetWorldMatrix() : Matrix4.Identity;

                entity.shader.setMat4("model", model);
                entity.shader.setMat4("view", view);
                entity.shader.setMat4("projection", projection);

                var wireframe = entity.GetComponent<WireFrameComponent>();
                if (wireframe != null && wireframe.RenderAsWire)
                {
                    GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Line);
                }
                else
                {
                    GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
                }

                var texComp = entity.GetComponent<AlbedoMapComponenet>();
                if (texComp != null)
                {
                    texComp.Bind();
                    entity.shader.setInt("Albedomap", (int)TextureType.Albedo);
                    entity.shader.setVec2("uvScale", new Vector2(texComp.UVScale.X, texComp.UVScale.Y));
                }

                var specComp = entity.GetComponent<SpecularMapComponent>();
                if (specComp != null)
                {
                    specComp.Bind();
                    entity.shader.setInt("Specularmap", (int)TextureType.Specular);
                }

                var normalComp = entity.GetComponent<NormalMapComponent>();
                if (normalComp != null)
                {
                    normalComp.Bind();
                    entity.shader.setInt("Normalmap", (int)TextureType.Normal);
                }

                var lightmapComp = entity.GetComponent<LightMapComponent>();
                if (lightmapComp != null)
                {
                    lightmapComp.Bind();
                    entity.shader.setInt("Lightmap", (int)TextureType.Light);
                }

                var mesh = entity.GetComponent<MeshComponent>();
                if (mesh != null)
                {
                    entity.shader.setVec4("color", new Vector4(mesh.color.X, mesh.color.Y, mesh.color.Z, mesh.color.W));
                    mesh.Draw();
                }

                var normalsComp = entity.GetComponent<ShowNormalsComponenet>();
                if (normalsComp != null)
                {
                    normalsComp.Draw(entity, view, projection);
                }

                var animateComp = entity.GetComponent<AnimateComponent>();
                if (animateComp != null)
                {
                    animateComp.Update(deltaTime);

                    entity.shader.setFloat("time", animateComp.time);
                    entity.shader.setFloat("speed", animateComp.speed);
                    entity.shader.setFloat("amplitude", animateComp.Amplitude);
                }

                if (lightEntity != null)
                {
                    var lightComp = lightEntity.GetComponent<LightComponent>();
                    var lightTransform = lightEntity.GetComponent<TransformComponent>();

                    entity.shader.setInt("lightType", (int)lightComp.Type);
                    entity.shader.setVec3("lightPos", lightTransform?.Position ?? Vector3.Zero);
                    entity.shader.setVec3("lightColor", lightComp.Color);
                    entity.shader.setFloat("lightIntensity", lightComp.Intensity);
                    entity.shader.setVec3("viewPos", activeCamera.Position);
                    entity.shader.setFloat("ambientLight", ambientLight);

                    entity.shader.setMat4("lightSpaceMatrix", lightSpaceMatrix);
                    entity.shader.setInt("shadowMap", (int)TextureType.Shadow);
                    entity.shader.setFloat("bias", shadowMap.bias);

                    if (lightComp.Type == LightType.Point)
                        entity.shader.setFloat("range", lightComp.Range);

                    if (lightComp.Type == LightType.Spot)
                    {
                        entity.shader.setVec3("lightPos", lightTransform?.Position ?? Vector3.Zero);
                        entity.shader.setVec3("lightDir", lightTransform?.Forward ?? Vector3.UnitZ);

                        entity.shader.setFloat("innerCutoff", MathF.Cos(lightComp.InnerCutoff));
                        entity.shader.setFloat("outerCutoff", MathF.Cos(lightComp.OuterCutoff));
                    }
                }
                var projectorEntity = Entities.FirstOrDefault(e => e.GetComponent<TextureProjectorComponent>() != null);
                if (projectorEntity != null)
                {
                    var projectorComp = projectorEntity.GetComponent<TextureProjectorComponent>();
                    projectorComp.Bind();
                    entity.shader.setInt("ProjectorTex", (int)TextureType.Projector);
                    entity.shader.setMat4("projectorView", projectorComp.GetViewMatrix());
                    entity.shader.setMat4("projectorProjection", projectorComp.GetProjectionMatrix());
                }
            }

            frameBuffer.Unbind();
            postProcessor.Render(frameBuffer.ColorTexture);
        }

        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
            Console.WriteLine($"Entity {entity.Id} added to scene {sceneId}.");
        }

        public void RemoveEntity(Entity entity)
        {
            Entities.Remove(entity);
            Console.WriteLine($"Entity {entity.Id} removed from scene {sceneId}.");
        }

        public void RecompileAllShaders()
        {
            foreach (var entity in Entities)
            {
                entity.RecompileShader();
            }

            Console.WriteLine("Recompiler all shaders");
        }

        public bool AnyEntityHas<T>() where T : class, IComponent
        {
            return Entities.Any(e => e.GetComponent<T>() != null);
        }

        public Entity? FirstEntityWith<T>() where T : class, IComponent
        {
            return Entities.FirstOrDefault(e => e.GetComponent<T>() != null);
        }
    }
}
