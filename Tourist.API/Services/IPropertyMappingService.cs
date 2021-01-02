using System.Collections.Generic;

namespace Tourist.API.Services
{
    public interface IPropertyMappingService
    {
        Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>();
        bool IsMappingExists<TSource, TDesination>(string fields);

        //因為不知道接下來可能需要判斷的對象是誰，所以再給接口函數加上泛型
        bool IsPropertiesExists<T>(string fields);
    }
}