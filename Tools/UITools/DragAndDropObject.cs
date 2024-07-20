using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;
using UnityEngine.Serialization;


namespace UITools
{
    /// <summary>
    /// Прикрепляется к UI объекту, который необходимо передвинуть на сцену. Предоставляет делегат для добавления метода, срабатывающего в момент отпускания объекта на сцену.
    /// </summary>
    public class DragAndDropObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IEndDragHandler, IDropHandler
    {
        [Serializable]
        public class OnPointerDownEvent : UnityEvent<PointerEventData> { }

        // Event delegates triggered on click.
        /// <summary>
        /// OnInvoke(PointerEventData pointerEventData)
        /// </summary>
        [FormerlySerializedAs("onPointerDownEvent")]
        [SerializeField]
        [Tooltip("Срабатывает во время нажатия на кнопку мыши либо на экран смартфона.")]
        private OnPointerDownEvent onPointerDownEvent = new OnPointerDownEvent();


        [Serializable]
        public class OnDragEvent : UnityEvent<PointerEventData> { }

        // Event delegates triggered on dragging object.
        /// <summary>
        /// OnInvoke(PointerEventData pointerEventData)
        /// </summary>
        [FormerlySerializedAs("onDragEvent")]
        [SerializeField]
        private OnDragEvent onDragEvent = new OnDragEvent();


        [Serializable]
        public class OnEndDragEvent : UnityEvent<PointerEventData> { }

        // Event delegates triggered on end drag object.
        /// <summary>
        /// OnInvoke(PointerEventData pointerEventData)
        /// </summary>
        [FormerlySerializedAs("onEndDragEvent")]
        [SerializeField]
        private OnEndDragEvent onEndDragEvent = new OnEndDragEvent();


        private Vector2 shiftVector;


        public void OnDrag(PointerEventData eventData)
        {
            //Debug.Log($"OnDrag. eventData.position: {eventData.position}, eventData.delta: {eventData.delta}, eventData.scrollDelta: {eventData.scrollDelta}");
            gameObject.transform.position = eventData.position - shiftVector;
            onDragEvent.Invoke(eventData);
        }

        public void OnDrop(PointerEventData eventData)
        {
            //Debug.Log($"OnDrop. eventData.position: {eventData.position}, eventData.delta: {eventData.delta}, eventData.scrollDelta: {eventData.scrollDelta}");
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            //Debug.Log($"OnEndDrag. eventData.position: {eventData.position}, eventData.delta: {eventData.delta}, eventData.scrollDelta: {eventData.scrollDelta}");
            onEndDragEvent.Invoke(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log($"OnPointerDown. eventData.position: {eventData.position}, eventData.delta: {eventData.delta}, eventData.scrollDelta: {eventData.scrollDelta}");
            shiftVector = eventData.position - (Vector2)gameObject.transform.position;
            onPointerDownEvent.Invoke(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log($"OnPointerUp. eventData.position: {eventData.position}, eventData.delta: {eventData.delta}, eventData.scrollDelta: {eventData.scrollDelta}");
        }

    }
}