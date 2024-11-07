using UnityEngine;
public class Peanut : MonoBehaviour
{
    bool isLow = false;
    bool isVeryLow = false;
    public Sprite normal;
    public Sprite low;
    public Sprite veryLow;
    private float previousHp;

    void Update()
    {
        float hp = GetComponent<Plant>().health;

        // Only call ChangeHp when health changes state
        if (hp != previousHp)
        {
            if (hp < 500 && hp > 200 && !isLow)
            {
                isLow = true;
                isVeryLow = false;
                ChangeHp();
            }
            else if (hp < 200 && !isVeryLow)
            {
                isLow = false;
                isVeryLow = true;
                ChangeHp();
            }
            else if (hp >= 500 && (isLow || isVeryLow))
            {
                isLow = false;
                isVeryLow = false;
                ChangeHp();
            }

            previousHp = hp;
        }
    }

    void ChangeHp()
    {
        if (isVeryLow)
        {
            GetComponent<SpriteRenderer>().sprite = veryLow;
        }
        else if (isLow)
        {
            GetComponent<SpriteRenderer>().sprite = low;
        }
        else
        {
            return;
        }
    }
}
