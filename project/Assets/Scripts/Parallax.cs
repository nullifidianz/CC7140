using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float lenght;
    public float speed = 2f;

    void Start()
    {
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.position += Vector3.right * (-horizontalInput) * speed * Time.deltaTime; // Adicionei o sinal negativo

        // Verifica se passou do limite à direita
        if (transform.position.x > lenght)
        {
            transform.position = new Vector3(-lenght, transform.position.y, transform.position.z);
        }
        // Verifica se passou do limite à esquerda
        else if (transform.position.x < -lenght)
        {
            transform.position = new Vector3(lenght, transform.position.y, transform.position.z);
        }
    }
}
