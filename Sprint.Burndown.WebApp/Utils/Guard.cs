using System;

namespace Sprint.Burndown.WebApp.Utils
{
    public static class Guard
    {
        public static void IsNotNull<T>(T value)
            where T : class
        {
            if (value == null)
                throw new ArgumentNullException();
        }

        public static void IsNotNull<T>(T value, string paramName)
            where T : class
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        public static void IsNotNull<T>(T value, string paramName, string message)
            where T : class
        {
            if (value == null)
                throw new ArgumentNullException(paramName, message);
        }

        public static void IsNotNull<T>(T? value)
            where T : struct
        {
            if (!value.HasValue)
                throw new ArgumentNullException();
        }

        public static void IsNotNull<T>(T? value, string paramName)
            where T : struct
        {
            if (!value.HasValue)
                throw new ArgumentNullException(paramName);
        }

        public static void IsNotNull<T>(T? value, string paramName, string message)
            where T : struct
        {
            if (!value.HasValue)
                throw new ArgumentNullException(paramName, message);
        }

        public static void IsNotNull(string value)
        {
            if (value == null)
                throw new ArgumentNullException();
        }

        public static void IsNotNull(string value, string paramName)
        {
            if (value == null)
                throw new ArgumentNullException(paramName);
        }

        public static void IsNotNull(string value, string paramName, string message)
        {
            if (value == null)
                throw new ArgumentNullException(paramName, message);
        }

        public static void IsNotEmpty(string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("string value must not be empty");
        }

        public static void IsNotEmpty(string value, string paramName)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("string value must not be empty", paramName);
        }

        public static void IsNotEmpty(string value, string paramName, string message)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException(message, paramName);
        }

        public static void GreaterThan<T>(T lowerLimit, T value)
            where T : IComparable<T>
        {
            if (value.CompareTo(lowerLimit) <= 0)
                throw new ArgumentOutOfRangeException();
        }

        public static void GreaterThan<T>(T lowerLimit, T value, string paramName)
            where T : IComparable<T>
        {
            if (value.CompareTo(lowerLimit) <= 0)
                throw new ArgumentOutOfRangeException(paramName);
        }

        public static void GreaterThan<T>(T lowerLimit, T value, string paramName, string message)
            where T : IComparable<T>
        {
            if (value.CompareTo(lowerLimit) <= 0)
                throw new ArgumentOutOfRangeException(paramName, message);
        }


        public static void LessThan<T>(T upperLimit, T value)
            where T : IComparable<T>
        {
            if (value.CompareTo(upperLimit) >= 0)
                throw new ArgumentOutOfRangeException();
        }

        public static void LessThan<T>(T upperLimit, T value, string paramName)
            where T : IComparable<T>
        {
            if (value.CompareTo(upperLimit) >= 0)
                throw new ArgumentOutOfRangeException(paramName);
        }

        public static void LessThan<T>(T upperLimit, T value, string paramName, string message)
            where T : IComparable<T>
        {
            if (value.CompareTo(upperLimit) >= 0)
                throw new ArgumentOutOfRangeException(paramName, message);
        }

        public static void IsTrue<T>(Func<T, bool> condition, T target)
        {
            if (!condition(target))
                throw new ArgumentException("condition was not true");
        }

        public static void IsTrue<T>(Func<T, bool> condition, T target, string paramName)
        {
            if (!condition(target))
                throw new ArgumentException("condition was not true", paramName);
        }

        public static void IsTrue<T>(Func<T, bool> condition, T target, string paramName, string message)
        {
            if (!condition(target))
                throw new ArgumentException(message, paramName);
        }

        public static T IsTypeOf<T>(object obj)
        {
            IsNotNull(obj);

            if (obj is T)
                return (T)obj;

            throw new ArgumentException($"{obj.GetType().Name} is not an instance of type {typeof(T).Name}");
        }

    }
}
