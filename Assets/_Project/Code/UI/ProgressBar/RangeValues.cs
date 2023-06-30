using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityRandom = UnityEngine.Random;

namespace DCFApixels
{
    static class RangeValueUtils
    {
        public static float NormalizeT(float t)
        {
            if (t > 1f)
                return t % 1f;

            if (t < 0)
                return 1 + (t % 1f);

            return t;
        }
    }

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RangeFloat : IEquatable<RangeFloat>
    {
        public static readonly RangeFloat one = new RangeFloat(0f, 1f);

        /// <summary>The start of this range</summary>
        public float x;
        /// <summary>The length of this range</summary>
        public float length;

        #region Properties
        public float AbsLength
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Mathf.Abs(length);
        }

        public float Min
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => x;
            set
            {
                length -= value - x;
                x = value;
            }
        }
        public float Max
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => x + length;
            set
            {
                float a = Max - value;
                length -= a;
            }
        }
        public float AbsMin
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Mathf.Min(Min, Max);
        }
        public float AbsMax
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Mathf.Max(Min, Max);
        }

        public float Center
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => length / 2f + x;
        }

        public bool IsPositive
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => length >= 0;
        }
        public bool IsNegative
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => length < 0;
        }
        #endregion

        #region Constructors
        /// <summary>Creates a new value range</summary>
        /// <param name="x">The start of the range</param>
        /// <param name="length">The length of the range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RangeFloat(float x, float length)
        {
            this.x = x;
            this.length = length;
        }
        /// <summary>Creates a new value range</summary>
        /// <param name="min">The start of the range</param>
        /// <param name="max">The end of the range</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RangeFloat MinMax(float min, float max)
        {
            return new RangeFloat(min, max - min);
        }
        #endregion

        #region Positive/Negative
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RangeFloat Inverce()
        {
            return new RangeFloat(Max, -length);
        }
        public RangeFloat Positive()
        {
            if (IsNegative)
                return Inverce();
            else
                return this;
        }
        public RangeFloat Negative()
        {
            if (IsPositive)
                return Inverce();
            else
                return this;
        }
        #endregion

        #region Lerp
        public float Lerp(float t)
        {
            return Mathf.Lerp(x, Max, RangeValueUtils.NormalizeT(t));
        }
        public float LerpClamp(float t)
        {
            return x + length * Mathf.Clamp01(t);
        }
        #endregion

        #region Percent
        public float Percent(float value)
        {
            return (value - Min) / Mathf.Abs(length);
        }
        public float PercentClamp(float value)
        {
            return Mathf.Clamp01(Percent(value));
        }
        #endregion

        #region Contains
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(float value) => value >= Min && value < Max;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(RangeFloat value) => value.Min >= Min && value.Max < Max;
        #endregion

        #region Other
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float GetRandom() => UnityRandom.Range(Min, Max);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Clamp(float value) => Mathf.Clamp(value, Min, Max);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() => $"x:{x} length:{length} min:{Min} max:{Max}";
        #endregion

        #region Equals
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) => obj is RangeFloat other && Equals(in other);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(RangeFloat other) => Equals(in other);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(in RangeFloat other) => length == other.length && x == other.x;
        #endregion

        #region GetHashCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() => HashCode.Combine(x, length);
        #endregion

        #region operators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RangeFloat(in RangeInteger range) => new RangeFloat(range.x, range.length);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator RangeFloat(float a) => new RangeFloat(0f, a);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in RangeFloat a, in RangeFloat b) => a.Equals(in b);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in RangeFloat a, in RangeFloat b) => !a.Equals(in b);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(in RangeFloat a, float b) => a.length == b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(in RangeFloat a, float b) => a.length != b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(in RangeFloat a, float b) => a.length > b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(in RangeFloat a, float b) => a.length < b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(in RangeFloat a, float b) => a.length >= b;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(in RangeFloat a, float b) => a.length <= b;
        #endregion
    }

    [Serializable]
    public struct RangeInteger //TODO rework this
    {
        public static readonly RangeInteger one = new RangeInteger(0, 1);
        public int x;
        public int length;
        public int AbsLength => Mathf.Abs(length);
        public float Average => length / 2f + x;

        #region Positive/Negative
        public bool IsPositive => length >= 0;
        public bool IsNegative => length < 0;

        public RangeInteger Inverce()
        {
            return new RangeInteger(Max, -length);
        }

        public RangeInteger Positive()
        {
            if (IsNegative)
                return Inverce();
            else
                return this;
        }

        public RangeInteger Negative()
        {
            if (IsPositive)
                return Inverce();
            else
                return this;
        }
        #endregion

        #region Constructors
        public RangeInteger(int x, int length)
        {
            this.x = x;
            this.length = length;
        }
        public static RangeInteger MinMax(int min, int max)
        {
            return new RangeInteger(min, max - min);
        }
        #endregion

        #region Min/Max
        public int Min
        {
            get => x;
            set
            {
                length -= value - x;
                x = value;
            }
        }
        public int Max
        {
            get => x + length;
            set
            {
                int a = Max - value;
                length -= a;
            }
        }
        public int AbsMin
        {
            get => Mathf.Min(Min, Max);
        }
        public int AbsMax
        {
            get => Mathf.Max(Min, Max);
        }
        #endregion

        #region Evaluate
        public float Evaluate(float t)
        {
            return x + length * RangeValueUtils.NormalizeT(t);
        }
        public float EvaluateClamp(float t)
        {
            return x + length * Mathf.Clamp01(t);
        }
        #endregion

        #region MathPercent
        public float MathPercent(float value)
        {
            return (value - Min) / (float)Mathf.Abs(length);
        }
        public float MathPercentClamp(float value)
        {
            return Mathf.Clamp01(MathPercent(value));
        }
        #endregion

        public int GetRandom()
        {
            return UnityRandom.Range(Min, Max);
        }

        public float Clamp(float value)
        {
            return Mathf.Clamp(value, Min, Max);
        }
        public int Clamp(int value)
        {
            return Mathf.Clamp(value, Min, Max);
        }
        public int Loop(int value, bool maxInclusive = false)
        {
            if (maxInclusive == false)
            {
                return value % Max;
            }
            else
            {
                return value % (Max + 1); //TODO проверить как работает в отрицательном диапозоне
            }
        }

        public bool Contains(float value)
        {
            return value >= Min && value < Max;
        }

        public override string ToString()
        {
            return $"x:{x} length:{length} min:{Min} max:{Max}";
        }

        public static explicit operator RangeInteger(RangeFloat range)
        {
            return new RangeInteger((int)range.x, (int)range.length);
        }
    }

    [Serializable]
    public struct RangeFloatPosition
    {
        public RangeFloat range;
        public float value;

        public void RandomizePosition()
        {
            value = range.GetRandom();
        }

        public float Value_Clamp
        {
            get => range.Clamp(value);
            set => this.value = range.Clamp(value);
        }

        public float Value_Percent
        {
            get => range.Percent(value);
            set => this.value = range.Lerp(value);
        }

        public float Value_Percent_Clamp
        {
            get => range.PercentClamp(value);
            set => this.value = range.LerpClamp(value);
        }
    }
    [Serializable]
    public struct RangeIntegerPosition
    {
        public RangeInteger range;
        public int value;

        public void RandomizePosition()
        {
            value = range.GetRandom();
        }

        public int Value_Clamp
        {
            get => range.Clamp(value);
            set => this.value = range.Clamp(value);
        }

        public float Value_Percent
        {
            get => range.MathPercent(value);
            set => this.value = Mathf.FloorToInt(range.Evaluate(value));
        }

        public float Value_Percent_Clamp
        {
            get => range.MathPercentClamp(value);
            set => this.value = Mathf.FloorToInt(range.EvaluateClamp(value));
        }
    }


    #region PropertyAttributes
    public class ClampedRangeAttribute : PropertyAttribute
    {
        public float min;
        public float max;
        public ClampedRangeAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
        public ClampedRangeAttribute() : this(0f, 1f) { }
    }

    public class MinMaxRangeAttribute : PropertyAttribute
    {
    }
    #endregion

    public static class RangeValueExtensions
    {
        public static RangeInteger GetRange(this ICollection collection)
        {
            return new RangeInteger(0, collection.Count);
        }
        public static RangeInteger GetRangeG<T>(this IReadOnlyList<T> list)
        {
            return new RangeInteger(0, list.Count);
        }
        public static RangeInteger GetRangeG<T>(this IReadOnlyCollection<T> collection)
        {
            return new RangeInteger(0, collection.Count);
        }

        public static bool ContainsIndex(this ICollection collection, int index)
        {
            return index >= 0 && index < collection.Count;
        }
        public static bool ContainsIndexG<T>(this IReadOnlyList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }
        public static bool ContainsIndexG<T>(this IReadOnlyCollection<T> collection, int index)
        {
            return index >= 0 && index < collection.Count;
        }
    }
}

