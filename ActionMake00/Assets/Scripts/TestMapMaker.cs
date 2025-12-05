using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/*using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{
    public Tile[] upNeighbours;//위에 갈수 있는 타일들
    public Tile[] rightNeighbours;//오른쪽에 갈수 있는 타일들
    public Tile[] downNeighbours;//아래쪽으로 갈수 있는 타일들
    public Tile[] leftNeighbours;//왼쪽으로 갈수 있는 타일들
}

public class Cell : MonoBehaviour
{
    public bool collapsed;//변했는지 안변했는지
    public Tile[] tileOptions;//내가 변할수 있는 타일목록

    public void CreateCell(bool collapseState, Tile[] tiles)//의사 생성자
    {
        collapsed = collapseState;
        tileOptions = tiles;
    }

}*/

public class TestMapMaker : MonoBehaviour
{
    [SerializeField] GameObject[] Maps;



    /*public int size;//맵 크기
    public Tile[] tiles;//모든 타일들
    public List<Cell> grid;//현재 모든 셀의 정보를 가지고 있을 리스트
    public Cell cellObj;//cellPrefab

    private int iterater = 0;//현재 진행도

    private void Start()
    {
        Initialize();
    }

    private void Initialize()//셀을 만들어 주고 리스트에 넣는 함수
    {
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell newCell = Instantiate(cellObj, new Vector2(x, y), Quaternion.identity);
                newCell.CreateCell(false, tiles);
                grid.Add(newCell);
            }
        }

        StartCoroutine(CheckEntropy());

    }

    private IEnumerator CheckEntropy()
    {
        List<Cell> tempList = new List<Cell>(grid);
        tempList.RemoveAll(x => x.collapsed);//현재 리스트에서 축소된건 전부 제거

        tempList.Sort((x, y) => x.tileOptions.Length - y.tileOptions.Length);//변할수 있는 가능성이 적은수부터 정렬

        //만약 길이가 크기가 가장 작은 배열이 여러개 일때는 랜덤하게 가져갈수 있도록 하기 위해
        int maxLength = tempList[0].tileOptions.Length;
        int stopIndex = default;

        for (int i = 1; i < tempList.Count; i++)
        {
            if (maxLength < tempList[i].tileOptions.Length)
            {
                stopIndex = i;
                break;
            }
        }

        if (stopIndex > 0)
            tempList.RemoveRange(stopIndex, tempList.Count - stopIndex);

        yield return new WaitForSeconds(0.01f);

        GenerationTile(tempList);
    }

    //현재 cell의 위치에 tile을 랜덤하게 뽑아서 깔아주기.
    private void GenerationTile(List<Cell> newGridList)
    {
        int randIndex = Random.Range(0, newGridList.Count);
        Cell currentCell = newGridList[randIndex];

        currentCell.collapsed = true;
        Tile selectTile = currentCell.tileOptions[Random.Range(0, currentCell.tileOptions.Length)];
        currentCell.tileOptions = new Tile[] { selectTile };

        Instantiate(selectTile, currentCell.transform.position, Quaternion.identity);

        UpdateGeneration(currentCell, selectTile);
    }



    private void UpdateGeneration(Cell currentCell, Tile selectTile)
    {
        List<Cell> neighborsByIndex = GetNeighborsByIndex(currentCell);//이웃셀 가져오기

        for (int i = 0; i < 4; i++)
        {
            if (neighborsByIndex[i] == null) continue;//이웃셀이 null이란 소리는 모서리 또는 테두리에 위치한 셀이란 소리.
            if (neighborsByIndex[i].collapsed) continue;//축소된 셀은 건너뛰고.

            //이웃셀을 가져올때는 위 오르쪽 아래 왼쪽 순으로 가져옴.

            //이웃셀의 타일중에서 현재 셀의 각 방향에 와도 되는 셀을 제외한 셀을 모든 지움.
            if (i == 0)//up
            {

                List<Tile> updatedOptions = new List<Tile>(neighborsByIndex[i].tileOptions);
                foreach (var item in updatedOptions)
                {
                    if (!selectTile.upNeighbours.Contains(item))
                    {
                        neighborsByIndex[i].tileOptions = neighborsByIndex[i].tileOptions.Where(t => t != item).ToArray();
                    }
                }
            }
            else if (i == 1)//right
            {
                List<Tile> updatedOptions = new List<Tile>(neighborsByIndex[i].tileOptions);
                foreach (var item in updatedOptions)
                {
                    if (!selectTile.rightNeighbours.Contains(item))
                    {
                        neighborsByIndex[i].tileOptions = neighborsByIndex[i].tileOptions.Where(t => t != item).ToArray();
                    }

                }
            }
            else if (i == 2)//down
            {
                List<Tile> updatedOptions = new List<Tile>(neighborsByIndex[i].tileOptions);
                foreach (var item in updatedOptions)
                {
                    if (!selectTile.downNeighbours.Contains(item))
                    {
                        neighborsByIndex[i].tileOptions = neighborsByIndex[i].tileOptions.Where(t => t != item).ToArray();
                    }
                }
            }
            else if (i == 3)//left
            {
                List<Tile> updatedOptions = new List<Tile>(neighborsByIndex[i].tileOptions);
                foreach (var item in updatedOptions)
                {
                    if (!selectTile.leftNeighbours.Contains(item))
                    {
                        neighborsByIndex[i].tileOptions = neighborsByIndex[i].tileOptions.Where(t => t != item).ToArray();
                    }
                }
            }
        }

        iterater++;//진행 상황 업데이트
        if (iterater < size * size)
        {
            StartCoroutine(CheckEntropy());//다시 진행
        }
    }

    //이웃셀들 가져오기
    //1차원 배열을 2차원처럼 쓰고 있기 때문에 이러한 수고가 필요함.
    //만약 옆에 셀이 없다면 null을 넣음
    private List<Cell> GetNeighborsByIndex(Cell currentCell)
    {
        List<Cell> neighbors = new List<Cell>();

        int currentCellIndex = Array.IndexOf(grid.ToArray(), currentCell);

        int upIndex = (currentCellIndex + size < grid.Count) ? currentCellIndex + size : -1;
        int rightIndex = (currentCellIndex % size < size - 1) ? currentCellIndex + 1 : -1;
        int downIndex = (currentCellIndex - size >= 0) ? currentCellIndex - size : -1;
        int leftIndex = (currentCellIndex % size > 0) ? currentCellIndex - 1 : -1;

        neighbors.Add(upIndex != -1 ? grid[upIndex] : null);
        neighbors.Add(rightIndex != -1 ? grid[rightIndex] : null);
        neighbors.Add(downIndex != -1 ? grid[downIndex] : null);
        neighbors.Add(leftIndex != -1 ? grid[leftIndex] : null);

        return neighbors;
    }*/
}
