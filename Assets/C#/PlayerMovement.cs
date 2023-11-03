using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private Animator anim;
    public LayerMask capaSuelo;
    public int saltosRealizados = 0;
    public bool estaSaltando = false;
    public bool enSuelo = true;


    [Header("Movimiento")]
    private float movimientoHorizontal = 0f;
    [SerializeField] private float fuerzaDeSalto;
    [SerializeField] private float velocidadDeMovimiento;

    private bool mirandoDerecha = true;

    public void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        enSuelo = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, capaSuelo);

        movimientoHorizontal = Input.GetAxisRaw("Horizontal") * velocidadDeMovimiento;
        bool estaCorriendo = Mathf.Abs(movimientoHorizontal) > 0;
        anim.SetBool("estaCorriendo", estaCorriendo);
        anim.SetBool("estaSaltando", estaSaltando);
        anim.SetBool("enSuelo", enSuelo);

        if (saltosRealizados != 0)
        {
            //Toca el suelo
            saltosRealizados = 0;
            enSuelo = false;
            estaSaltando = true;
            anim.SetBool("estaSaltando", true);
            anim.SetBool("enSuelo", false);
        }
        else
        {
            enSuelo = true;
            estaSaltando = false;
            anim.SetBool("estaSaltando", false);
            anim.SetBool("enSuelo", true);
        }

        Debug.Log("saltosRealizados: " + saltosRealizados);
        Debug.Log("enSuelo: " + enSuelo);
        Debug.Log("estaSaltando: " + estaSaltando);


        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            anim.SetBool("estaCorriendo", true);
        }
        else
        {
            anim.SetBool("estaCorriendo", false);
        }

        //Salto sin horizontal
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))
        {
            Saltar();
        }

        //Salto con horizontal
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) && !estaSaltando)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Saltar();
            }
        }
    }

    void FixedUpdate()
    {
        Mover(movimientoHorizontal * Time.fixedDeltaTime);
    }

    // Mueve al personaje segun el valor de mirandoDerecha en una direccion u otra
    private void Mover(float mover)
    {
        if (mover > 0 && !mirandoDerecha)
        {
            Girar();
        }
        else if (mover < 0 && mirandoDerecha)
        {
            Girar();
        }
        rb2D.velocity = new Vector2(mover, rb2D.velocity.y);
    }

    // Da la vuelta al personaje
    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    private void Saltar()
    {
        if (enSuelo)
        {
            if (Mathf.Abs(rb2D.velocity.y) < 0.01f)
            {
                rb2D.velocity = new Vector2(rb2D.velocity.x, 0);
                rb2D.AddForce(Vector2.up * Mathf.Sqrt(2 * fuerzaDeSalto * Mathf.Abs(Physics2D.gravity.y)), ForceMode2D.Impulse);
            }
            saltosRealizados++;
            estaSaltando = false;
            anim.SetBool("estaSaltando", false);
            anim.SetBool("enSuelo", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D colision)
    {
        if (colision.gameObject.CompareTag("Pared"))
        {
            Debug.Log("Colisionamos con una pared.");
        }
    }

}
