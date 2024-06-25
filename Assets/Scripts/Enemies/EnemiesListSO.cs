using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy List")]
public class EnemiesListSO : ScriptableObject
{
    public List<EnemySO> EnemiesList = new List<EnemySO>();

}
