using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class SlopedStairUnderside : MonoBehaviour
{
    [Tooltip("Vertical thickness of the sloped stair side/stringer")]
    public float sideThickness = 0.6f;

    [ContextMenu("Slope Stair Underside")]
    public void MakeUndersideSloped()
    {
        ProBuilderMesh pb = GetComponent<ProBuilderMesh>();
        if (pb == null) return;

        IList<Vector3> oldVerts = pb.positions;
        IList<Face> oldFaces = pb.faces;
        int count = oldVerts.Count;

        // 1. Find the flat base Y coordinate of the stair mesh
        float minY = float.MaxValue;
        for (int i = 0; i < count; i++)
        {
            if (oldVerts[i].y < minY) minY = oldVerts[i].y;
        }

        // Helper: calculates the sloped underside Y height at any (X, Z) coordinate
        float GetSlopedY(float x, float z)
        {
            float lowestStepAboveY = float.MaxValue;
            for (int j = 0; j < count; j++)
            {
                Vector3 upper = oldVerts[j];
                if (upper.y > minY + 0.005f &&
                    Mathf.Abs(upper.x - x) < 0.02f &&
                    Mathf.Abs(upper.z - z) < 0.02f)
                {
                    if (upper.y < lowestStepAboveY)
                        lowestStepAboveY = upper.y;
                }
            }

            if (lowestStepAboveY < float.MaxValue)
                return Mathf.Max(minY, lowestStepAboveY - sideThickness);

            return minY;
        }

        // 2. Lift all bottom base vertices on the inner and outer side walls
        List<Vector3> newVerts = new List<Vector3>(count);
        for (int i = 0; i < count; i++)
        {
            Vector3 v = oldVerts[i];
            if (Mathf.Abs(v.y - minY) < 0.005f)
            {
                v.y = GetSlopedY(v.x, v.z);
            }
            newVerts.Add(v);
        }

        // 3. Build solid downward-facing underside faces beneath each step tread
        List<Face> newFaces = new List<Face>();

        foreach (Face face in oldFaces)
        {
            IList<int> idx = face.indexes;
            if (idx.Count < 3) continue;

            Vector3 p0 = oldVerts[idx[0]];
            Vector3 p1 = oldVerts[idx[1]];
            Vector3 p2 = oldVerts[idx[2]];
            Vector3 normal = Vector3.Cross(p1 - p0, p2 - p0).normalized;

            // Skip any old downward-facing underside faces
            if (normal.y < -0.5f) continue;

            newFaces.Add(new Face(face));

            // If this is a horizontal top step tread (normal points UP), cap the underside below it
            if (normal.y > 0.9f)
            {
                Dictionary<int, int> bottomVertMap = new Dictionary<int, int>();

                foreach (int oldIndex in face.distinctIndexes)
                {
                    Vector3 topPos = oldVerts[oldIndex];
                    Vector3 bottomPos = new Vector3(topPos.x, GetSlopedY(topPos.x, topPos.z), topPos.z);

                    bottomVertMap[oldIndex] = newVerts.Count;
                    newVerts.Add(bottomPos);
                }

                // Reverse triangle winding (0, 2, 1) so the underside face points DOWNWARD
                int[] bottomIndices = new int[idx.Count];
                for (int t = 0; t < idx.Count; t += 3)
                {
                    bottomIndices[t] = bottomVertMap[idx[t + 2]];
                    bottomIndices[t + 1] = bottomVertMap[idx[t + 1]];
                    bottomIndices[t + 2] = bottomVertMap[idx[t]];
                }

                Face undersideFace = new Face(bottomIndices);
                undersideFace.submeshIndex = face.submeshIndex;
                newFaces.Add(undersideFace);
            }
        }

        // 4. Rebuild the ProBuilder mesh, auto-UVs, normals, and collider
        pb.RebuildWithPositionsAndFaces(newVerts, newFaces);
        pb.ToMesh();
        pb.Refresh();

        MeshCollider col = GetComponent<MeshCollider>();
        if (col != null) col.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
    }
}