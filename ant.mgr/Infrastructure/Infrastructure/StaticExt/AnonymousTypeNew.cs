using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.StaticExt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// http://hugoware.net/blog/anonymous-types-dynamic-programming-with-csharp
    /// A convenient method of accessing the values of an 
    /// anonymous type without needing to define a separate class
    /// </summary>
    public class AnonymousTypeNew
    {

        #region Constants

        //flag to mark when no default was provided for accessing properties
        private static readonly object NoDefaultProvided = new object();
        private const string EXCEPTION_COULD_NOT_INVOKE_METHOD =
            "Unable to call method '{0}' using the provided parameters. This could be a casting problem or a method that does not exist.";
        private const string EXCEPTION_COULD_NOT_CALL_METHOD =
                "Unable to call command 'Use'. This could be due to a casting problem or a Property that does not exist.";
        private const string EXCEPTION_COULD_NOT_FIND_PROPERTY =
                "Unable to find a Property named '{0}' (as type {1})";
        private const string EXCEPTION_COULD_NOT_ACCESS_PROPERTY =
                "Unable to access the Property named '{0}' (as type {1}). Inner Exception: {2}.";

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Anonymous type from the value provided
        /// </summary>
        public AnonymousTypeNew(object type)
        {
            this._Init(type);
        }

        /// <summary>
        /// Creates an empty Anonymous Type
        /// </summary>
        public AnonymousTypeNew()
        {
            this._Init(new { });
        }

        /// <summary>
        /// Creates a new anonymous type from the object provided
        /// </summary>
        public static AnonymousTypeNew Create(object obj)
        {
            return new AnonymousTypeNew(obj);
        }

        /// <summary>
        /// Creates an empty anonymous type
        /// </summary>
        public static AnonymousTypeNew Create()
        {
            return new AnonymousTypeNew();
        }

        #endregion

        #region Private Members

        //holds the type of the Anonymous value
        private Type _Type;

        //the actual Anonymous value
        private object _Object;

        //holds a list of the values for this type
        private Dictionary<string, object> _Values = new Dictionary<string, object>();

        #endregion

        #region Calling Methods

        /// <summary>
        /// Attempts to execute a method that has been added to this type
        /// </summary>
        public void Call(string name)
        {
            this.Call<object>(name, null);
        }

        /// <summary>
        /// Attempts to execute a method with parameters that has been added to this type
        /// </summary>
        public void Call(string name, params object[] @params)
        {
            this.Call<object>(name, @params);
        }

        /// <summary>
        /// Attempts to execute a method that has been added to this type and return the value
        /// </summary>
        public TResult Call<TResult>(string name)
        {
            return this.Call<TResult>(name, null);
        }

        /// <summary>
        /// Attempts to execute a method with parameters that has been added to this type and return the value
        /// </summary>
        public TResult Call<TResult>(string name, params object[] @params)
        {
            try
            {
                return (TResult)this.Get<Delegate>(name).DynamicInvoke(@params);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    string.Format(AnonymousTypeNew.EXCEPTION_COULD_NOT_INVOKE_METHOD, name),
                    ex
                    );
            }
        }

        #endregion

        #region Accessing Properties

        /// <summary>
        /// Checks if this Anonymous Type has the specified property
        /// </summary>
        public bool Has(string property)
        {
            return (this._Values.ContainsKey(property));
        }

        /// <summary>
        /// Finds the property and returns the value.
        /// </summary>
        public T Get<T>(string property)
        {
            return (T)this._Get<object>(property, AnonymousTypeNew.NoDefaultProvided);
        }


        /// <summary>
        /// Finds the property and returns the value. If no value was found, 
        /// the default value is returned instead.
        /// </summary>
        public T Get<T>(string property, T @default)
        {
            return (T)this._Get<T>(property, @default);
        }

        /// <summary>
        /// Sets the value of a property on an anonymous type
        /// </summary>
        /// <remarks>Anonymous types are read-only - this saves a value to another location</remarks>
        public void Set(string property, object value)
        {
            this.Set<object>(property, value);
        }

        /// <summary>
        /// Sets the value of a property on an anonymous type
        /// </summary>
        /// <remarks>Anonymous types are read-only - this saves a value to another location</remarks>
        public void Set<T>(string property, T value)
        {

            //check for the value
            if (!this.Has(property))
            {
                this._Values.Add(property, value);

            }
            else
            {

                //try and return the value
                try
                {
                    this._Values[property] = value;
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        string.Format(
                            AnonymousTypeNew.EXCEPTION_COULD_NOT_ACCESS_PROPERTY,
                            property,
                            (value == null ? "null" : value.GetType().Name),
                            ex.Message
                            ),
                            ex);
                }
            }

        }

        #endregion

        #region Private Methods

        //creates the anonymous type
        private void _Init(object type)
        {

            //make sure we aren't recasting a AnonymousTypeNew class
            if (type is AnonymousTypeNew)
            {
                this._Object = ((AnonymousTypeNew)type)._Object;
                this._Type = ((AnonymousTypeNew)type)._Type;
            }
            else
            {
                this._Type = type.GetType();
                this._Object = type;
            }

            //save each property value for use
            foreach (PropertyInfo property in this._Type.GetProperties())
            {
                this._Values.Add(
                    property.Name,
                    property.GetValue(this._Object, null)
                    );
            }

        }


        //Handles actually retreving a value for a type
        private T _Get<T>(string property, object @default)
        {

            //check for the value
            if (!this.Has(property))
            {
                if (@default.Equals(AnonymousTypeNew.NoDefaultProvided)) { throw new Exception(string.Format(EXCEPTION_COULD_NOT_FIND_PROPERTY, property, typeof(T).Name)); }
                return (T)@default;
            }

            //try and return the value
            try
            {
                return (T)this._Values[property];
            }
            catch (Exception ex)
            {
                if (@default.Equals(AnonymousTypeNew.NoDefaultProvided)) { return (T)@default; }
                throw new Exception(
                    string.Format(
                        AnonymousTypeNew.EXCEPTION_COULD_NOT_ACCESS_PROPERTY,
                        property,
                        typeof(T).Name, ex.Message
                        ),
                        ex);
            }

        }

        #endregion

        #region Setting Methods

        /// <summary>
        /// Appends a lambda as a function to this anonymous type
        /// </summary>
        public void SetMethod(string name, Action action)
        {
            this.Set<Action>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type
        /// </summary>
        public void SetMethod<T0>(string name, WithAction<T0> action)
        {
            this.Set<WithAction<T0>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type
        /// </summary>
        public void SetMethod<T0, T1>(string name, WithAction<T0, T1> action)
        {
            this.Set<WithAction<T0, T1>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type
        /// </summary>
        public void SetMethod<T0, T1, T2>(string name, WithAction<T0, T1, T2> action)
        {
            this.Set<WithAction<T0, T1, T2>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type
        /// </summary>
        public void SetMethod<T0, T1, T2, T3>(string name, WithAction<T0, T1, T2, T3> action)
        {
            this.Set<WithAction<T0, T1, T2, T3>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type
        /// </summary>
        public void SetMethod<T0, T1, T2, T3, T4>(string name, WithAction<T0, T1, T2, T3, T4> action)
        {
            this.Set<WithAction<T0, T1, T2, T3, T4>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type
        /// </summary>
        public void SetMethod<T0, T1, T2, T3, T4, T5>(string name, WithAction<T0, T1, T2, T3, T4, T5> action)
        {
            this.Set<WithAction<T0, T1, T2, T3, T4, T5>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type
        /// </summary>
        public void SetMethod<T0, T1, T2, T3, T4, T5, T6>(string name, WithAction<T0, T1, T2, T3, T4, T5, T6> action)
        {
            this.Set<WithAction<T0, T1, T2, T3, T4, T5, T6>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type
        /// </summary>
        public void SetMethod<T0, T1, T2, T3, T4, T5, T6, T7>(string name, WithAction<T0, T1, T2, T3, T4, T5, T6, T7> action)
        {
            this.Set<WithAction<T0, T1, T2, T3, T4, T5, T6, T7>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type
        /// </summary>
        public void SetMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8>(string name, WithAction<T0, T1, T2, T3, T4, T5, T6, T7, T8> action)
        {
            this.Set<WithAction<T0, T1, T2, T3, T4, T5, T6, T7, T8>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type
        /// </summary>
        public void SetMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string name, WithAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> action)
        {
            this.Set<WithAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type that returns a value
        /// </summary>
        public void SetMethod<TResult>(string name, Func<TResult> action)
        {
            this.Set<Func<TResult>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type that returns a value
        /// </summary>
        public void SetMethod<T0, TResult>(string name, WithResultAction<T0, TResult> action)
        {
            this.Set<WithResultAction<T0, TResult>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type that returns a value
        /// </summary>
        public void SetMethod<T0, T1, TResult>(string name, WithResultAction<T0, T1, TResult> action)
        {
            this.Set<WithResultAction<T0, T1, TResult>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type that returns a value
        /// </summary>
        public void SetMethod<T0, T1, T2, TResult>(string name, WithResultAction<T0, T1, T2, TResult> action)
        {
            this.Set<WithResultAction<T0, T1, T2, TResult>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type that returns a value
        /// </summary>
        public void SetMethod<T0, T1, T2, T3, TResult>(string name, WithResultAction<T0, T1, T2, T3, TResult> action)
        {
            this.Set<WithResultAction<T0, T1, T2, T3, TResult>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type that returns a value
        /// </summary>
        public void SetMethod<T0, T1, T2, T3, T4, TResult>(string name, WithResultAction<T0, T1, T2, T3, T4, TResult> action)
        {
            this.Set<WithResultAction<T0, T1, T2, T3, T4, TResult>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type that returns a value
        /// </summary>
        public void SetMethod<T0, T1, T2, T3, T4, T5, TResult>(string name, WithResultAction<T0, T1, T2, T3, T4, T5, TResult> action)
        {
            this.Set<WithResultAction<T0, T1, T2, T3, T4, T5, TResult>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type that returns a value
        /// </summary>
        public void SetMethod<T0, T1, T2, T3, T4, T5, T6, TResult>(string name, WithResultAction<T0, T1, T2, T3, T4, T5, T6, TResult> action)
        {
            this.Set<WithResultAction<T0, T1, T2, T3, T4, T5, T6, TResult>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type that returns a value
        /// </summary>
        public void SetMethod<T0, T1, T2, T3, T4, T5, T6, T7, TResult>(string name, WithResultAction<T0, T1, T2, T3, T4, T5, T6, T7, TResult> action)
        {
            this.Set<WithResultAction<T0, T1, T2, T3, T4, T5, T6, T7, TResult>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type that returns a value
        /// </summary>
        public void SetMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string name, WithResultAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, TResult> action)
        {
            this.Set<WithResultAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, TResult>>(name, action);
        }

        /// <summary>
        /// Appends a lambda as a function to this anonymous type that returns a value
        /// </summary>
        public void SetMethod<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string name, WithResultAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> action)
        {
            this.Set<WithResultAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>(name, action);
        }


        #endregion

        #region Calling Methods

        /* Yikes, a lot of methods here */

        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public void With<T0>(WithAction<T0> with)
        {
            this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public void With<T0, T1>(WithAction<T0, T1> with)
        {
            this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public void With<T0, T1, T2>(WithAction<T0, T1, T2> with)
        {
            this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public void With<T0, T1, T2, T3>(WithAction<T0, T1, T2, T3> with)
        {
            this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public void With<T0, T1, T2, T3, T4>(WithAction<T0, T1, T2, T3, T4> with)
        {
            this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public void With<T0, T1, T2, T3, T4, T5>(WithAction<T0, T1, T2, T3, T4, T5> with)
        {
            this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public void With<T0, T1, T2, T3, T4, T5, T6>(WithAction<T0, T1, T2, T3, T4, T5, T6> with)
        {
            this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public void With<T0, T1, T2, T3, T4, T5, T6, T7>(WithAction<T0, T1, T2, T3, T4, T5, T6, T7> with)
        {
            this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public void With<T0, T1, T2, T3, T4, T5, T6, T7, T8>(WithAction<T0, T1, T2, T3, T4, T5, T6, T7, T8> with)
        {
            this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public void With<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(WithAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> with)
        {
            this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public TReturn With<T0, TReturn>(WithResultAction<T0, TReturn> with)
        {
            return (TReturn)this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public TReturn With<T0, T1, TReturn>(WithResultAction<T0, T1, TReturn> with)
        {
            return (TReturn)this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public TReturn With<T0, T1, T2, TReturn>(WithResultAction<T0, T1, T2, TReturn> with)
        {
            return (TReturn)this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public TReturn With<T0, T1, T2, T3, TReturn>(WithResultAction<T0, T1, T2, T3, TReturn> with)
        {
            return (TReturn)this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public TReturn With<T0, T1, T2, T3, T4, TReturn>(WithResultAction<T0, T1, T2, T3, T4, TReturn> with)
        {
            return (TReturn)this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public TReturn With<T0, T1, T2, T3, T4, T5, TReturn>(WithResultAction<T0, T1, T2, T3, T4, T5, TReturn> with)
        {
            return (TReturn)this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public TReturn With<T0, T1, T2, T3, T4, T5, T6, TReturn>(WithResultAction<T0, T1, T2, T3, T4, T5, T6, TReturn> with)
        {
            return (TReturn)this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public TReturn With<T0, T1, T2, T3, T4, T5, T6, T7, TReturn>(WithResultAction<T0, T1, T2, T3, T4, T5, T6, T7, TReturn> with)
        {
            return (TReturn)this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public TReturn With<T0, T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(WithResultAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, TReturn> with)
        {
            return (TReturn)this._Invoke(with);
        }


        /// <summary>
        /// Maps each variable name to a matching property then calls the 
        /// provided delegate with the parameters. This method IS case-sensitive
        /// </summary>
        public TReturn With<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(WithResultAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> with)
        {
            return (TReturn)this._Invoke(with);
        }

        /// <summary>
        /// Invokes a delegate method using the name and type mappings to the correct
        /// type in the anonymous type
        /// </summary>
        private object _Invoke(Delegate with)
        {
            try
            {

                //retrieve each of the parameters by using the name
                //found in the parameter list
                object[] arguments = with.Method.GetParameters().Select(o => this.Get<object>(o.Name)).ToArray();
                return with.Method.Invoke(with, arguments);
            }
            catch (Exception ex)
            {
                throw new Exception(AnonymousTypeNew.EXCEPTION_COULD_NOT_CALL_METHOD, ex);
            }
        }

        #endregion

    }

    #region Delegates For Use Command

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With
    /// </summary>
    public delegate void WithAction<T0>(T0 p0);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With
    /// </summary>
    public delegate void WithAction<T0, T1>(T0 p0, T1 p1);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With
    /// </summary>
    public delegate void WithAction<T0, T1, T2>(T0 p0, T1 p1, T2 p2);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With
    /// </summary>
    public delegate void WithAction<T0, T1, T2, T3>(T0 p0, T1 p1, T2 p2, T3 p3);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With
    /// </summary>
    public delegate void WithAction<T0, T1, T2, T3, T4>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With
    /// </summary>
    public delegate void WithAction<T0, T1, T2, T3, T4, T5>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With
    /// </summary>
    public delegate void WithAction<T0, T1, T2, T3, T4, T5, T6>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With
    /// </summary>
    public delegate void WithAction<T0, T1, T2, T3, T4, T5, T6, T7>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With
    /// </summary>
    public delegate void WithAction<T0, T1, T2, T3, T4, T5, T6, T7, T8>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With
    /// </summary>
    public delegate void WithAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9);


    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With but also returns a result
    /// </summary>
    public delegate TResult WithResultAction<T0, TResult>(T0 p0);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With but also returns a result
    /// </summary>
    public delegate TResult WithResultAction<T0, T1, TResult>(T0 p0, T1 p1);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With but also returns a result
    /// </summary>
    public delegate TResult WithResultAction<T0, T1, T2, TResult>(T0 p0, T1 p1, T2 p2);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With but also returns a result
    /// </summary>
    public delegate TResult WithResultAction<T0, T1, T2, T3, TResult>(T0 p0, T1 p1, T2 p2, T3 p3);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With but also returns a result
    /// </summary>
    public delegate TResult WithResultAction<T0, T1, T2, T3, T4, TResult>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With but also returns a result
    /// </summary>
    public delegate TResult WithResultAction<T0, T1, T2, T3, T4, T5, TResult>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With but also returns a result
    /// </summary>
    public delegate TResult WithResultAction<T0, T1, T2, T3, T4, T5, T6, TResult>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With but also returns a result
    /// </summary>
    public delegate TResult WithResultAction<T0, T1, T2, T3, T4, T5, T6, T7, TResult>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With but also returns a result
    /// </summary>
    public delegate TResult WithResultAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8);

    /// <summary>
    /// Delegate to be used with AnonymousTypeNew.With but also returns a result
    /// </summary>
    public delegate TResult WithResultAction<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(T0 p0, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8, T9 p9);


    #endregion
}
