using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Breadnone;
using Breadnone.Extension;
using System.Threading;
using System.Threading.Tasks;
using System;

#if UNITY_EDITOR
using UnityEditor.UIElements;
using UnityEditor;
#endif

    [CustomEditor(typeof(EaseTest))]
    public class STweenMoveEditor : UnityEditor.Editor
    {
        private VisualElement objectContainer;
        private VisualElement mainRoot;
        private VisualElement objectContainerContent;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();

            var first = new VisualElement();
            var second = new VisualElement();
            first.style.flexDirection = FlexDirection.Row;
            second.style.flexDirection = FlexDirection.Row;

            root.Add(first);
            root.Add(second);

            var lbl = new Button{text = "PlayMoves"};
            lbl.style.height = 30;
            lbl.style.width = Length.Percent(20);

            var lbl4 = new Button{text = "PlayCurves"};
            lbl4.style.height = 30;
            lbl4.style.width = Length.Percent(20);

            var lbl2 = new Button{text = "CancelAll"};
            lbl2.style.height = 30;
            lbl2.style.width = Length.Percent(20);
            
            var lbl3 = new Button{text = "Populate"};
            lbl3.style.height = 30;
            lbl3.style.width = Length.Percent(20);

            var lbl5 = new Button{text = "Reset"};
            lbl5.style.height = 30;
            lbl5.style.width = Length.Percent(20);

            var lbl6 = new Button{text = "PlayAllCurves"};
            lbl6.style.height = 40;
            lbl6.style.width = Length.Percent(20);
            
            var lbl7 = new Button{text = "PlayAllSecond"};
            lbl7.style.height = 40;
            lbl7.style.width = Length.Percent(20);

            var t = target  as EaseTest;

            lbl3.clicked += ()=>
            {
                t.Populate();
            };

            lbl.clicked +=()=>
            {
                t.RunAllMoves();
            };

            lbl2.clicked +=()=>
            {
                t.CancelAll();
            };

            lbl4.clicked += ()=>
            {
                t.RunCurves();
            };

            lbl5.clicked += ()=>
            {
                t.Reset();
            };

            lbl6.clicked += ()=>
            {   
                t.SpawnLottaCurves();
            };

            lbl7.clicked += ()=>
            {
                t.RunSecondMoves();
            };

            first.Add(lbl);
            first.Add(lbl4);
            first.Add(lbl2);
            first.Add(lbl5);
            first.Add(lbl3);

            second.Add(lbl6);
            second.Add(lbl7);
            
            root.Add(first);
            root.Add(second);
            return root;
        }
    }
public class EaseTest : MonoBehaviour
{
    [SerializeField] private List<STweenMove> moves;
    [SerializeField] private List<STweenRotate> rotates = new();
    [SerializeField] private List<Vector3> defaultposs;
    [SerializeField] private GameObject movingObj;
    [SerializeField] private Vector3 movdefpos;
    [SerializeField] private Canvas canvy;
    [SerializeField] private List<STweenMove> secondmoves = new();
    [SerializeField] private List<Vector3> secondpos = new();
    // Start is called before the first frame update
    
