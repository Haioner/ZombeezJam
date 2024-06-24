using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Room List")]
public class RoomListSO : ScriptableObject
{
    public List<RoomController> rooms = new List<RoomController>();
}
