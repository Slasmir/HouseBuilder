using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class HouseEditor : EditorWindow {



    // Add menu named "My Window" to the Window menu
    [MenuItem("HouseCreator/Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        HouseEditor window = (HouseEditor)EditorWindow.GetWindow(typeof(HouseEditor));
        window.Show();
    }


    HouseCreatorCollection m_houseCreatorCollection;
    string ErrorMessage = "";
    void OnGUI()
    {
        m_houseCreatorCollection = (HouseCreatorCollection)EditorGUILayout.ObjectField(m_houseCreatorCollection, typeof(HouseCreatorCollection), true);

        if(GUILayout.Button("Create House From Selection")){
            if(m_houseCreatorCollection == null)
            {
                ErrorMessage = "Select a House Creator Collection";
                return;
            } else
            {
                ErrorMessage = "";
            }

            GameObject[] CurrentSelection = Selection.gameObjects;
            if (CurrentSelection.LongLength != 0)
            {
                List<MeshFilter> allSelectedRenders = new List<MeshFilter>();
                foreach(GameObject go in CurrentSelection)
                {
                    if (go.GetComponent<Renderer>() != null)
                        allSelectedRenders.Add(go.GetComponent<MeshFilter>());
                }

                if (allSelectedRenders.Count > 0)
                {
                    GameObject CombinedMeshes = CombineMeshes(allSelectedRenders);
                    HouseCreatorBase HCB = CombinedMeshes.AddComponent<HouseCreatorBase>();
                    HCB.GenerateHouse(m_houseCreatorCollection);
                }
                else
                    Debug.Log("Select Something with a rendere");
            }
            else
            {
                ErrorMessage = "You need to select something";
            }
        }

        GUILayout.Label("");
        if (ErrorMessage != "")
        {
            GUILayout.Label("Error:");
            GUILayout.Label(ErrorMessage);
        }
    }

    GameObject CombineMeshes(List<MeshFilter> Meshses)
    {
        Vector3 Corner = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
        CombineInstance[] combine = new CombineInstance[Meshses.Count];
        for (int i = 0; i < Meshses.Count; i++)
        {
            Vector3[] verts = Meshses[i].sharedMesh.vertices;
            for (int v = 0; v < verts.Length; v++)
            {
                Vector3 CheckPos = Meshses[i].transform.position + verts[v];
                if (CheckPos.x < Corner.x)
                    Corner.x = CheckPos.x;
                if (CheckPos.y < Corner.y)
                    Corner.y = CheckPos.y;
                if (CheckPos.z < Corner.z)
                    Corner.z = CheckPos.z;
            }
            combine[i].mesh = Meshses[i].sharedMesh;
            combine[i].transform = Meshses[i].transform.localToWorldMatrix;
        }

        GameObject CombinedGO = new GameObject();
        CombinedGO.transform.position = Corner;
        MeshRenderer mr = CombinedGO.AddComponent<MeshRenderer>();
        mr.enabled = false;
        MeshFilter mf = CombinedGO.AddComponent<MeshFilter>();

        mf.sharedMesh = new Mesh();
        mf.sharedMesh.CombineMeshes(combine);
        MeshHelper.Subdivide(mf.sharedMesh, 3);

        return CombinedGO;
    }
}
