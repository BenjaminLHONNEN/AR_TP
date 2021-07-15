using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOnSwipe : MonoBehaviour
{
    public float Scale = 0.1f;
   /* public float ScaleY = 0.1f;
    public float ScaleZ = 0.1f;*/

    private Vector2 startPosition;

    Vector3 temp; 

    void Update()
    {
        HandleSizeChange();

        /*transform.localScale = new Vector3(Scale * Time.deltaTime, Scale * Time.deltaTime, Scale * Time.deltaTime);
        
         temp = transform.localScale;

         temp = (new Vector4(0f, Scale * Time.deltaTime, 0f));
         temp.y += Time.deltaTime;
         temp.z += Time.deltaTime;
        
        transform.localScale = temp;*/

    }

    private void HandleSizeChange()
    {
        Touch[] touches = Input.touches;
        if (touches.Length >= 1)
        {
            Touch currentTouch = touches[0];
            if (currentTouch.phase == TouchPhase.Began)
            {
                startPosition = currentTouch.position;
            }
            else if (currentTouch.phase == TouchPhase.Ended)
            {
                Vector2 endPosition = currentTouch.position;
                HandleSwipe(startPosition, endPosition);
            }
        }
    }
    private void HandleSwipe(Vector2 startPosition, Vector2 endPosition)
    {
        bool isUpwardSwipe = startPosition.y < endPosition.y;
        bool isDownwardSwipe = startPosition.y > endPosition.y;

        //float swipeDistance = Vector2.Distance(startPosition, endPosition);
        if (isUpwardSwipe)
        {
            Scale += 0.1f;
           /* ScaleY += 0.1f;
            ScaleZ += 0.1f;*/
        }
        else if (isDownwardSwipe)
        {
            Scale -= 0.1f;
          /*  ScaleY -= 0.1f;
            ScaleZ -= 0.1f;*/
        }
        transform.localScale = new Vector3(Scale ,Scale ,Scale);
    }
}