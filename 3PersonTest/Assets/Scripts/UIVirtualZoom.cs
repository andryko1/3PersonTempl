using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIVirtualZoom : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
  
    [System.Serializable]
    public class FloatEvent : UnityEvent<int> { }
    
    [Header("Rect References")]
    public RectTransform containerRect;
    public RectTransform handleRect;

    [Header("Settings")]
    public float joystickRange = 50f;
    public float magnitudeMultiplier = 1f;
    public bool invertXOutputValue;
    public bool invertYOutputValue;

    public int pointer1Id = -1;
    public int pointer2Id = -1;

    public Vector2 pointer1Pos;
    public Vector2 pointer2Pos;

     public Vector2 pointer3Pos;

    public float distanceP1P2Start=0;
    public float distanceP1P2=0;

    [Header("Output")]
    public FloatEvent scrollOutputEvent;

    void Start()
    {
        SetupHandle();
    }

    private void SetupHandle()
    {
        if(handleRect)
        {
            UpdateHandleRectPosition(Vector2.zero);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       // Debug.Log("OnPointerDown111");
        if (pointer1Id == -1) {            
            pointer1Id=eventData.pointerId;
            pointer1Pos=eventData.position;            
        } else if (pointer2Id == -1) {
            pointer2Id=eventData.pointerId;
            pointer2Pos=eventData.position;
        }
        if  (pointer1Id != -1 && (pointer2Id != -1)) {
            distanceP1P2Start =   (pointer1Pos-pointer2Pos).magnitude;  
        } 
        OnDrag(eventData);

        //int TouchCount = T

    }

    public void OnDrag(PointerEventData eventData)
    {
        
       

        
        // Debug.Log("OnPointerDown222");
        // Debug.Log(eventData.position);
        // Debug.Log(eventData.pointerId.ToString() );

         if (pointer1Id == eventData.pointerId ) {
             pointer1Pos=eventData.position;
        } else if (pointer2Id == eventData.pointerId ) {
            pointer2Pos=eventData.position;
        }

        if  (pointer1Id != -1 && (pointer2Id != -1)) {
            distanceP1P2 =   (pointer1Pos-pointer2Pos).magnitude;  
        } 

        if  ((distanceP1P2Start-distanceP1P2)>50) {
            OutputScrollValue(1);
            distanceP1P2Start = distanceP1P2;
        } else if  ((distanceP1P2Start-distanceP1P2)<-50) {
            OutputScrollValue(-1);
            distanceP1P2Start = distanceP1P2;
        }                 

        RectTransformUtility.ScreenPointToLocalPointInRectangle(containerRect, eventData.position, eventData.pressEventCamera, out Vector2 position);
        
        position = ApplySizeDelta(position);
        
        Vector2 clampedPosition = ClampValuesToMagnitude(position);

        Vector2 outputPosition = ApplyInversionFilter(position);

        //OutputPointerEventValue(outputPosition * magnitudeMultiplier);

        if(handleRect)
        {
            UpdateHandleRectPosition(clampedPosition * joystickRange);
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {  //  Debug.Log("OnPointerUp111");
       // OutputPointerEventValue(Vector2.zero);

        if(handleRect)
        {
             UpdateHandleRectPosition(Vector2.zero);
        }

        if (pointer1Id == eventData.pointerId ) {
            pointer1Id = -1;
        } else if (pointer2Id == eventData.pointerId ) {
            pointer2Id = -1;
        }
    }

    private void OutputScrollValue(int scroll)
    {    
        Debug.Log("scroll - "+scroll.ToString());    
        scrollOutputEvent.Invoke(scroll);
    }

    private void UpdateHandleRectPosition(Vector2 newPosition)
    {
        handleRect.anchoredPosition = newPosition;
    }

    Vector2 ApplySizeDelta(Vector2 position)
    {
        float x = (position.x/containerRect.sizeDelta.x) * 2.5f;
        float y = (position.y/containerRect.sizeDelta.y) * 2.5f;
        return new Vector2(x, y);
    }

    Vector2 ClampValuesToMagnitude(Vector2 position)
    {
        return Vector2.ClampMagnitude(position, 1);
    }

    Vector2 ApplyInversionFilter(Vector2 position)
    {
        if(invertXOutputValue)
        {
            position.x = InvertValue(position.x);
        }

        if(invertYOutputValue)
        {
            position.y = InvertValue(position.y);
        }

        return position;
    }

    float InvertValue(float value)
    {
        return -value;
    }
}
