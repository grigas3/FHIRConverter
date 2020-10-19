using FHIRConverter.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FHIRConverter.Exceptions;
using Hl7.Fhir.Model;

namespace FHIRConverter.Helpers
{
    public class FHIRTranslator
    {

        /// <summary>
        /// Create Instance of Object
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object CreateInstance(string path)
        {

            if (!path.Contains("."))
                path = "Hl7.Fhir.Model." + path;
            var ret=Assembly.GetAssembly(typeof(Hl7.Fhir.Model.Account)).CreateInstance(path);
            return ret;

        }


        private static Type GetObjectType(string type)
        {

            if (type.Contains("."))
                return (Type.GetType(type));
            else
                return  (Type.GetType("System." + type));
        }
        /// <summary>
        /// Cast to type. No case sensitive 
        /// Supported types
        /// Double
        /// Int32
        /// DateTime
        /// Decimal
        /// 
        /// </summary>
        /// <param name="v"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static object CastToType(object v,string type )
        {

            try
            {

                if (type == "Double")
                {
                    return (Decimal?)Convert.ToDouble(v, System.Globalization.CultureInfo.InvariantCulture);
                }
                else if (type == "Int32")
                {
                    return (Decimal?)Convert.ToInt32(v);
                }
                else if (type == "DateTime")
                    return DateTime.Parse(v as string);
                else if (type == "Decimal")
                    return Convert.ToDecimal(v, System.Globalization.CultureInfo.InvariantCulture);

              
                else if (type == "DateTimeOffset")
                {
                    if (v is string)
                    {
                        return ConvertDateTimeOffset(v as string);
                    }
                    else if (v is long)
                        return DateTimeOffset.FromUnixTimeSeconds((long) v);
                    else
                    {
                        return new DateTimeOffset();

                    }

                }


                else return null;

            }
            catch (Exception ex)
            {

                return null;
            }


        }


        private static DateTimeOffset ConvertDateTimeOffset(string v)
        {
            if(string.IsNullOrEmpty(v))
                return new DateTimeOffset();

            if ((v as string).Split('-').Length != 3)
            return new DateTimeOffset();

            return new DateTimeOffset(int.Parse((v as string).Split('-')[0]),
                int.Parse((v as string).Split('-')[1]),
                int.Parse((v as string).Split('-')[2]), 0, 0, 0, new TimeSpan());

        }
        /// <summary>
        //
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="root"></param>
        /// <param name="value"></param>
        /// <param name="type"></param>
        public static void AssignValue(PropertyInfo p1,object root,object value,string type,string valueTemplate=null)
        {

            if (!string.IsNullOrEmpty(valueTemplate) && value is string)
            {

                value =valueTemplate.Replace("{value}",value as string);
            }

            if (type == null || type.ToLower() == "default")
            {
                p1.SetValue(root, value);
                return;
            }

            if (value.GetType().Name == type)
            {
                p1.SetValue(root, value);
            }
            else
            {
               p1.SetValue(root, CastToType(value,type));

            }

        }


        /// <summary>
        /// Set Property to an object
        /// </summary>
        /// <param name="root"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static void SetProperty(object root,
            string property,
            object value,
            string propertyType=null,
            string valueTemplate=null
            )
        {

            if (root == null)
                throw new ArgumentNullException();

            if (property == null)
                throw new ArgumentNullException();

            var t = root.GetType();
            string type = propertyType;
            string subPropertyType = null;
            if (property.Contains("/"))
            {
                var vals = property.Split('/');
                if (propertyType != null)
                {
                    if(!propertyType.Contains("/"))
                    {
                        throw new Exception("Property type should have the same elements with the property path"); 

                    }
                    type = propertyType.Split('/')[0];
                    subPropertyType = propertyType.Substring(propertyType.IndexOf('/') + 1, propertyType.Length - propertyType.IndexOf('/') - 1);
                }
                var subProperty = property.Substring(property.IndexOf('/') + 1, property.Length - property.IndexOf('/') - 1);

                if (IsSame(type, t))
                {
                    SetProperty(root, subProperty, value, subPropertyType, valueTemplate);
                    return;
                }

                var p1 = t.GetProperty(vals[0]);
                //Take next property in nested object

                if (p1 == null)
                    throw new FHIRPropertyNotFound();

                var v=p1.GetValue(root);
                if (v == null)
                {


                    object newProp = null;
                    if (type == null || type.ToLower() == "default")
                        newProp = CreateInstance(p1.PropertyType.FullName);
                    else
                    {

                        if (propertyType.Contains("."))  //Full type name is specified
                            newProp = CreateInstance(type);
                        else
                        {
                            newProp = CreateInstance("Hl7.Fhir.Model." + type); //Add FHIR Model namespacwe

                        }
                    }

                  
                    SetProperty(newProp, subProperty, value,subPropertyType, valueTemplate);
                    p1.SetValue(root, newProp);

                }
                else if (v!=null&&p1.PropertyType.IsGenericType)
                {

                    object newProp = null;
                    if (type == null || type.ToLower() == "default")
                        newProp = CreateInstance(p1.PropertyType.FullName);
                    else
                    {

                        if (propertyType.Contains("."))  //Full type name is specified
                            newProp = CreateInstance(type);
                        else
                            newProp = CreateInstance("Hl7.Fhir.Model." + type);  //Add FHIR Model namespacwe
                    }

                    var c=new Identifier();
                    SetProperty(newProp, subProperty, value, subPropertyType, valueTemplate);

                    p1.PropertyType.GetMethod("Add")?.Invoke(v, new[] {newProp});
                }
                else
                {

                    if (p1.PropertyType.Name == type)
                    {
                        SetProperty(v, subProperty, value, subPropertyType, valueTemplate);
                        
                        
                        
                    }
                    else
                        AssignValue(p1, root, value, subPropertyType, valueTemplate);                    

                }

            }
            else
            {
                //Assign directly to object property
                var p = t.GetProperty(property);              
                AssignValue(p, root, value,type, valueTemplate);

                
            }
                
        }



        private static bool IsSame(string type,Type t)
        {

            if (type.Contains("."))
                return type.Equals(t.FullName);
            else
            {

               return ("Hl7.Fhir.Model." + type).Equals(t.FullName);
            }

        }

        /// <summary>
        /// Set Property to an object
        /// </summary>
        /// <param name="root"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public static object SetProperty(string property, string propertyType , object value,string valueTemplate=null)
        {
            if (string.IsNullOrEmpty(propertyType))
                throw new ArgumentNullException();

            if (string.IsNullOrEmpty(property))
                throw new ArgumentNullException();

            string type = null;
            string subPropertyType = null;
            if (property.Contains("/"))
            {
                var vals = property.Split('/');
                if (propertyType != null)
                {
                    if (!propertyType.Contains("/"))
                    {
                        throw new Exception("Property type should have the same elements with the property path");

                    }
                    type = propertyType.Split('/')[0];
                    subPropertyType = propertyType.Substring(propertyType.IndexOf('/') + 1, propertyType.Length - propertyType.IndexOf('/') - 1);
                }


                
                var subProperty = property.Substring(property.IndexOf('/') + 1, property.Length - property.IndexOf('/') - 1);

              
                object newProp = null;                
                newProp = CreateInstance(type);

                SetProperty(newProp, subProperty, value,subPropertyType,valueTemplate);

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
