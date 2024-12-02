using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calculator
{
    public static Vector2 GetOverlapValue(Bounds self, Bounds target)
    {
        // 겹친 영역의 크기 계산
        float overlapX = Mathf.Min(self.max.x, target.max.x) - Mathf.Max(self.min.x, target.min.x);
        float overlapY = Mathf.Min(self.max.y, target.max.y) - Mathf.Max(self.min.y, target.min.y);
        return new Vector2(overlapX, overlapY);

    }
}
