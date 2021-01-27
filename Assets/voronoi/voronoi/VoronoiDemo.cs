using UnityEngine;
using System.Collections.Generic;
using Delaunay;
using Delaunay.Geo;
using System.Linq;
using UnityEngine.AI;
using UnityEngine;
using System.Linq;
using System.Collections;

public class VoronoiDemo : MonoBehaviour
{

    public Material land;
    public const int NPOINTS = 80;
    public const int WIDTH = 2000;
    public const int HEIGHT = 2000;
    public const int HOUSES = 40; //60
    public const int FLATS = 40;
    public const int WORKPLACES = 5;
    public const int TREES = 10;
    public float freqx = 0.02f, freqy = 0.018f, offsetx = 0.43f, offsety = 0.22f;
    public GameObject road;
    private List<Vector2> m_points;
    private List<LineSegment> m_edges = null;
    private List<LineSegment> m_spanningTree;
    private List<LineSegment> m_delaunayTriangulation;
    private Texture2D tx;
    public RobotFreeAnim agent;
    public NavMeshSurface surface;
    public Transform parent;
    private GameObject flat;
    public GameObject BaseWork;
    public GameObject BaseHouse;
    public GameObject BaseFlat;
    public GameObject Flat2;
    public GameObject Flat3;
    public GameObject Tree;
    public Light Lt;
    public int cpt = 0;
    public bool proceedGen = true;
    private float[,] createMap()    
    {
        float[,] map = new float[WIDTH, HEIGHT];
        for (int i = 0; i < WIDTH; i++)
            for (int j = 0; j < HEIGHT; j++)
                map[i, j] = Mathf.PerlinNoise(freqx * i + offsetx, freqy * j + offsety);
        return map;
    }

    void Build()
    {
        surface.BuildNavMesh();
    }
    void Generate()
    {
        RobotFreeAnim a = (RobotFreeAnim)Instantiate(agent, new Vector3(2, 0, 2), Quaternion.identity);
        float size = Random.Range(0.4f, 1.6f);
        a.transform.localScale = new Vector3(size,size,size);
   
    }
    void Start()
    {
        //System.Random generator = new System.Random();
        flat = GameObject.Find("BaseFlat");

        float[,] map = createMap();
        float threshold = 0.72f;


        Color[] pixels = createPixelMap(map);


        m_points = new List<Vector2>();
        List<uint> colors = new List<uint>();
        for (int i = 0; i < NPOINTS; i++)
        {
            int x = (int)Random.Range(0, WIDTH - 1);
            int y = (int)Random.Range(0, HEIGHT - 1);
            int iter = 0;
            while (map[x, y] < threshold && iter < 10)
            {
                x = (int)Random.Range(0, WIDTH - 1);
                y = (int)Random.Range(0, HEIGHT - 1);
                iter++;
            }
            colors.Add((uint)0);
            Vector2 vec = new Vector2(x, y);
            m_points.Add(vec);
        }

        /* Generate Graphs */
        Delaunay.Voronoi v = new Delaunay.Voronoi(m_points, colors, new Rect(0, 0, WIDTH, HEIGHT));
        m_edges = v.VoronoiDiagram();
        m_spanningTree = v.SpanningTree(KruskalType.MINIMUM);
        m_delaunayTriangulation = v.DelaunayTriangulation();

        /* Shows Voronoi diagram */
        Color color = Color.blue;
        for (int i = 0; i < m_edges.Count; i++)
        {
            LineSegment seg = m_edges[i];
            Vector2 left = (Vector2)seg.p0;
            Vector2 right = (Vector2)seg.p1;
            Vector2 segment = (right - left) / WIDTH * 10;
            float a = Vector2.SignedAngle(Vector2.right, right - left);
           
            GameObject go = Instantiate(road, new Vector3(left.y / HEIGHT * 10 - 5, 0, left.x / WIDTH * 10 - 5), Quaternion.Euler(0, a + 90, 0));
           
          //  GameObject go2 = Instantiate(House, new Vector3(left.y / HEIGHT * 10 - 5, 0.303f, left.x / WIDTH * 10 - 5), Quaternion.Euler(0, a + 90, 0));


            
            
            go.transform.localScale = new Vector3(segment.magnitude, 0.001f, 0.1f);
            DrawLine(pixels, left, right, color);
            go.transform.SetParent(parent, false);
            
        }



        /* Shows Delaunay triangulation */

        color = Color.blue;
        if (m_delaunayTriangulation != null)
        {
           
            for (int i = 0; i < m_delaunayTriangulation.Count; i++)
            {
                LineSegment seg = m_delaunayTriangulation[i];
                Vector2 left = (Vector2)seg.p0;
                Vector2 right = (Vector2)seg.p1;
                
                DrawPoint(pixels,  right, color);
                float rightcoordsx = right.x / WIDTH * 10 - 5;
                float rightcoordsy = right.y / HEIGHT * 10 - 5;
                //   Debug.Log(rightcoordsx + " " + rightcoordsy);
                var spawnPoint = new Vector3(rightcoordsy, 0.303f, rightcoordsx);
                var hitColliders = Physics.OverlapSphere(spawnPoint, 0.1f);
                if (hitColliders.Length==0)
                {
                   // GameObject go2 = Instantiate(House, new Vector3(rightcoordsy, 0.1515f, rightcoordsx), Quaternion.Euler(0, Random.Range(0,180), 0));
                   // go2.transform.position = go2.transform.position * 2;
                }

            }
        }

      

        //while (listFlats.Count >=1)
        //{
        //    allFlats = UnityEngine.Object.FindObjectsOfType<house>();
        //    listFlats = allFlats.ToList();
        //    float a = Random.Range(-9.5f, 9.2f);
        //    float b = Random.Range(-9.5f, 9.2f);
        //    var spawnPoint = new Vector3(a, 0.303f, b);
        //    var hitColliders = Physics.OverlapSphere(spawnPoint, 0.1f);
        //    if (hitColliders.Length == 0)
        //    {
        //        GameObject go2 = Instantiate(House, new Vector3(a, 0.303f, b), Quaternion.Euler(0, Random.Range(0, 180), 0));
        //        // go2.transform.position = go2.transform.position * 2;
        //    }
        //}


        /* Shows spanning tree */
        /*
        color = Color.black;
        if (m_spanningTree != null) {
            for (int i = 0; i< m_spanningTree.Count; i++) {
                LineSegment seg = m_spanningTree [i];				
                Vector2 left = (Vector2)seg.p0;
                Vector2 right = (Vector2)seg.p1;
                DrawLine (pixels,left, right,color);
            }
        }

        /* Apply pixels to texture */
        tx = new Texture2D(WIDTH, HEIGHT);
        land.SetTexture("_MainTex", tx);
        tx.SetPixels(pixels);
        tx.Apply();

        //for (int i = 0; i < 80;)
        //{
        //    float iia = Random.Range(-9.5f, 9.2f);
        //    float iib = Random.Range(-9.5f, 9.2f);

        //    var spawnPoint2 = new Vector3(iia, 0.5f, iib);
        //    var hitColliders2 = Physics.OverlapSphere(spawnPoint2, 0.4f);

        //    if (hitColliders2.Length == 0)
        //    {
        //        GameObject go2 = Instantiate(BaseWork, new Vector3(iia, 0.303f, iib), Quaternion.Euler(0, Random.Range(0, 360), 0));
        //        i++;
        //    }
        //    else
        //    {
        //        Debug.Log("DONT DO");

        //    }

        //}
      

        Build();



     

    }


