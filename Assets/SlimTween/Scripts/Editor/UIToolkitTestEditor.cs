using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Breadnone;

namespace Breadnone.Editor
{
    [CustomEditor(typeof(UIToolkitTest))]
    public class UIToolkitTestEditor : UnityEditor.Editor
{
    bool isAnimating = false;
    public override VisualElement CreateInspectorGUI()
    {
        var root = new VisualElement();
        root.style.width = Length.Percent(100);
        root.style.height = Length.Percent(100);

        var btn = new Label { text = "LabelA" };
        var lbl = new Label { text = "LabelB" };
        var btnStartAnim = new Button { text = "play" };
        btnStartAnim.style.marginTop = 30;
        btnStartAnim.style.width = Length.Percent(30);
        btnStartAnim.style.height = Length.Percent(20);

        btn.style.backgroundColor = Color.blue;
        btn.style.flexGrow = 1;
        btn.style.width = Length.Percent(50);
        btn.style.height = Length.Percent(20);

        lbl.style.flexGrow = 1;
        lbl.style.width = Length.Percent(50);
        lbl.style.height = Length.Percent(20);
        lbl.style.backgroundColor = Color.red;

        btnStartAnim.clicked += () =>
        {
            if (!isAnimating)
            {
                btnStartAnim.text = "stop";
                isAnimating = true;
                Animate(true, btn, 100, 2f);
                Animate(true, lbl, 100, 1.5f);
            }
            else
            {
                btnStartAnim.text = "play";
                isAnimating = false;
                Animate(false, btn, 50, 1f);
                Animate(false, lbl, 50, 1f);
            }
        };

        root.Add(btn);
        root.Add(lbl);
        root.Add(btnStartAnim);
        return root;
    }

    void Animate(bool state, VisualElement vis, float percent, float duration)
    {
        if (state)
        {
            vis.lerpWidth(20, percent, duration, isPercent: true).setLoop(4).setPingPong(0);
        }
        else
        {
            STween.Cancel(vis);
            vis.style.width = Length.Percent(percent);
        }
    }
}
}