using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    public enum MouseState
    {
        none,
        canMove,
        cantMove
    }

    MouseState state;

    public List<GameObject> childs_list = new List<GameObject>();

    public bool IsCellChanged
    {
        get
        {
            return !(lastCell.x == curCell.x && lastCell.y == curCell.y);
        }
    }


    private Vector3Int lastCell = new Vector3Int(0,0,0);
    private Vector3Int curCell = new Vector3Int(0,0,0);

    private Vector3 MouseWorldPos
    {
        get
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            return worldPos;
        }
    }
    private Vector3Int CellPos
    {
        get
        {
            Vector3Int pos = PathFinderManager.instance.grid.WorldToCell(MouseWorldPos);
            return pos;
        }
    }


    void Update()
    {
        transform.position = Input.mousePosition;

        lastCell = curCell;
        curCell = CellPos;

        if(GameManager.instance.gameInputMode == GameManager.InputMode.animation)
        {
            SetState(MouseState.cantMove);
        }
        else
        {
            SetState(MouseState.none);
        }

    }

    public void SetState(MouseState _state)
    {
        if (state == _state)
            return;

        state = _state;

        InActiveAllChild();

        switch(state)
        {
            case MouseState.none:
                GetChild("None").SetActive(true);
                break;
            case MouseState.canMove:
                GetChild("CanMove").SetActive(true);
                break;
            case MouseState.cantMove:
                GetChild("CantMove").SetActive(true);
                break;
            default:
                break;
                
        }
    }

    private GameObject GetChild(string _name)
    {
        foreach(var child in childs_list)
        {
            if(child.name == _name)
            {
                return child;
            }
        }

        return null;
    }

    private void InActiveAllChild()
    {
        foreach(var child in childs_list)
        {
            child.SetActive(false);
        }
    }
}
