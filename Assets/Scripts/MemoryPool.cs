using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryPool 
{
    // Start is called before the first frame update
    private class PoolItem
    {
        public bool isActive;
        public GameObject gameObject;
    }

    private int increaseCount = 5;
    private int maxCount;
    private int activeCount;

    private GameObject poolObject;
    private List<PoolItem> poolItemList;

    public int MaxCount => maxCount;
    public int ActiveCount => activeCount;


    public MemoryPool(GameObject poolObject)
    {
        maxCount = 0;
        activeCount = 0;
        this.poolObject = poolObject;


        poolItemList = new List<PoolItem>();

        InstantiateObjects();
    }

    /// <summary>
    /// increaseCount 단위로 오브젝트를 생성
    /// </summary>
    public void InstantiateObjects()
    {
        maxCount += increaseCount;

        for ( int i = 0; i < increaseCount; ++i )
        {
            PoolItem poolItem = new PoolItem();

            poolItem.isActive = false;
            poolItem.gameObject = GameObject.Instantiate(poolObject);
            poolItem.gameObject.SetActive(false);

            poolItemList.Add(poolItem);
        }
    }

    public void DestroyObjects()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for ( int i = 0; i < count; ++ i )
        {
            GameObject.Destroy(poolItemList[i].gameObject);
        }

        poolItemList.Clear();
    }

    /// 현재 생성해서 관리하는 모든 오브젝트 개수의 현재 활성화 상태면 오브젝트 개수 비교
    /// 모든 오브젝트가 활성화 상태이면 새로운 오브젝트 필요

    public GameObject ActivatePoolItem()
    {
        if (poolItemList == null) return null;

        if ( maxCount == activeCount )
        {
            InstantiateObjects();
        }

        int count = poolItemList.Count;
        for ( int i = 0; i < count; ++ i )
        {
            PoolItem poolItem = poolItemList[i];

            if ( poolItem.isActive == false )
            {
                activeCount++;

                poolItem.isActive = true;
                poolItem.gameObject.SetActive(true);

                return poolItem.gameObject;
            }
        }



        return null;
    }

    /// <summary>
    ///  현재 사용이 완료된 오브젝트를 비활성화 상태로 설정
    /// </summary>
    /// <param name="removeObject"></param>

    public void DeactivatePoolItem(GameObject removeObject)
    {
        if (poolItemList == null || removeObject == null) return;

        int count = poolItemList.Count;
        for ( int i = 0; i < count; ++ i)
        {
            PoolItem poolItem = poolItemList[i];

            if ( poolItem.gameObject == removeObject )
            {
                activeCount--;

                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);

                return;
            }
        }
    }

    /// 게임에 사용중이 모든 오브젝트를 비활성화 상태로 설정
    /// 
    public void DeactivateAllPoolItems()
    {
        if (poolItemList == null) return;

        int count = poolItemList.Count;
        for (int i = 0; i < count; ++i)
        {
            PoolItem poolItem = poolItemList[i];

            if (poolItem.gameObject != null && poolItem.isActive == true)
            {
                poolItem.isActive = false;
                poolItem.gameObject.SetActive(false);
            }
        }

        activeCount = 0;
    }
    /// 
}
