using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;
/*
 * @author 戴佳霖
 * DoTween
 */
public class DotweenUtlity
{
    public static Tweener DOMove(Transform target, Vector3 endValue, float duration, int loops = -1, LoopType loopType = LoopType.Yoyo, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOMove(endValue, duration);
        tweener.SetLoops(loops, loopType);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    public static Tweener DOLocalMove(Transform target, Vector3 endValue, float duration, int loops = -1, LoopType loopType = LoopType.Yoyo, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOLocalMove(endValue, duration);
        tweener.SetLoops(loops, loopType);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }


    public static Tweener DOScale(Transform target, Vector3 endValue, float duration, int loops = -1, LoopType loopType = LoopType.Yoyo, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOScale(endValue, duration);
        tweener.SetLoops(loops, loopType);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    public static Tweener DORotate(Transform target, Vector3 endValue, float duration, RotateMode mode = RotateMode.Fast, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DORotate(endValue, duration, mode);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    public static Tweener DOColor(Material target, Color endValue, float duration, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOColor(endValue, duration);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    public static Tweener DOColor(Image target, Color endValue, float duration, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOColor(endValue, duration);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    public static Tweener DOColor(RawImage target, Color endValue, float duration, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOColor(endValue, duration);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    public static Tweener DOColor(SpriteRenderer target, Color endValue, float duration, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOColor(endValue, duration);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    public static Tweener DOFade(AudioSource target, float endValue, float duration, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOFade(endValue, duration);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    public static Tweener DOLookAt(Transform target, Vector3 towards, float duration, AxisConstraint axisConstraint = AxisConstraint.None, Vector3? up = null, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOLookAt(towards, duration, axisConstraint, up);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    public static Tweener DOOffset(Material target, Vector2 endValue, float duration, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOOffset(endValue, duration);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    public static Tweener DOPath(Transform target, Vector3[] path, float duration, PathType pathType = PathType.Linear, PathMode pathMode = PathMode.Full3D, int resolution = 10, Color? gizmoColor = null, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOPath(path, duration, pathType, pathMode, resolution, gizmoColor);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    public static Tweener DOShakePosition(Transform target, float duration, float strength = 1f, int vibrato = 10, float randomness = 90f, bool snapping = false, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOShakePosition(duration, strength, vibrato, randomness, snapping);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    public static Tweener DOShakePosition(Camera target, float duration, float strength = 90f, int vibrato = 10, float randomness = 90f, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOShakePosition(duration, strength, vibrato, randomness);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    public static Tweener DOTiling(Material target, Vector2 endValue, float duration, float delay = 0, System.Action doComplete = null)
    {
        Tweener tweener = target.DOTiling(endValue, duration);
        SetTweenerComplete(tweener, delay, doComplete);
        return tweener;
    }

    private static void SetTweenerComplete(Tweener tweener, float delay = 0, System.Action doComplete = null)
    {
        if (delay > 0)
            tweener.SetDelay(delay);
        tweener.OnComplete(() =>
        {
            if (doComplete != null)
            {
                doComplete();
                doComplete = null;
            }
        });
    }
}
