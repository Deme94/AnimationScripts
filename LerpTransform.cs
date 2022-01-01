using UnityEngine;

// Esta clase permite mover o escalar un objeto desde sus valores iniciales hasta los valores finales (especificados en el inspector) con una duración
public class LerpTransform : AnimationScript
{
    [Header("Settings")]
    public float duration;
    public bool loop;
    public float pauseDuration;
    private float pauseTime;
    private bool pause;

    [Header("Position")]
    public bool setPosition;
    public bool local;
    public Vector3 finalPosition;
    public bool useFinalTransform;
    public Transform finalTransform;
    Vector3 startPosition;

    [Header("Rotation")]
    public bool setRotation;
    public bool localRot;
    public Quaternion finalRotation;
    Quaternion startRotation;

    [Header("Scale")]
    public bool setScale;
    public Vector3 finalScale;
    Vector3 startScale;

    float startTime;
    float t;

    private void OnEnable()
    {
        pause = false;
        startTime = Time.time;
        if (local) startPosition = theAnimatedObject.localPosition;
        else startPosition = theAnimatedObject.position;
        startRotation = theAnimatedObject.rotation;
        startScale = theAnimatedObject.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        t = (Time.time - startTime) / duration;
        if (duration == 0) t = 1;
        if (setPosition)
        {
            if (!useFinalTransform)
            {
                if(local) theAnimatedObject.localPosition = Vector3.Lerp(startPosition, finalPosition, t);
                else theAnimatedObject.position = Vector3.Lerp(startPosition, finalPosition, t);
            }
            else
            {
                theAnimatedObject.position = Vector3.Lerp(startPosition, finalTransform.position, t);
            }
        }
        if (setRotation)
        {
            if(finalRotation.x == 0 && finalRotation.y == 0 && finalRotation.z == 0)
            {
                theAnimatedObject.rotation = Quaternion.identity;
            }
            else if(localRot)
                theAnimatedObject.localRotation = Quaternion.Lerp(startRotation, finalRotation, t);
            else
                theAnimatedObject.rotation = Quaternion.Lerp(startRotation, finalRotation, t);
        }
        if (setScale)
        {
            theAnimatedObject.localScale = Vector3.Lerp(startScale, finalScale, t);
        }

        if(t >= 1)
        {
            if (loop)
            {
                if (!pause)
                {
                    pause = true;
                    pauseTime = Time.time;
                }

                if (local) theAnimatedObject.localPosition = startPosition;
                else theAnimatedObject.position = startPosition;
                theAnimatedObject.rotation = startRotation;
                theAnimatedObject.localScale = startScale;

                if (Time.time > pauseTime + pauseDuration)
                {
                    pause = false;
                    t = 0;
                    startTime = Time.time;
                }
            }
            else
            {
                this.enabled = false;
            }
        }
    }

    public override float GetDuration()
    {
        return duration;
    }
}
