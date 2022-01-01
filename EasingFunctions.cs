using UnityEngine;

public enum EasingFunction 
{ 
    EaseInOutElastic, EaseOutElastic,
    EaseOutBounce, EaseInBounce,
    EaseInQuint, EaseOutQuint,
    EaseInOutBack, EaseOutBack
}
public class EasingFunctions : AnimationScript
{    
    [Header("Duration of the animation")]
    [Tooltip("Duration of the animation in seconds.")]
    [SerializeField]
    private float duration;

    [Header("Start from [0-1]")]
    [Tooltip("Percentage of the easing function length the animation will start from." +
        "\nExample: T=0.5 makes the animation starts from the half of the easing function.")]
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float initialT;
    private float t;
    private float time;
    private float value; // This is the value returned by the easing function

    [Header("Select Function")]
    [Tooltip("Easing function that will be applied." +
        "\nCheck www.easings.net for more info.")]
    [SerializeField]
    private EasingFunction easingFunction;
    [Tooltip("Exponent power of the easing function." +
        "\nIt modifies the spikes of the function.")]
    [Range(1.0f, 4.0f)]
    [SerializeField]
    private float intensity; // Default=1

    [Header("Set Position")]
    [Tooltip("(Optional) Attach the final position transform from the scene hierarchy.")]
    [SerializeField]
    private Transform finalPosition;
    private float rangeX;
    private float rangeY;
    private float rangeZ;
    private bool setFinalPosition;
    private float startPosX;
    private float startPosY;
    private float startPosZ;

    [Header("Set Rotation")]
    [Tooltip("(Optional) It rotates the object to the final euler angles.")]
    [SerializeField]
    private bool setRotation;
    [Tooltip("The final rotation for the animated object (degrees)")]
    [SerializeField]
    private Vector3 finalEulerAngles;
    private float rangeXRot;
    private float rangeYRot;
    private float rangeZRot;
    private float startRotX;
    private float startRotY;
    private float startRotZ;
    [Header("Set Slope")]
    [Tooltip("Smooth value for the slope.")]
    [Range(0, 1f)]
    [SerializeField]
    private float eulerIntensity;
    [Tooltip("(Optional) The slope affects the X axis.")]
    [SerializeField]
    private bool eulerX;
    [Tooltip("(Optional) The slope affects the Y axis.")]
    [SerializeField]
    private bool eulerZ;
    [SerializeField]
    private bool inverseEuler;
    private int signEuler;
    Vector2 previousPoint;
    [Header("Look At (rotate towards a target)")]
    [Tooltip("(Optional) The target transform the animated object will look at.")]
    [SerializeField]
    private Transform lookAt;
    private bool lookAtTarget;
    
    [Header("Set Scale")]
    [Tooltip("(Optional) Amount of distance (meters) the animated object will be resized.")]
    [SerializeField]
    private float scaleRange;
    private Vector3 startScale;

    private void OnEnable()
    {
        if (intensity == 0) intensity = 1;

        t = initialT;
        time = duration * t;

        startPosX = theAnimatedObject.position.x;
        startPosY = theAnimatedObject.position.y;
        startPosZ = theAnimatedObject.position.z;

        if (finalPosition != null)
        {
            rangeX = finalPosition.position.x - startPosX;
            rangeY = finalPosition.position.y - startPosY;
            rangeZ = finalPosition.position.z - startPosZ;
            setFinalPosition = true;
        }

        if (lookAt != null || setRotation)
        {
            startRotX = theAnimatedObject.eulerAngles.x;
            startRotY = theAnimatedObject.eulerAngles.y;
            startRotZ = theAnimatedObject.eulerAngles.z;
            Quaternion finalRot = Quaternion.identity;
            if (lookAt != null)
            {
                finalRot = Quaternion.LookRotation(lookAt.position - theAnimatedObject.position);
                lookAtTarget = true;
            }
            else if (setRotation)
            {
                if (finalEulerAngles != null)
                {
                    finalRot = Quaternion.Euler(finalEulerAngles);
                }
            }
            if (finalRot.eulerAngles.x - startRotX > 180)
                rangeXRot = finalRot.eulerAngles.x - (startRotX + 360);
            else if (finalRot.eulerAngles.x - startRotX < -180)
                rangeXRot = finalRot.eulerAngles.x - (startRotX -360);
            else rangeXRot = finalRot.eulerAngles.x - startRotX;

            if(finalRot.eulerAngles.y - startRotY > 180)
                rangeYRot = finalRot.eulerAngles.y - (startRotY+360);
            else if(finalRot.eulerAngles.y - startRotY < -180)
                rangeYRot = finalRot.eulerAngles.y - (startRotY-360);
            else rangeYRot = finalRot.eulerAngles.y - startRotY;

            if (finalRot.eulerAngles.z - startRotZ > 180)
                rangeZRot = finalRot.eulerAngles.z - (startRotZ + 360);
            if (finalRot.eulerAngles.z - startRotZ < -180)
                rangeZRot = finalRot.eulerAngles.z - (startRotZ - 360);
            else rangeZRot = finalRot.eulerAngles.z - startRotZ;
        }

        startScale = theAnimatedObject.localScale;

        if (inverseEuler) signEuler = -1;
        else signEuler = 1;
    }

