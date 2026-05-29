using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject blockPrefab;
    
    // 유니티 인스펙터에서 길이를 쉽게 조절할 수 있도록 변수 추가
    [SerializeField] private int spawnCount = 30;    // 한 줄당 생성할 블록 개수 (기존 6 -> 30으로 증가)
    [SerializeField] private int spawnSpacing = 10;  // 블록 사이의 Z축 간격

    int position_z;

    void Start()
    {
        // 1. 왼쪽 라인 장애물 생성
        position_z = 20;
        ObjectSpawn(-7);

        // 2. 오른쪽 라인 장애물 생성 (왼쪽과 엇갈리게 배치)
        position_z = 15;
        ObjectSpawn(7);
    }

    private void ObjectSpawn(int position_x)
    {
        // 설정한 spawnCount(30번)만큼 반복하여 훨씬 먼 거리까지 생성
        for (int i = 0; i < spawnCount; i++)
        {
            Instantiate(blockPrefab, new Vector3(position_x, 1f, position_z), blockPrefab.transform.rotation);
            position_z += spawnSpacing; // 지정한 간격만큼 다음 Z축 위치 이동
        }
    }
}