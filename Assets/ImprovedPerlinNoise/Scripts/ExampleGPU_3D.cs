using UnityEngine;
using System.Collections;

namespace ImprovedPerlinNoiseProject
{
    public class ExampleGPU_3D : MonoBehaviour
    {
        public NOISE_STLYE m_stlye = NOISE_STLYE.FBM;

        public int m_seed = 0;

        public float m_frequency = 10.0f;

        public float m_lacunarity = 2.0f;

        public float m_gain = 0.5f;

        private Renderer m_renderer;

        private GPUPerlinNoise m_perlin;

        void Start()
        {
            m_perlin = new GPUPerlinNoise(m_seed);

            m_perlin.LoadResourcesFor3DNoise();

            m_renderer = GetComponent<Renderer>();

            m_renderer.material.SetTexture("_PermTable2D", m_perlin.PermutationTable2D);
            m_renderer.material.SetTexture("_Gradient3D", m_perlin.Gradient3D);
        }

        void Update()
        {
            m_renderer.material.SetFloat("_Frequency", m_frequency);
            m_renderer.material.SetFloat("_Lacunarity", m_lacunarity);
            m_renderer.material.SetFloat("_Gain", m_gain);
            m_renderer.material.SetFloat("_NoiseStyle", (float)m_stlye);
        }

    }

}
