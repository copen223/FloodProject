using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Struct;
using UnityEngine.UI;

public class ActorUIController : MonoBehaviour
{
    // 单位对象
    public GameObject actor;

    // UI对象
    public GameObject dirSign;
    public GameObject dirSign_up;
    public GameObject dirSign_down;
    public Text dirSign_up_text;
    public Text dirSign_down_text;

    public GameObject healPoint_text;
    public GameObject healPoint;

    public GameObject action_text;
    public GameObject healPointReduce_text;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if(actor!=null)
        {
            ActorMono actor_mono = actor.GetComponent<ActorMono>();
            Grid grid = PathFinderManager.instance.grid;

            Vector3 worldPos = actor_mono.WorldPos + grid.cellSize.y * new Vector3(0, 1.5f, 0);
            worldPos.x = actor_mono.WorldPos.x;
            if(dirSign.activeInHierarchy)
                dirSign.transform.position = Camera.main.WorldToScreenPoint(worldPos);

            worldPos += grid.cellSize.y * new Vector3(0, -2.3f, 0);
            if(healPoint.activeInHierarchy)
                healPoint.transform.position = Camera.main.WorldToScreenPoint(worldPos);

            worldPos += grid.cellSize.y * new Vector3(0f, 2.3f, 0);

            if (healPointReduce_text.activeInHierarchy)
            {
                Color color = healPointReduce_text.GetComponent<Text>().color;
                healPointReduce_text.GetComponent<Text>().color = new Color(color.r, color.g, color.b, color.a -= Time.deltaTime);
                healPointReduce_text.transform.position = Camera.main.WorldToScreenPoint(worldPos);

                if(color.a<=0)
                {
                    healPointReduce_text.SetActive(false);
                }
            }

            worldPos += grid.cellSize.y * new Vector3(0f, -0.4f, 0);
            if (action_text.activeInHierarchy)
            {
                action_text.transform.position = Camera.main.WorldToScreenPoint(worldPos);
                Color color = action_text.GetComponent<Text>().color;
                action_text.GetComponent<Text>().color = new Color(color.r, color.g, color.b, color.a -= Time.deltaTime);
                if (color.a <= 0)
                {
                    action_text.SetActive(false);
                }
            }
        }
    }

    private void ChangeDirSign(CardSign up,CardSign down)
    {
        ChangeDirSignColor(up, dirSign_up);
        ChangeDirSignText(up, dirSign_up_text);
        ChangeDirSignColor(down, dirSign_down);
        ChangeDirSignText(down, dirSign_down_text);
    }

    private void ChangeDirSignColor(CardSign sign,GameObject signUI)
    {
        if (sign.type == CardSign.Type.atk)
        {
            signUI.GetComponent<Image>().color = Color.red;
        }
        else if (sign.type == CardSign.Type.dfd)
        {
            signUI.GetComponent<Image>().color = Color.green;
        }
        else if (sign.type == CardSign.Type.none)
        {
            signUI.GetComponent<Image>().color = Color.white;
        }
    }

    private void ChangeDirSignText(CardSign sign,Text text)
    {
        if(sign.intensity <= 0)
        {
            text.text = "";
        }
        else
        {
            text.text = CardSign.SignTypeName(sign.subType) + sign.intensity;
        }
    }

    public void UpdateDirSignUI(bool isActive)
    {
        if(!isActive)
        {
            dirSign.SetActive(false);
            return;
        }

        dirSign.SetActive(true);

        ActorMono actor_mono = actor.GetComponent<ActorMono>();

        ChangeDirSign(actor_mono.FocusedCard.sign_up, actor_mono.FocusedCard.sign_down);
    }

    public void UpdateHpUI(bool isActive)
    {
        if (!isActive)
        {
            dirSign.SetActive(false);
            return;
        }

        healPoint.SetActive(true);

        ActorMono actor_mono = actor.GetComponent<ActorMono>();
        
        healPoint_text.GetComponent<Text>().text = actor_mono.healPoint + "";
        
    }

    public void UpdateFloatText(string text,int pos)
    {
        if(pos == 0)
        {
            healPointReduce_text.GetComponent<Text>().text = text;
            healPointReduce_text.GetComponent<Text>().color = new Color(1, 0, 0, 1);
            healPointReduce_text.SetActive(true);
        }
        if(pos == 1)
        {
            action_text.GetComponent<Text>().text = text;
            action_text.GetComponent<Text>().color = new Color(0, 0, 0, 1);
            action_text.SetActive(true);
        }
    }
}
