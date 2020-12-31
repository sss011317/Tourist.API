using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Services;
using System.Linq.Dynamic.Core;
namespace Tourist.API.Helper
{
    public static class IQueryableExtensions
    {
        //在C#中 我們通常會使用參數列表中傳入關鍵字this這種操作來處理方法或類的拓展
        //比方說現在我們現在要拓展的就是IQueryable<T> source 這就代表我們的ApplySort是對IQueryable的拓展
        public static IQueryable<T> ApplySort<T>(
            this IQueryable<T> source,
            string orderBy,
            Dictionary<string,PropertyMappingValue> mappingDictionary
            )
        {
            if(source == null)
            {
                throw new ArgumentException("source");
            }
            if(mappingDictionary == null)
            {
                throw new ArgumentException("mappingDictionary");
            }
            if (string.IsNullOrWhiteSpace(orderBy))
            {
                return source;
            }

            var orderByString = string.Empty;
            var orderByAfterSplit = orderBy.Split(',');

            foreach(var order in orderByAfterSplit)
            {
                var trimmedOrder = order.Trim();

                //通過字串' desc' 來判斷升冪還是降冪
                var orderDescending = trimmedOrder.EndsWith(" desc");
                //刪除升冪或是降冪字串 "asc" or "desc" 來獲得屬性的名稱
                var indexOfFirstSpace = trimmedOrder.IndexOf(" ");
                var propertyName = indexOfFirstSpace == -1
                    ? trimmedOrder
                    : trimmedOrder.Remove(indexOfFirstSpace);

                if (!mappingDictionary.ContainsKey(propertyName))
                {
                    throw new ArgumentException($"Key mapping for {propertyName} is missing!");
                }
                var propertyMappingValue = mappingDictionary[propertyName];
                if(propertyMappingValue == null)
                {
                    throw new ArgumentException("propertyMappingValue");
                }

                foreach(var destinationProperty in
                    propertyMappingValue.DestinationProperties.Reverse())
                {
                    //給IQueryable 添加排序字串
                    orderByString = orderByString +
                        (string.IsNullOrWhiteSpace(orderByString) ? string.Empty : ", ")
                        + destinationProperty
                        + (orderDescending ? " descending" : " ascending");
                }
            }
            return source.OrderBy(orderByString);
        }
    }
}
