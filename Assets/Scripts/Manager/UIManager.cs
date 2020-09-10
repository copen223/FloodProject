using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Struct;

public class UIManager : MonoBehaviour
{
    // 普通UI对象
    public List<GameObject> UI_object_list = new List<GameObject>();

    // 手卡
    public List<CardMono> CardUI_hand_list = new List<CardMono>();
    // 鼠标
    public MouseController mouse;
    public GameObject uiArea;
    // 路径
    public List<GameObject> pathPrefabs_list = new List<GameObject>();
    private List<GameObject> path_zy_list = new List<GameObject>();
    //private List<GameObject> path_zs_list = new List<GameObject>();
    //private List<GameObject> path_zx_list = new List<GameObject>();
    //private List<GameObject> path_ys_list = new List<GameObject>();
    //private List<GameObject> path_yx_list = new List<GameObject>();
    private List<GameObject> path_sx_list = new List<GameObject>();

    public Transform pathParent;

    // 单位 Actor
    public GameObject actor_prefab;
    public List<GameObject> actors_list = new List<GameObject>();
    public Transform actorParent;



    public static UIManager instance;

    public bool IsHandUsing
    {
        get
        {
            bool isUsing = false;
            foreach(var card in CardUI_hand_list)
            {
                if (card.state != CardMono.State.none && card.state != CardMono.State.focused) 
                    isUsing = true;
            }
            return isUsing;
        }
    }

