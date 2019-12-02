using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedModifier {
    private float speedModifier;

    private bool infinite;
    private float remain; //Life time (when reaching 0 speed modifier should be destroyed)

    public SpeedModifier(float modifier) {
        init(modifier, true, 0f);
    }

    public SpeedModifier(float modifier, bool isInfinite, float duration) {
        init(modifier, isInfinite, duration);
    }

    private void init(float modifier, bool isInfinite, float duration) {
        this.speedModifier = modifier;
        this.infinite = isInfinite;
        this.remain = duration;
    }

    public bool isValid() {
        return (infinite) ? true : remain > 0;
    }

    public float getModifier() {
        return (isValid()) ? speedModifier : 1f;
    }

    public void decrement(float f) {
        if (remain > 0)
            remain -= f;
    }
}
