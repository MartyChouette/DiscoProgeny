using System.Collections;
using UnityEngine;

public class Dancer : BasicAICreature
{
    public Animator danceAnimator;
    public bool isDancing = false;
    public int[] danceTypes = {-1, 1, 2, 3}; // -1 = not dancing
    public int currentDanceType = -1;
    public bool isIntoxicated = false;
    
    float probStartDance = .4f;
    float probStopDance = .3f;

    bool inCoroutine = false;


    // Get & Set Intoxication Levels
    public bool GetIntoxication() {
        return isIntoxicated;
    }
    public void SetIntoxication(bool tof) {
        isIntoxicated = tof;
    }


    // Get & Set Dance Type
    public int GetCurrentDanceType() {
        return currentDanceType;
    }

    public void SetCurrentDanceType(int danceType) {
        currentDanceType = danceType;
        danceAnimator.SetInteger("danceType", currentDanceType);
    }


    public virtual void Update() {
        if (! isDancing && ! isIntoxicated) {
            if (! inCoroutine) StartCoroutine(RandomStartDancing());
        }
        else if (isDancing && !isIntoxicated) {
            if (! inCoroutine) StartCoroutine(RandomStopDancing());
        }
        else if (isIntoxicated) {
            ChooseRandomDanceType();
            isDancing = true;
            isIntoxicated = false;
        }
    }

    IEnumerator RandomStartDancing() {
        inCoroutine = true;

        yield return new WaitForSeconds(3f);

        float randomProb = Random.value;
        if (randomProb < probStartDance) {
            ChooseRandomDanceType();
            isDancing = true;
        }

        inCoroutine = false;
    }

    IEnumerator RandomStopDancing() {
        inCoroutine = true;

        yield return new WaitForSeconds(5f);

        float randomProb = Random.value;
        if (randomProb < probStopDance) {
            currentDanceType = -1;
            danceAnimator.SetInteger("danceType", currentDanceType);
            isDancing = false;
        }
        
        inCoroutine = false;
    }

    public void ChooseRandomDanceType() {
        currentDanceType = Random.Range(1,4);
        danceAnimator.SetInteger("danceType", currentDanceType);
    }
}
