using System.Collections;
using UnityEngine;

/* Esta clase permite hacer una secuencia de animaciones (Monobehaviour) mediante una corrutina que dura lo que dura la secuencia, 
 * es reutilizable y se puede aplicar a cualquier objeto que tenga animaciones tipo LerpTransform, SinFunctions o EasingFunctions.
 * Admite cualquier script de animacion, por lo que se puede hacer una secuencia con animaciones de GameObjects distintos */

/* Hereda de AnimationScript, esto permite hacer una secuencia de AnimSecuences, o sea, una secuencia compuesta de otras secuencias */
public class AnimSecuence : AnimationScript
{
    [SerializeField] private bool loop; // Si es true, la secuencia se repetira indefinidamente
    [SerializeField] private float pauseLoopTime; // Pausa entre secuencias
    [SerializeField] private AnimationScript[] animations; // Array de animaciones (va en orden del 0 al array.Length)
    [SerializeField] private float[] waitTimeBeforeAnimation; /* Tiempos de espera antes de la animacion con el mismo index (va en el mismo orden que las animaciones)
                                                                  *  DEBE TENER EL MISMO TAMAÑO QUE EL ARRAY DE ANIMATIONS */

    void OnEnable()
    {
        StartSecuence();
    }

    private void StartSecuence()
    {
        StartCoroutine(SecuenceCoroutine());
    }
    IEnumerator SecuenceCoroutine()
    {
        float animDuration;
        for (int i = 0; i < animations.Length; i++)
        {
            // Esperamos el tiempo que hayamos establecido antes de empezar la animacion[i]
            if (waitTimeBeforeAnimation[i] > 0)
            {
                yield return new WaitForSeconds(waitTimeBeforeAnimation[i]); // Tiempo de espera una vez acabada

            }
            animations[i].enabled = true; // Se activa la animacion (se desactiva sola cuando acaba)
            animDuration = animations[i].GetDuration();

            // Este bucle permite activar animaciones al mismo tiempo 
            while (waitTimeBeforeAnimation[i] == -1)
            {
                animations[i+1].enabled = true; // Se activa la animacion (se desactiva sola cuando acaba)

                // Coge la duracion de la animacion mas larga (de las animaciones simultaneas)
                if(animDuration < animations[i+1].GetDuration())
                    animDuration = animations[i+1].GetDuration();
                i++;
            }

            yield return new WaitForSeconds(animDuration); // Tiempo que dura la animacion
        }

        if (loop) Invoke(nameof(StartSecuence), pauseLoopTime);
        else enabled = false;
    }

    public override float GetDuration()
    {
        float duration = 0;
        for (int i = 0; i < animations.Length; i++)
        {
            duration += animations[i].GetDuration();
            duration += waitTimeBeforeAnimation[i];
        }
        return duration;
    }
}
