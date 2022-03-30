using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public const float itemFadeDuration = 0.35f;
    public const float fadeAlpha = 0.45f;

    // ʱ�����
    public const float secondThreshold = 0.12f;
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 30;
    public const int seasonHold = 3;

    // ����ת��
    public const float fadeDuration = 1f;

    // �����������
    public const int reapAmount = 2;

    // NPC�����ƶ�
    public const float gridCellSize = 1;
    public const float gridCellDiagonalSize = 1.41f;
    public const float pixelSize = 0.05f;
    public const float animationBreakTime = 0.05f;
    public const int maxGridSize = 9999;
}
