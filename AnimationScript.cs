
using UnityEngine;

// Todos los scripts que hereden de este podran ser agregados a la secuencia de animaciones que gestiona el script AnimSecuence
// Este script obliga a implementar el metodo GetDuration() necesario para hacer la secuencia con los tiempos de espera de cada animacion
// Todas las animaciones comienzan cuando se habilita el script (OnEnable)
public abstract class AnimationScript : MonoBehaviour
{
    [Header("Attach the animated GameObject")]
    [Tooltip("Attach the GameObject from the scene hierarchy that is going to be animated.")]
    [SerializeField]
    protected Transform theAnimatedObject;

    public abstract float GetDuration();
}
