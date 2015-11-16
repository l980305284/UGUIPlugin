using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

/*
 * @author 戴佳霖
 * 
 */
public class DoTweenDemo : MonoBehaviour
{
    public Image image;
    public Text text;
    public Slider slider;
    public Transform target;
    private PathType pathType = PathType.CatmullRom;
    private Vector3[] waypoints = new[] {
		new Vector3(4, 2, 6),
		new Vector3(8, 6, 14),
		new Vector3(4, 6, 14),
		new Vector3(0, 6, 6),
		new Vector3(-3, 0, 0)
	};

    void Start()
    {
        DoColor();
        DoComplete();
        DoText();
        DoPath();
        DoSlider();
        DOTween.PlayAll();
    }
    private void DoColor()
    {
        DotweenUtlity.DOColor(image, Color.red, 2f, 0, DoTweenComplete);
    }

    private void DoPath()
    {
        Tween tweener = target.DOPath(waypoints, 2, pathType).SetOptions(true).SetLookAt(0.001f);
        tweener.SetEase(Ease.Linear).SetLoops(-1);
    }

    private void DoComplete()
    {
        Tweener tweener = DotweenUtlity.DOLocalMove(image.transform, new Vector3(200, 200, 0), 2f, -1, LoopType.Yoyo);
        tweener.SetEase(Ease.InOutElastic);
    }

    private void DoText()
    {
        text.DOText("This text will replace the existing one", 2).SetEase(Ease.Linear).SetAutoKill(false).Pause();
    }

    private void DoSlider()
    {
        slider.DOValue(1, 1.5f).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).Pause();
    }

    private void DoTweenComplete()
    {
        Debug.Log("complete!");
    }

}