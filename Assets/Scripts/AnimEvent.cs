using UnityEngine;
public class AnimEvent : MonoBehaviour
{
    public void ShowFire()
    {
        Stage3Panel.Instance.ShowFire();
    }

    public void HideFire()
    {
        Stage3Panel.Instance.HideFire();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ShowSmoke()
    {
        Stage3Panel.Instance.ShowSmoke();
    }

    public void HideSmoke()
    {
        Stage3Panel.Instance.HideSmoke();
    }

    public void ShowControactInfo(int type = 0)
    {
        Stage3Panel.Instance.ShowControactInfo(type);
    }
}
