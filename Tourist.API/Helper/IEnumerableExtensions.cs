using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Tourist.API.Helper
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>( //ShapeData函數應該與數據元類型一致
            this IEnumerable<TSource> source, //this 代表就是IEnumerable數據元 而數據元列表類型要可以匹配任一類型
            string fileds //塑型字段輸出名稱
            )
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var expandoObjectList = new List<ExpandoObject>();
            //避免列表中遍歷數據，創建一個屬性訊息列表 使用C#反射機制PropertyInfo
            //PropertyInfo 將會包含對象屬性所有的訊息
            var propertyInfoList = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fileds))
            {
                //如果fileds為空 希望返回動態類型對象ExpandoObject所有的屬性
                var propertyInfos = typeof(TSource) //輸入對象的類型
                    .GetProperties(BindingFlags.IgnoreCase //通過調用GetProperties來獲取數據元TSource一系列的屬性訊息
                    |BindingFlags.Public | BindingFlags.Instance);
                //BindingFlags  搜索執行方式 IgnoreCase(忽略大小寫) Public(查找public方法) Instance(獲得Instance方法)
                //C#反射的小知識# 通過C#查找方法的時候默認只能查到public方法，如果想找到private的方法，必須使用BindingFlag
                //如果使用BindFlags.NonPublic，這個時候反射會找到包含private,proctect的成員變數

                propertyInfoList.AddRange(propertyInfos);
            }
            else
            {
                //逗號分隔字段的字串
                var filedsAfterSplit = fileds.Split(',');

                foreach(var filed in filedsAfterSplit)
                {
                    //去頭尾空格
                    var propertyName = filed.Trim();
                    //獲取單個屬性訊息
                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName, 
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null) throw new Exception($"屬性{propertyName} 找不到{typeof(TSource)}");
                    propertyInfoList.Add(propertyInfo);
                }
            }
            //遍歷所有的數據元，也就是函數第一個傳入的參數source
            foreach(TSource sourceObject in source)
            {
                //對於每一個數據元對象sourceObject 我們都要為他創建一個新的對象
                //這個對象就是動態類型對象，也就是數據塑型對象ExpandObject
                var dataShapeObject = new ExpandoObject();
                //需要嵌套一個for循環來遍歷生成的屬性訊息列表propertyInfoList，並在循環中輸入鄉對應的屬性
                foreach(var propertyInfo in propertyInfoList)
                {
                    //獲得對應屬性的真實數據
                    var propertyValue = propertyInfo.GetValue(sourceObject);
                    //把真實數據傳入到動態類型對象中
                    //將dataShapeObject轉化成字典 string代表屬性名稱字串 object代表屬性真實數據
                    ((IDictionary<string, object>)dataShapeObject)
                        .Add(propertyInfo.Name, propertyValue);
                }
                expandoObjectList.Add(dataShapeObject);
            }
            return expandoObjectList;
        }
    }
}
