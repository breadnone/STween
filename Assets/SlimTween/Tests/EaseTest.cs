using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Breadnone;
using Breadnone.Extension;


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
            root.style.flexDirection = FlexDirection.Row;

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
            lbl3.style.height = 30;
            lbl3.style.width = Length.Percent(20);
            
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
            
            root.Add(lbl);
            root.Add(lbl4);
            root.Add(lbl2);
            root.Add(lbl5);
            root.Add(lbl3);
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
    // Start is called before the first frame update
    
    public void RunAllMoves()
    {
        for(int i = 0; i < moves.Count; i++)
        {
            moves[i].Play();
        }
    }
    int lastindex = -1;
    public void RunCurves()
    {
        CancelAll();
        TweenClass smove = null;
        float power = 600;

        for(int i = 0; i < moves.Count; i++)
        {
            if(i == 0)
            {
                var mid = new Vector3(moves[i+1].gameObject.transform.position.x, moves[i+1].gameObject.transform.position.y + power, moves[i+1].gameObject.transform.position.z);
                smove = STween.spline(movingObj.transform, mid, moves[i+2].gameObject.transform.position, 0.6f);
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
                            var t = STween.splineFrom(movingObj.transform, defaultposs[i], mid, defaultposs[i+2], 0.6f).halt(true);

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
    public void CancelAll()
    {        
        Reset();
    }
    public void Populate()
    {
        var gos = GameObject.Find("EasingTests");
        movingObj = GameObject.Find("moveParts");
        
        if(Vector3.Distance(Vector3.zero, movdefpos) > 0.001f)
        {
            movdefpos = movingObj.transform.position;
        }

        moves = new List<STweenMove>();
        var trans = gos.GetComponentsInChildren<STweenMove>();

        foreach(var mv in trans)
        {
            moves.Add(mv);
        }

        if(defaultposs.Count == 0)
        {
            defaultposs = new List<Vector3>();
            
            foreach(var pos in moves)
            {
                defaultposs.Add(pos.gameObject.transform.position);
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

        movingObj.transform.position = movdefpos;
        moves.Sort((v1, v2) => v1.gameObject.transform.position.x.CompareTo(v2.gameObject.transform.position.x));
        defaultposs.Sort((v1, v2) => v1.x.CompareTo(v2.x));
    }
}
