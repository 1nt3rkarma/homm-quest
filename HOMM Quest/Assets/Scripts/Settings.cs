using UnityEngine;


[CreateAssetMenu(fileName = "System Settings", menuName = "Character system Settings")]
public class Settings : ScriptableObject
{
    [Tooltip("Управление осущесвтялется на мобильном устройстве?")]
    public bool touchControls = false;

}

