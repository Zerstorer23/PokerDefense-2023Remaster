using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    [SerializeField] Cinemachine.CinemachineVirtualCamera vcam;
    [SerializeField] GameObject touchFollower;
    [SerializeField] MapPanel mapPanel;
    // Start is called before the first frame update
    private void Awake()
    {
        vcam = GetComponent<Cinemachine.CinemachineVirtualCamera>();
        SetMoveSpeed();
    }
    private Vector2 nowPos, prePos;
    private Vector3 movePos;
    public float Speed = 0.25f;
    public float orthoZoomSpeed = 0.5f;      //줌인,줌아웃할때 속도(OrthoGraphic모드 용)  
    public float zoomMax = 9;
    public float zoomMin = 3;
    bool isShaking = false;

    [SerializeField] bool backTouch = false;
    [SerializeField] int numTouch = 0;

    // Update is called once per frame
    void SetMoveSpeed()
    {
        return; 
        Speed = 0.25f * Screen.width / 1080f;
        Speed = Speed * vcam.m_Lens.OrthographicSize / 7;

    }

    void Update()
    {
        //    backTouch = BackgroundUI.backgroundTouch;
        //  numTouch = BackgroundUI.numTouch;
        if (ClickManager.GetCurrentUser() != MouseUser.NONE
            || !mapPanel.GetVisibility()
            //  || !BackgroundUI.backgroundTouch
            ) return;


        CheckMove();
        CheckZoom();
    }

    public  void DoShake(float intense=7f, float time = 0.5f) {
        if (isShaking) return;
        isShaking = true;
        StartCoroutine(ProcessShake(intense, time));
    }

    private IEnumerator ProcessShake(float shakeIntensity, float shakeTiming)
    {
        Noise(1, shakeIntensity);
        yield return new WaitForSeconds(shakeTiming);
        Noise(0, 0);
        isShaking = false;
    }

    public void Noise(float amplitudeGain, float frequencyGain)
    {
        Debug.Log("Shaking " + amplitudeGain + " / " + frequencyGain);
        var noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = amplitudeGain;
        noise.m_FrequencyGain = frequencyGain;
    }
    void CheckMove()
    {
        // Debug.Log(Input.touchCount);
        if (Input.touchCount == 0) return;
        Touch touchZero = Input.GetTouch(0);

        if (touchZero.phase == TouchPhase.Began)//터치 시작하면
        {
            prePos = ((touchZero.position));
        //    Debug.Log("Begin touch");
        }

        else if (touchZero.phase == TouchPhase.Moved)//드래그 중이면
        {
         //      Debug.Log("move touch");
            nowPos = touchZero.position - touchZero.deltaPosition;
            movePos = (Vector3)(prePos - nowPos);
            Vector3 moveAmount = movePos * Time.deltaTime * Speed;
            touchFollower.transform.Translate(moveAmount);
            Camera.main.transform.Translate(moveAmount);
             prePos = ((touchZero.position)) - ((touchZero.deltaPosition));
        }
        if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled)
        {

           //   Debug.Log("end touch");
            //  Vector3 relativePosition = vcam.transform.InverseTransformDirection(Vector3.zero - vcam.transform.position);

            Vector3 modPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, touchFollower.transform.position.z);
           // Debug.Log("touch " + touchFollower.transform.position + " vs " + modPos);
            touchFollower.transform.position = modPos;
        }
    }
    void CheckZoom()
    {
        if (Input.touchCount == 2) //손가락 2개가 눌렸을 때
        {
            Touch touchZero = Input.GetTouch(0); //첫번째 손가락 터치를 저장
            Touch touchOne = Input.GetTouch(1); //두번째 손가락 터치를 저장

            //터치에 대한 이전 위치값을 각각 저장함
            //처음 터치한 위치(touchZero.position)에서 이전 프레임에서의 터치 위치와 이번 프로임에서 터치 위치의 차이를 뺌
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition; //deltaPosition는 이동방향 추적할 때 사용
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // 각 프레임에서 터치 사이의 벡터 거리 구함
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude; //magnitude는 두 점간의 거리 비교(벡터)
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // 거리 차이 구함(거리가 이전보다 크면(마이너스가 나오면)손가락을 벌린 상태_줌인 상태)
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;


            vcam.m_Lens.OrthographicSize += deltaMagnitudeDiff * orthoZoomSpeed;
            vcam.m_Lens.OrthographicSize = Mathf.Max(vcam.m_Lens.OrthographicSize, zoomMin);
            vcam.m_Lens.OrthographicSize = Mathf.Min(vcam.m_Lens.OrthographicSize, zoomMax);
            SetMoveSpeed();
        }
    }
}
