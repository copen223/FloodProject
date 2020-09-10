using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveComponent : MonoBehaviour
{
    // 移动相关参数
    public Vector2 disWithDir;
    private bool isMoving;

    // 碰撞检测
    public bool IsColliding_down { get; private set; }
    public bool IsColliding_left { get; private set; }
    public bool IsColliding_right { get; private set; }
    public bool IsColliding_up { get; private set; }
    public bool IsColliding_advance { get; private set; }
    public bool IsColliding_ladder_down { get; private set; }
    public bool IsColliding_ladder_up { get; private set; }

    public float ray_up_dis;
    public float ray_down_dis;
    public float ray_x_dis;

    public int ray_y_num;
    public int ray_x_num;

    public float ray_x_start_offset;
    public float ray_up_start_offset;
    public float ray_down_start_offset;

    public float ray_advance_offset;

    private float ray_offset_up;
    private float ray_offset_down;
    private float ray_offset_left;
    private float ray_offset_right;

    private float move_offset_up;
    private float move_offset_down;
    private float move_offset_left;
    private float move_offset_right;

    private void Update()
    {
        CheckIfCollide();

        DoMove();
    }

    private void CheckIfCollide()
    {

        ray_offset_left = disWithDir.x < 0 ? (-disWithDir.x) : 0;
        ray_offset_right = disWithDir.x > 0 ? disWithDir.x : 0;
        ray_offset_up = disWithDir.y > 0 ? disWithDir.y : 0;
        ray_offset_down = disWithDir.y < 0 ? (-disWithDir.y) : 0;
        
        float ray_x_interval = (ray_up_dis + ray_down_dis - ray_up_start_offset - ray_down_start_offset) / ray_x_num;
        // 向左射线
        do
        {
            IsColliding_left = false;

            List<RaycastHit2D> hit_list = new List<RaycastHit2D>();

            for (int i = 0; i < ray_x_num; i++)
            {
                Vector2 start = (Vector2)transform.position + Vector2.up * (i * ray_x_interval - ray_down_dis + ray_down_start_offset);
                RaycastHit2D[] hits = Physics2D.RaycastAll(start, Vector2.left, ray_x_dis + ray_offset_left);
                Debug.DrawRay(start, Vector2.left * (ray_x_dis + ray_offset_left), Color.green);

                foreach(var hit in hits)
                {
                    if(!hit_list.Contains(hit))
                    {
                        hit_list.Add(hit);
                    }
                }
            }

            foreach (var hit_left in hit_list)
            {
                if (hit_left.collider.tag == "Obstacle")
                {
                    IsColliding_left = true;
                    float newOffset = -hit_left.point.x + transform.position.x - ray_x_dis;
                    if (newOffset < 0)
                        newOffset = 0;
                    if (move_offset_left > newOffset)
                    {
                        move_offset_left = newOffset;
                    }

                }
            }
        }
        while (false);
        // 向右射线
        do
        {
            IsColliding_right = false;

            List<RaycastHit2D> hit_list = new List<RaycastHit2D>();

            for (int i = 0; i < ray_x_num; i++)
            {
                Vector2 start = (Vector2)transform.position + Vector2.up * (i * ray_x_interval - ray_down_dis + ray_down_start_offset);
                RaycastHit2D[] hits = Physics2D.RaycastAll(start, Vector2.right, ray_x_dis + ray_offset_right);
                Debug.DrawRay(start, Vector2.right * (ray_x_dis + ray_offset_right), Color.red);

                foreach (var hit in hits)
                {
                    if (!hit_list.Contains(hit))
                    {
                        hit_list.Add(hit);
                    }
                }
            }

            foreach (var hit_right in hit_list)
            {
                if (hit_right.collider.tag == "Obstacle")
                {
                    IsColliding_right = true;

                    float newOffset = hit_right.point.x - transform.position.x - ray_x_dis;
                    if (newOffset < 0)
                        newOffset = 0;
                    if (move_offset_right > newOffset)
                    {
                        move_offset_right = newOffset;
                    }
                }
            }
        }
        while (false);
        // 向下射线
        float ray_y_interval = (2 * (ray_x_dis - ray_x_start_offset)) / ray_y_num;
        do
        {
            IsColliding_down = false;
            IsColliding_advance = false;
            IsColliding_ladder_down = false;

            List<RaycastHit2D> hit_list = new List<RaycastHit2D>();

            for (int i = 0; i < ray_y_num; i++)
            {
                Vector2 start = (Vector2)transform.position + Vector2.right * (-ray_x_dis + i * ray_y_interval + ray_x_start_offset);
                RaycastHit2D[] hits = Physics2D.RaycastAll(start, Vector2.down, ray_down_dis + ray_offset_down);
                Debug.DrawRay(start, Vector2.down * (ray_down_dis + ray_offset_down), Color.red);

                RaycastHit2D[] hits_advance = Physics2D.RaycastAll(start, Vector2.down, ray_down_dis*(1 + ray_advance_offset) + ray_offset_down );

                foreach (var hit_advance in hits_advance)
                {
                    if (hit_advance)
                    {
                        if (hit_advance.collider.tag == "Obstacle")
                            IsColliding_advance = true;
                    }
                }

                foreach (var hit in hits)
                {
                    if (!hit_list.Contains(hit))
                    {
                        hit_list.Add(hit);
                    }
                }


                
            }

            foreach (var hit_down in hit_list)
            {
                if (hit_down.collider.tag == "Obstacle")
                {
                    IsColliding_down = true;

                    float newOffset = -hit_down.point.y + transform.position.y - ray_down_dis ;
                    if (newOffset < 0)
                        newOffset = 0;
                    Debug.Log(newOffset + " " + move_offset_down);

                    move_offset_down = newOffset;

                    if (move_offset_down > newOffset)
                    {
                        move_offset_down = newOffset;
                    }
                }

                if(hit_down.collider.tag == "Ladder")
                {
                    IsColliding_ladder_down = true;
                }
            }

            if(IsColliding_ladder_down ==false)
            {
                foreach (var hit_down in hit_list)
                {
                    //Debug.Log(hit_down.collider + ""+Time.time) ;
                }
            }
        }
        while (false);
        // 向上射线
        do
        {
            IsColliding_up = false;
            IsColliding_ladder_up = false;

            List<RaycastHit2D> hit_list = new List<RaycastHit2D>();

            for (int i = 0; i < ray_y_num; i++)
            {
                Vector2 start = (Vector2)transform.position + Vector2.right * (i * ray_y_interval - ray_x_dis +ray_x_start_offset);
                RaycastHit2D[] hits = Physics2D.RaycastAll(start, Vector2.up, ray_up_dis + ray_offset_up);
                Debug.DrawRay(start, Vector2.up * (ray_up_dis + ray_offset_up), Color.red);

                foreach (var hit in hits)
                {
                    if (!hit_list.Contains(hit))
                    {
                        hit_list.Add(hit);
                    }
                }

            }

            foreach (var hit_up in hit_list)
            {
                if (hit_up.collider.tag == "Obstacle")
                {
                    IsColliding_up = true;

                    float newOffset = hit_up.point.y - transform.position.y - ray_up_dis;
                    if (newOffset < 0)
                        newOffset = 0;
                    if (move_offset_up > newOffset)
                    {
                        move_offset_up = newOffset;
                    }
                }

                if(hit_up.collider.tag == "Ladder")
                {
                    IsColliding_ladder_up = true;
                }
            }
        }
        while (false);

    }

    private void DoMove()
    {
        if (isMoving)
        {
            if (IsColliding_right)
            {
                if (disWithDir.x > 0)
                {
                    disWithDir.x = move_offset_right;
                }
            }

            if (IsColliding_left)
            {
                if (disWithDir.x < 0)
                {
                    disWithDir.x = - move_offset_left;
                }
            }

            if (IsColliding_up)
            {
                if (disWithDir.y > 0)
                {
                    disWithDir.y = move_offset_up;
                }
            }

            if (IsColliding_down)
            {
                if (disWithDir.y < 0)
                {
                    disWithDir.y = - move_offset_down;
                }
            }

            transform.Translate(disWithDir);
        }
    }

    public void Move(Vector2 disWithDir)
    {
        this.disWithDir = disWithDir;
        isMoving = true;
    }

    public void Move()
    {
        isMoving = true;
    }

    public void Stop()
    {
        isMoving = false;
    }
}
