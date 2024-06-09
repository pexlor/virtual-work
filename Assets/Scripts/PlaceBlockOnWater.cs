using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;


public class PlaceBlockOnWater : MonoBehaviour
{
    public Camera mainCamera; // 主摄像机，用于从鼠标点击位置发射射线
    public GameObject blockPrefab; // 方块预制体，将在水面上实例化
    private GameObject rod;
    private GameObject rodpoint;
    GameObject newBlock;
    GameObject oldBlock;

    private Bezier3D bezier3D;
    public LayerMask ignoredLayer;
    private LineRenderer lineRenderer;
    private StartFishGame StartFishGame;
    private FishSystem fishSystem;
    private int FishState = -1;
    private Vector3 FishPosition;
    private Vector3 FishTargetPosition;
    public float speed = 5; // 鱼的移动速度
    public float radius = 0.1f; // 鱼移动的圆的半径


    public float maxTime = 5f; // 规定的最大时间（秒）

    private float elapsedTime = 0f; // 已经过去的时间
    private int clickCount = 0; // 点击计数
    private int targetClickCount; // 随机生成的目标点击次数

    public static InputDevice[] hands = new InputDevice[2];//双手
   // public XRRayInteractor rayInteractor;
    RaycastHit hit;
    public XRRayInteractor leftInteractor; //以左手的射线交互器为例


    private GameObject HookedText;
    private GameObject CaughtText;
    private GameObject FailedText;
    public Image fishimage;

