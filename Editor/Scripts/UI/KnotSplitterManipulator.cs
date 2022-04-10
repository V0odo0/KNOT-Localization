using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Knot.Localization.Editor
{
    public class KnotSplitterManipulator : MouseManipulator
    {
        public event Action<Vector2> BeginDrag;
        public event Action<Vector2> Drag;

        private Vector2 _startDragPos;


        public KnotSplitterManipulator(Action<Vector2> beginDrag = null, Action<Vector2> drag = null)
        {
            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });

            BeginDrag = beginDrag;
            Drag = drag;
        }


        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback(new EventCallback<MouseDownEvent>(OnMouseDown));
            target.RegisterCallback(new EventCallback<MouseMoveEvent>(OnMouseMove));
            target.RegisterCallback(new EventCallback<MouseUpEvent>(OnMouseUp));
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback(new EventCallback<MouseDownEvent>(OnMouseDown));
            target.UnregisterCallback(new EventCallback<MouseMoveEvent>(OnMouseMove));
            target.UnregisterCallback(new EventCallback<MouseUpEvent>(OnMouseUp));
        }


        void OnMouseDown(MouseDownEvent evt)
        {
            if (target.HasMouseCapture() || !CanStartManipulation(evt))
                return;

            target.CaptureMouse();
            evt.StopPropagation();

            _startDragPos = evt.mousePosition;

            BeginDrag?.Invoke(evt.mousePosition);
        }

        void OnMouseMove(MouseMoveEvent evt)
        {
            if (!target.HasMouseCapture() || !target.HasMouseCapture())
                return;

            evt.StopPropagation();

            Drag?.Invoke(evt.mousePosition - _startDragPos);
        }

        void OnMouseUp(MouseUpEvent evt)
        {
            if (!target.HasMouseCapture())
                return;

            target.ReleaseMouse();
        }
    }
}
