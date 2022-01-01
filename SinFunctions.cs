using UnityEngine;

// Este script permite animar cualquier objeto en 4 modos (horizontal, vertical, circular y size), con parámetros personalizables
public class SinFunctions : AnimationScript
{
    public float amount;            // Cantidad de desplazamiento
    public float speed;             // Velocidad de desplazamiento
    [Header("Optionals")]
    public bool stopAtInitialPos;   // True = la animación se detiene en el punto de partida. False = se detiene en cualquier posición.
    public float duration;          // Duración de la animación (0 o menos si no se quiere establecer una duración)
    public float repeatAfterSeconds;// Repite la animación cada x segundos. 0 o menor no hace la repetición. Pensado para animaciones en bucle, no animaciones controladas por eventos y otros scripts.
    public int numberOfRepeats;     // Cantidad de veces que pausa la animación y se repite.
    int repeatCount = 1;

    float startTime;                // Momento en el que comienza la animación
    public float t;                 // Tiempo que ha pasado desde que comienza la duración

    Vector3 startPosition;          // Posición inicial antes de empezar
    Vector3 startEuler;             // Rotación (euler) inicial antes de empezar
    Vector3 startScale;             // Escala inicial antes de empezar
    float startValue;               // Valor (position o euler) inicial antes de empezar
    
    float shake;                    // Cantidad de desplazamiento que se suma
    float sign;                     // Signo del desplazamiento (permite desplazar en un único sentido, en vez de hacia los dos)
    Vector3 shakeDirection;         // Dirección de vibración (eje X, Y, Z)
    Vector3 shakeFinalDirection;    // Dirección final de desplazamiento con el valor shake que se va a sumar

    public enum ShakeType           
    {
        Vertical,
        Horizontal,
        Circular,
        SizeBig,
        SizeBigSmall
    }
    public ShakeType shakeType;

    // Empieza la animación cuando se activa el script, con los parámetros que tenga establecidos
    // En este método se guardan todos los momentos y posiciones iniciales, para que la animación sea relativa a estos valores
    private void OnEnable()
    {
        sign = 1;
        startTime = Time.time;
        startPosition = theAnimatedObject.position;
        startEuler = theAnimatedObject.eulerAngles;
        startScale = theAnimatedObject.localScale;

        if (shakeType == ShakeType.Horizontal) 
        {
            shakeDirection = Vector3.right;
            startValue = theAnimatedObject.position.x;
        }
        else if(shakeType == ShakeType.Vertical)
        {
            shakeDirection = Vector3.up;
            startValue = theAnimatedObject.position.y;
        }
        else if (shakeType == ShakeType.Circular)
        {
            shakeDirection = Vector3.forward * 89;
            startValue = theAnimatedObject.eulerAngles.z;
        }
        else if (shakeType == ShakeType.SizeBig || shakeType == ShakeType.SizeBigSmall)
        {
            shakeDirection = Vector3.one;
            startValue = theAnimatedObject.localScale.magnitude;
        }
    }

    // Update is called once per frame
    void Update()
    {
        t = Time.time - startTime;

        shake = Mathf.Sin(t * speed) * amount;
        shakeFinalDirection = shakeDirection * shake * sign;

        Shake();

        if (duration > 0 && t > duration)
        {
            StopShake();
        }

    }

    // Desplaza en función del ShakeType establecido (mueve/rota/escala)
    private void Shake()
    {
        if (shakeType == ShakeType.Horizontal || shakeType == ShakeType.Vertical)
        {
            theAnimatedObject.position = startPosition + shakeFinalDirection;
        }
        else if(shakeType == ShakeType.Circular)
        {
            theAnimatedObject.eulerAngles = startEuler + shakeFinalDirection;
        }
        else if(shakeType == ShakeType.SizeBig)
        {
            // Este if es opcional, para que solo escale en positivo y nunca decrezca el objeto
            // Se puede comentar el bloque si no se desea este efecto
            if (startScale.magnitude > (startScale + shakeFinalDirection).magnitude)
            {
                sign = -1;
                shakeFinalDirection = -shakeFinalDirection;
            }
            theAnimatedObject.localScale = startScale + shakeFinalDirection;
        }
        else if(shakeType == ShakeType.SizeBigSmall)
        {
            theAnimatedObject.localScale = startScale + shakeFinalDirection;
        }
    }

    // Detiene la animación cuando la posición sea la misma que la inicial, deteniéndola en el punto de partida
    public void StopShake()
    {
        if (stopAtInitialPos)
        {
            float currentValue = 0;
            float offset = 0.05f;
            if (shakeType == ShakeType.Horizontal)
            {
                currentValue = theAnimatedObject.position.x;
            }
            else if (shakeType == ShakeType.Vertical)
            {
                currentValue = theAnimatedObject.position.y;
            }
            else if (shakeType == ShakeType.Circular)
            {
                currentValue = theAnimatedObject.eulerAngles.z;
                offset = 3;
            }
            else if (shakeType == ShakeType.SizeBig || shakeType == ShakeType.SizeBigSmall)
            {
                currentValue = theAnimatedObject.localScale.magnitude;
                offset = 2;
            }

            if (Mathf.Abs(currentValue - startValue) < offset)
            {
                if (shakeType == ShakeType.Horizontal || shakeType == ShakeType.Vertical)
                {
                    theAnimatedObject.position = startPosition;
                }
                else if (shakeType == ShakeType.Circular)
                {
                    theAnimatedObject.eulerAngles = startEuler;
                }
                else if(shakeType == ShakeType.SizeBig || shakeType == ShakeType.SizeBigSmall)
                {
                    theAnimatedObject.localScale = startScale;
                }

                if (repeatAfterSeconds > 0 && repeatCount < numberOfRepeats) // Repetir tras una pausa
                {
                    Invoke(nameof(EnableShake), repeatAfterSeconds);
                    repeatCount++;
                }
                this.enabled = false;
            }
        }
        else
        {
            if (repeatAfterSeconds > 0 && repeatCount < numberOfRepeats) // Repetir tras una pausa
            {
                Invoke(nameof(EnableShake), repeatAfterSeconds);
                repeatCount++;
            }
            this.enabled = false;
        }

    }

    private void EnableShake()
    {
        this.enabled = true;
    }

    public void Restart()
    {
        t = 0;
        startTime = Time.time;
    }

    public override float GetDuration()
    {
        return duration;
    }
}
