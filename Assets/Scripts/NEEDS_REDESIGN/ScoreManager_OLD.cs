using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager_OLD : MonoBehaviour
{
    public static ScoreManager_OLD inst;
    public int startingHealth;
    public int startingMoney;

    public Text moneyText;
    public Slider healthSlider;
    public Text rebuildText;
    public GameObject rebuildButton;
    public Enemy enemy;

    public AudioClip atDestination;
    public AudioClip malAtDestination;
    public AudioClip destoyed;
    private AudioSource audioSource;

    private int money;
    private int health;
    private int rebuildCost = 0;

    void Awake()
    {
        inst = this;
    }

    public int Money
    {
        get
        {
            return money;
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rebuildButton.SetActive(false);
        health = startingHealth;
        money = startingMoney;
        UpdateOnMoneyValueChanged();
    }

    public void Spend(int amount)
    {
        money -= amount;
        UpdateOnMoneyValueChanged();
    }

    public void Earn(int amount, bool sell = false)
    {
        if(health > 0 || sell)
        {
            audioSource.PlayOneShot(atDestination);
            money += amount;
            UpdateOnMoneyValueChanged();
        }
    }

    public void Damage(int amount)
    {
        audioSource.PlayOneShot(malAtDestination);
        //enemy.Earn();
        if (health > amount)
        {
            health -= amount;
            rebuildCost = 50;
            rebuildButton.SetActive(true);
            UpdateOnMoneyValueChanged();
        }
        else
        {
            health = 0;
            rebuildCost = 500;
            UpdateOnMoneyValueChanged();
        }
    }

    public void Heal()
    {
        if(money >= rebuildCost)
        {
            health = startingHealth;
            money -= rebuildCost;
            rebuildButton.SetActive(false);
            UpdateOnMoneyValueChanged();
        }
    }

    private void UpdateOnMoneyValueChanged()
    {
        if (EntityManager.inst.isMultiplayer)
        {
            Shared.inst.gameMetrics.whitehat_cash = money;
            Shared.inst.syncEvents.Add(new SyncEvent(MessageTypes.UPDATE_WHITE_HAT_MONEY, money.ToString()));
        }


        moneyText.text = "$" + money.ToString() + ".00";
        healthSlider.value = ((float)health / (float)startingHealth);
        rebuildText.text = "Rebuild: $" + rebuildCost.ToString();
    }

    public void PlayDestroySound()
    {
        audioSource.PlayOneShot(destoyed);
    }
}