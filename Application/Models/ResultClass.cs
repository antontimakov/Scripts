// ResultClass.cs
using System;

namespace Catatonia.Application.Models
{
    [System.Serializable]
    public class ResultClass<T>
    {
        public string time;
        public string status;
        public T received;
    }
}