    void GenerateWorkplaces()
    {
        
      
        float iia = Random.Range(-4f, 4f);
        float iib = Random.Range(-4f, 4f);

        var spawnPoint2 = new Vector3(iia, 0.5f, iib);
        var hitColliders2 = Physics.OverlapSphere(spawnPoint2, 0.4f);

        if (hitColliders2.Length == 0)
        {
            GameObject go2 = Instantiate(BaseWork, new Vector3(iia, 0.39f, iib), Quaternion.Euler(0, Random.Range(0, 360), 0));

        }
    }
    void GenerateHouses()
    {

        float noise = Random.Range(0, 1);
        float side = Random.Range(0f, 3f);
        float sideb = Random.Range(0f, 3f);
        float iia = (side>1)?Random.Range(-9.5f, -5.5f+noise): Random.Range(5.5f - noise, 9.2f);
        float iib = (side > 1) ? Random.Range(-9.5f, 9.5f) : Random.Range(-9.5f,9.5f);

        var spawnPoint2 = new Vector3(iia, 5, iib);
        var hitColliders2 = Physics.OverlapSphere(spawnPoint2, 0.4f);

        if (hitColliders2.Length == 0)
        {
            GameObject go2 = Instantiate(BaseHouse, (sideb>1)?new Vector3(iia, 0.16f, iib): new Vector3(iib, 0.16f, iia), Quaternion.Euler(0, Random.Range(0, 360), 0));

        }
    }

