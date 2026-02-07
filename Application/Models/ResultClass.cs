// ResultClass.cs
using System;

namespace Catatonia.Application.Models
{
    [System.Serializable]
    public class ResultClass<T>
    {
        public string Time;
        public string Status;
        public T Received;
    }
}
