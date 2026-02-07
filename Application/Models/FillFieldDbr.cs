// Application/Models/FillFieldDbr.cs
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Catatonia.Application.Models
{
    [System.Serializable]
    public class FillFieldDbr
    {
        public string Name;
        public int X;
        public int Y;
        public bool IsPlantable;
        public bool IsHarvestable;
        public bool IsWeed;
        public int Lifetime;
        public string UpdatedAt;
        public DateTime? UpdatedAtModefied;
    }
}
