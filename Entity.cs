using System.Numerics;
using System.Collections.Generic;
using System.Linq;

namespace Graphics
{
    public class Entity
    {
        public int Id { get; private set; }
        private static int nextId = 0;
        public Shader shader;
        public bool useTexture;
        public float aspectRatio;
        public Scene owner;
        public string name;
        public Entity parent { get; private set; }
        public List<Entity> Children { get; } = new List<Entity>();
        public List<IComponent> Components = new List<IComponent>();

        public Entity(float aspectRatio, Scene owner)
        {
            Id = nextId++;
            name = $"Entity {Id}";
            this.aspectRatio = aspectRatio;
            shader = new Shader(VertexShader(), FragmentShader(), this);
            
            this.owner = owner;
        }

        public void AddComponent(IComponent component)
        {
            if (component is MeshComponent)
            {
                var existingMesh = Components.OfType<MeshComponent>().FirstOrDefault();
                if (existingMesh != null)
                {
                    Components.Remove(existingMesh);
                    Console.WriteLine($"Entity {Id}: Replaced mesh {existingMesh.Name} with {((MeshComponent)component).Name}");
                }
                else
                {
                    Console.WriteLine($"Entity {Id}: Added mesh {((MeshComponent)component).Name}");
                }
            }
            if (component is SkyboxComponent skybox)
            {
                Components.Clear();
                shader.Dispose();
                shader = new Shader(skybox.VertexShader(), skybox.FragmenShader(), this);
            }

            Components.Add(component);

            if (component is AnimateComponent)
            {
                RecompileShader();
                Console.WriteLine($"Entity {Id}: Shader recompiled to include animation support");
            }

            if (component is TextureProjectorComponent)
            {
                owner.RecompileAllShaders();
            }

            if (component is LightComponent)
            {
                owner.RecompileAllShaders();
            }
        }

        public void RemoveComponent(IComponent component) => Components.Remove(component);

        public T? GetComponent<T>() where T : class, IComponent => Components.OfType<T>().FirstOrDefault();
        public IComponent GetComponent(Type type) => Components.FirstOrDefault(c => c.GetType() == type);

