using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Node
{
    public int x; // inicalizamos una variable con el nombre de x el cual funcionara para movernos en ese vector (ejemplo)
    public int y; // inicalizamos una variable con el nombre en y con el cual nos moveremos 

    //creamos un parent del nodo que ira haciendo a los hijos
    public Node Parent;

     
    public float g_Cost;
    public float fTerrainCost;
    public bool bWalkable; //

    public Node(int in_x, int in_y)
    {
        this.x = in_x; // mandamos  a x el valor de in_x
        this.y = in_y; // mandamos  a x el valor de in_y
        this.Parent = null; //El padre es nulo
        this.g_Cost = int.MaxValue; //el costo del terreno empieza con el valor maximo
        this.fTerrainCost = 1; // el costo del terro es 1 
        this.bWalkable = true; // caminar se convierte en tru
    }

    public override string ToString()
    {
        return x.ToString() + " , " + y.ToString();
    }
}
public class Graph
{
    public List<Node> Nodes;
}
//En general esta clase se encarga de dibujar el tablero en donde nuestro monito se ira moviendo por todo el espacio
public class ClassGrid
{
    public int iHeight; // inicializamos la altura del tablero
    public int iWidth; // inizalizamos el ancho del tablero

    
    private float fTileSize; //Dibujar el grid
    private Vector3 v3OriginPosition; //Busca la posicion inicial

    public Node[,] Nodes; //creamos un array donde empezaremos a guardar los nodos
    public TextMesh[,] debugTextArray; // creamos un array donde guardaremos el texto que imprimera a donde se ira cada uno de nuestros nodos

    public bool bShowDebug = true; // comenzara a imprimir
    public GameObject debugGO = null; //crea un objeto con cada uno de los textos

