﻿using System;
using System.Collections;
using System.Collections.Generic;
using Framework.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.AI.Editor
{
    public class BehaviourTreeEditorNode : GraphNode
    {
        public static readonly int NodeWidth = 120;
        public static readonly int NodeHeight = 40;

        private readonly BehaviourTree TreeAsset;
        private readonly BehaviourTreeNode TreeNode;
        private readonly BehaviourTreeEditorPresenter Presenter;

        private Rect TextRect = new Rect();

        public override string Name => TreeNode.Name;

        public BehaviourTreeEditorNode(BehaviourTree tree, BehaviourTreeNode node, BehaviourTreeEditorPresenter presenter)
        {
            TreeAsset = tree;
            TreeNode  = node;
            Presenter = presenter;
            Position  = TreeNode.EditorPosition;

            Size = new Vector3(NodeWidth, NodeHeight);

            WindowTitle = GUIContent.none;
            WindowStyle = SpaceEditorStyles.GraphNodeBackground;
        }

        protected override void OnSelected(bool value)
        {
            if (Editor.CurrentMouseMode == null)
                Selection.activeObject = TreeNode;
        }

        protected override void OnGUI(int id)
        {
            WindowStyle = Selected
                ? SpaceEditorStyles.GraphNodeBackgroundSelected
                : SpaceEditorStyles.GraphNodeBackground;
            TreeNode.EditorPosition = Position;

            GUI.Box(drawRect, GUIContent.none, WindowStyle);
            DrawConnectDots(drawRect);
        }

        protected override void OnDrawContent(int id)
        {
            var textDimensions = EditorStyles.largeLabel.CalcSize(new GUIContent(Name));

            TextRect.Set(drawRect.x + 0.5f * (drawRect.width - textDimensions.x), drawRect.y + 0.5f * (drawRect.height - textDimensions.y), textDimensions.x, textDimensions.y);
            GUI.Label(TextRect, new GUIContent(Name, TreeNode.Description), EditorStyles.largeLabel);
        }

        void DrawConnectDots(Rect dotRect)
        {
            Vector2 offset = Vector2.zero;

            if (TreeNode.IsParentNode())
            {
                var asParent = TreeNode.AsParentNode();
                var nodeCount = asParent.GetChildNodes().Count;

                if (nodeCount > 0)
                {
                    if(asParent.HasChildrenSlots())
                        offset.x -= ConnectorSize.x * nodeCount * 0.5f;

                    for (int i = 0; i < nodeCount; i++)
                    {
                        GUI.Box (
                            new Rect(dotRect.center.x - ConnectorSize.x * 0.5f + offset.x, dotRect.yMax - ConnectorSize.y * 0.25f, ConnectorSize.x, ConnectorSize.y),
                            new GUIContent(), SpaceEditorStyles.DotFlowTarget
                        );

                        GUI.Box (
                            new Rect(dotRect.center.x - ConnectorSize.x * 0.5f + offset.x, dotRect.yMax - ConnectorSize.y * 0.25f, ConnectorSize.x, ConnectorSize.y),
                            new GUIContent(), SpaceEditorStyles.DotFlowTargetFill
                        );

                        GUI.color = Color.white;

                        offset.x += ConnectorSize.x;
                    }
                }

                if (asParent.HasChildrenSlots())
                {
                    Vector2 pos  = offset + new Vector2(dotRect.center.x - ConnectorSize.x * 0.5f, dotRect.yMax - ConnectorSize.y * 0.25f);
                    Rect    rect = new Rect(pos.x, pos.y, ConnectorSize.x, ConnectorSize.y);

                    if (rect.Contains(Event.current.mousePosition))
                    {
                        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                        {
                            Presenter.OnEmptyDotClicked(this, rect.center);
                        }
                    }

                    GUI.Box(rect, GUIContent.none, SpaceEditorStyles.DotFlowTarget);
                }
            }
        }
    }
}