#region Editor
#if UNITY_EDITOR
namespace DCFApixels.Editors
{
    using UnityEditor;

    [CustomPropertyDrawer(typeof(ClampedRangeAttribute), true)]
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute), true)]
    [CustomPropertyDrawer(typeof(RangeFloat), true)]
    [CustomPropertyDrawer(typeof(RangeInteger), true)]
    public class RangedValueDrawer : PropertyDrawer
    {
        private const float SPACING = 4f;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            int indentLevel = EditorGUI.indentLevel;
            float defaultlabelWidth = EditorGUIUtility.labelWidth;

            SerializedProperty xProp = property.FindPropertyRelative("x");
            SerializedProperty lengthProp = property.FindPropertyRelative("length");

            Rect labelRect = position;
            labelRect.width = defaultlabelWidth;

            Rect fieldRect = position;
            fieldRect.xMin = labelRect.xMax;

            string minmaxTooltip;

            switch (xProp.propertyType)
            {
                case SerializedPropertyType.Integer:
                    minmaxTooltip = new RangeInteger(xProp.intValue, lengthProp.intValue).ToString();
                    break;
                case SerializedPropertyType.Float:
                    minmaxTooltip = new RangeFloat(xProp.floatValue, lengthProp.floatValue).ToString();
                    break;
                default:
                    minmaxTooltip = "error";
                    break;
            }

            label.tooltip = minmaxTooltip;
            EditorGUI.LabelField(labelRect, label);
            EditorGUI.BeginProperty(position, label, property);

            if (attribute is MinMaxRangeAttribute minMaxRangeAttribute)
            {
                DrawMinMaxField(fieldRect, property, minMaxRangeAttribute, xProp, lengthProp);
                goto exit;
            }

            if (attribute is ClampedRangeAttribute clampedRangeAttribute)
            {
                DrawClampedRangeField(fieldRect, property, clampedRangeAttribute, xProp, lengthProp);
                goto exit;
            }

            DrawDefaultField(fieldRect, xProp, lengthProp);

            exit:;
            EditorGUIUtility.labelWidth = defaultlabelWidth;
            EditorGUI.indentLevel = indentLevel;
            EditorGUI.EndProperty();
        }

        private void DrawDefaultField(Rect fieldRect, SerializedProperty xProp, SerializedProperty lengthProp)
        {
            float width = fieldRect.width / 2f - SPACING / 2f;

            Rect minRect = fieldRect;
            minRect.width = width;
            minRect.x = fieldRect.xMin;

            Rect maxRect = fieldRect;
            maxRect.width = width;
            maxRect.x = minRect.xMax + SPACING;

            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 12f;
            EditorGUI.PropertyField(minRect, xProp);
            EditorGUIUtility.labelWidth = 42f;
            EditorGUI.PropertyField(maxRect, lengthProp);
        }
        private void DrawClampedRangeField(Rect fieldRect, SerializedProperty property, ClampedRangeAttribute attribute, SerializedProperty xProp, SerializedProperty lengthProp)
        {
            EditorGUI.indentLevel = 0;

            float rightFieldWidth = 24;
            float verticalSpacing = EditorGUIUtility.standardVerticalSpacing;

            if (fieldRect.width <= rightFieldWidth * 4.5f)
            {
                rightFieldWidth = 0;
                verticalSpacing = 0;
            }

            Rect leftFieldRect = fieldRect;
            leftFieldRect.xMax -= rightFieldWidth * 2f + verticalSpacing * 2f;

            Rect rightFieldRect1 = fieldRect;
            rightFieldRect1.x = leftFieldRect.xMax + verticalSpacing;
            rightFieldRect1.width = rightFieldWidth;

            Rect rightFieldRect2 = rightFieldRect1;
            rightFieldRect2.x = rightFieldRect1.xMax + verticalSpacing;

            switch (xProp.propertyType)
            {
                case SerializedPropertyType.Integer:
                    {
                        int min = xProp.intValue;
                        int max = lengthProp.intValue + min;

                        MinMaxSlider(leftFieldRect, ref min, ref max, (int)attribute.min, (int)attribute.max);

                        min = EditorGUI.IntField(rightFieldRect1, min);
                        max = EditorGUI.IntField(rightFieldRect2, max);
                        max = Mathf.Max(min, max);

                        xProp.intValue = min;
                        lengthProp.intValue = Mathf.Abs(max) - min;

                        property.serializedObject.ApplyModifiedProperties();
                    }
                    break;
                case SerializedPropertyType.Float:
                    {
                        float min = xProp.floatValue;
                        float max = lengthProp.floatValue + min;

                        EditorGUI.MinMaxSlider(leftFieldRect, ref min, ref max, attribute.min, attribute.max);

                        min = EditorGUI.FloatField(rightFieldRect1, min);
                        max = EditorGUI.FloatField(rightFieldRect2, max);
                        max = Mathf.Max(min, max);

                        xProp.floatValue = min;
                        lengthProp.floatValue = Mathf.Abs(max) - min;

                        property.serializedObject.ApplyModifiedProperties();
                    }
                    break;
                default:
                    GUI.Label(fieldRect, "error");
                    break;
            };
        }
        private void DrawMinMaxField(Rect fieldRect, SerializedProperty property, MinMaxRangeAttribute attribute, SerializedProperty xProp, SerializedProperty lengthProp)
        {
            float width = fieldRect.width / 2f - SPACING / 2f;

            Rect minRect = fieldRect;
            minRect.width = width;
            minRect.x = fieldRect.xMin;

            Rect maxRect = fieldRect;
            maxRect.width = width;
            maxRect.x = minRect.xMax + SPACING;

            EditorGUIUtility.labelWidth = 24f;
            EditorGUI.indentLevel = 0;

            switch (xProp.propertyType)
            {
                case SerializedPropertyType.Integer:
                    {
                        int min = xProp.intValue;
                        int max = lengthProp.intValue + min;

                        min = EditorGUI.IntField(minRect, min < max ? "Min" : "Max", min);
                        max = EditorGUI.IntField(maxRect, min < max ? "Max" : "Min", max);

                        xProp.intValue = min;
                        lengthProp.intValue = Mathf.Abs(max) - min;

                        property.serializedObject.ApplyModifiedProperties();
                    }
                    break;
                case SerializedPropertyType.Float:
                    {
                        float min = xProp.floatValue;
                        float max = lengthProp.floatValue + min;

                        min = EditorGUI.FloatField(minRect, min < max ? "min" : "max", min);
                        max = EditorGUI.FloatField(maxRect, min < max ? "max" : "min", max);

                        xProp.floatValue = min;
                        lengthProp.floatValue = Mathf.Abs(max) - min;

                        property.serializedObject.ApplyModifiedProperties();
                    }
                    break;
                default:
                    GUI.Label(fieldRect, "error");
                    break;
            }
        }

        private void MinMaxSlider(Rect position, ref int minValue, ref int maxValue, float minLimit, float maxLimit)
        {
            float minValueFloat = minValue;
            float maxValueFloat = maxValue;
            EditorGUI.MinMaxSlider(position, ref minValueFloat, ref maxValueFloat, minLimit, maxLimit);
            minValue = Mathf.RoundToInt(minValueFloat);
            maxValue = Mathf.RoundToInt(maxValueFloat);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif
#endregion
