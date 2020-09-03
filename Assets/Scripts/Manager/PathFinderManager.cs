using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using UnityEngine;

public class PathFinderManager : MonoBehaviour
{
    public static PathFinderManager instance;

    private void Awake()
    {
        instance = this;
    }

    public Grid grid;
    public GameObject prefab;

    private NodeMap nodeMap;

    public Vector2Int startPos;
    public Vector2Int endPos;
    
    public void SearchPathTo(Vector2Int start,Vector2Int end)
    {
        AStar aStar = new AStar(start, end, nodeMap/*,this*/);

        List<Vector2Int> path = aStar.Search();

        if(path ==null)
        {
            Debug.LogError("paht is null");
            return; 
        }

        foreach(Vector2Int pos in path)
        {
            Vector2 worldPos = grid.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));
            GameObject.Instantiate(prefab, worldPos, Quaternion.identity);
        }
    }

    public List<Vector3> SearchPathTo(Vector3 start,Vector3 end)
    {
        Vector3Int _start = grid.WorldToCell(start);
        Vector3Int _end = grid.WorldToCell(end);

        endPos =(Vector2Int) _end;
        startPos = (Vector2Int)_start;

        AStar aStar = new AStar((Vector2Int)_start, (Vector2Int)_end, nodeMap);

        List<Vector2Int> path = aStar.Search();

        if (path == null)
        {
            Debug.LogError("paht is null");
            return null;
        }

        

        List<Vector3> worldPath = new List<Vector3>();

        foreach (var node in path)
        {
            Vector2 worldPos = grid.GetCellCenterWorld(new Vector3Int(node.x, node.y, 0));
            worldPath.Add(worldPos);
            //GameObject.Instantiate(prefab, worldPos, Quaternion.identity);
        }

        return worldPath;
    }

    public void CreatSign(Vector2Int pos)
    {
        Vector2 worldPos = grid.GetCellCenterWorld(new Vector3Int(pos.x, pos.y, 0));
        GameObject.Instantiate(prefab, worldPos, Quaternion.identity);
    }

    private void Start()
    {
        UpdateNodeMap();
    }

    public void UpdateNodeMap()
    {
        nodeMap = new NodeMap(grid);
    }

}


public class AStar
{
    public AStar(Vector2Int _start,Vector2Int _end,NodeMap _map/*,PathFinder _finder*/)
    {
        start = _map.GetNodeByGridPos(_start.x, _start.y);
        end = _map.GetNodeByGridPos(_end.x, _end.y);
        now = start;
        map = _map;
        //finder = _finder;
    }

    //PathFinder finder;

    Node start;
    Node end;
    Node now;
    NodeMap map;

    List<Node> open_list = new List<Node>();
    List<Node> close_list = new List<Node>();


