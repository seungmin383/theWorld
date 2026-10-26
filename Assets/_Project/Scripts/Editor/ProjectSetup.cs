using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace TheWorld.Editor
{
    /// <summary>Initial setup and a repeatable check for the starter project.</summary>
    public static class ProjectSetup
    {
        public const string MainScenePath = "Assets/_Project/Scenes/Main.unity";
        private const string MaterialFolder = "Assets/_Project/Art/Materials/";
        private const string CubePrefabPath = "Assets/_Project/Prefabs/Gameplay/PF_PrototypeCube.prefab";
        private const string VolumeProfilePath = "Assets/_Project/Settings/Rendering/MainVolumeProfile.asset";

        // Invoked once by Unity's -executeMethod argument. Existing scenes and assets are preserved.
        public static void Initialize()
        {
            PlayerSettings.companyName = "TheWorld";
            PlayerSettings.productName = "The World";
            PlayerSettings.bundleVersion = "0.1.0";
            PlayerSettings.colorSpace = ColorSpace.Linear;
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Standalone, "com.theworld.game");
            EditorSettings.serializationMode = SerializationMode.ForceText;
            EditorSettings.defaultBehaviorMode = EditorBehaviorMode.Mode3D;
            EditorSettings.projectGenerationRootNamespace = "TheWorld";

            if (!File.Exists(MainScenePath))
            {
                CreateStarterScene();
            }

            if (!EditorBuildSettings.scenes.Any(scene => scene.path == MainScenePath))
            {
                EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(MainScenePath, true) }
                    .Concat(EditorBuildSettings.scenes.Where(scene => File.Exists(scene.path)))
                    .ToArray();
            }

            AssetDatabase.SaveAssets();
            ValidateProject();
        }

        [MenuItem("The World/Project/Open Main Scene")]
        public static void OpenMainScene()
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(MainScenePath);
            }
        }

        [MenuItem("The World/Project/Validate")]
        public static void ValidateProject()
        {
            Require(GraphicsSettings.defaultRenderPipeline is UniversalRenderPipelineAsset,
                "The default render pipeline must be URP.");
            Require(EditorSettings.serializationMode == SerializationMode.ForceText,
                "Asset serialization must use Force Text.");
            Require(PlayerSettings.colorSpace == ColorSpace.Linear, "Color space must be Linear.");
            Require(AssetDatabase.LoadAssetAtPath<SceneAsset>(MainScenePath) != null,
                "The main scene is missing.");
            Require(EditorBuildSettings.scenes.Any(scene => scene.enabled && scene.path == MainScenePath),
                "The main scene must be enabled in the build scene list.");
            Require(AssetDatabase.LoadAssetAtPath<GameObject>(CubePrefabPath) != null,
                "The prototype cube prefab is missing.");
            Require(AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath) != null,
                "The main volume profile is missing.");

            var scene = SceneManager.GetSceneByPath(MainScenePath);
            var openedForValidation = !scene.isLoaded;
            if (openedForValidation)
            {
                scene = EditorSceneManager.OpenScene(MainScenePath, OpenSceneMode.Additive);
            }

            try
            {
                var roots = scene.GetRootGameObjects();
                var cameras = roots.SelectMany(root => root.GetComponentsInChildren<Camera>(true)).ToArray();
                Require(cameras.Length == 1 && cameras[0].CompareTag("MainCamera"),
                    "The starter scene must contain one camera tagged MainCamera.");
                Require(roots.Sum(root => root.GetComponentsInChildren<AudioListener>(true).Length) == 1,
                    "The starter scene must contain one AudioListener.");
                Require(roots.SelectMany(root => root.GetComponentsInChildren<Light>(true))
                    .Any(light => light.type == LightType.Directional), "A directional light is missing.");
                Require(roots.SelectMany(root => root.GetComponentsInChildren<Collider>(true))
                    .Any(collider => collider.name == "Ground"), "The ground collider is missing.");

                foreach (var root in roots)
                {
                    foreach (var transform in root.GetComponentsInChildren<Transform>(true))
                    {
                        Require(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject) == 0,
                            $"Missing script on {transform.name}.");
                    }

                    foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
                    {
                        Require(renderer.sharedMaterials.All(material => material != null && material.shader != null),
                            $"Missing material or shader on {renderer.name}.");
                    }
                }
            }
            finally
            {
                if (openedForValidation)
                {
                    EditorSceneManager.CloseScene(scene, true);
                }
            }

            Debug.Log("[The World] Project validation passed: URP, main scene, build registration, camera, audio, lighting, colliders, scripts and materials.");
        }

        private static void CreateStarterScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var environment = new GameObject("Environment");
            var gameplay = new GameObject("Gameplay");
            var lighting = new GameObject("Lighting");

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(environment.transform);
            ground.transform.localScale = new Vector3(2f, 1f, 2f);
            ground.GetComponent<Renderer>().sharedMaterial = GetOrCreateMaterial("M_Ground", new Color(0.3f, 0.36f, 0.42f));
            GameObjectUtility.SetStaticEditorFlags(ground, StaticEditorFlags.BatchingStatic);

            var cubePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CubePrefabPath);
            if (cubePrefab == null)
            {
                var source = GameObject.CreatePrimitive(PrimitiveType.Cube);
                source.name = "Prototype Cube";
                source.GetComponent<Renderer>().sharedMaterial = GetOrCreateMaterial("M_Prototype", new Color(0.12f, 0.5f, 0.85f));
                cubePrefab = PrefabUtility.SaveAsPrefabAsset(source, CubePrefabPath);
                Object.DestroyImmediate(source);
            }

            var cube = (GameObject)PrefabUtility.InstantiatePrefab(cubePrefab, scene);
            cube.transform.SetParent(gameplay.transform);
            cube.transform.position = new Vector3(0f, 0.5f, 0f);

            var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(8f, 6f, -8f);
            cameraObject.transform.LookAt(new Vector3(0f, 0.5f, 0f));
            var camera = cameraObject.GetComponent<Camera>();
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 300f;
            camera.fieldOfView = 55f;
            cameraObject.AddComponent<UniversalAdditionalCameraData>().renderPostProcessing = true;

            var sunObject = new GameObject("Sun", typeof(Light), typeof(UniversalAdditionalLightData));
            sunObject.transform.SetParent(lighting.transform);
            sunObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var sun = sunObject.GetComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 2f;
            sun.shadows = LightShadows.Soft;
            RenderSettings.sun = sun;
            RenderSettings.skybox = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Skybox.mat");
            RenderSettings.ambientMode = AmbientMode.Skybox;

            var volumeObject = new GameObject("Global Volume", typeof(Volume));
            volumeObject.transform.SetParent(lighting.transform);
            var volume = volumeObject.GetComponent<Volume>();
            volume.isGlobal = true;
            volume.sharedProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(VolumeProfilePath);

            Require(EditorSceneManager.SaveScene(scene, MainScenePath), "Could not save the main scene.");
        }

        private static Material GetOrCreateMaterial(string name, Color color)
        {
            var path = MaterialFolder + name + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material != null)
            {
                return material;
            }

            var shader = Shader.Find("Universal Render Pipeline/Lit");
            Require(shader != null, "URP Lit shader is unavailable.");
            material = new Material(shader) { name = name };
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Smoothness", 0.2f);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException("[The World] " + message);
            }
        }
    }
}