    //hace una clase publica donde le inicializar sus valores
    public ClassGrid(int in_height, int in_width, float in_fTileSize = 10.0f, Vector3 in_v3OriginPosition = default)
    {
        iHeight = in_height;
        iWidth = in_width;

        InitGrid();
        this.fTileSize = in_fTileSize;
        this.v3OriginPosition = in_v3OriginPosition;

        if (bShowDebug)
        {
            debugGO = new GameObject("GridDebugParent"); //crea el padre del tablero
            debugTextArray = new TextMesh[iHeight, iWidth]; // crea el tablero
            for (int y = 0; y < iHeight; y++)
            {
                for (int x = 0; x < iWidth; x++)
                {
                    debugTextArray[y, x] = CreateWorldText(Nodes[y, x].ToString(),
                    debugGO.transform, GetWorldPosition(x, y) + new Vector3(fTileSize * 0.5f, fTileSize * 0.5f),
                    30, Color.white, TextAnchor.MiddleCenter);
                    //debugTextArray[y, x] = new TextMesh(x, y);

                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);

                }
            }
            Debug.DrawLine(GetWorldPosition(0, iHeight), GetWorldPosition(iWidth, iHeight), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(iWidth, 0), GetWorldPosition(iWidth, iHeight), Color.white, 100f);
        }
    }
    public void InitGrid()
    {
        Nodes = new Node[iHeight, iWidth];

        for (int y = 0; y < iHeight; y++)
        {
            for (int x = 0; x < iWidth; x++)
            {
                Nodes[y, x] = new Node(x, y);
            }
        }
    }

    //Encuentra un camino entre el inicio y el final
    public List<Node> DepthFirstSearch(int in_startX, int in_startY, int in_endX, int in_endY)
    {

        Node StartNode = GetNode(in_startY, in_startX);
        Node EndNode = GetNode(in_endY, in_endX);

        if (StartNode == null || EndNode == null)
        {
            //mensaje de error
            Debug.LogError("Invalid coordinates in DepthFirstSearch");
            return null;
        }

        Stack<Node> OpenList = new Stack<Node>();
        List<Node> CloseList = new List<Node>();

        OpenList.Push(StartNode);

        while (OpenList.Count > 0)
        {
          
            Node currentNode = OpenList.Pop();
            Debug.Log("Curent Node is: " + currentNode.x + "," + currentNode.y);

            //verifica si ya llego al destino
            if (currentNode == EndNode)
            {
                //encontramos un camino
                Debug.Log("Camino encontrado");
                // Necesitamos construir
                List<Node> path = Backtrack(currentNode);
                EnumeratePath(path);
                return path;
            }


            
            if (CloseList.Contains(currentNode))
            {
                continue;
            }

            CloseList.Add(currentNode);


            //Comienza a visitar a sus vecinos para encontrar la ruta mas comoda
            List<Node> currentNeighbors = GetNeighbors(currentNode);


            //Meterlos a la pila en el orden inversoo para que al sacarlos nos den el orden
            for (int x = currentNeighbors.Count - 1; x >= 0; x--)
            {
                //Solo queremos nodos que no esten en la lista cerrada
                if (currentNeighbors[x].bWalkable && !CloseList.Contains(currentNeighbors[x]))
                {
                    
                    currentNeighbors[x].Parent = currentNode;
                    OpenList.Push(currentNeighbors[x]);
                }
            }


        }
        Debug.LogError("No path found between start and end.");

        return null;
    }

    public Node GetNode(int x, int y)
    {
        //Checamos si las coordenadas dadas son validas dentro de nuestra cuadricula
        if (x < iWidth && x >= 0 && y < iHeight && y >= 0)
        {
            return Nodes[x, y];
        }
        return null;
    }

    public List<Node> GetNeighbors(Node in_currentNode)
    {
        List<Node> out_Neighbors = new List<Node>();
        //visitamos al nodo de arriba
        int x = in_currentNode.x;
        int y = in_currentNode.y;
        if (GetNode(y + 1, x) != null)
        {
            out_Neighbors.Add(Nodes[y + 1, x]);
        }
        if (GetNode(y, x - 1) != null)
        {
            out_Neighbors.Add(Nodes[y, x - 1]);
        }
        // Checamos a la derecha
        if (GetNode(y, x + 1) != null)
        {
            out_Neighbors.Add(Nodes[y, x + 1]);
        }
        if (GetNode(y - 1, x) != null)
        {
            out_Neighbors.Add(Nodes[y - 1, x]);
        }


        return out_Neighbors;
    }

    public List<Node> Backtrack(Node in_node)
    {
        List<Node> out_Path = new List<Node>();
        Node current = in_node;
        while (current.Parent != null)
        {
            out_Path.Add(current);
            current = current.Parent;
        }
        out_Path.Add(current);
        out_Path.Reverse();

        return out_Path;
    }

    public void EnumeratePath(List<Node> in_path)
    {
        int iCounter = 0;

        foreach (Node n in in_path)
        {
            iCounter++;
            debugTextArray[n.y, n.x].text = n.ToString() +
                 Environment.NewLine + "Step: " + iCounter.ToString();
        }
    }
    public static TextMesh CreateWorldText(string in_text, Transform in_parent = null,
        Vector3 in_localPosition = default, int in_iFontSize = 32, Color in_color = default,
        TextAnchor in_textAnchor = TextAnchor.UpperLeft, TextAlignment in_textAlignment = TextAlignment.Left)
    {
        //if (in_color == null)
        //{
        //    in_color = Color.white;
        //}
        GameObject MyObject = new GameObject(in_text, typeof(TextMesh));
        MyObject.transform.parent = in_parent;
        MyObject.transform.localPosition = in_localPosition;

        TextMesh myTM = MyObject.GetComponent<TextMesh>();
        myTM.text = in_text;
        myTM.anchor = in_textAnchor;
        myTM.alignment = in_textAlignment;
        myTM.fontSize = in_iFontSize;
        myTM.color = in_color;


        return myTM;
    }


    public Vector3 GetWorldPosition(int x, int y)
    {
        //Nos regresa la posicion en mundo del tile/cuadro especificado por x y y
        //POr eso lo multiplicamos por el ftilesize
        //dado que tienen lo mismo de alto y ancho por cuadro
        return new Vector3(x, y) * fTileSize + v3OriginPosition;
    }
   
    public int GetDistance(Node in_a, Node in_b)
    {
        int x_diff = (in_a.x - in_b.x);
        int y_diff = (in_b.y - in_a.y);
        return (int)Mathf.Sqrt(Mathf.Pow(x_diff, 2) + Mathf.Pow(y_diff, 2)); //calcula la distancia con la formula general
    }

    //Creamos una lista de nodos con el BreadthFirstSearch, mandandole la posicion inicial y a donde tiene que llegar a su meta
    public List<Node> BreadthFirstSearch(int in_startX, int in_startY, int in_endX, int in_endY)
    {

        Node StartNode = GetNode(in_startY, in_startX);
        Node EndNode = GetNode(in_endY, in_endX);

        if (StartNode == null || EndNode == null)
        {
            Debug.LogError("Invalid coordinates in DeepthFirstSearch");
            return null;
        }

        Queue<Node> OpenList = new Queue<Node>(); //El Queue es una cola, como la que realizamos en una tienda y comienza a hacer las cosas por prioridad
        List<Node> ClosedList = new List<Node>(); //Representa una lista fuertemente tipada de objetos a los que se puede acceder por índice

        OpenList.Enqueue(StartNode); //Añade un nodo nuevo al final de la fila

        
        int iP = 0;

        while (OpenList.Count > 0)
        {
            //Mientras haya nodos en la lista abierta, vamos a buscar un camino
            //Obtenemos el primer nodo de la lista abierta
            Node currentNode = OpenList.Dequeue();
            Debug.Log("Current Node is: " + currentNode.x + ", " + currentNode.y);

            if (currentNode == EndNode) //Verifica si ya ha llegado a su destino 
            {
                Debug.Log("Camino encontrado");

                List<Node> path = Backtrack(currentNode); //Para constuir ese camino necesitamos un backtraccking
                EnumeratePath(path);

                return path;
            }

            
            if (ClosedList.Contains(currentNode))
            {
                continue;
            }

            ClosedList.Add(currentNode);

            
            List<Node> currentNeighbors = GetNeighbors(currentNode); //Vamos a visitar los vecinos para buscar el camino mas pequeño

            foreach (Node neighbor in currentNeighbors)
            {
                if (ClosedList.Contains(neighbor))
                    continue;

                
                neighbor.Parent = currentNode;//Si no lo contiene, entonces lo agregamos a la lista Abierta
                OpenList.Enqueue(neighbor); //Lo mandamos a llamar para cada vecino
                iP++; //Ajustamos la prioridad, para que cada nuevo que entre sea añada al último
            }

            string RemainingNodes = "Nodes in open list are: "; //nos imprime los nodos
            foreach (Node n in OpenList)
                RemainingNodes += "(" + n.x + ", " + n.y + ") - ";
            Debug.Log(RemainingNodes);

        }

        Debug.LogError("No path found between start and end.");

        return null;
    }



}

//Investigaciones https://learn.microsoft.com/en-us/dotnet/api/system.collections.queue?view=net-7.0 utilice el repositorio que nos mando en class
//al inicio me estreso un poco porque al no entender bien la forma en la que funciona la Queue y unicamente cambiando cosas se me explotaba la 
//computadora hasta que con un tutorial entendi como hacer las filas
//https://www.youtube.com/watch?v=kzBcsBMChHM&ab_channel=hdeleon.net tutorial 


//De igual manera para entender mas la forma de busqueda estuve viendo algunos videos.
//https://www.youtube.com/watch?v=HZ5YTanv5QE&t=2s&ab_channel=MichaelSambol