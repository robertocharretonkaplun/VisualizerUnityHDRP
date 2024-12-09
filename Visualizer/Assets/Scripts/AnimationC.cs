using UnityEngine;
/// <summary>
/// Clase que maneja la animación de un objeto.
/// </summary>
public class AnimationC : MonoBehaviour
{
    public bool isUp = false; // Variable que indica si el objeto está arriba.
    public bool isDown = false; // Variable que indica si el objeto está abajo.
    private Animator animator; // Referencia al componente Animator del objeto.
  
    void Start()
    {
        animator = GetComponent<Animator>(); // Obtener el componente Animator del objeto.
        isUp = true; // Inicializar la variable isUp en true.
        animator.SetBool("Dup", true); // Establecer el parámetro "Dup" del Animator en true.
    }

    private void Update()
    {
       

    }
    public void ButtonClick() // Función que se llama cuando se hace clic en el botón.
    {
        if (isUp==true && isDown==false)
        {
            MoveDown();
            animator.SetBool("Dup", false);
            
        }
        else if (isDown==true && isUp==false)
        {
            MoveUp();
            animator.SetBool("Ddown", false);
           
        }
    }
    public void MoveUp() // Función que mueve el objeto hacia arriba.
    {
        Debug.Log("Up");
        animator.SetBool("Dup", true);
        isUp = true;
        isDown = false;

    }
    public void MoveDown() // Función que mueve el objeto hacia abajo.
    {
        Debug.Log("Down");
        animator.SetBool("Ddown", true);
        isDown = true;
        isUp = false;
    }
}