using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Breadnone.Editor;
using System;
using System.Linq;

namespace Breadnone.Extension
{
    [CustomEditor(typeof(STweenQueue))]
    public class STQueueEditor : UnityEditor.Editor
    {
        ScrollView scroll;
        Label selectedElement;
        Label selectedCell;
        int clickCount = 0;
        double lastClicked;
        int maxLayerLength = 5;
        DropdownField drop;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var t = target as STweenQueue;
            
            root.Add(PaintToolbar(t));
            root.Add(PaintTimeline(t));
            root.Add(PaintProperty(t));
            return root;
        }
        VisualElement PaintToolbar(STweenQueue t)
        {
            var vis = new VisualElement();
            vis.style.width = new Length(100, LengthUnit.Percent);
            vis.style.height = new Length(30);
            vis.style.flexDirection = FlexDirection.Row;
            vis.style.justifyContent = Justify.FlexStart;

            var addBtn = new Button();
            addBtn.style.height = new Length(20);
            addBtn.style.width = new Length(45/4, LengthUnit.Percent);
            addBtn.text = "+";

            var remBtn = new Button();
            remBtn.style.height = new Length(20);
            remBtn.style.width = new Length(45/4, LengthUnit.Percent);
            remBtn.text = "-";

            var prevBtn = new Button();
            prevBtn.style.height = new Length(20);
            prevBtn.style.width = new Length(45/4, LengthUnit.Percent);
            prevBtn.text = "<";

            var nexBtn = new Button();
            nexBtn.style.height = new Length(20);
            nexBtn.style.width = new Length(45/4, LengthUnit.Percent);
            nexBtn.text = ">";

            var lyrCount = new TextField();
            lyrCount.SetValueWithoutNotify(maxLayerLength.ToString());
            lyrCount.style.height = new Length(20);
            lyrCount.style.width = new Length(40/6, LengthUnit.Percent);
            lyrCount.SetEnabled(false);

            var btnAddLayerCount = new Button();
            btnAddLayerCount.style.height = new Length(20);
            btnAddLayerCount.style.width = new Length(45/3, LengthUnit.Percent);
            btnAddLayerCount.text = "+ layer";

            var btnMinLayerCount = new Button();
            btnMinLayerCount.style.height = new Length(20);
            btnMinLayerCount.style.width = new Length(45/3, LengthUnit.Percent);
            btnMinLayerCount.text = "- layer";

            addBtn.clicked += ()=>
            {
                scroll.Add(AddCells(t));
            };

            remBtn.clicked += ()=>
            {

            };

            vis.Add(addBtn);
            vis.Add(remBtn);
            vis.Add(prevBtn);
            vis.Add(nexBtn);
            vis.Add(lyrCount);
            vis.Add(btnAddLayerCount);
            vis.Add(btnMinLayerCount);
            return vis;
        }
        VisualElement PaintTimeline(STweenQueue t)
        {
            var vis = new VisualElement();
            vis.style.width = new Length(100, LengthUnit.Percent);
            vis.style.height = new Length(120, LengthUnit.Pixel);
            vis.style.justifyContent = Justify.Center;

            scroll = new ScrollView();
            scroll.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            scroll.horizontalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            scroll.contentContainer.style.flexDirection = FlexDirection.Row;
            scroll.style.width = new Length(100, LengthUnit.Percent);
            scroll.style.height = new Length(100, LengthUnit.Percent);

            vis.Add(scroll);
            return vis;
        }
        bool odd = false;
        Label AddCells(STweenQueue t)
        {
            var lbl = new Label();
            lbl.style.width = new Length(100, LengthUnit.Pixel);
            lbl.style.height = new Length(100, LengthUnit.Percent);

            AddCellLayer(lbl);

            if(!odd)
            {
                odd = true;
                lbl.style.backgroundColor = Color.grey;
            }
            else
            {
                odd = false;
                lbl.style.backgroundColor = Color.gray;
            }

            return lbl;
        }
        Color deftextcol;
        Color defbcgcol;
        /// <summary>
        /// Add cells layer.
        /// </summary>
        /// <param name="cell"></param>
        void AddCellLayer(VisualElement cell)
        {
            for(int i = 0; i < maxLayerLength; i++)
            {
                var layer = new Label();
                deftextcol = layer.style.color.value;

                layer.style.unityTextAlign = TextAnchor.MiddleCenter;
                layer.style.height = new Length(100/maxLayerLength, LengthUnit.Percent);
                layer.style.borderBottomColor = HexColor("4c4a48");
                layer.style.borderRightColor = HexColor("4c4a48");
                layer.style.borderBottomWidth = 1f;
                layer.style.borderRightWidth = 1;
                layer.style.width = new Length(100, LengthUnit.Percent);
                cell.Add(layer);

                defbcgcol = layer.style.backgroundColor.value;

                layer.RegisterCallback<MouseDownEvent>(x=>
                {
                    if(selectedCell != null)
                    {
                        if(string.IsNullOrEmpty(selectedCell.text))
                        {
                            selectedCell.style.color = deftextcol;
                            selectedCell.style.backgroundColor = defbcgcol;
                        }
                    }
                    
                    selectedCell = layer;
                    layer.style.backgroundColor = Color.green;
                    layer.style.color = Color.black;

                    if(string.IsNullOrEmpty(layer.text))
                    {
                        drop.SetValueWithoutNotify("<select tween>");
                    }
                    else
                    {
                        drop.SetValueWithoutNotify(layer.text);
                    }

                    if(clickCount > 0)
                    {
                        if(EditorApplication.timeSinceStartup - lastClicked < 0.8)
                        {
                            Debug.Log("Double cliicked!");
                        }

                        clickCount = 0;
                    }
                    else
                    {
                        clickCount++;
                        lastClicked = EditorApplication.timeSinceStartup;
                    }
                });
            }
        }
        /// <summary>
        /// Cell property area.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        VisualElement PaintProperty(STweenQueue t)
        {
            var vis = new VisualElement();
            vis.style.width = new Length(100, LengthUnit.Percent);
            vis.style.height = new Length(100);

            //separator
            var sep = new Label{text = "----Queue properties----"};
            sep.style.width = new Length(100, LengthUnit.Percent);
            sep.style.height = new Length(20);
            sep.style.unityTextAlign = TextAnchor.MiddleCenter;

            var tweenRoot = new VisualElement();
            tweenRoot.style.flexDirection = FlexDirection.Row;

            drop = new DropdownField();
            drop.style.width = new Length(60, LengthUnit.Percent);
            drop.choices = new List<string>();
            drop.SetValueWithoutNotify("<select tween>");
            PopulateAllTweens();
            drop.choices = names;

            drop.RegisterValueChangedCallback(x=>
            {
                selectedCell.text = x.newValue;
                ChangeChildActiveState(selectedCell.parent, selectedCell);
            });

            var lblDrop = new Label{text = "TWEEN : "};
            lblDrop.style.width = new Length(40, LengthUnit.Percent);
            tweenRoot.Add(lblDrop);
            tweenRoot.Add(drop);

            vis.Add(sep);
            vis.Add(tweenRoot);
            return vis;
        }
        void ChangeChildActiveState(VisualElement paret, VisualElement except)
        {
            var enm = paret.Children().ToList();

                for(int i = 0; i < enm.Count; i++)
                {
                    if(except != enm[i])
                    {
                        selectedCell.SetEnabled(false);
                    }
                }
        }
        private VisualElement Controller(STweenQueue t)
        {
            var root = VTweenTemplate.PlayControl();
            root.root.style.justifyContent = Justify.FlexStart;
            root.play.style.width = new Length(40/3, LengthUnit.Percent);
            root.cancel.style.width = new Length(40/3, LengthUnit.Percent);

            root.play.text = "play";
            root.cancel.text = "cancel";

            //var tplay = CreateTexture2D();
            //var gui = new GUIContent(tplay);
            //gui.image = EditorGUIUtility.IconContent("Animation.Play").image;
            //root.play.style.backgroundImage = tplay;
            //root.play.style.backgroundColor = Color.clear;

            root.play.clicked += ()=>
            {
                t.Play();
            };

            root.cancel.clicked += ()=>
            {
                t.Cancel();
            };

            return root.root;
        }
        void MouseFunc(VisualElement vis, System.Action func)
        {
            vis.RegisterCallback<MouseDownEvent>((x)=> func());
        }
        void ButtonFunc(Button btn, System.Action func)
        {
            btn.clicked += func;
        }
        Color HexColor(string hexColor)
        {
            ColorUtility.TryParseHtmlString(hexColor, out var col);
            return col;
        }
        Texture2D CreateTexture2D()
        {
                var txttwod = new Texture2D(200, 200, TextureFormat.ARGB32, false);
                Color fillColor = Color.clear;
                Color[] fillPixels = new Color[txttwod.width * txttwod.height];

                for (int j = 0; j < fillPixels.Length; j++)
                {
                    fillPixels[j] = fillColor;
                }

                txttwod.SetPixels(fillPixels);
                txttwod.Apply();
            return txttwod;
        }
        List<object> tweens = new();
        List<string> names = new();
        public void PopulateAllTweens()
        {
            var mov = FindObjectsOfType<STweenMove>();
            var scale = FindObjectsOfType<STweenScale>();
            var rotate = FindObjectsOfType<STweenRotate>();
            
            for(int i = 0; i < mov.Length; i++)
            {
                names.Add(mov[i].id.ToString());
                tweens.Add(mov[i]);
            }
            for(int i = 0; i < scale.Length; i++)
            {
                names.Add(scale[i].id.ToString());
                tweens.Add(scale[i]);
            }
            for(int i = 0; i < rotate.Length; i++)
            {
                names.Add(rotate[i].id.ToString());
                tweens.Add(rotate[i]);
            }
        }
    }
}