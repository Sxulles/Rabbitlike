using AutoMapper;

namespace Rabbitlike.Utils.Mapping
{
    public static class AutomapperBase
    {
        public static IMapper Mapper;

        static AutomapperBase()
        {
            var config = new MapperConfiguration(cfg =>
            {
                (AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetExportedTypes())
                        .Where(t => t.GetInterfaces().Contains(typeof(IMapperBase)) && !t.IsGenericType) as List<Type> ?? [])
                        .ForEach(type =>
                        {
                            (type.GetMethod(nameof(IMapperBase.Mapping))
                                ?? type.GetInterface(nameof(IMapperBase))!.GetMethod(nameof(IMapperBase.Mapping))!)
                                .Invoke(Activator.CreateInstance(type), [cfg]);
                        });
            });

            Mapper = config.CreateMapper();
        }
    }
}
