using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Knot.Localization.Components
{
    /// <summary>
    /// Base class that is used to store the reference to localizable component's property or method 
    /// </summary>
    /// <typeparam name="TValueType"></typeparam>
    public abstract class KnotTargetMethod<TValueType>
    {
        /// <summary>
        /// Target component
        /// </summary>
        public Object Object
        {
            get => _object;
            set => _object = value;
        }
        [SerializeField] private Object _object;

        /// <summary>
        /// Target method or property name
        /// </summary>
        public string MethodName
        {
            get => _methodName;
            set => _methodName = value;
        }
        [SerializeField] private string _methodName;

        
        protected KnotTargetMethod() { }

        protected KnotTargetMethod(Object obj, string methodName)
        {
            _object = obj;
            _methodName = methodName;
        }
        

        public virtual bool TrySetValue(TValueType value)
        {
            if (Object == null || string.IsNullOrEmpty(MethodName))
                return false;

            try
            {
                Object.GetType().GetMethod(MethodName, BindingFlags.Public | BindingFlags.Instance).Invoke(_object, new object[] { value });
            }
            catch (Exception e)
            {
                KnotLocalization.Log($"Could not set value to {_object.name}. {e}", LogType.Error);
            }

            return true;
        }
    }
}