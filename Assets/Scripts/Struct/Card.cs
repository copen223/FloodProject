using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Struct
{
    public class Card
    {
        public string cardName;
        public ActorMono holder;
        public CardSign sign_up;
        public CardSign sign_down;
        public List<CardEffect> effects_list = new List<CardEffect>();
        public enum CastType
        {
            无,
            指向单体,
            射线单体
        }

        public CastType cast_type;
        public int memory_cost;
        public int cast_extent_x;
        public int cast_extent_y;

        public bool isFocused;
        public int focusCount;


        public float damage_multiply;

        public bool IfCanCast(ActorMono _holde,ActorMono actor)
        {
            float dis_x = UnityEngine.Mathf.Abs(actor.WorldPos.x - _holde.WorldPos.x);
            float dis_y = UnityEngine.Mathf.Abs(actor.WorldPos.y - _holde.WorldPos.y);
            if (cast_type == CastType.指向单体)
            {
                if (dis_x > cast_extent_x + 0.5f || dis_y > cast_extent_y + 0.5f)
                {
                    return false;
                }
                else
                    return true;
            }

            if(cast_type == CastType.射线单体)
            {
                float dis = Vector3.Distance(actor.WorldPos, _holde.WorldPos);
                UnityEngine.RaycastHit2D[] hits = Physics2D.RaycastAll(_holde.WorldPos, (actor.WorldPos - _holde.WorldPos).normalized, dis);
                RaycastHit2D hit = new RaycastHit2D();
                for(int i = 0;i<hits.Length;i++)
                {
                    hit = hits[i];
                    if (hit.collider.tag == "Obstacle")
                    {
                        return false;
                    }
                    if (hit.collider.GetComponent<ActorMono>() != null)
                    {
                        if (hit.collider.GetComponent<ActorMono>() == actor)
                            return true;
                    }
                }
            }

            return false;
        }
    }
}