        public string VertexShader()
        {
            string header = @"
#version 330 core
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec2 aTexCoord;
layout(location = 2) in vec3 aNormal;

out vec2 TexCoord;
out vec3 FragPos;
out vec3 Normal;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
            ";

            if (GetComponent<AlbedoMapComponenet>() != null || (GetComponent<SpecularMapComponent>() != null) || (GetComponent<NormalMapComponent>() != null) || (GetComponent<LightMapComponent>() != null))
                header += "uniform vec2 uvScale;\n";

            if (GetComponent<AnimateComponent>() != null)
            {
                header += "uniform float time;\n\r";
                header += "uniform float speed;\n\r";
                header += "uniform float amplitude\n\r;";
            }

            string main = @"
 void main()
{
    vec3 pos = aPos;
";

            if (GetComponent<AnimateComponent>() != null)
            {
                main += @"
    float factor = pos.y;
    pos.x += sin(time * speed + pos.z * 5.0) * amplitude * factor;
    pos.z += cos(time * speed + pos.x * 5.0) * amplitude * factor;
";
            }

            main += @"
    FragPos = vec3(model * vec4(pos, 1.0));
    Normal = mat3(transpose(inverse(model))) * aNormal;
    gl_Position = projection * view * model * vec4(pos, 1.0);
";

            main += useTexture ? "TexCoord = aTexCoord * uvScale;\n" : "TexCoord = aTexCoord;\n";
            main += "}\n\r";

            return header + main;
        }

        public string FragmentShader()
        {
            bool hasLight = owner?.AnyEntityHas<LightComponent>() ?? false;
            Entity? lightcomponent = owner?.FirstEntityWith<LightComponent>();
            bool hasProj = owner?.AnyEntityHas<TextureProjectorComponent>() ?? false;

            string header = @"
#version 330 core
out vec4 FragColor;

in vec2 TexCoord;
in vec3 FragPos;
in vec3 Normal;

uniform vec4 color;
";

            if (hasProj)
            {
                header += @"
uniform sampler2D ProjectorTex;
uniform mat4 projectorView;
uniform mat4 projectorProjection;";
            }

            if (hasLight)
            {
                header += @"
uniform int lightType;
uniform vec3 lightPos;
uniform vec3 lightDir;
uniform vec3 lightColor;
uniform float lightIntensity;
uniform float ambientLight;
uniform vec3 viewPos;
uniform float range;
uniform float innerCutoff;
uniform float outerCutoff;
uniform sampler2D shadowMap;
uniform mat4 lightSpaceMatrix; 
uniform float bias;
";
            }

            if (GetComponent<AlbedoMapComponenet>() != null)
                header += "uniform sampler2D Albedomap;\n\r";
            if (GetComponent<SpecularMapComponent>() != null)
                header += "uniform sampler2D Specularmap;\n\r";
            if (GetComponent<NormalMapComponent>() != null)
                header += "uniform sampler2D Normalmap;\n\r";
            if (GetComponent<LightMapComponent>() != null)
                header += "uniform sampler2D Lightmap;\n\r";

            if (hasLight)
            {
                header += @"
float CalculateShadow(vec4 fragPosLightSpace)
{
    vec3 projCoords = fragPosLightSpace.xyz / fragPosLightSpace.w;
    projCoords = projCoords * 0.5 + 0.5;

    if (projCoords.z > 1.0)
        return 0.0;

    float currentDepth = projCoords.z;


    float shadow = 0.0;
    vec2 texelSize = 1.0 / textureSize(shadowMap, 0);
    for(int x = -1; x <= 1; ++x)
    {
        for(int y = -1; y <= 1; ++y)
        {
            float pcfDepth = texture(shadowMap, projCoords.xy + vec2(x, y) * texelSize).r;
            shadow += currentDepth - bias > pcfDepth ? 1.0 : 0.0;
        }
    }
    shadow /= 9.0;

    return shadow;
}

";
            }

            string main = @"
void main()
{
    vec4 baseColor = color;

    if(baseColor.a < 0.1)
        discard;
";

            if (GetComponent<AlbedoMapComponenet>() != null)
                main += "baseColor = texture(Albedomap, TexCoord) * color;\n";

            main += "vec3 norm = normalize(Normal);\n";

            if (GetComponent<NormalMapComponent>() != null)
            {
                main += @"
    vec3 normalTex = texture(Normalmap, TexCoord).rgb;
    normalTex = normalTex * 2.0 - 1.0;
    norm = normalize(normalTex);
";
            }

            main += @"
    vec4 resultFinal;
";


            if (hasLight)
            {
                main += @"
    vec4 baseLit = baseColor;

    vec3 toLight = normalize(lightPos - FragPos);
    vec4 fragPosLightSpace = lightSpaceMatrix * vec4(FragPos, 1.0);
    float shadow = CalculateShadow(fragPosLightSpace);
    float diff = max(dot(norm, toLight), 0.0);
    vec3 diffuse = (1.0 - shadow) * diff * lightColor * lightIntensity;

    vec3 resultColor = (ambientLight + diffuse) * baseLit.rgb;
    resultFinal = vec4(resultColor, baseLit.a);
";
            }
            if (hasLight)
            {
                main += @"
    if (lightType == 1) {
        float distance = length(lightPos - FragPos);
        float attenuation = 1.0 / (1.0 + distance / range);
       float diffPoint = max(dot(norm, toLight), 0.0);

        vec3 ambient = ambientLight * baseLit.rgb;
        vec3 diffuse = diffPoint * lightColor * lightIntensity;

        vec3 colorPoint = (ambient + diffuse) * attenuation;

        resultFinal = vec4(colorPoint, baseLit.a);
    }";
}
            if (hasLight)
            {
                main += @"
    else if (lightType == 2) {
        float theta = dot(toLight, normalize(-lightDir));
        float epsilon = innerCutoff - outerCutoff;
        float intensitySpot = clamp((theta - outerCutoff) / epsilon, 0.0, 1.0);

        vec3 ambient = ambientLight * baseLit.rgb;

        float diff = max(dot(norm, toLight), 0.0);
        vec3 diffuse = diff * lightColor * lightIntensity * intensitySpot;

        vec3 finalColor = ambient + diffuse;
        resultFinal = vec4(finalColor, baseLit.a);
    }
";
            }

            else
            {

                main += @"
    resultFinal = baseColor;
";
            }

            if (GetComponent<LightMapComponent>() != null)
                main += "resultFinal *= texture(Lightmap, TexCoord);\n";

            if (hasProj)
            {
                main += @"
    vec4 projCoords = projectorProjection * projectorView * vec4(FragPos, 1.0);
    projCoords /= projCoords.w;

    if (projCoords.x >= -1.0 && projCoords.x <= 1.0 &&
    projCoords.y >= -1.0 && projCoords.y <= 1.0 &&
    projCoords.z >= -1.0 && projCoords.z <= 1.0)
    {
        vec2 uv = projCoords.xy * 0.5 + 0.5;
        vec4 projectedTex = texture(ProjectorTex, uv);
        resultFinal *= projectedTex;
    }";
            }

            main += @"
    FragColor = resultFinal;
    }
";

            return header + main;
        }

        public void SetUseTexture(bool value)
        {
            useTexture = value;
            RecompileShader();
        }

        public void setUpAnimation()
        {
            RecompileShader();
        }

        public void RecompileShader()
        {
            shader.Dispose();
            shader = new Shader(VertexShader(), FragmentShader(), this);
        }

        public void setName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                this.name = $"Entity {Id}";
            this.name = newName;
        }

        public void SetParent(Entity newParent)
        {
            if (newParent == null)
            {
                this.parent?.Children.Remove(this);
                this.parent = null;
                return;
            }

            if (newParent == this)
            {
                Console.WriteLine("Cannot set entity as its own parent!");
                return;
            }

            if (IsDescendantOf(newParent))
            {
                Console.WriteLine("Cannot set parent: would create a cycle!");
                return;
            }

            this.parent?.Children.Remove(this);

            this.parent = newParent;
            newParent.Children.Add(this);
        }

        private bool IsDescendantOf(Entity potentialChild)
        {
            foreach (var child in Children)
            {
                if (child == potentialChild || child.IsDescendantOf(potentialChild))
                    return true;
            }
            return false;
        }
    }
}
