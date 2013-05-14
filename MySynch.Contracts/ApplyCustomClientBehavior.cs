using System;

namespace MySynch.Contracts
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ApplyCustomClientBehavior:Attribute
    {
        public string BehaviorType { get; set; }
    }
}
