namespace Rabbitlike.Utils.Mapping
{
    public static class ApplicationBaseMapper
    {
        public static ResultModel ToModel<ResultModel>(this object obj)
        {
            if (typeof(ResultModel) == typeof(bool))
                return (ResultModel)(object)true;
            return AutomapperBase.Mapper.Map<ResultModel>(obj);
        }
        public static ResultModel ToModel<T, ResultModel>(this T obj)
        {
            return AutomapperBase.Mapper.Map<ResultModel>(obj);
        }
    }
}
