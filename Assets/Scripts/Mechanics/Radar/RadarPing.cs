    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarPing : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float dissapearTimer;
    private float dissapearTimerMax;
    private Color colour;

    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        dissapearTimerMax = 1f;
        dissapearTimer = 0f;
        colour = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        dissapearTimer += Time.deltaTime;
        colour.a = Mathf.Lerp(dissapearTimerMax, 0f, dissapearTimer / dissapearTimerMax);
        spriteRenderer.color = colour;

        if(dissapearTimer >= dissapearTimerMax)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetColour(Color color)
    {
        this.colour = color;
    }

    public void SetDisappearTimer(float maxTimer)
    {
        this.dissapearTimerMax = maxTimer;
        dissapearTimer = 0f;
    }
}