    public void RunAllMoves()
    {
        for(int i = 0; i < moves.Count; i++)
        {
            moves[i].Play();
        }
    }
    public void RunSecondMoves()
    {
        for(int i = 0; i < secondmoves.Count; i++)
        {
            secondmoves[i].Play();
        }
    }
    public void RunCurves(GameObject go = null, bool doncancel = false)
    {
        if(go == null)
        {
            go = movingObj;
        }

        if(!doncancel)
        {
            CancelAll();
        }

        TweenClass smove = null;
        float power = 600;
        go.SetActive(true);

        for(int i = 0; i < moves.Count; i++)
        {
            if(i == 0)
            {
                var mid = new Vector3(moves[i+1].gameObject.transform.position.x, moves[i+1].gameObject.transform.position.y + power, moves[i+1].gameObject.transform.position.z);
                smove = STween.spline(go.transform, mid, moves[i+2].gameObject.transform.position, 0.9f, true, true);
            }
            else
            {
                if(i % 2 == 0)
                {
                    if(i + 2 < moves.Count)
                    {
                        if(smove != null)
                        {
                            var mid = new Vector3(defaultposs[i+1].x, defaultposs[i+1].y + power, defaultposs[i+1].z);
                            var t = STween.splineFrom(go.transform, defaultposs[i], mid, defaultposs[i+2], 0.9f, true, true).halt(true);

                            (smove as ISlimRegister).RegisterLastOnComplete(()=>
                            {
                                t.halt(false);
                            });

                            smove = t;
                        }
                    }
                }
            }
        }
    }
    public async void SpawnLottaCurves()
    {
        List<GameObject> tmp = new List<GameObject>();
        while(clones.Count > 0)
        {
            RunCurves(clones[^1], true);
            tmp.Add(clones[^1]);
            clones.Remove(clones[^1]);
            await Task.Delay(TimeSpan.FromSeconds(0.2));
        }
        clones.Clear();
        clones = tmp;
    }
    public void CancelAll()
    {        
        Reset();
    }
    [SerializeField] private List<GameObject> clones = new List<GameObject>();
    public void Populate()
    {
        var gos = GameObject.Find("EasingTests");
        var gogos = GameObject.Find("EasingTests (1)");
        movingObj = GameObject.Find("moveParts");

        if(gos != null)
        canvy = gos.GetComponent<Canvas>();

        if(movingObj != null)
        {
        if(Vector3.Distance(Vector3.zero, movdefpos) > 0.001f)
        {
            movdefpos = movingObj.transform.position;
        }
        }
        moves = new List<STweenMove>();
        secondmoves = new();
        
        var ttrans = gogos.GetComponentsInChildren<STweenMove>();

        if(gos != null)
        {        
            var trans = gos.GetComponentsInChildren<STweenMove>();
            foreach(var mv in trans)
            {
                moves.Add(mv);
            }
        }
        foreach(var tw in ttrans)
        {
            secondmoves.Add(tw);
        }

        if(defaultposs.Count == 0)
        {
            defaultposs = new List<Vector3>();
            if(moves.Count > 0)
            {
            foreach(var pos in moves)
            {
                defaultposs.Add(pos.gameObject.transform.position);
            }
            }
        }

        if(secondpos.Count == 0)
        {
            secondpos = new();

            foreach(var pos in secondmoves)
            {
                secondpos.Add(pos.gameObject.transform.position);
            }
        }

        if(clones.Count > 0)
        {
            for (int i = clones.Count; i --> 0; )
            {
                if(clones[i] != null)
                {
                    clones[i].transform.position = movingObj.transform.position;
                }
            }
        }
        else
        {
            clones = new();
            
            for(int i = 0; i < 50; i++)
            {
                var go = GameObject.Instantiate(movingObj, movingObj.transform.position, movingObj.transform.rotation);
                go.transform.SetParent(canvy.transform, true);
                clones.Add(go);
            }
        }
    }
    public void RunAllRotation()
    {
        for(int i = 0; i < rotates.Count; i++)
        {
            rotates[i].Stop();
        }
    }
    public void Reset()
    {
        STween.CancelAll();

        for(int i = 0; i < moves.Count; i++)
        {
            moves[i].gameObject.transform.position = defaultposs[i];
        }

        for(int i = 0; i < secondmoves.Count; i++)
        {
            secondmoves[i].gameObject.transform.position = secondpos[i];
        }

        movingObj.transform.position = movdefpos;
        moves.Sort((v1, v2) => v1.gameObject.transform.position.x.CompareTo(v2.gameObject.transform.position.x));
        defaultposs.Sort((v1, v2) => v1.x.CompareTo(v2.x));

        if(clones.Count > 0)
        {
            for (int i = clones.Count; i --> 0; )
            {
                if(clones[i] != null)
                {
                    clones[i].transform.position = movingObj.transform.position;
                    clones[i].SetActive(false);
                }
            }
        }
        else
        {
            clones = new();
            
            for(int i = 0; i < 50; i++)
            {
                var go = GameObject.Instantiate(movingObj, movingObj.transform.position, movingObj.transform.rotation);
                go.transform.SetParent(canvy.transform, true);
                clones.Add(go);
            }
        }
    }
}
