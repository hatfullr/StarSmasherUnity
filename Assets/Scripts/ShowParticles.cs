using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ShowParticles : MonoBehaviour
{
    [SerializeField] private FileReader _fileReader;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField, Min(0)] private int fileIndex;
    
    [HideInInspector] public FileReader fileReader { get => _fileReader; }
    [HideInInspector] public new ParticleSystem particleSystem { get => _particleSystem; }

    private Data data;
    private bool particleSystemSetup = false;

    public void SetupParticleSystem() => StartCoroutine(nameof(_SetupParticleSystem));
    
    private IEnumerator _SetupParticleSystem()
    {
        var main = particleSystem.main;
        main.maxParticles = data.nParticles;
        
        var emission = particleSystem.emission;
        ParticleSystem.Burst[] bursts = new ParticleSystem.Burst[1];
        bursts[0].count = main.maxParticles;
        bursts[0].cycleCount = 1;
        bursts[0].probability = 1f;
        bursts[0].repeatInterval = Mathf.Infinity;
        bursts[0].time = 0f;
        emission.SetBursts(bursts);
        
        particleSystem.Play(); // Trigger the burst
        yield return new WaitForEndOfFrame();
        particleSystem.Pause(); // Freeze the particles
        particleSystemSetup = true;
    }
    
    public void Show() => StartCoroutine(nameof(_Show));

    private IEnumerator _Show()
    {
        data = fileReader.Read(fileIndex);
        yield return new WaitForEndOfFrame();
        SetupParticleSystem();
        while (!particleSystemSetup) yield return new WaitForEndOfFrame();
        
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        int amount = particleSystem.GetParticles(particles);

        Vector3[] positions = data.GetPositions();
        float[] sizes = data.GetSizes();
        
		
		//StarSmasher d = data as StarSmasher;
		//float max = 1f / Mathf.Log10((float)d.data.temperatures.Max());
		
        for (int i = 0; i < amount; i++)
        {
            particles[i].position = positions[i];
            particles[i].startSize = sizes[i];
            //particles[i].startColor = Color.Lerp(Color.red, Color.white, Mathf.Log10((float)d.data.temperatures[i]) * max);
        }

        particleSystem.SetParticles(particles, amount);
        
        yield return new WaitForEndOfFrame();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ShowParticles))]
public class ShowParticlesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        ShowParticles script = target as ShowParticles;

        bool previous = GUI.enabled;
        GUI.enabled = Application.isPlaying;
        if (GUILayout.Button("Show selected file index"))
        {
            script.Show();
        }
        GUI.enabled = previous;
    }
}
#endif