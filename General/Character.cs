using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;//�¼�
public class Character : MonoBehaviour,ISaveable
{
    [Header("�¼�����")]
    public VoidEventSO newGameEvent;
    [Header("��������")]
    public float maxHealth;//���Ѫ��
    public float currentHealth;//��ǰѪ��
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    [Header("�����޵�")]
    public float invulnerableDuration;
    public float invulnerableCounter;
    public bool invulnerable;

    public UnityEvent<Character> OnHealthChange;
    public UnityEvent<Transform> OnTakeDamage;//���������¼�����ҳ������ʾ����������������
    public UnityEvent OnDie;//���������¼�

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
            invulnerableCounter -= Time.deltaTime;//�޵м�����-ȥ��ǰ��ȥ��ʱ��
            if(invulnerableCounter<=0)
            {
                invulnerable = false;//���޵�ʱ��С��0���˳��޵�״̬
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
            return;//�ռ�ȥѪ�� ��ʱ������޵�״̬������������Ѫ����
        }
        //Debug.Log(attacker.damage);
        if(currentHealth-attacker.damage>0)
        {
            currentHealth -= attacker.damage;//��ǰѪ����ȥ�˺�
            TriggerInvulnerable();//�����޵�
            //ִ������
            OnTakeDamage?.Invoke(attacker.transform);//ִ��ע��������������˵ķ���
        }
        else
        {
            currentHealth = 0;
            //��������
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
       if(data.characterPosDict.ContainsKey(GetDataID().ID))//�����޸�
        {
            data.characterPosDict[GetDataID().ID] = new Data.SerializeVector3(transform.position);
            data.floatSaveData[GetDataID().ID + "health"] = this.currentHealth;//����Ѫ��
            data.floatSaveData[GetDataID().ID + "power"] = this.currentPower;//��������
        }
        else//�������
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

            //֪ͨui����
            OnHealthChange?.Invoke(this);
        }
    }
}
