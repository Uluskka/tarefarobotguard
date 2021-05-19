using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using Panda;

public class AI : MonoBehaviour
{
    public Transform player; //posica do player.
    public Transform bulletSpawn; //posicao da bala.
    public Slider healthBar;   //barra de vida.
    public GameObject bulletPrefab; //prefb das balas.

    NavMeshAgent agent;
    public Vector3 destination; // The movement destination.
    public Vector3 target;      // The position to aim to.
    float health = 100.0f;      //vida
    float rotSpeed = 5.0f;

    float visibleRange = 80.0f;
    float shotRange = 40.0f; //alcance do disparo da bala

    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = shotRange - 5; //for a little buffer
        InvokeRepeating("UpdateHealth",5,0.5f);
    }

    void Update()
    {
        //informa o valor da barra de vida.
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0,60,0);
    }

    void UpdateHealth() //aplica a vida.
    {
       if(health < 100) 
        health ++;
    }

    void OnCollisionEnter(Collision col) //metodo de colidir a bala no inimigo e fazer ele perde vida.
    {
        if(col.gameObject.tag == "bullet")
        {
            health -= 10;
        }
    }

    [Task]
    public void PickRandomDestination()
    {
        //metodo para deixa os movimentos do bot randomizado.
        Vector3 dest = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        agent.SetDestination(dest);
        Task.current.Succeed();
    }

    [Task]
    public void MoveToDestination()
    {
        //passa as informacoes inspector do script.
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("t={0:0.00}", Time.time);
        if(agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }
    }
}

