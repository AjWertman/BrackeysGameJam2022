using UnityEngine;

public class Whale : MonoBehaviour
{
    bool isActive = false;

    public void SetIsActive(bool _isActive)
    {
        isActive = _isActive;
    }

    public bool IsActive()
    {
        return isActive;
    }
}
