using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drive : MonoBehaviour { // movimentacao do personagem.

	float speed = 20.0F; //velociade em que se move
    float rotationSpeed = 120.0F; //velociade da rotacao.
    public GameObject bulletPrefab; //prefeb da bala
    public Transform bulletSpawn; //metodo para a bala spawn

    void Update() {
        float translation = Input.GetAxis("Vertical") * speed; //velociade da rotacao na vertical.
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed; //velocidade da rotacao na horinzontal.
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        if(Input.GetKeyDown("space")) //metodo para disparar as balas e colidir com o inimigo.
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }
    }
}