    double DelayTime = 5;
    double timer = 0;
    private void Start()
    {
        rod = GameObject.FindGameObjectWithTag("Start");
        rodpoint = GameObject.Find("p5");
        lineRenderer = GetComponent<LineRenderer>();
        StartFishGame = GameObject.Find("StartGameObject").GetComponent<StartFishGame>();
        mainCamera = Camera.main;
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        // 设置线的宽度
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        // 设置线的顶点数量
        lineRenderer.positionCount = 2;

        hands[0] = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);//左手
        hands[1] = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);//右手

        HookedText = GameObject.Find("HookedText");
        CaughtText = GameObject.Find("CaughtText");
        FailedText = GameObject.Find("FailedText");
        //fishimage = GameObject.Find("fishImage");

        HookedText.SetActive(false);
        CaughtText.SetActive(false);
        FailedText.SetActive(false);
        fishimage.gameObject.SetActive(false);

    }

    //省略获取脚本的方法，根据名称标签等根据项目而定
    public RaycastHit RayTest()
    {
        //接收out输出
        RaycastHit rayInfo;
        //获取左手当前的射线结果RaycastHit
        leftInteractor.TryGetCurrent3DRaycastHit(out rayInfo);
        //后续就可以通过rayInfo获取射线击中的碰撞体等等操作
        if (rayInfo.collider != null) {
            Debug.Log("RagInfo");
        }
        return rayInfo;
    }


    private void Fishmove()
    {
        if(newBlock.transform.position == FishTargetPosition)
        {
            Vector2 randomCirclePoint;
            randomCirclePoint.x = Random.Range(-1f, 1f) * radius;
            randomCirclePoint.y = Random.Range(-1f, 1f) * radius;
            FishTargetPosition = new Vector3(FishPosition.x + randomCirclePoint.x, FishPosition.y, FishPosition.z + randomCirclePoint.y);
        }
        newBlock.transform.position = Vector3.MoveTowards(newBlock.transform.position, FishTargetPosition, speed * Time.deltaTime);
    }

    private void ThrowRod()
    {
        if (FishState <= 0 && hands[1].TryGetFeatureValue(CommonUsages.triggerButton, out bool istriggerButton) && istriggerButton)
        {
            Debug.Log("右手按下了扳机trigger键");
            //接收out输出
            RaycastHit rayInfo;
            //获取左手当前的射线结果RaycastHit
            leftInteractor.TryGetCurrent3DRaycastHit(out rayInfo);
            //后续就可以通过rayInfo获取射线击中的碰撞体等等操作
            if (rayInfo.collider != null)
            {
                Debug.Log("RagInfo:" + rayInfo.collider.gameObject.layer);
            }
            // 检测射线是否与水面（或其他碰撞器）相交

            if (rayInfo.collider != null && rayInfo.collider.gameObject.layer == 4)
            {
               // xrLineVisual.invalidColorGradient = validColorGradient;
                if (oldBlock != null)
                {
                    Destroy(oldBlock);
                }
                // 在交点位置实例化一个方块
                
                newBlock = Instantiate(blockPrefab, rayInfo.point, Quaternion.identity);
                bezier3D = rod.GetComponent<Bezier3D>();
                newBlock.SetActive(true);
                bezier3D.forceItem = newBlock.GetComponent<ForceItem>();
                oldBlock = newBlock;
                fishSystem = new FishSystem();
                FishState = 0;
                lineRenderer.enabled = true;
                Debug.Log("Hit valid object: " + rayInfo.collider.name);
            }
               
        }
    }

    //生成一个随机次数和时间，在规定时间内点击足够多的次数就吊起否则失败

    private void Fishup()
    {
        elapsedTime += Time.deltaTime;
        if(elapsedTime > maxTime)
        {
            FishState = 3;
            elapsedTime = 0;
            bezier3D.forceItem = rodpoint.GetComponent<ForceItem>();
            Destroy(newBlock);
            newBlock = null;
            lineRenderer.enabled = false;
            clickCount = 0;
        }
        if (hands[1].TryGetFeatureValue(CommonUsages.triggerButton, out bool istriggerButton) && istriggerButton)
        {
            clickCount++;
            if(clickCount > targetClickCount && elapsedTime < maxTime)
            {
                FishState = 2;
                elapsedTime = 0;
                bezier3D.forceItem = rodpoint.GetComponent<ForceItem>();
                Destroy(newBlock);
                newBlock = null;
                lineRenderer.enabled = false;
                clickCount = 0;
            }
        }
        
    }
    private void ShowYao()
    {
        //Text HookedText = GameObject.Find("HookedText").GetComponent<Text>();
        HookedText.SetActive(true);
    }

    private void ShowFish()
    {
        //Text CaughtText = GameObject.Find("CaughtText").GetComponent<Text>();
        HookedText.SetActive(false);
        CaughtText.SetActive(true);

       // string imagePath ="fish02";
       // GameManager.instance.LoadTexture(fishimage, imagePath);
        fishimage.gameObject.SetActive(true);



    }

    private void FailedFish()
    {
        //Text FailedText = GameObject.Find("FailedText").GetComponent<Text>();
        HookedText.SetActive(false);
        FailedText.SetActive(true);

    }

    private void Over()
    {
        CaughtText.SetActive(false);
        fishimage.gameObject.SetActive(false);
        FailedText.SetActive(false);
    }

    private void Update()
    {
     
        RayTest();
        if (StartFishGame.GameStart)
        {
            // 检查鼠标左键是否被按下
            
            if(newBlock!=null)
            {
                Vector3 BlockPosition = newBlock.transform.position;
                BlockPosition.y += 0.2f;
                lineRenderer.SetPosition(0, rodpoint.transform.position);
                lineRenderer.SetPosition(1, BlockPosition);
            }

            switch(FishState)
            {
                case -1:
                    ThrowRod();
                    break;
                case 0://鱼未咬勾
                    if(fishSystem.Fishing())
                    {
                        FishState = 1;
                        FishPosition = newBlock.transform.position;
                        FishTargetPosition = newBlock.transform.position;
                        targetClickCount = Random.Range(15, 25);
                    }
                    ThrowRod();
                    break;
                case 1://鱼咬勾
                    ShowYao();
                    Fishmove();
                    Fishup();
                    //博弈游戏
                    break;
                case 2://吊起来鱼
                    //展示鱼
                    ShowFish();
                    timer += Time.deltaTime;
                    if (timer >= DelayTime)
                    {
                        // 执行延时操作
                        Debug.Log("Delayed action executed in Update!");
                        FishState = -1;
                        timer = 0;
                        Over();
                        //actionExecuted = true;
                    }
                    break;
                case 3://钓鱼失败
                    FailedFish();
                    timer += Time.deltaTime;
                    if (timer >= DelayTime)
                    {
                        // 执行延时操作
                        Debug.Log("Delayed action executed in Update!");
                        FishState = -1;
                        timer = 0;
                        Over();
                        //actionExecuted = true;
                    }
                    
                    break;
            }
        }

    }
}
