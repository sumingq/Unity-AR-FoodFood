using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARTrackedMultiImageManager : MonoBehaviour
{
    [SerializeField]
    // 이미지를 인식했을 때 출력되는 프리팹 목록
    private GameObject[] trackedPrefabs; 

    // 이미지를 인식했을 때 출력되는 오브젝트 목록
    // 키(이미지의 이름)와 값(GameObject)의 쌍을 저장
    private Dictionary<string,GameObject> spawnedObject = new Dictionary<string, GameObject>();
    private ARTrackedImageManager trackedImageManager;

    private void Awake()
    {
        // AR Session Origin 오브젝트에 컴포넌트로 적용했을 때 사용 가능
        trackedImageManager = GetComponent<ARTrackedImageManager>();

        // trackedPrefabs 배열에 있는 모든 프리팹을 Instantiate()로 생성한 후
        // spawnedObject dictionary에 저장, 비활성화
        // 카메라에 이미지가 인식되면 이미지와 동일한 이름의 key에 있는 value 오브젝트를 출력
        foreach (GameObject prefab in trackedPrefabs)
        {
            GameObject clone = Instantiate(prefab); // 오브젝트 생성
            clone.name = prefab.name; // 생성한 오브젝트의 이름 설정
            clone.SetActive(false); // 오브젝트 비활성화
            spawnedObject.Add(clone.name, clone); // Dictionary 자료구조에 오브젝트 저장
        }
    }

    // ARTrackedImageManager의 이벤트 핸들러를 등록 및 해제
    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    //  AR 이미지 트래킹 관련 이벤트가 발생했을 때 호출됨
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 카메라에 이미지가 인식됐을 때
        foreach(var trackedImage in eventArgs.added)
        {
            UpdateImage(trackedImage);
        }
        // 카메라에 이미지가 인식되어 업데이트되고 있을 때
        foreach (var trackedImage in eventArgs.updated)
        {
            UpdateImage(trackedImage);
        }
        // 인식되고 있는 이미지가 카메라에서 사라졌을 때
        foreach (var trackedImage in eventArgs.removed)
        {
            spawnedObject[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        GameObject trackedObject = spawnedObject[name];

        // 이미지의 추적 상태가 추적중(Tracking) 일 때
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            trackedObject.transform.position = trackedImage.transform.position;
            trackedObject.transform.rotation = trackedImage.transform.rotation;
            trackedObject.SetActive(true);
        }
        else
        {
            trackedObject.SetActive(false);
        }
    }
}
