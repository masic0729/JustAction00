using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleReplacer : MonoBehaviour
{
    [SerializeField] Vector3 targetPosition;

    public Vector3 GetParticlePosition() => targetPosition;
}
