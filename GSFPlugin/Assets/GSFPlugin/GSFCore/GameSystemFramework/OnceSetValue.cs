using System;
using System.Collections.Generic;
using System.Text;

namespace GameSystem
{
    [Serializable]
    public sealed class OnceSetValue<T>
    {
        private T value;
        private bool valueLock = false;

        public OnceSetValue()
        {
            value = default(T);
        }

        /// <summary>
        /// Constructor with self-defined default value
        /// </summary>
        /// <param name="value">self-defined default value</param>
        public OnceSetValue(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Get or set value
        /// </summary>
        public T Value
        {
            get { return value; }
            set
            {
                if(!valueLock)
                {
                    valueLock = true;
                    this.value = value;
                }
                else
                {
                    throw new InvalidOperationException("Value has been set.");
                }
            }
        }

        public void Set(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Lock permission of setting value
        /// </summary>
        public void Lock()
        {
            valueLock = true;
        }

        public static implicit operator T(OnceSetValue<T> v)
        {
            return v.value;
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    [Serializable]
    public sealed class OnceSetValue<T1, T2>
    {
        private T1 value1;
        private T2 value2;
        private bool valueLock = false;

        public OnceSetValue()
        {
            value1 = default(T1);
            value2 = default(T2);
        }

        public OnceSetValue(T1 value1, T2 value2)
        {
            this.value1 = value1;
            this.value2 = value2;
        }

        public T1 Value1
        {
            get { return value1; }
            set
            {
                if (!valueLock)
                {
                    valueLock = true;
                    value1 = value;
                }
                else
                {
                    throw new InvalidOperationException("Value has been set.");
                }
            }
        }

        public T2 Value2
        {
            get { return value2; }
            set
            {
                if (!valueLock)
                {
                    valueLock = true;
                    value2 = value;
                }
                else
                {
                    throw new InvalidOperationException("Value has been set.");
                }
            }
        }

        public void Set(T1 value1, T2 value2)
        {
            this.value1 = value1;
            this.value2 = value2;
            valueLock = true;
        }

        /// <summary>
        /// Lock permission of setting value
        /// </summary>
        public void Lock()
        {
            valueLock = true;
        }

        public static implicit operator Tuple<T1, T2>(OnceSetValue<T1, T2> v)
        {
            return new Tuple<T1, T2>(v.value1, v.value2);
        }
    }

    [Serializable]
    public sealed class OnceSetValue<T1, T2, T3>
    {
        private T1 value1;
        private T2 value2;
        private T3 value3;
        private bool valueLock = false;

        public OnceSetValue()
        {
            value1 = default(T1);
            value2 = default(T2);
            value3 = default(T3);
        }

        public OnceSetValue(T1 value1, T2 value2, T3 value3)
        {
            this.value1 = value1;
            this.value2 = value2;
            this.value3 = value3;
        }

        public T1 Value1
        {
            get { return value1; }
            set
            {
                if (!valueLock)
                {
                    valueLock = true;
                    value1 = value;
                }
                else
                {
                    throw new InvalidOperationException("Value has been set.");
                }
            }
        }

        public T2 Value2
        {
            get { return value2; }
            set
            {
                if (!valueLock)
                {
                    valueLock = true;
                    value2 = value;
                }
                else
                {
                    throw new InvalidOperationException("Value has been set.");
                }
            }
        }

        public T3 Value3
        {
            get { return value3; }
            set
            {
                if (!valueLock)
                {
                    valueLock = true;
                    value3 = value;
                }
                else
                {
                    throw new InvalidOperationException("Value has been set.");
                }
            }
        }

        public void Set(T1 value1, T2 value2, T3 value3)
        {
            this.value1 = value1;
            this.value2 = value2;
            this.value3 = value3;
            valueLock = true;
        }

        /// <summary>
        /// Lock permission of setting value
        /// </summary>
        public void Lock()
        {
            valueLock = true;
        }

        public static implicit operator Tuple<T1, T2, T3>(OnceSetValue<T1, T2, T3> v)
        {
            return new Tuple<T1, T2, T3>(v.value1, v.value2, v.value3);
        }
    }
}
