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
        InvokeRepeating("UpdateHealth", 5, 0.5f);
    }

    void Update()
    {
        //informa o valor da barra de vida.
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0);
    }

    void UpdateHealth() //aplica a vida.
    {
        if (health < 100)
            health++;
    }

    void OnCollisionEnter(Collision col) //metodo de colidir a bala no inimigo e fazer ele perde vida.
    {
        if (col.gameObject.tag == "bullet")
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
        if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        {
            Task.current.Succeed();
        }

    }

    [Task]
    //manda para um destino pre definido.
    public void PickDestination (int x, int z)
    {
        Vector3 dest = new Vector3(x, 0, z);
        agent.SetDestination(dest);
        Task.current.Succeed();
    }
    [Task]
    public void TargetPlayer() //passa as informcao para o player e identifica ele.
    {
        target = player.transform.position;
        Task.current.Succeed();
    }

    [Task]
    public bool Fire() //instancia o prefb da bala.
    {
        GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.transform.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward * 2000);

        return true;
    }

    [Task]
    public void LookAtTarget() //trava a mira no jogador.
    {
        Vector3 direction = target - this.transform.position;

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);

        if(Task.isInspected)
            Task.current.debugInfo = string.Format("angle={0}",
            Vector3.Angle(this.transform.forward, direction));

        if(Vector3.Angle(this.transform.forward, direction) < 5.0f)
        {
            Task.current.Succeed();
        }
    }

    //metodo para encontrar o player ou a parede.
    [Task]
    bool SeePlayer()
    {
        Vector3 distance = player.transform.position - this.transform.position;
        RaycastHit hit;
        bool seeWall = false;
        Debug.DrawRay(this.transform.position, distance, Color.red);
        if (Physics.Raycast(this.transform.position, distance, out hit))
        {
            if(hit.collider.gameObject.tag == "wall")
            {
                seeWall = true;
            }
        }
        if (Task.isInspected)
            Task.current.debugInfo = string.Format("wall{0}", seeWall);

        if (distance.magnitude < visibleRange && !seeWall)
            return true;
        else
            return false;
    }   
     //muda o angulo para posicao.
    [Task]
    bool Turn(float angle)
    {
        var p = this.transform.position + Quaternion.AngleAxis(angle, Vector3.up) * this.transform.forward;
        target = p;
        return true;
    }

}

