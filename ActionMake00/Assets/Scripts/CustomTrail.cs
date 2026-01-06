using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mesh 기반 Weapon Trail (Tip/Base 2점 샘플링)
/// - TrailRenderer가 아니라 MeshFilter/MeshRenderer를 갱신해서 "면" 트레일을 만든다.
/// - 회전은 Tip/Base의 상대 위치 변화로 자연스럽게 반영된다.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CustomTrail : MonoBehaviour
{
    [Header("References")]
    [Tooltip("칼 뿌리(손잡이 쪽) 포인트")]
    public Transform trailBase;

    [Tooltip("칼 끝 포인트")]
    public Transform trailTip;

    [Header("Trail Settings")]
    [Tooltip("트레일이 남아있는 시간(초)")]
    [Min(0.01f)] public float lifeTime = 0.25f;

    [Tooltip("샘플이 추가되기 위한 최소 이동 거리(미터) - 너무 작으면 버텍스 폭증")]
    [Min(0f)] public float minDistance = 0.01f;

    [Tooltip("트레일이 너무 길어지는 걸 방지하는 최대 샘플 개수")]
    [Range(4, 256)] public int maxSamples = 64;

    [Tooltip("트레일을 끈 뒤에도 자연스럽게 사라지게 할지")]
    public bool fadeOutAfterStop = true;

    [Header("Visual")]
    [Tooltip("버텍스 컬러 알파로 페이드(머티리얼이 Vertex Color를 사용해야 함)")]
    public bool useVertexColorFade = true;

    [Tooltip("메쉬를 로컬 기준으로 만들지(권장). 트레일 오브젝트가 움직여도 안정적.")]
    public bool buildMeshInLocalSpace = true;

    private struct Sample
    {
        public Vector3 basePos;
        public Vector3 tipPos;
        public float time;
    }

    private readonly List<Sample> _samples = new();
    private Mesh _mesh;
    private MeshFilter _mf;

    private bool _emitting;

    // 최근 샘플링 비교용
    private Vector3 _lastBaseWorld;
    private Vector3 _lastTipWorld;
    private bool _hasLast;

    private readonly List<Vector3> _verts = new();
    private readonly List<int> _tris = new();
    private readonly List<Vector2> _uvs = new();
    private readonly List<Color> _colors = new();

    void Awake()
    {
        _mf = GetComponent<MeshFilter>();
        _mesh = new Mesh
        {
            name = "CustomTrail"
        };
        // 65k 버텍스 넘길 일은 거의 없지만, 안전하게 두고 싶으면 아래 주석 해제
        // _mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        _mf.sharedMesh = _mesh;
        ClearTrailMesh();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F8))
        {
            StartTrail();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            StopTrail();
        }

    }
    void LateUpdate()
    {
        if (trailBase == null || trailTip == null)
            return;

        float now = Time.time;

        // 1) 샘플 추가 (공격 중/emit 중일 때만)
        if (_emitting)
        {
            Vector3 b = trailBase.position;
            Vector3 t = trailTip.position;

            if (!_hasLast)
            {
                _lastBaseWorld = b;
                _lastTipWorld = t;
                _hasLast = true;

                AddSample(b, t, now);
                AddSample(b, t, now); // 시작 끊김 방지용 더블 샘플
            }
            else
            {
                float move = Mathf.Max(
                    Vector3.Distance(_lastBaseWorld, b),
                    Vector3.Distance(_lastTipWorld, t)
                );

                if (move >= minDistance)
                {
                    _lastBaseWorld = b;
                    _lastTipWorld = t;
                    AddSample(b, t, now);
                }
            }
        }

        // 2) 오래된 샘플 제거 (emit 중이 아니어도 fadeOutAfterStop이면 계속 제거)
        if (_samples.Count > 0)
        {
            PruneOldSamples(now);
        }

        // 3) 메쉬 갱신
        if (_samples.Count >= 2)
        {
            BuildMesh(now);
        }
        else
        {
            // 샘플이 너무 적으면 메쉬를 비워서 깔끔하게
            ClearTrailMesh();
        }
    }

    /// <summary>공격 시작 시 호출: 트레일 기록 시작</summary>
    public void StartTrail()
    {
        _emitting = true;
        _hasLast = false;
        _samples.Clear();
        ClearTrailMesh();
    }

    /// <summary>공격 종료 시 호출: 트레일 기록 중단</summary>
    public void StopTrail()
    {
        _emitting = false;
        _hasLast = false;

        if (!fadeOutAfterStop)
        {
            _samples.Clear();
            ClearTrailMesh();
        }
    }

    private void AddSample(Vector3 baseWorld, Vector3 tipWorld, float time)
    {
        // 샘플 상한
        if (_samples.Count >= maxSamples)
        {
            _samples.RemoveAt(0);
        }

        _samples.Add(new Sample
        {
            basePos = baseWorld,
            tipPos = tipWorld,
            time = time
        });
    }

    private void PruneOldSamples(float now)
    {
        // 수명 지난 샘플 제거
        // (앞에서부터 오래된 샘플이므로 while로 빠르게)
        while (_samples.Count > 0 && (now - _samples[0].time) > lifeTime)
        {
            _samples.RemoveAt(0);
        }
    }

    private void BuildMesh(float now)
    {
        _verts.Clear();
        _tris.Clear();
        _uvs.Clear();
        _colors.Clear();

        int n = _samples.Count;

        // 버텍스/UV/컬러 채우기
        // 샘플 i마다 base/tip 두 버텍스 생성
        for (int i = 0; i < n; i++)
        {
            Sample s = _samples[i];

            Vector3 b = s.basePos;
            Vector3 t = s.tipPos;

            if (buildMeshInLocalSpace)
            {
                b = transform.InverseTransformPoint(b);
                t = transform.InverseTransformPoint(t);
            }

            _verts.Add(b); // 2*i
            _verts.Add(t); // 2*i + 1

            // UV: U는 길이(0~1), V는 base=0 tip=1
            float u = (n <= 1) ? 0f : (i / (float)(n - 1));
            _uvs.Add(new Vector2(u, 0f));
            _uvs.Add(new Vector2(u, 1f));

            if (useVertexColorFade)
            {
                // 최신(끝) 쪽이 진하고, 오래된(시작) 쪽이 희미하게
                // time 기반 페이드: age가 lifeTime에 가까울수록 0
                float age = now - _samples[i].time;
                float a = Mathf.Clamp01(1f - (age / lifeTime));
                _colors.Add(new Color(1f, 1f, 1f, a));
                _colors.Add(new Color(1f, 1f, 1f, a));
            }
        }

        // 삼각형 인덱스: (i -> i+1) 구간마다 2개
        // v0 = base[i], v1 = tip[i], v2 = base[i+1], v3 = tip[i+1]
        // tris: (v0, v1, v3), (v0, v3, v2)
        for (int i = 0; i < n - 1; i++)
        {
            int v0 = (i * 2);
            int v1 = (i * 2) + 1;
            int v2 = (i * 2) + 2;
            int v3 = (i * 2) + 3;

            _tris.Add(v0);
            _tris.Add(v1);
            _tris.Add(v3);

            _tris.Add(v0);
            _tris.Add(v3);
            _tris.Add(v2);
        }

        _mesh.Clear();
        _mesh.SetVertices(_verts);
        _mesh.SetTriangles(_tris, 0);
        _mesh.SetUVs(0, _uvs);

        if (useVertexColorFade)
            _mesh.SetColors(_colors);

        _mesh.RecalculateBounds();
        // 라이트 안 쓰면 노멀 필요 없어서 생략 가능
        // _mesh.RecalculateNormals();
    }

    private void ClearTrailMesh()
    {
        _mesh.Clear();
    }
}
