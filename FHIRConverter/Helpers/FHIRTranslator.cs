using System;
using System.Globalization;
using System.Reflection;
using FHIRConverter.Exceptions;
using FHIRConverter.Logging;
using Hl7.Fhir.Model;

namespace FHIRConverter.Helpers
{
    public class FHIRTranslator
    {
        #region Consts
        private const string FHIRNamespace = "Hl7.Fhir.Model.";
        #endregion
        /// <summary>
        ///     Create Instance of Object
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object CreateInstance(string path)
        {
            if (!path.Contains("."))
                path = FHIRNamespace + path;
            var ret = Assembly.GetAssembly(typeof(Account)).CreateInstance(path);
            return ret;
        }


        private static Type GetObjectType(string type)
        {
            if (type.Contains("."))
                return Type.GetType(type);
            return Type.GetType("System." + type);
        }

        /// <summary>
        ///     Cast to type. No case sensitive
        ///     Supported types
        ///     Double
        ///     Int32
        ///     DateTime
        ///     Decimal
        /// </summary>
        /// <param name="v"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object CastToType(object v, string type)
        {
            try
            {
                if (type == "Double") return (decimal?)Convert.ToDouble(v, CultureInfo.InvariantCulture);

                if (type == "Int32") return (decimal?)Convert.ToInt32(v);

                if (type == "DateTime") return DateTime.Parse(v as string);

                if (type == "Decimal") return Convert.ToDecimal(v, CultureInfo.InvariantCulture);


                if (type == "DateTimeOffset")
                {
                    if (v is string)
                        return ConvertDateTimeOffset(v as string);
                    if (v is long)
                        return DateTimeOffset.FromUnixTimeSeconds((long)v);
                    return new DateTimeOffset();
                }


                return null;
            }
            catch (Exception ex)
            {

                Logger.Log(LogLevel.Error, ex);
                return null;
            }
        }


        private static DateTimeOffset ConvertDateTimeOffset(string v)
        {
            if (string.IsNullOrEmpty(v))
                return new DateTimeOffset();

            if (v.Split('-').Length != 3)
                return new DateTimeOffset();

            return new DateTimeOffset(int.Parse(v.Split('-')[0]),
                int.Parse(v.Split('-')[1]),
                int.Parse(v.Split('-')[2]), 0, 0, 0, new TimeSpan());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="root"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="valueTemplate"></param>
        public static void AssignValue(PropertyInfo p1,
            object root,
            object value,
            string type,
            string valueTemplate = null)
        {
            if (!string.IsNullOrEmpty(valueTemplate) && value is string)
                value = valueTemplate.Replace("{value}", value as string);

            if (type == null || type.ToLower() == "default")
            {
                p1.SetValue(root, value);
                return;
            }

            p1.SetValue(root, value.GetType().Name == type ? value : CastToType(value, type));
        }


        /// <summary>
        ///     Set Property to an object
        /// </summary>
        /// <param name="root"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void SetProperty(object root,
            string property,
            object value,
            string propertyType = null,
            string valueTemplate = null
        )
        {
            if (root == null)
                throw new ArgumentNullException();

            if (property == null)
                throw new ArgumentNullException();

            var t = root.GetType();
            var type = propertyType;
            string subPropertyType = null;
            if (property.Contains("/"))
            {
                var vals = property.Split('/');
                if (propertyType != null)
                {
                    if (!propertyType.Contains("/"))
                        throw new Exception("Property type should have the same elements with the property path");
                    type = propertyType.Split('/')[0];
                    subPropertyType = propertyType.Substring(propertyType.IndexOf('/') + 1,
                        propertyType.Length - propertyType.IndexOf('/') - 1);
                }

                var subProperty =
                    property.Substring(property.IndexOf('/') + 1, property.Length - property.IndexOf('/') - 1);

                if (IsSame(type, t))
                {
                    SetProperty(root, subProperty, value, subPropertyType, valueTemplate);
                    return;
                }

                var p1 = t.GetProperty(vals[0]);
                //Take next property in nested object

                if (p1 == null)
                    throw new FHIRPropertyNotFound();

                var v = p1.GetValue(root);
                if (v == null)
                {
                    object newProp = null;
                    if (type == null || type.ToLower() == "default")
                    {
                        newProp = CreateInstance(p1.PropertyType.FullName);
                    }
                    else
                    {
                        if (propertyType.Contains(".")) //Full type name is specified
                            newProp = CreateInstance(type);
                        else
                            newProp = CreateInstance(FHIRNamespace + type); //Add FHIR Model namespacwe
                    }


                    SetProperty(newProp, subProperty, value, subPropertyType, valueTemplate);
                    p1.SetValue(root, newProp);
                }
                else if (p1.PropertyType.IsGenericType)
                {
                    object newProp = null;
                    if (type == null || type.ToLower() == "default")
                    {
                        newProp = CreateInstance(p1.PropertyType.FullName);
                    }
                    else
                    {
                        newProp = propertyType.Contains(".") ? CreateInstance(type) : CreateInstance(FHIRNamespace + type);
                    }

                    var c = new Identifier();
                    SetProperty(newProp, subProperty, value, subPropertyType, valueTemplate);

                    p1.PropertyType.GetMethod("Add")?.Invoke(v, new[] { newProp });
                }
                else
                {
                    if (p1.PropertyType.Name == type)
                        SetProperty(v, subProperty, value, subPropertyType, valueTemplate);
                    else
                        AssignValue(p1, root, value, subPropertyType, valueTemplate);
                }
            }
            else
            {
                //Assign directly to object property
                var p = t.GetProperty(property);
                AssignValue(p, root, value, type, valueTemplate);
            }
        }




        private static bool IsSame(string type, Type t)
        {
            if (type.Contains("."))
                return type.Equals(t.FullName);
            return (FHIRNamespace + type).Equals(t.FullName);
        }

        /// <summary>
        ///     Set Property to an object
        /// </summary>
        /// <param name="property"></param>
        /// <param name="propertyType"></param>
        /// <param name="value"></param>
        /// <param name="valueTemplate"></param>
        public static object SetProperty(string property,
            string propertyType,
            object value,
            string valueTemplate = null)
        {
            if (string.IsNullOrEmpty(propertyType))
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(property))
                throw new ArgumentNullException();

            string type = null;
            string subPropertyType = null;
            if (property.Contains("/"))
            {
                if (!propertyType.Contains("/"))
                    throw new Exception("Property type should have the same elements with the property path");
                type = propertyType.Split('/')[0];
                subPropertyType = propertyType.Substring(propertyType.IndexOf('/') + 1,
                    propertyType.Length - propertyType.IndexOf('/') - 1);

                var subProperty =
                    property.Substring(property.IndexOf('/') + 1, property.Length - property.IndexOf('/') - 1);
                
                object newProp = null;
                newProp = CreateInstance(type);
                SetProperty(newProp, subProperty, value, subPropertyType, valueTemplate);

                return newProp;
            }
            else
            {
                var newProp = CreateInstance(propertyType);
                return newProp;
            }
        }
    }
}