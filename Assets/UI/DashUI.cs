using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
    private RawImage imageCooldown;
    private Animator skill;
    private float cooldown;
    private bool isCooldown;
    private Color c, maxC;
    void Start(){
        cooldown = GameObject.Find("Player").GetComponent<RBController>().dashCooldown;
        imageCooldown = GameObject.Find("Dash").GetComponent<RawImage>();
        c = imageCooldown.color;
        maxC = imageCooldown.color;
        c.a = 0;
        maxC.a = 1;
        skill = GetComponent<Animator>();
    }
    void Update()
    {
        RegisterInput();
        AdjustSkillAlpha();
    }
    void RegisterInput(){
        if(Input.GetKeyDown(KeyCode.V) && !isCooldown)
            isCooldown = true;
    }
    void AdjustSkillAlpha(){
        if(isCooldown){
            StartCoroutine(FadeIn());
            isCooldown = false;
            c.a = 0;
        }
    }
    //On isCooldown slowly turns the skill's image alpha from 0f to 1f and starts the  ability ready animation
    //Initiated by AdjustSkillAlpha
    IEnumerator FadeIn()
    {
        float elapsedTime = 0.0f;
        imageCooldown.color = c;
        while (elapsedTime < cooldown)
        {
            yield return null;
            elapsedTime += Time.deltaTime ;
            c.a = 0 + Mathf.Clamp01(elapsedTime / cooldown);
            imageCooldown.color = c;
        }
        skill.Play("DashBlink");
    }
}