    private void OnDisable()
    {
        t = 0;
    }

    // Update is called once per frame
    void Update()
    {
        previousPoint = new Vector2(time / duration, value);
        time += Time.deltaTime;
        t = Mathf.Lerp(0, 1, time / duration);

        switch (easingFunction) 
        {
            case EasingFunction.EaseInOutElastic:
                value = EaseInOutElastic(t);
                break;
            case EasingFunction.EaseOutElastic:
                value = EaseOutElastic(t);
                break;
            case EasingFunction.EaseOutBounce:
                value = EaseOutBounce(t);
                break;
            case EasingFunction.EaseInBounce:
                value = EaseInBounce(t);
                break;
            case EasingFunction.EaseOutBack:
                value = EaseOutBack(t);
                break;
            case EasingFunction.EaseInQuint:
                value = EaseInQuint(t);
                break;
            case EasingFunction.EaseOutQuint:
                value = EaseOutQuint(t);
                break;
            case EasingFunction.EaseInOutBack:
                value = EaseInOutBack(t);
                break;
        }

        value = Mathf.Pow(value, intensity);

        if (setFinalPosition)
        {
            theAnimatedObject.position = new Vector3(
                startPosX + value * rangeX,
                startPosY + value * rangeY,
                startPosZ + value * rangeZ);
        }
        if (eulerZ)
        {
            theAnimatedObject.eulerAngles = new Vector3(
            theAnimatedObject.eulerAngles.x,
            theAnimatedObject.eulerAngles.y,
            signEuler * Vector2.SignedAngle(new Vector2(1, 0), new Vector2(time / duration - previousPoint.x, value - previousPoint.y)) * eulerIntensity);
        }
        if (eulerX)
        {
            theAnimatedObject.eulerAngles = new Vector3(
            signEuler * Vector2.SignedAngle(new Vector2(1, 0), new Vector2(time / duration - previousPoint.x, value - previousPoint.y)) * eulerIntensity,
            theAnimatedObject.eulerAngles.y,
            theAnimatedObject.eulerAngles.z
            );
        }
        if (lookAtTarget || setRotation)
        {
            theAnimatedObject.eulerAngles = new Vector3(
            startRotX + rangeXRot * value,
            startRotY + rangeYRot * value,
            startRotZ + rangeZRot * value
            );
        }

        if (scaleRange > 0)
        {
            theAnimatedObject.localScale = startScale + new Vector3(value, value, value) * scaleRange;
        }

        if (t > 0.999)
        {
            this.enabled = false;
        }
    }

    public override float GetDuration()
    {
        return (duration - duration * initialT);
    }

    // ------------------------------------------------------ EASING FUNCTIONS ----------------------------------------------------------------
    // EaseInOutElastic
    float EaseInOutElastic(float t)
    {
        float c5 = (2 * Mathf.PI) / 4.5f;

        return t == 0
          ? 0
          : t == 1
          ? 1
          : t < 0.5
          ? -(Mathf.Pow(2, 20 * t - 10) * Mathf.Sin((20 * t - 11.125f) * c5)) / 2
          : (Mathf.Pow(2, -20 * t + 10) * Mathf.Sin((20 * t - 11.125f) * c5)) / 2 + 1;
    }

    // EaseOutElastic
    float EaseOutElastic(float t)
    {
        float c4 = (2 * Mathf.PI) / 3;

        return t == 0
          ? 0
          : t == 1
          ? 1
          : Mathf.Pow(2, -10 * t) * Mathf.Sin((t * 10 - 0.75f) * c4) + 1;
    }

    // EaseInElastic
    float EaseInElastic(float t)
    {
        float c4 = (2 * Mathf.PI) / 3;

        return t == 0
          ? 0
          : t == 1
          ? 1
          : -Mathf.Pow(2, 10 * t - 10) * Mathf.Sin((t * 10 - 10.75f) * c4);
    }

    // EaseOutBounce
    float EaseOutBounce(float t)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (t < 1 / d1)
        {
            return n1 * t * t;
        }
        else if (t < 2 / d1)
        {
            return n1 * (t -= 1.5f / d1) * t + 0.75f;
        }
        else if (t < 2.5 / d1)
        {
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;
        }
        else
        {
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }
    }

    // EaseInBounce
    float EaseInBounce(float t)
    {
        return 1 - EaseOutBounce(1 - t);
    }

    // EaseInOutBounce
    float EaseInOutBounce(float t) {
        return t < 0.5
          ? (1 - EaseOutBounce(1 - 2 * t)) / 2
          : (1 + EaseOutBounce(2 * t - 1)) / 2;
    }

    // EaseOutBack
    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;

        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

    // EaseInQuint
    float EaseInQuint(float t)
    {
        return t * t * t * t * t;
    }

    // EaseOutQuint
    float EaseOutQuint(float t)
    {
        return 1 - Mathf.Pow(1 - t, 5);
    }

    // EaseInOutBack
    float EaseInOutBack(float t) {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return t < 0.5
          ? (Mathf.Pow(2 * t, 2) * ((c2 + 1) * 2 * t - c2)) / 2
          : (Mathf.Pow(2 * t - 2, 2) * ((c2 + 1) * (t* 2 - 2) + c2) + 2) / 2;
    }
}
