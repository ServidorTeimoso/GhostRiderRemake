using UnityEngine;

public class GhostRiderMovimento : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float velocidadeMovimento = 5.0f;
    [SerializeField] private float tempoEsperaSemMovimento = 0.6f;
    [SerializeField] private float velocidadeRotacao = 10.0f;

    [Header("Configurações de Ataque")]
    [SerializeField] private float tempoDuracaoAtaque = 0.5f;
    [SerializeField] private float tempoEsperaAposAtaque = 1.0f;
    [SerializeField] private float tempoParaSegundoAtaque = 1.0f; // Tempo máximo para o segundo ataque em segundos

    private Rigidbody rb;
    private Animator animator;
    private bool estaMovendo = false;
    private bool podeMover = true;
    private float tempoSemMovimento = 0.0f;
    private Vector3 direcaoMovimento = Vector3.zero;

    private int contadorAtaques = 0;
    private const int maximoAtaquesCombo = 3;
    private bool aguardandoProximoAtaque = false;
    private bool permitirSegundoAtaque = false;
    private float tempoDecorrido = 0.0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (podeMover)
        {
            Movimentar();
            AtualizarAnimacao();

            if (Input.GetKeyDown(KeyCode.Space) && !aguardandoProximoAtaque)
            {
                RealizarAtaque();
            }

            if (permitirSegundoAtaque && aguardandoProximoAtaque)
            {
                tempoDecorrido += Time.deltaTime;

                if (tempoDecorrido >= tempoParaSegundoAtaque)
                {
                    permitirSegundoAtaque = false;
                    contadorAtaques = 0; // Se passar do tempo para o segundo ataque, reinicia o combo
                    tempoDecorrido = 0.0f;
                    animator.SetBool("ATAK1", false); // Limpa a animação do primeiro ataque
                }
            }
        }
    }

    private void Movimentar()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movimento = new Vector3(moveHorizontal, 0.0f, moveVertical).normalized;

        if (movimento != Vector3.zero)
        {
            Vector3 posicaoAtual = transform.position;
            Vector3 posicaoDesejada = posicaoAtual + movimento;

            direcaoMovimento = (posicaoDesejada - posicaoAtual).normalized;

            Quaternion novaRotacao = Quaternion.LookRotation(direcaoMovimento);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, novaRotacao, velocidadeRotacao * Time.deltaTime);

            Vector3 novaPosicao = transform.position + direcaoMovimento * velocidadeMovimento * Time.fixedDeltaTime;
            rb.MovePosition(novaPosicao);

            if (!estaMovendo)
            {
                estaMovendo = true;
            }

            tempoSemMovimento = 0.0f;
        }
        else
        {
            estaMovendo = false;
            tempoSemMovimento += Time.deltaTime;

            if (tempoSemMovimento >= tempoEsperaSemMovimento)
            {
                animator.SetBool("Walk", false);
            }
        }
    }

    private void RealizarAtaque()
    {
        podeMover = false;
        aguardandoProximoAtaque = true;
        contadorAtaques++;
        estaMovendo = false;
        animator.SetBool("Walk", false);

        if (contadorAtaques == 1)
        {
            animator.SetBool("ATAK1", true);
            permitirSegundoAtaque = true;
            Invoke(nameof(FinalizarAtaque), tempoDuracaoAtaque);
        }
        else if (contadorAtaques == 2 && permitirSegundoAtaque)
        {
            animator.SetBool("ATAK1", false);
            animator.SetBool("ATAK2", true);
            permitirSegundoAtaque = false; // Reseta a janela de tempo para o segundo ataque
            Invoke(nameof(FinalizarAtaque), tempoDuracaoAtaque);
        }
        else
        {
            contadorAtaques = 0; // Reinicia o combo se não realizar o segundo ataque dentro do tempo permitido
            animator.SetBool("ATAK1", true);
            Invoke(nameof(FinalizarAtaque), tempoDuracaoAtaque);
        }
    }

    private void FinalizarAtaque()
    {
        animator.SetBool("ATAK1", false);
        animator.SetBool("ATAK2", false);

        aguardandoProximoAtaque = false;
        podeMover = true;
        Invoke(nameof(HabilitarMovimento), tempoEsperaAposAtaque);
    }

    private void HabilitarMovimento()
    {
        podeMover = true;
    }

    private void AtualizarAnimacao()
    {
        if (estaMovendo && !aguardandoProximoAtaque)
        {
            animator.SetBool("Walk", true);
        }
        else
        {
            if (tempoSemMovimento >= tempoEsperaSemMovimento || aguardandoProximoAtaque)
            {
                animator.SetBool("Walk", false);
            }
        }
    }
}
