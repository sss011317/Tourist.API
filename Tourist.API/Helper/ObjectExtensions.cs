using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Tourist.API.Helper
{
    /// <summary>
    /// ObjectExtension 與 IEnumerableExtension 差異
    /// 首先返回值得不同 一個是object 一個是IEnumerable
    /// 分開兩個拓展文件最主要就是基於性能的考慮
    /// 因為在IEnumerable進行反射操作的開銷是非常巨大的
    /// 所以遍免在for循環中進行反射操作，通常會以propertyInfo以列表的形式保存起來，在後續程式碼重複使用
    /// 而在object對象拓展中，沒有必要保存屬性列表，所以只是簡單獲取數據訊息，在單一對象拓展中會對每個對象都做一次反射操作
    /// 而在IEnumerable中避免了此操作，所以從運行效率的角度來說，對數據塑型分開操作，可以顯著提昇系統處理能力
    /// </summary>
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(
            this TSource source,
            string fields)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var dataShapedObject = new ExpandoObject();

            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource)
                    .GetProperties(BindingFlags.IgnoreCase
                    | BindingFlags.Public | BindingFlags.Instance);
                foreach(var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);

                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }
                return dataShapedObject;
            }
            else
            {
                var fieldsAfterSplit = fields.Split(',');

                foreach (var field in fieldsAfterSplit)
                {
                    // trim each field, as it might contain leading 
                    // or trailing spaces. Can't trim the var in foreach,
                    // so use another var.
                    var propertyName = field.Trim();

                    // use reflection to get the property on the source object
                    // we need to include public and instance, b/c specifying a 
                    // binding flag overwrites the already-existing binding flags.
                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasn't found " +
                            $"on {typeof(TSource)}");
                    }

                    // get the value of the property on the source object
                    var propertyValue = propertyInfo.GetValue(source);

                    // add the field to the ExpandoObject
                    ((IDictionary<string, object>)dataShapedObject)
                        .Add(propertyInfo.Name, propertyValue);
                }

                // return the list
                return dataShapedObject;

            }
        }
    }
}