    void GenerateTrees()
    {

        float noise = Random.Range(0, 1);
        float side = Random.Range(0f, 3f);
        float sideb = Random.Range(0f, 3f);
        float iia = (side > 1) ? Random.Range(-9.5f, -5.5f + noise) : Random.Range(5.5f - noise, 9.2f);
        float iib = (side > 1) ? Random.Range(-9.5f, 9.5f) : Random.Range(-9.5f, 9.5f);

        var spawnPoint2 = new Vector3(iia, 2, iib);
        var hitColliders2 = Physics.OverlapSphere(spawnPoint2, 0.2f);

        if (hitColliders2.Length == 0)
        {
            GameObject go2 = Instantiate(Tree, (sideb > 1) ? new Vector3(iia, 0, iib) : new Vector3(iib, 0, iia), Quaternion.Euler(0, Random.Range(0, 360), 0));

        }
    }
    void GenerateFlats()
    {

        float noise = Random.Range(0, 1);
        float iia = Random.Range(-5.5f-noise, 5.5f+noise);
        float iib = Random.Range(-5.5f-noise, 5.5f+noise);
        
        float heightrandomizer = Random.Range(0f, 4f);
        float angle = Random.Range(0, 360);
        var spawnPoint2 = new Vector3(iia, 0.5f, iib);
        var hitColliders2 = Physics.OverlapSphere(spawnPoint2, 0.4f);

        if (hitColliders2.Length == 0)
        {
       
            if (heightrandomizer>2.4f)
            {
                GameObject go2 = Instantiate(Flat3, new Vector3(iia, 0.303f, iib), Quaternion.Euler(0, angle, 0));
            }
            if (heightrandomizer>1f && heightrandomizer<=2f)
            {
                GameObject go2 = Instantiate(Flat2, new Vector3(iia, 0.303f, iib), Quaternion.Euler(0, angle, 0));

            }
            if (heightrandomizer<=1f)
            {
                GameObject go2 = Instantiate(BaseFlat, new Vector3(iia, 0.303f, iib), Quaternion.Euler(0, angle, 0));

            }
         

        }
    }


    /* Functions to create and draw on a pixel array */
    private Color[] createPixelMap(float[,] map)
    {
        Color[] pixels = new Color[WIDTH * HEIGHT];
        for (int i = 0; i < WIDTH; i++)
            for (int j = 0; j < HEIGHT; j++)
            {
                pixels[i * HEIGHT + j] = Color.Lerp(Color.white, Color.black, map[i, j]);
            }
        return pixels;
    }
    private void DrawPoint(Color[] pixels, Vector2 p, Color c)
    {
        if (p.x < WIDTH && p.x >= 0 && p.y < HEIGHT && p.y >= 0)
            pixels[(int)p.x * HEIGHT + (int)p.y] = c;
    }
    // Bresenham line algorithm
    private void DrawLine(Color[] pixels, Vector2 p0, Vector2 p1, Color c)
    {
        int x0 = (int)p0.x;
        int y0 = (int)p0.y;
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        while (true)
        {
            if (x0 >= 0 && x0 < WIDTH && y0 >= 0 && y0 < HEIGHT)
                pixels[x0 * HEIGHT + y0] = c;

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
    void Update()
    {
        var allWork = UnityEngine.Object.FindObjectsOfType<Work>();
        var allHouses = UnityEngine.Object.FindObjectsOfType<essai>();
        var allBlocks = UnityEngine.Object.FindObjectsOfType<DowntownB>();
        var allFlats = UnityEngine.Object.FindObjectsOfType<Flat>();
        var allTrees = UnityEngine.Object.FindObjectsOfType<Tree>();


        if ((Input.GetKeyDown("space")))
        {
            proceedGen=false;
          
            
        }
    
        RobotFreeAnim[] allRobots = UnityEngine.Object.FindObjectsOfType<RobotFreeAnim>();
   
        if ((allWork.Count() < WORKPLACES || allBlocks.Count() < FLATS || allHouses.Count() < HOUSES || allTrees.Count()<TREES) && proceedGen )
        {
            if (allWork.Count() < WORKPLACES)
            {
                GenerateWorkplaces();
            }
            if (allBlocks.Count() < FLATS)
            {
                GenerateFlats();
            }
            if (allHouses.Count() < HOUSES)
            {
                GenerateHouses();
            }
            if (allTrees.Count() < TREES)
            {
                GenerateTrees();
            }
        }
        
        {
            
            foreach (var flat in allFlats)
            {
                flat.Generate();
            }
         
            foreach (var robot in allRobots)
            {
                if (robot.workplace==null)
                {
             
                    robot.workplace = allWork[Random.Range(0, allWork.Count() - 1)];
                   // robot.GoToWork();

                }
             
              
            }
        }
        if ((Input.GetKeyDown(KeyCode.N)))
        {
            if (Lt.intensity == 0.1f)
            {
                Lt.intensity = 0.9f;
                foreach (var flat in allFlats)
                {
                    flat.Generate();
                }
                foreach (var robot in allRobots)
                {
                   robot.GoToWork();

                }

            }
            else
            {
                Lt.intensity = 0.1f;

                foreach (var workplace in allWork)
                {
                    workplace.Generate();

                }
            
            }
           
        }
        if (Lt.intensity == 0.1f)
        {
            foreach (var workplace in allWork)
            {
                if (workplace.robotsInside>0)
                {
                    workplace.Generate();
                }
            }
        
        }

    }
}