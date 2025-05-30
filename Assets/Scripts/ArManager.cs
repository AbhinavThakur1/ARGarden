using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ArManager : MonoBehaviour
{
    [SerializeField] GameObject[] _flowers;
    [SerializeField] Button _unselect, _delete;
    private ARRaycastManager raycastManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    [SerializeField] GameObject _container;
    GameObject _selectedObj;
    RaycastHit _hit;

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        _unselect.onClick.AddListener(UnSelect);
        _delete.onClick.AddListener(Delete);
    }

    IEnumerator Start()
    {
        if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        }

        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            Debug.Log("Camera permission granted");
            // Now safe to start AR Session or other camera use
        }
        else
        {
            Debug.LogError("Camera permission denied");
            // Inform user and handle fallback or exit
        }
    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 touchPosition = Input.GetTouch(0).position;
            Physics.Raycast(Camera.main.ScreenPointToRay(touchPosition), out _hit);
            if (_hit.collider != null && _selectedObj == null)
            {
                if (_hit.collider.CompareTag("Player"))
                {
                    _selectedObj = _hit.collider.gameObject;
                    _container.SetActive(true);
                }
            }

            if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                if (_selectedObj == null)
                {
                    Instantiate(_flowers[Random.Range(0, _flowers.Length - 1)], hitPose.position, hitPose.rotation);
                }
                else
                {
                    _selectedObj.transform.position = hitPose.position;
                }
            }
        }
    }

    void UnSelect()
    {
        _selectedObj = null;
        _container.SetActive(false);
    }

    void Delete()
    {
        Destroy(_selectedObj);
        _container.SetActive(false);
    }
}