using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public float speed_min;
    public float speed_value;
    public float speed_battle;
    public Vector2 offset;
    public Vector2 offset_battle;
    public float shake_max;
    // Update is called once per frame
    void Update()
    {
        switch (GameManager.instance.gameState)
        {
            case GameManager.State.Explore:ChaseTarget();break;
            case GameManager.State.Battle:MoveByInput();break;
        }
        
    }

    private void ChaseTarget()
    {
        Vector2 pos = (Vector2)target.transform.position + offset;

        Vector2 dis = pos - (Vector2)transform.position;

        Vector2 dir = dis.normalized;

        float speed = dis.sqrMagnitude * speed_value > speed_min ? dis.sqrMagnitude * speed_value : speed_min;

        if (dis.sqrMagnitude < shake_max)
            return;

        transform.Translate(dir * speed * Time.deltaTime);
    }

    private void MoveByInput()
    {
        if (BattleManager.instance.actor_curTurn == null)
            return;

        if(BattleManager.instance.actor_curTurn.group == ActorMono.Group.monster)
        {
            Vector3 pos = BattleManager.instance.actor_curTurn.transform.position;
            transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            return;
        }

        Vector2 dir = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.A))
        {
            dir += new Vector2(-1, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            dir += new Vector2(1, 0);
        }
        if (Input.GetKey(KeyCode.W))
        {
            dir += new Vector2(0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir += new Vector2(0, -1);
        }

        transform.Translate(dir * speed_battle * Time.deltaTime);
    }

    public void MoveToTarget(GameObject target)
    {
        Vector3 pos = target.transform.position;
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }
}