    public bool IsAtUIArea
    {
        get
        {
            bool isAting = false;
            RectTransform trans = uiArea.GetComponent<RectTransform>();
            if (Input.mousePosition.y <= trans.rect.height)
            {
                isAting = true;
            }
            return isAting;

        }
    }



    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InitActorList();
    }

    /// <summary>
    /// 激活UI对象与否，非激活对象脚本不会生效
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isActive"></param>
    public void ActiveUI(string name,bool isActive)
    {
        foreach (var ui in UI_object_list)
        {
            if (ui.name == name)
            {
                ui.SetActive(isActive);
            }
        }
    }

    /// <summary>
    /// 改变UI对象的位置
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pos"></param>
    public void TranslateUIPos(string name,Vector2 pos)
    {
        foreach (var ui in UI_object_list)
        {
            if (ui.name == name)
            {
                ui.transform.position = pos;
            }
        }
    }

    /// <summary>
    /// 隐藏UI对象与否，隐藏对象脚本仍有效
    /// </summary>
    /// <param name="name"></param>
    /// <param name="isHidden"></param>
    public void HideUI(string name,bool isHidden)
    {
        foreach (var ui in UI_object_list)
        {
            if (ui.name == name)
            {
                if(isHidden)
                {
                    ui.transform.localScale = Vector3.zero;
                }
                else
                {
                    ui.transform.localScale = new Vector3(1, 1, 0);
                }
            }
        }
    }

    /// <summary>
    /// 改变TextUI的文本
    /// </summary>
    /// <param name="name"></param>
    /// <param name="text"></param>
    public void UpdateUIText(string name,string text)
    {
        foreach (var ui in UI_object_list)
        {
            if (ui.name == name)
            {
                ui.GetComponent<Text>().text = text;
            }
        }
    }

    /// <summary>
    /// 根据手卡列表更新手卡显示
    /// </summary>
    /// <param name="cards_list"></param>
    public void UpdateHandUI(List<Card> cards_list)
    {
        for (int i = 0; i < CardUI_hand_list.Count; i++)
        {
            CardUI_hand_list[i].gameObject.SetActive(false);
        }

        for (int i =0;i < cards_list.Count;i++)
        {
            CardUI_hand_list[i].gameObject.SetActive(true);
            CardUI_hand_list[i].SetCard(cards_list[i]);
            //CardUI_hand_list[i].WaitForSeconds(1f);
        }

        for (int i = 0; i < cards_list.Count; i++)
        {
            if (CardUI_hand_list[i].state != CardMono.State.focused)
            {
                CardUI_hand_list[i].state = CardMono.State.none;
                CardUI_hand_list[i].OnNone();
            }
            //CardUI_hand_list[i].WaitForSeconds(1f);
        }

        //for (int i= 0; i < cards_list.Count;i++)
        //{
        //    CardUI_hand_list[i].OnNone();
        //}
    }

    public void SetUIScale(string name,Vector3 _scale)
    {
        foreach (var ui in UI_object_list)
        {
            if (ui.name == name)
            {
                ui.transform.localScale = _scale;
            }
        }
    }

    // 鼠标相关
    public void SetMouseState(MouseController.MouseState _state)
    {
        mouse.SetState(_state);
    }

    // 路径相关

    enum PathType
    {
        UpDown,
        LeftRight,
        //LeftUp,
        //LeftDown,
        //RightUp,
        //RightDown    
    }

    private void SetPathAt(PathType pathType,Vector3 worldPos)
    {
        GameObject obj = GetPathObject(pathType);
        obj.GetComponent<WorldUIController>().WorldPos = worldPos;
        obj.SetActive(true);
    }

    public void ShowPath(List<Vector3> path_list)
    {
        InActivePath();
        for (int i = 0; i < path_list.Count - 1; i++)
        {
            var p1 = path_list[i];
            var p2 = path_list[i + 1];

            Vector3 pos = (p2 + p1) / 2;

            if (p2.x == p1.x)
            {
                // sx
                SetPathAt(PathType.UpDown, pos);
            }

            if (p2.y == p1.y)
            {
                SetPathAt(PathType.LeftRight, pos);
            }
            
        }
    }
    
    public void InActivePath()
    {
        foreach(var obj in path_sx_list)
        {
            obj.SetActive(false);
        }
        //foreach (var obj in path_zs_list)
        //{
        //    obj.SetActive(false);
        //}
        //foreach (var obj in path_zx_list)
        //{
        //    obj.SetActive(false);
        //}
        foreach (var obj in path_zy_list)
        {
            obj.SetActive(false);
        }
        //foreach (var obj in path_ys_list)
        //{
        //    obj.SetActive(false);
        //}
        //foreach (var obj in path_yx_list)
        //{
        //    obj.SetActive(false);
        //}
    }

    private GameObject GetPathObject(PathType pathType)
    {
        switch(pathType)
        {
            case PathType.UpDown:
                foreach(var ob in path_sx_list)
                {
                    if (!ob.activeInHierarchy)
                        return ob;
                }
                GameObject obj1 = Instantiate(pathPrefabs_list[0], pathParent);
                path_sx_list.Add(obj1);
                obj1.SetActive(false);
                return obj1;
            //case PathType.RightUp:
            //    foreach (var ob in path_ys_list)
            //    {
            //        if (!ob.activeInHierarchy)
            //            return ob;
            //    }
            //    GameObject obj2 = Instantiate(pathPrefabs_list[1], pathParent);
            //    path_sx_list.Add(obj2);
            //    obj2.SetActive(false);
            //    return obj2;
            //case PathType.RightDown:
            //    foreach (var ob in path_yx_list)
            //    {
            //        if (!ob.activeInHierarchy)
            //            return ob;
            //    }
            //    GameObject obj3= Instantiate(pathPrefabs_list[2], pathParent);
            //    path_sx_list.Add(obj3);
            //    obj3.SetActive(false);
            //    return obj3;
            //case PathType.LeftUp:
            //    foreach (var ob in path_zs_list)
            //    {
            //        if (!ob.activeInHierarchy)
            //            return ob;
            //    }
            //    GameObject obj4 = Instantiate(pathPrefabs_list[3], pathParent);
            //    path_sx_list.Add(obj4);
            //    obj4.SetActive(false);
            //    return obj4;
            //case PathType.LeftDown:
            //    foreach (var ob in path_zx_list)
            //    {
            //        if (!ob.activeInHierarchy)
            //            return ob;
            //    }
            //    GameObject obj5 = Instantiate(pathPrefabs_list[4], pathParent);
            //    path_sx_list.Add(obj5);
            //    obj5.SetActive(false);
            //    return obj5;
            case PathType.LeftRight:
                foreach (var ob in path_zy_list)
                {
                    if (!ob.activeInHierarchy)
                        return ob;
                }
                GameObject obj6 = Instantiate(pathPrefabs_list[5], pathParent);
                path_zy_list.Add(obj6);
                obj6.SetActive(false);
                return obj6;

        }
        return null;
    }



    // 单位相关

    public void InitActorList()
    {
        if(actors_list.Count <=0)
        {
            for (int i = 0; i < 5; i++)
            {
                GameObject gb = GameObject.Instantiate(actor_prefab, actorParent);
                actors_list.Add(gb);
            }
        }
    }

    public void SetActorsUI(List<GameObject> actorObjs_list)
    {
        for(int i =0;i<actorObjs_list.Count;i++)
        {
            actors_list[i].GetComponent<ActorUIController>().actor = actorObjs_list[i];
            actors_list[i].GetComponent<ActorUIController>().UpdateHpUI(true);
        }
        for(int i= actorObjs_list.Count;i<actors_list.Count;i++)
        {
            actors_list[i].GetComponent<ActorUIController>().actor = null;
            actors_list[i].GetComponent<ActorUIController>().UpdateHpUI(false);
        }
    }

    public void UpdateActorDirSignUI(GameObject actor,bool isActive)
    {
        for(int i =0;i<actors_list.Count;i++)
        {
            GameObject actorUI = actors_list[i];
            if(actorUI.GetComponent<ActorUIController>().actor == actor)
            {
                actorUI.GetComponent<ActorUIController>().UpdateDirSignUI(isActive);
                return;
            }
        }
    }

    public void UpdateActorHpUI(GameObject actor,bool isActive)
    {
        for (int i = 0; i < actors_list.Count; i++)
        {
            GameObject actorUI = actors_list[i];
            if (actorUI.GetComponent<ActorUIController>().actor == actor)
            {
                actorUI.GetComponent<ActorUIController>().UpdateHpUI(isActive);
                //if(!actor.activeInHierarchy || actor.GetComponent<ActorMono>().healPoint <=0)
                //    actorUI.GetComponent<ActorUIController>().UpdateHpUI(false);
                return;
            }
        }
    }

    public void UpdateActorFloatUI(GameObject actor,string text, int pos)
    {
        for (int i = 0; i < actors_list.Count; i++)
        {
            GameObject actorUI = actors_list[i];
            if (actorUI.GetComponent<ActorUIController>().actor == actor)   
            {
                actorUI.GetComponent<ActorUIController>().UpdateFloatText(text,pos);
                return;
            }
        }
    }
    public void UpdateActorFloatUI(GameObject actor, string text, int pos,Color color)
    {
        for (int i = 0; i < actors_list.Count; i++)
        {
            GameObject actorUI = actors_list[i];
            if (actorUI.GetComponent<ActorUIController>().actor == actor)
            {
                actorUI.GetComponent<ActorUIController>().UpdateFloatText(text, pos, color);
                return;
            }
        }
    }
    public void UpdateActorFloatUI(GameObject actor, string text, int pos,bool isActive)
    {
        for (int i = 0; i < actors_list.Count; i++)
        {
            GameObject actorUI = actors_list[i];
            if (actorUI.GetComponent<ActorUIController>().actor == actor)
            {
                actorUI.GetComponent<ActorUIController>().UpdateFloatText(text, pos);
                return;
            }
        }
    }
}
