using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private void Start()
    {
        component_move = gameObject.GetComponent<MoveComponent>();
    }

    private void Update()
    {
        if (controlMode == ControlMode.Battle)
            return;

        switch(state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Walk:
                Walk();
                break;
            case State.Jump:
                Jump();
                break;
            case State.Climb:
                Climb();
                break;

        }
    }


    public enum State
    {
        Idle,
        Walk,
        Jump,
        Climb
    }

    public enum ControlMode
    {
        Explore,
        Battle
    }

    public State state;
    public ControlMode controlMode;

    // 各状态属性
    public float walk_speed;
    public float jump_speed_start;
    private float jump_speed;
    public float jump_addSpeed;
    public float jump_value;
    public float climb_speed;

    // 组件对象
    private MoveComponent component_move;

    private void Idle()
    {
        component_move.Stop();

        if((component_move.IsColliding_ladder_up && Input.GetKeyDown(KeyCode.W)) || (component_move.IsColliding_ladder_down && Input.GetKeyDown(KeyCode.S)))
        {
            state = State.Climb;
            Climb();
            return;
        }
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            state = State.Walk;
            Walk();
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            state = State.Jump;
            jump_speed = jump_speed_start;
            Jump();
            return;
        }
        if (!component_move.IsColliding_down && !component_move.IsColliding_ladder_down)
        {
            state = State.Jump;
            jump_speed = 0;
            Jump();
            return;
        }
    }

    private void Walk()
    {
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            state = State.Idle;
            Idle();
            return;
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            state = State.Jump;
            jump_speed = jump_speed_start;
            Jump();
            return;
        }
        if(!component_move.IsColliding_down && !component_move.IsColliding_ladder_down)
        {
            state = State.Jump;
            jump_speed = 0;
            Jump();
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

        component_move.Move(dir * walk_speed * Time.deltaTime);
    
    }

    private void Jump()
    {
        if(component_move.IsColliding_down && jump_speed <= 0)
        {
            state = State.Idle;
            Idle();
            return;
        }
        if ((component_move.IsColliding_ladder_up && Input.GetKeyDown(KeyCode.W)) || (component_move.IsColliding_ladder_down && Input.GetKeyDown(KeyCode.S)))
        {
            state = State.Climb;
            Climb();
            return;
        }

        if (component_move.IsColliding_up && jump_speed>0)
        {
            jump_speed = 0;
        }
        if(Input.GetKeyDown(KeyCode.Space) && jump_speed<0)
        {
            if(component_move.IsColliding_advance)
                jump_speed = jump_speed_start;
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
        if(Input.GetKeyUp(KeyCode.Space) && jump_speed > 0)
        {
            jump_speed *= jump_value;
        }

        jump_speed += jump_addSpeed * Time.deltaTime;

        Vector2 dis = new Vector2(dir.x * walk_speed, jump_speed);

        component_move.Move(dis * Time.deltaTime);
    }

    private void Climb()
    {
        if(!(component_move.IsColliding_ladder_up && component_move.IsColliding_ladder_down))
        {
            if (!((Input.GetKey(KeyCode.S) && !component_move.IsColliding_ladder_up)|| (Input.GetKey(KeyCode.W) && !component_move.IsColliding_ladder_down)))
            {
                state = State.Idle;
                Idle();
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            state = State.Jump;
            jump_speed = jump_speed_start;
            Jump();
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
        if(Input.GetKey(KeyCode.W))
        {
            dir += new Vector2(0, 1);
        }
        if(Input.GetKey(KeyCode.S))
        {
            dir += new Vector2(0, -1);
        }


        Vector2 dis = new Vector2(dir.x * walk_speed, dir.y * climb_speed);

        component_move.Move(dis * Time.deltaTime);
    }


    public void OnBattle()
    {
        controlMode = ControlMode.Battle;
        component_move.Stop();
    }
}
