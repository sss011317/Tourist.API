using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tourist.API.Dtos;
using Tourist.API.Models;

namespace Tourist.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private Dictionary<string, PropertyMappingValue> _touristRoutePropertyMapping =
          new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
          {
               { "Id", new PropertyMappingValue(new List<string>(){ "Id" }) },
               { "Title", new PropertyMappingValue(new List<string>(){ "Title" })},
               { "Rating", new PropertyMappingValue(new List<string>(){ "Rating" })},
               { "OriginalPrice", new PropertyMappingValue(new List<string>(){ "OriginalPrice" })},
          };

        private IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(
                new PropertyMapping<TouristRouteDto, TouristRoute>(
                    _touristRoutePropertyMapping));
        }

        //Dictionary<字串類型,目標模型的屬性>
        //該函數是由一個數據元映射到另一個類型的目標，所以我們還需要給函數加上泛型定義
        public Dictionary<string, PropertyMappingValue> GetPropertyMapping
            <TSource, TDestination>()
        {
            //1.獲得匹配的映射對象
            var matchingMapping =
                _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();

            if (matchingMapping.Count() == 1)
            {
                return matchingMapping.First()._mappingDictionary;
                //若仍找到對象，就返回該結果的字典
            }
            //若找不到對象就拋出異常
            throw new Exception(
                $"Canot find exact property mapping instance for<{typeof(TSource)},{typeof(TDestination)}>");
        }

        public bool IsMappingExists<TSource,TDesination>(string fields)
        {
            var propertyMapping = GetPropertyMapping<TSource, TDesination>();
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            //逗號來分隔自段與字串
            var fieldAfterSplit = fields.Split(",");

            foreach(var field in fieldAfterSplit)
            {
                //去掉空格
                var trimmedField = field.Trim();
                //獲得屬性名稱字串
                var indexOfFirstSpace = trimmedField.IndexOf(" ");
                var propertuName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertuName))
                {
                    return false;
                }
            }
            return true;

        }
    }
}
