using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMonsterStats : MonoBehaviour
{
    public Text MonsterName;
    public Image MonsterHP;
    public Text HPText;
    public BaseEnemy target;

    private void Update()
    {
        MonsterHP.transform.position = Camera.main.WorldToScreenPoint(target.hpPos.position);
        MonsterName.transform.position = Camera.main.WorldToScreenPoint(target.namePos.position);
    }

    public void Init(BaseEnemy enemy, string Name)    
    {
        target = enemy;
        MonsterName.text = Name;
        enemy.OnDeath += EnemyDead;
    }

    void EnemyDead()
    {
        target.OnDeath -= EnemyDead;
        Destroy(gameObject);
    }
}
