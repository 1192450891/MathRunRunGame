using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TransformUtil
{
    public static Transform GetOrCreateNode(string v)
    {
        var ts = v.Split('/');
        var t = TransformUtil.Find(null, ts[0]);
        if (!t)
            t = new GameObject(ts[0]).transform;
        for (int i = 1; i < ts.Length; i++)
        {
            var tn = ts[i];
            var st = t.Find(tn);
            if (!st)
            {
                st = new GameObject(tn).transform;
                st.parent = t;
                st.localPosition = Vector3.zero;
                st.localRotation = Quaternion.identity;
                st.localScale = Vector3.one;
            }
            t = st;
        }
        return t;
    }

    public static void HideSideTransforms(Transform t)
    {
        if (!t.parent) return;
        for (int i = 0; i < t.parent.childCount; i++)
        {
            var ct = t.parent.GetChild(i);
            if (ct != t)
                ct.gameObject.SetActive(false);
        }
        t.gameObject.SetActive(true);
    }
    public static Transform GetRootParentTransform(Transform current)
    {
        while (current.parent)
        {
            current = current.parent;
        }
        return current;
    }

    public static Transform GetSceneRoot(string rootName)
    {
        var roots = SceneManager.GetActiveScene().GetRootGameObjects();
        for (int i = 0; i < roots.Length; i++)
        {
            if (roots[i].name == rootName)
                return roots[i].transform;
        }
        return null;
    }

    public static Transform[] GetRootGameObjects(string filter)
    {
        var roots = SceneManager.GetActiveScene().GetRootGameObjects();
        List<Transform> ts = new List<Transform>();
        for (int i = 0; i < roots.Length; i++)
        {
            if (roots[i].name.Contains(filter))
                ts.Add(roots[i].transform);
        }
        return ts.ToArray();
    }

    public static Transform Find(Transform t, string transName)
    {
        if (!t)
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                t = Find(roots[i].transform, transName);
                if (t)
                    return t;
            }
            return null;
        }
        if (t.name.ToLower() == transName.ToLower())
        {
            return t;
        }
        else
        {
            for (int i = 0; i < t.childCount; i++)
            {
                Transform childTrans = Find(t.GetChild(i), transName);
                if (childTrans != null)
                {
                    return childTrans;
                }
            }
            return null;
        }
    }

    public static Transform FindContains(Transform t, string ss, bool isEndWord = false)
    {
        var tn = t.name.ToLower();
        var sn = ss.ToLower();
        var idx = tn.LastIndexOf(sn);
        if (idx > -1)
        {
            if (isEndWord)
            {
                if (idx == tn.Length - sn.Length)
                    return t;
            }
            else
                return t;
        }

        for (int i = 0; i < t.childCount; i++)
        {
            Transform childTrans = FindContains(t.GetChild(i), ss, isEndWord);
            if (childTrans != null)
            {
                return childTrans;
            }
        }
        return null;
    }

    static Queue<Transform> q = new Queue<Transform>();
    static List<Transform> l = new List<Transform>();
    public static Transform BftFind(Transform t, string transName)
    {
        var tname = transName.ToLower();
        q.Clear();
        if (t)
        {
            q.Enqueue(t);
        }
        else
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                q.Enqueue(roots[i].transform);
            }
        }
        //int level = 0;
        while (q.Count > 0)
        {
            //Debug.Log("Level = " + level + " count = " + q.Count);
            l.Clear();
            var c = q.Count;
            for (int i = 0; i < c; i++)
            {
                var qt = q.Dequeue();
                //Debug.Log(qt.name);
                if (qt.name.ToLower() == tname)
                    return qt;
                l.Add(qt);
            }
            //Debug.Log("@@ Level = " + level + " count = " + q.Count);
            for (int i = 0; i < l.Count; i++)
            {
                var qt = l[i];
                for (int j = 0; j < qt.childCount; j++)
                {
                    q.Enqueue(qt.GetChild(j));
                }
            }
            //Debug.Log("## Level = " + level + " count = " + q.Count);
            //level = level + 1;
        }
        return null;
    }

    public static Transform BftFindContains(Transform t, string transName, bool isLastWord = false)
    {
        var tname = transName.ToLower();
        Queue<Transform> q = new Queue<Transform>();
        if (t)
        {
            q.Enqueue(t);
        }
        else
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                q.Enqueue(roots[i].transform);
            }
        }
        while (q.Count > 0)
        {
            var qt = q.Dequeue();
            var qtn = qt.name.ToLower();
            var idx = qtn.LastIndexOf(tname);
            if (idx > -1)
            {
                if (isLastWord)
                {
                    if (idx == qtn.Length - tname.Length)
                        return qt;
                }
                else
                    return qt;
            }
            if (qt.childCount > 0)
            {
                for (int i = 0; i < qt.childCount; i++)
                {
                    q.Enqueue(qt.GetChild(i));
                }
            }
        }
        return null;
    }

    public static bool WalkTransform(Transform t, System.Func<Transform, bool> walker)
    {
        if (walker(t))
            return true;
        for (int i = 0; i < t.childCount; i++)
        {
            if (WalkTransform(t.GetChild(i), walker))
                return true;
        }
        return false;
    }

    public static void BftWalkTransform(Transform t, System.Func<Transform, bool> walker)
    {
        if (walker(t))
            return;
        Queue<Transform> q = new Queue<Transform>();
        if (t)
        {
            q.Enqueue(t);
        }
        else
        {
            var roots = SceneManager.GetActiveScene().GetRootGameObjects();
            for (int i = 0; i < roots.Length; i++)
            {
                q.Enqueue(roots[i].transform);
            }
        }
        while (q.Count > 0)
        {
            var qt = q.Dequeue();
            if (walker(qt))
                return;
            if (qt.childCount > 0)
            {
                for (int i = 0; i < qt.childCount; i++)
                {
                    q.Enqueue(qt.GetChild(i));
                }
            }
        }
    }

    public static void ResetTransform(Transform t)
    {
        t.localPosition = Vector3.zero;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
    }

    public static void AddToUIRoot(Transform t)
    {
        //if (UIRoot.list.Count > 0)
        //{
        //	t.SetParent(UIRoot.list[0].transform);
        //	ResetTransform (t);
        //}
        //else
        //{
        //	Debug.LogError("No UIRoot,you need one!");
        //}
    }

    public static float distancePoint2Line(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
    {
        float fProj = Vector3.Dot(point - linePoint1, (linePoint1 - linePoint2).normalized);
        return Mathf.Sqrt((point - linePoint1).sqrMagnitude - fProj * fProj);
    }
}
