using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;//事件
public class Character : MonoBehaviour,ISaveable
{
    [Header("事件监听")]
    public VoidEventSO newGameEvent;
    [Header("基本属性")]
    public float maxHealth;//最大血量
    public float currentHealth;//当前血量
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("受伤无敌")]
    public float invulnerableDuration;
    public float invulnerableCounter;
    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;//创建受伤事件，在页面中显示。传输的是坐标进来
    public UnityEvent OnDie;//创建死亡事件

    private void Awake()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
    }
    private void NewGame()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        OnHealthChange?.Invoke(this);
    }

    private void OnEnable()
    {
        newGameEvent.OnEventRaised += NewGame;
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= NewGame;
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }
    private void Update()
    {
        if(invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;//无敌计数器-去当前过去的时间
            if(invulnerableCounter<=0)
            {
                invulnerable = false;//当无敌时间小于0，退出无敌状态
            }
        }
        if (currentPower < maxPower)
        {
            currentPower += Time.deltaTime * powerRecoverSpeed;
        }
    }



    public void TakeDamage(Attack attacker)
    {
        if(invulnerable)
        {
            return;//刚减去血量 的时候进入无敌状态，所以跳过扣血步骤
        }
        //Debug.Log(attacker.damage);
        if(currentHealth-attacker.damage>0)
        {
            currentHealth -= attacker.damage;//当前血量减去伤害
            TriggerInvulnerable();//触发无敌
            //执行受伤
            OnTakeDamage?.Invoke(attacker.transform);//执行注册过来的所有受伤的方法
        }
        else
        {
            currentHealth = 0;
            //触发死亡
            OnDie?.Invoke();
        }
        OnHealthChange?.Invoke(this);
    }

    private void TriggerInvulnerable()
    {
        if(!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }
    public void OnSlide(int cost)
    {
        currentPower -= cost;
        OnHealthChange?.Invoke(this);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water"))
        {
            if(currentHealth>0)
            { 
                currentHealth = 0;
                OnHealthChange?.Invoke(this);
                OnDie?.Invoke();
            }
          
        }
    }

    public DataDifination GetDataID()
    {
        return GetComponent<DataDifination>();
    }

    public void GetSaveData(Data data)
    {
       if(data.characterPosDict.ContainsKey(GetDataID().ID))//有则修改
        {
            data.characterPosDict[GetDataID().ID] = new Data.SerializeVector3(transform.position);
            data.floatSaveData[GetDataID().ID + "health"] = this.currentHealth;//保存血量
            data.floatSaveData[GetDataID().ID + "power"] = this.currentPower;//保存能量
        }
        else//无则添加
        {
            data.characterPosDict.Add(GetDataID().ID, new Data.SerializeVector3(transform.position));
            data.floatSaveData.Add(GetDataID().ID + "health", this.currentHealth);
            data.floatSaveData.Add(GetDataID().ID + "power", this.currentPower);
        }
    }

    public void LoadData(Data data)
    {
        if(data.characterPosDict.ContainsKey(GetDataID().ID))
        {
            transform.position = data.characterPosDict[GetDataID().ID].ToVector3();
            this.currentHealth = data.floatSaveData[GetDataID().ID + "health"];
            this.currentPower = data.floatSaveData[GetDataID().ID + "power"];

            //通知ui更新
            OnHealthChange?.Invoke(this);
        }
    }
}
