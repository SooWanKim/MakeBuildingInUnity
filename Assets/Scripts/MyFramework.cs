using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class MyFramework : MonoBehaviour
{
    public Material meshMeterial;
    public GameObject parentMesh;

    Dictionary<string, GameObject> _dongObjects = new Dictionary<string, GameObject> ();
    private readonly string JSON_FILE_PATH = "/Samples/json/dong.json";

    private void Start ()
    {
        Build (false);
    }
    public void Clear ()
    {
        foreach (var obj in _dongObjects)
        {
            DestroyImmediate (obj.Value);
        }

        if (parentMesh != null)
        {
            List<GameObject> childList = new List<GameObject> ();
            for (int i = 0; i < parentMesh.transform.childCount; i++)
            {
                var child = parentMesh.transform.GetChild (i);
                childList.Add (child.gameObject);
            }
            foreach (var child in childList)
            {
                DestroyImmediate (child);
            }
        }
        _dongObjects.Clear ();
    }

    async public void Build (bool angleCheck)
    {
        Clear ();

        Dictionary<string, List<List<Vector3>>> sampleMesh = ReadJson ();

        await MakeMeshAsync (sampleMesh, angleCheck);
    }

    private Dictionary<string, List<List<Vector3>>> ReadJson ()
    {
        string file_path = Application.dataPath + JSON_FILE_PATH;
        string file_text = File.ReadAllText (file_path);
        var api = JsonConvert.DeserializeObject<APIResponse> (file_text);

        Dictionary<string, List<List<Vector3>>> sampleMesh = new Dictionary<string, List<List<Vector3>>> ();

        foreach (var dongData in api.data)
        {
            foreach (var roomType in dongData.roomtypes)
            {
                List<Vector3> polyData = new List<Vector3> ();

                foreach (var base64 in roomType.coordinatesBase64s)
                {
                    var bytes = Convert.FromBase64String (base64);
                    var floatArray = new float[bytes.Length / 4];
                    Buffer.BlockCopy (bytes, 0, floatArray, 0, bytes.Length);

                    Vector3 vertex;

                    for (int i = 0; i < floatArray.Length; i += 3)
                    {
                        vertex = new Vector3 (floatArray[i], floatArray[i + 2], floatArray[i + 1]);
                        if (polyData.Contains (vertex) == true)
                            continue;

                        polyData.Add (vertex);
                    }
                }

                if (polyData.Count > 0)
                {
                    if (sampleMesh.ContainsKey (dongData.meta.동) == true)
                    {
                        sampleMesh[dongData.meta.동].Add (polyData);
                    }
                    else
                    {
                        sampleMesh.Add (dongData.meta.동, new List<List<Vector3>> () { polyData });
                    }
                }
            }
        }

        sampleMesh = sampleMesh.OrderBy (obj => obj.Key).ToDictionary (obj => obj.Key, obj => obj.Value);

        return sampleMesh;
    }

    async public Task MakeMeshAsync (Dictionary<string, List<List<Vector3>>> sampleMesh, bool angleCheck = false)
    {
        foreach (var mesh in sampleMesh)
        {
            foreach (var dongList in mesh.Value)
            {
                MakeMesh (mesh.Key, dongList, angleCheck);
                await Task.Yield ();
            }
        }
    }

    public void MakeMesh (string dongName, List<Vector3> polyData, bool angleCheck = false)
    {
        GameObject parentObj;
        if (_dongObjects.ContainsKey (dongName) == true)
        {
            parentObj = _dongObjects[dongName];
        }
        else
        {
            parentObj = new GameObject (dongName);
            parentObj.transform.parent = parentMesh.transform;
            _dongObjects.Add (dongName, parentObj);
        }

        GameObject newGameObject = new GameObject ("Mesh");
        newGameObject.transform.parent = parentObj.transform;

        var meshFilter = newGameObject.AddComponent<MeshFilter> ();
        var meshRenderer = newGameObject.AddComponent<MeshRenderer> ();
        meshRenderer.material = meshMeterial;

        meshFilter.mesh = CreateMesh (dongName, polyData, meshRenderer.sharedMaterial, angleCheck);
    }

    private Mesh CreateMesh (string dongName, List<Vector3> polyData, Material meshMaterial, bool angleCheck = false)
    {
        Mesh mesh = new Mesh ();

        List<Vector3> vertexData = new List<Vector3> ();

        float yMin = polyData.Min (v => v.y);
        float yMax = polyData.Max (v => v.y);
        var minYList = polyData
            .Where (n => n.y == yMin)
            .ToList ();

        var maxYList = polyData
            .Where (n => n.y == yMax)
            .ToList ();

        float xMax = minYList.Max (v => v.x);
        Vector3 maxXVector = minYList.First (v => v.x == xMax);

        float zMax = minYList.Max (v => v.z);
        Vector3 maxZVector = minYList.First (v => v.z == zMax);

        float xMin = minYList.Min (v => v.x);
        Vector3 minXVector = minYList.First (v => v.x == xMin);

        float zMin = minYList.Min (v => v.z);
        Vector3 minZVector = minYList.First (v => v.z == zMin);

        vertexData.Add (minZVector);
        vertexData.Add (minXVector);
        vertexData.Add (maxZVector);
        vertexData.Add (maxXVector);

        xMax = maxYList.Max (v => v.x);
        maxXVector = maxYList.First (v => v.x == xMax);

        zMax = maxYList.Max (v => v.z);
        maxZVector = maxYList.First (v => v.z == zMax);

        xMin = maxYList.Min (v => v.x);
        minXVector = maxYList.First (v => v.x == xMin);

        zMin = maxYList.Min (v => v.z);
        minZVector = maxYList.First (v => v.z == zMin);

        vertexData.Add (minZVector);
        vertexData.Add (minXVector);
        vertexData.Add (maxZVector);
        vertexData.Add (maxXVector);

        Vector3[] totalVertexData = new Vector3[]
        {
            //bottom
            vertexData[0], vertexData[1], vertexData[2], vertexData[3],
            //top
            vertexData[7], vertexData[6], vertexData[5], vertexData[4],
            //front
            vertexData[4], vertexData[5], vertexData[1], vertexData[0],
            //left
            vertexData[7], vertexData[4], vertexData[0], vertexData[3],
            //right
            vertexData[5], vertexData[6], vertexData[2], vertexData[1],
            //back
            vertexData[6], vertexData[7], vertexData[3], vertexData[2],
        };

        // triangle
        int[] triangles = new int[]
        {
            // Bottom
            3,
            1,
            0,
            3,
            2,
            1,

            // Left
            3 + 4 * 1,
            1 + 4 * 1,
            0 + 4 * 1,
            3 + 4 * 1,
            2 + 4 * 1,
            1 + 4 * 1,

            // Front
            3 + 4 * 2,
            1 + 4 * 2,
            0 + 4 * 2,
            3 + 4 * 2,
            2 + 4 * 2,
            1 + 4 * 2,

            // Back
            3 + 4 * 3,
            1 + 4 * 3,
            0 + 4 * 3,
            3 + 4 * 3,
            2 + 4 * 3,
            1 + 4 * 3,

            // Right
            3 + 4 * 4,
            1 + 4 * 4,
            0 + 4 * 4,
            3 + 4 * 4,
            2 + 4 * 4,
            1 + 4 * 4,

            // Top
            3 + 4 * 5,
            1 + 4 * 5,
            0 + 4 * 5,
            3 + 4 * 5,
            2 + 4 * 5,
            1 + 4 * 5,

        };

        // uv
        Vector2 _00 = new Vector2 (0f, 0f);
        Vector2 _10 = new Vector2 (0.5f, 0f);
        Vector2 _01 = new Vector2 (0f, 1f);
        Vector2 _11 = new Vector2 (0.5f, 1f);

        Vector2 _2_00 = new Vector2 (0.5f, 0f);
        Vector2 _2_10 = new Vector2 (0.75f, 0f);
        Vector2 _2_01 = new Vector2 (0.5f, 1f);
        Vector2 _2_11 = new Vector2 (0.75f, 1f);

        Vector2 _3_00 = new Vector2 (0.75f, 1f);
        Vector2 _3_10 = new Vector2 (1f, 0);
        Vector2 _3_01 = new Vector2 (0.75f, 1f);
        Vector2 _3_11 = new Vector2 (1f, 1f);

        Vector2[] uvs = new Vector2[]
        {
            // Bottom
            _11,
            _01,
            _00,
            _10,

            // Top
            _11,
            _01,
            _00,
            _10,

            // Front
            _11,
            _01,
            _00,
            _10,

            // left
            _11,
            _01,
            _00,
            _10,

            // Right
            _11,
            _01,
            _00,
            _10,

            // Back
            _11,
            _01,
            _00,
            _10,
        };

        if (angleCheck == false)
        {
            float frontDistance = Vector3.Distance (vertexData[4], vertexData[5]);
            float leftDistance = Vector3.Distance (vertexData[7], vertexData[4]);

            if (frontDistance < leftDistance)
            {
                uvs[8] = _2_11;
                uvs[9] = _2_01;
                uvs[10] = _2_00;
                uvs[11] = _2_10;
                uvs[20] = _2_11;
                uvs[21] = _2_01;
                uvs[22] = _2_00;
                uvs[23] = _2_10;
            }
            else
            {
                uvs[12] = _2_11;
                uvs[13] = _2_01;
                uvs[14] = _2_00;
                uvs[15] = _2_10;
                uvs[16] = _2_11;
                uvs[17] = _2_01;
                uvs[18] = _2_00;
                uvs[19] = _2_10;
            }
            uvs[0] = _3_11;
            uvs[1] = _3_01;
            uvs[2] = _3_00;
            uvs[3] = _3_10;
            uvs[4] = _3_11;
            uvs[5] = _3_01;
            uvs[6] = _3_00;
            uvs[7] = _3_10;
        }

        mesh.vertices = totalVertexData.ToArray ();
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.Optimize ();
        mesh.RecalculateNormals ();
        mesh.RecalculateBounds ();

        if (angleCheck == true)
        {
            Vector2[] tempuvs = mesh.uv;
            for (int i = 0; i < mesh.normals.Length; i++)
            {
                float degree = AngleDegree (mesh.normals[i], Vector3.forward);
                Debug.Log ($"{degree}_{mesh.normals[i]}");
                if (180 <= degree && degree <= 220)
                {
                    if (tempuvs[i].x > 0.0f)
                        tempuvs[i].x = 0.5f;
                }
                else
                {
                    if (mesh.normals[i] == Vector3.up || mesh.normals[i] == Vector3.down)
                    {
                        if (tempuvs[i].x == 0.0f)
                            tempuvs[i].x = 0.75f;
                        else
                            tempuvs[i].x = 1f;
                    }
                    else
                    {
                        if (tempuvs[i].x == 0.0f)
                            tempuvs[i].x = 0.5f;
                        else
                            tempuvs[i].x = 0.75f;
                    }
                }
            }
            mesh.uv = tempuvs;
        }

        int height = (int) (yMax / 3.0f);
        meshMaterial.SetTextureScale ("_BaseMap", new Vector2 (1f, height));
        return mesh;
    }

    GUIStyle _contentStyle; //= new GUIStyle (GUI.skin.button);
    private void OnGUI ()
    {
        _contentStyle = new GUIStyle (GUI.skin.button);
        _contentStyle.alignment = TextAnchor.MiddleLeft;
        if (GUILayout.Button ("1. 생성(면의 넓이, 위치로 uv맵핑)", _contentStyle, GUILayout.Width (300), GUILayout.Height (30)) == true)
        {
            Build (false);
        }
        if (GUILayout.Button ("2. 생성(각도계산으로 uv맵핑. 기능구현 실패)", _contentStyle, GUILayout.Width (300), GUILayout.Height (30)) == true)
        {
            Build (true);
        }
    }

    public float AngleDegree (Vector3 from, Vector3 to)
    {
        Quaternion qua = Quaternion.FromToRotation (from, to);
        return qua.eulerAngles.y;
    }

    public float AngleDegree2 (Vector3 from, Vector3 to)
    {
        return Mathf.Acos (Vector2.Dot (from, to)) * Mathf.Rad2Deg;
    }
}