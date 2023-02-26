﻿namespace MmdMapMaid.Core.Models.Vmd.MotionLoop;

public class DuplicationCounter
{
    /// <summary>
    /// 周期
    /// </summary>
    public int Frequency
    {
        get; set;
    }

    /// <summary>
    /// 拍子
    /// </summary>
    public int Beat
    {
        get; set;
    }

    /// <summary>
    /// ループ回数
    /// </summary>
    public int LoopCount
    {
        get; set;
    }

    public int CountOffset
    {
        get; set;
    }

    /// <summary>
    /// 複製回数
    /// </summary>
    public int ElementCount => Math.Max(0, (int)Math.Ceiling(Beat * LoopCount / (double)Frequency) + CountOffset);
}
