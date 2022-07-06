using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class XRayManager : MonoBehaviour
{
    private static XRayManager instance;
    public static XRayManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<XRayManager>();
            }
            return instance;
        }
    }
    public Button btnXray;
    public Material transparentMeterial;
    private bool isMakingXRay;
    public bool IsMakingXRay
    {
        get
        {
            return isMakingXRay;
        }
        set
        {
            isMakingXRay = value;
            btnXray.GetComponent<Image>().sprite = isMakingXRay ? Resources.Load<Sprite>(PathConfig.XRAY_CLICKED_IMAGE) : Resources.Load<Sprite>(PathConfig.XRAY_UNCLICK_IMAGE);
        }
    }

    public void HandleXRayView(bool currentXRayStatus)
    {
        IsMakingXRay = currentXRayStatus;
        if (IsMakingXRay)
        {
            ChangeMaterial(transparentMeterial, ObjectManager.Instance.OriginObject);
        }
        else
        {
            ChangeMaterial(ObjectManager.Instance.OriginOrganMaterial, ObjectManager.Instance.OriginObject);
        }
    }
    public void ChangeMaterial(Material material, GameObject obj)
    {
        int childCount = obj.transform.childCount;
        if (childCount > 0)
        {
            for (int i = 0; i < childCount; i++)
            {
                if (obj.transform.GetChild(i).gameObject.tag == TagConfig.LABEL_TAG)
                {
                    obj.GetComponent<MeshRenderer>().material = material;
                    return;
                }
                else
                {
                    ChangeMaterial(material, obj.transform.GetChild(i).gameObject);
                }
            }
        }
        else
        {
            obj.GetComponent<MeshRenderer>().material = material;
            return;
        }
    }
}
