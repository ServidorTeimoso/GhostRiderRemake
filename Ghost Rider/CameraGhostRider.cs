using UnityEngine;

public class CamerGhostRider : MonoBehaviour
{
    public Transform motoqueiro;
    public float velocidadeCamera = 3.0f;
    public float quantidadeRotacao = 10.0f;
    public float distanciaAproximada = 2.0f;
    public float distanciaNormal = 5.0f;

    private Vector3 offset;
    private Quaternion rotacaoInicial;

    void Start()
    {
        offset = transform.position - motoqueiro.position;
        rotacaoInicial = transform.rotation;
    }

    void Update()
    {
        transform.position = motoqueiro.position + offset;

        // Inclinar a câmera suavemente para a esquerda (tecla A)
        if (Input.GetKey(KeyCode.A))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacaoInicial * Quaternion.Euler(0, 0, quantidadeRotacao), Time.deltaTime * velocidadeCamera);
        }

        // Inclinar a câmera suavemente para a direita (tecla D)
        if (Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacaoInicial * Quaternion.Euler(0, 0, -quantidadeRotacao), Time.deltaTime * velocidadeCamera);
        }

        // Resetar a rotação da câmera quando as teclas não estão sendo pressionadas
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, rotacaoInicial, Time.deltaTime * velocidadeCamera);
        }

        // Aproximar a câmera suavemente quando a tecla W é pressionada
        if (Input.GetKey(KeyCode.W))
        {
            offset = Vector3.Lerp(offset, offset.normalized * distanciaAproximada, Time.deltaTime * velocidadeCamera);
        }
        else
        {
            // Voltar à distância normal quando a tecla W é solta
            offset = Vector3.Lerp(offset, offset.normalized * distanciaNormal, Time.deltaTime * velocidadeCamera);
        }
    }
}