    public List<Vector2Int> Search()
    {
        // 最小费用点
        Node node_minCost = map.GetNodeByGridPos(start.x, start.y); 

        open_list.Add(start);

        int circleCount =0;

        // 开始搜索
        while (open_list.Count != 0 && circleCount < 100)
        {
            circleCount += 1;
            float cost = 999f;

            // 在openlist中决定当前节点
            for (int i =0;i<open_list.Count;i++)
            {
                Node node = open_list[i];
                float node_cost = node.GetCost(end.x, end.y);

                if(node_cost <= cost)
                {
                    now = node;
                    cost = node_cost;
                    if(cost < node_minCost.GetCost(end.x,end.y))
                    {
                        node_minCost = node;
                    }
                }
            }
            

            // 结束搜索
            if (now.x == end.x && now.y == end.y)
            {
                // 如果目标不可站立，则目标转为最近的可站立位置
                while(!now.CanWalk)
                {
                    now = now.parent;
                }

                List<Vector2Int> path_list = new List<Vector2Int>();

                int count = 0;

                while(now != start && count < 100)
                {
                    count += 1;
                    path_list.Add(new Vector2Int(now.x, now.y));
                    now = now.parent;
                }
                path_list.Add(new Vector2Int(now.x, now.y));

                // 如果path消耗大于移动点数 则取最近的点
                while((path_list.Count - 1) * BattleManager.instance.moveCost_varCell > BattleManager.instance.actor_curTurn.movePoint)
                {
                    path_list.RemoveAt(0);
                }

                path_list.Reverse();
                return path_list; 

            }
            // 搜索相邻位置
            else
            {
                open_list.Remove(now);
                close_list.Add(now);
                // 检定四个相邻节点
                Node left = map.GetNodeByGridPos(now.x - 1, now.y);
                Node right = map.GetNodeByGridPos(now.x + 1, now.y);
                Node up = map.GetNodeByGridPos(now.x, now.y + 1);
                Node down = map.GetNodeByGridPos(now.x, now.y - 1);

                List<Node> adjoin_list = new List<Node> { left,right,up,down};

                foreach(var node in adjoin_list)
                {
                    if(!open_list.Contains(node) && (!close_list.Contains(node)) &&  node.CanPass)
                    {
                        open_list.Add(node);
                        node.parent = now;
                    }
                }

                
            }

        }
        // 如果没有找到路径，则以最小费用点作为路径终点
        now = node_minCost;

        while (!now.CanWalk)
        {
            now = now.parent;
        }

        List<Vector2Int> minPath_list = new List<Vector2Int>();

        int num = 0;

        while (now != start && num < 100)
        {
            num += 1;
            minPath_list.Add(new Vector2Int(now.x, now.y));
            now = now.parent;
        }
        minPath_list.Add(new Vector2Int(now.x, now.y));

        while ((minPath_list.Count - 1) * BattleManager.instance.moveCost_varCell > BattleManager.instance.actor_curTurn.movePoint)
        {
            minPath_list.RemoveAt(0);
        }

        minPath_list.Reverse();

        if (minPath_list.Count <= 0)
            return null;
        return minPath_list;
    }
}

public class NodeMap
{
    public NodeMap(Grid _grid)
    {
        node_list = new List<Node>();
        grid = _grid;
    }

    private Grid grid;

    private List<Node> node_list;

    public Node GetNodeByGridPos(int x, int y)
    {
        foreach (var n in node_list)
        {
            if(n.x ==x && n.y == y)
            {
                return n;
            }
        }

        // 不存在,创建新node
        //Debug.Log("创建新node" + " " + x + "" + y);
        Node node = new Node(x, y);
        node.map = this;
        SetNodeType(node);
        node_list.Add(node);
        return node;   
    }

    private void SetNodeType(Node node)
    {
        Vector2 rayPos = grid.GetCellCenterWorld(new Vector3Int(node.x, node.y, 0));
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

        if (hit.collider == null)
        {
            node.type = Node.Type.none;
        }
        else
        {
            if (hit.collider.tag == "Obstacle")
            {
                node.type = Node.Type.obstacle;
            }

            if (hit.collider.tag == "Ladder")
            {
                node.type = Node.Type.ladder;
            }
        }
    }
}

public class Node
{
    public Node(int x,int y)
    {
        this.x = x;
        this.y = y;
    }

    public int x;
    public int y;

    public Node parent;
    public NodeMap map;

    public enum Type
    {
        none,
        obstacle,
        ladder
    }

    public Type type;

    

    public float GetCost (int _x,int _y)
    {
        return (_x - x) * (_x - x) + (_y - y) * (_y - y);
    }

    public bool CanWalk
    {
        set
        {

        }
        get
        {
            // 障碍物不可行走
            if (type == Type.obstacle)
                return false;

            // 非障碍物
            // 底下是障碍物可行走
            if (map.GetNodeByGridPos(x, y - 1).type == Type.obstacle)
            {
                return true;
            }

            //底下是梯子但本身不是梯子 可行走
            if (map.GetNodeByGridPos(x, y - 1).type == Type.ladder && type != Type.ladder)
            {
                return true;
            }

            if (type == Type.ladder)
            {
                return true;
            }    

            return false;
        }
    }
    public bool CanPass
    {
        set
        {

        }
        get
        {
            // 能走就能通过
            if(CanWalk)
                return true;

            // 能跳也能通过
            if(map.GetNodeByGridPos(x, y - 1).CanWalk &&(map.GetNodeByGridPos(x-1, y).CanWalk || map.GetNodeByGridPos(x+1, y).CanWalk))
            {
                return true;
            }

            //// 能攀爬就能通过
            //if(type == Type.ladder)
            //{
            //    return true;
            //}

            return false;
        }
    }


}