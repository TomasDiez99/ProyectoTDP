using System;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class FadeInScreenBehaviour : MonoBehaviour
    {
        Image image;
        public float speed = 0.33f;

        float a;
        void Start()
        {
            image = GetComponent<Image>();
            a = 1.5f; // para que tarde un cachito en full black
        }

        void Update()
        {
            a = Mathf.MoveTowards(a, 0, Time.deltaTime * speed);
            var c = image.color;
            c.a = a;
            image.color = c;


        }
    }
}