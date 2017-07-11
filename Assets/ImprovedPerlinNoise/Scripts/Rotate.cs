using UnityEngine;
using System.Collections;

namespace ImprovedPerlinNoiseProject
{

    public class Rotate : MonoBehaviour
    {
        public float m_speed = 1.0f;

        void Update()
        {
            transform.Rotate(new Vector3(0, Time.deltaTime * m_speed, 0));

        }
    }
}
