using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class ScrollCamera : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler,
    IPointerUpHandler, IDragHandler
{
    public new Camera camera;
    public bool handle;

    public float max = 90;
    public float min = 30;
    public float speed = 8f;

    private Vector3 _origin;
    private Vector3 _end;
    private Rotate _rotate;

    [FormerlySerializedAs("RenderTexture")]
    public RenderTexture renderTexture;

    public bool onlyMin;
    public bool onlyMax;

    private void Start()
    {
        _rotate = FindObjectOfType<Rotate>();
    }

    private void Update()
    {
        if (camera.targetTexture == null)
        {
            camera.targetTexture = renderTexture;
        }

		// TouchManager.Instance.HandleTouchInteraction();
//         if (!handle)
//         {
//             return;
//         }

//         #if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
//             if (Input.touchCount == 2)
//             {
//                 var touch0 = Input.GetTouch(0);
//                 var touch1 = Input.GetTouch(1);

//                 if (touch0.phase == TouchPhase.Moved &&
//                     touch1.phase == TouchPhase.Moved)
//                 {
//                     var curDist = touch0.position - touch1.position;

//                     var prevDist = touch0.position -
//                                     touch0.deltaPosition -
//                                     (touch1.position - touch1.deltaPosition);

//                     var delta = curDist.magnitude - prevDist.magnitude;

//                     if (delta > 0)
//                     {
//                         StartCoroutine(ScrollMinCamera());
//                     }
//                     else if (delta < 0)
//                     {
//                         StartCoroutine(ScrollMaxCamera());
//                     }
//                     else{
//                         StopAllCoroutines();
//                     }
//                 }
//             }
        
// #else
//         var scroll = Input.GetAxisRaw("Mouse ScrollWheel");
//         if (scroll > 0f)
//         {
//             StartCoroutine(ScrollMaxCamera());
//         }
//         else if (scroll < 0f)
//         {
//             StartCoroutine(ScrollMinCamera());
//         }
//         else
//         {
//             StopAllCoroutines();
//         }
//         #endif
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        handle = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        handle = false;
    }

    private IEnumerator ScrollMaxCamera()
    {
        if (camera.fieldOfView < max)
        {
                camera.fieldOfView += 0.75f;
                yield return null;
        }
    }

    public bool IsVisibleCamera(Renderer renderer)
    {
        return GeometryUtility
            .TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), renderer.bounds);
    }

    private IEnumerator ScrollMinCamera()
    {
        if (camera.fieldOfView > min)
        {
            bool breakable=false;
            foreach(var x in _rotate.GetComponentsInChildren<MeshRenderer>())
            {
                if (!IsVisibleCamera(x))
                {
                    breakable=true;
                    yield break;
                }
            }
            if(breakable)
            {
                yield break;
            }
                camera.fieldOfView -= 0.75f;
                yield return null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _origin = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _origin = _end = Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _end = Input.mousePosition;

        if ((_end - _origin) != Vector3.zero)
        {
            _rotate.transform.Rotate(Vector3.up, -((_end.x - _origin.x) * speed * Mathf.Deg2Rad));
        }

        _origin = _end;
    }
}