// ElemModel.cs
using System;

namespace Catatonia.Application.Models
{
    [System.Serializable]
    public class ElemModel
    {
        public int elem_id;
        public string elem_name;
        public int x;
        public int y;
        public bool elem_plantable;
        public bool elem_harvestable;
    }
}
