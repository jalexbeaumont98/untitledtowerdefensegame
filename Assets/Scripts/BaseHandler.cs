using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHandler : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameState gameState;

    public delegate void BaseEvent();

    public event BaseEvent OnBaseHit;

    private void Start()
    {
        gameState = GameState.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log("Collision");

        if (collision == null) return;

        if (collision.gameObject.CompareTag("Enemy")) {

            Debug.Log("Is Enemy");

            EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

            if (enemy == null) return;

            gameState.SetHealth(enemy.GetDamage());
            enemy.Die(true);

            OnBaseHit?.Invoke();
        }

        else { Debug.Log("Not Enemy"); }

        

    }

}
