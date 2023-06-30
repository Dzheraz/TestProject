using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace DCFApixels
{
    [Serializable]
    public struct SerializableNullable<T> where T : struct
    {
        [SerializeField]
        private bool _hasValue;
        [SerializeField]
        private T _value;

        public bool HasValue => _hasValue;
        public T Value
        {
            get
            {
                if (!_hasValue)
                    throw new InvalidOperationException("NoValue");

                return _value;
            }
        }

        public SerializableNullable(T value)
        {
            _value = value;
            _hasValue = true;
        }

        public T GetValueOrDefault() => _hasValue ? _value : default;
        public T GetValueOrDefault(T defaultValue) => _hasValue ? _value : defaultValue;

        public override bool Equals(object other)
        {
            if (!_hasValue) return other == null;
            if (other == null) return false;
            return _value.Equals(other);
        }

        public override int GetHashCode()
        {
            return _hasValue ? _value.GetHashCode() : 0;
        }
        public override string ToString()
        {
            return _hasValue ? _value.ToString() : "";
        }

        public static implicit operator SerializableNullable<T>(T value)
        {
            return new SerializableNullable<T>(value);
        }
        public static explicit operator T(SerializableNullable<T> value)
        {
            return value.Value;
        }

        public static implicit operator SerializableNullable<T>(T? other)
        {
            if (other == null)
                return new SerializableNullable<T>();

            return new SerializableNullable<T>() { _value = other.Value, _hasValue = true };
        }
        public static implicit operator T?(SerializableNullable<T> other)
        {
            if (other.HasValue == false)
                return null;

            return other._value;
        }

        public static bool operator !=(SerializableNullable<T> value, T? other)
        {
            if (other == null)
                return value._hasValue;

            return !SerializableNullable.Equals(value, new SerializableNullable<T>() { _value = other.Value, _hasValue = other.HasValue });
        }
        public static bool operator ==(SerializableNullable<T> value, T? other)
        {
            if (other == null)
                return !value._hasValue;

            return !SerializableNullable.Equals(value, new SerializableNullable<T>() { _value = other.Value, _hasValue = other.HasValue });
        }
    }

    [System.Runtime.InteropServices.ComVisible(true)]
    public static class SerializableNullable
    {
        [System.Runtime.InteropServices.ComVisible(true)]
        public static int Compare<T>(SerializableNullable<T> n1, SerializableNullable<T> n2) where T : struct
        {
            if (n1.HasValue)
            {
                if (n2.HasValue)
                    return Comparer<T>.Default.Compare(n1.Value, n2.Value);
                return 1;
            }
            if (n2.HasValue)
                return -1;
            return 0;
        }

        [System.Runtime.InteropServices.ComVisible(true)]
        public static bool Equals<T>(SerializableNullable<T> n1, SerializableNullable<T> n2) where T : struct
        {
            if (n1.HasValue)
            {
                if (n2.HasValue)
                    return EqualityComparer<T>.Default.Equals(n1.Value, n2.Value);
                return false;
            }
            if (n2.HasValue)
                return false;
            return true;
        }

        public static Type GetUnderlyingType(Type nullableType)
        {
            if ((object)nullableType == null)
            {
                throw new ArgumentNullException("nullableType");
            }
            Contract.EndContractBlock();
            Type result = null;
            if (nullableType.IsGenericType && !nullableType.IsGenericTypeDefinition)
            {
                Type genericType = nullableType.GetGenericTypeDefinition();
                if (ReferenceEquals(genericType, typeof(SerializableNullable<>)))
                {
                    result = nullableType.GetGenericArguments()[0];
                }
            }
            return result;
        }
    }

    public class NullablePropertyAttribute : PropertyAttribute { }
}

#if UNITY_EDITOR
namespace DCFApixels.Editors
{
    using UnityEditor;
    [CustomPropertyDrawer(typeof(NullablePropertyAttribute))]
    public class SerializableNullableDrawer : PropertyDrawer
    {
        private const float CHECKBOX_WIDTH = 16;
        private const float Spacing = 4;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Color defaultColor;
            SerializedProperty hasValueProp = property.FindPropertyRelative("_hasValue");
            SerializedProperty valueProp = property.FindPropertyRelative("_value");

            if (valueProp.propertyType == SerializedPropertyType.Generic)
            {
                defaultColor = GUI.color;
                GUI.color = Color.red;
                GUI.enabled = false;
                EditorGUI.TextField(position, label, "Remove [NullableProperty]");
                GUI.color = defaultColor;
                GUI.enabled = true;
                return;
            }

            Rect mainPosition = position;
            mainPosition.xMax -= CHECKBOX_WIDTH + Spacing;
            Rect checkboxPosition = position;
            checkboxPosition.xMin = position.xMax - CHECKBOX_WIDTH;

            if (hasValueProp.boolValue)
            {
                EditorGUI.PropertyField(mainPosition, valueProp, label);
            }
            else
            {
                Rect labelPos = mainPosition;
                labelPos.width = EditorGUIUtility.labelWidth;
                Rect label2Pos = mainPosition;
                label2Pos.xMin = labelPos.xMax + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.LabelField(labelPos, label);
                GUI.enabled = false;
                GUI.Label(label2Pos, "NULL", EditorStyles.textField);
                GUI.enabled = true;
            }

            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            defaultColor = GUI.color;
            GUI.color = Color.yellow;
            hasValueProp.boolValue = EditorGUI.Toggle(checkboxPosition, hasValueProp.boolValue);
            hasValueProp.serializedObject.ApplyModifiedProperties();
            GUI.color = defaultColor;
            EditorGUI.indentLevel = indentLevel;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty hasValueProp = property.FindPropertyRelative("_hasValue");
            SerializedProperty valueProp = property.FindPropertyRelative("_value");

            if (valueProp.propertyType == SerializedPropertyType.Generic)
            {
                return EditorGUI.GetPropertyHeight(property);
            }

            if (hasValueProp.boolValue == false)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            return EditorGUI.GetPropertyHeight(valueProp);
        }
    }
}
#endif

