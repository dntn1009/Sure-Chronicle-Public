using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedRandomUtility
{
    public static T GetWeightedRandom<T>(List<WeightedItem<T>> weightedItems)
    {
        // ���� �׸��� ������ �⺻�� ��ȯ
        if (weightedItems.Count == 0)
            return default(T);

        // ��� ����ġ�� ���Ͽ� ������ ����
        float totalWeight = 0f;
        foreach (var weightedItem in weightedItems)
        {
            totalWeight += weightedItem.weight;
        }

        // 0���� ���� ������ ���� ���� ����
        float randomValue = UnityEngine.Random.value * totalWeight;

        // ���� ���� ��� ������ ���ϴ��� Ȯ���Ͽ� �׸� ����
        foreach (var weightedItem in weightedItems)
        {
            randomValue -= weightedItem.weight;
            Debug.Log(randomValue);
            if (randomValue <= 0)
            {
                Debug.Log("complete " + randomValue);
                return weightedItem.item;
            }
        }

        // ������� �Դٸ� ���� �߸��� ���� �ƴϹǷ� ������ �׸� ��ȯ
        return weightedItems[weightedItems.Count - 1].item;
    }
}
