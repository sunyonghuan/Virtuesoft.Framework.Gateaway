namespace Virtuesoft.Framework.Gateaway;
/// <summary>
/// 
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 添加接口集合
    /// 将所有的接口添加到 ServiceCollection 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddGateawayCollection(this IServiceCollection services)
    {
        GateawayDescriptorCollection gateaways = new GateawayDescriptorCollection();
        var gs = AppDomain.CurrentDomain.GetAssemblies()
                .Where(t => t.GetTypes().Where(c => (c.IsSubclassOf(typeof(GateawayBase)))).Any())
                .SelectMany(t => t.GetTypes().Where(c => c.IsSubclassOf(typeof(GateawayBase))));
        foreach (var g in gs)
        {
            foreach (var m in g.GetMethods(BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(t => !t.IsSpecialName))
            {
                var descriptor = new GateawayDescriptor(g, m);
                if (gateaways.Where(t => t.Path.Equals(descriptor.Path)).Any())
                    throw new Exception($"重复的接口:{descriptor.Path}");
                gateaways.Add(descriptor);
            }
            services.AddTransient(g);
        }
        services.AddSingleton(gateaways);
        return services;
    }
    /// <summary>
    /// 添加接口参数驱动
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddGateawayProviders(this IServiceCollection services)
    {
        var providers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(t => t.GetTypes().Where(t => t.IsSubclassOf(typeof(PrameterProviderBase)))
            .Select(x => x));
        var convertors = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(t => t.GetTypes().Where(x => typeof(IPrameterConvertor).IsAssignableFrom(x) && !x.IsInterface)
            .Select(x => x));
        var providerCollection = new PrameterProviderCollection();
        foreach (var provider in providers)
        {
            services.AddScoped(provider);
            providerCollection.TryAdd(provider.GetCustomAttribute<ContentTypeAttribute>()?.ContentType, provider);
        }
        foreach (var convertor in convertors)
            services.AddScoped(convertor);
        return services.AddSingleton(providerCollection)
            .AddHttpContextAccessor();
    }
    /// <summary>
    /// 添加接口
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddGateaway(this IServiceCollection services, Action<GateawayOptions> options = null)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        if (options is not null)
            services.Configure(options);
        return services
                .AddGateawayProviders()
                .AddGateawayCollection()
                .AddScoped<GateawayMiddleware>();
    }
    /// <summary>
    /// 添加接口认证授权
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddGateawayAuthentication(this IServiceCollection services, Action<GateawayAuthenticationOption> options) => services
        .AddGateawayAuthentication<DefaultAuthenticationService>(options);
    /// <summary>
    /// 添加接口认证授权
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddGateawayAuthentication<T>(this IServiceCollection services, Action<GateawayAuthenticationOption> options) where T : class, IGateawayAuthenticationService
        => services.Configure(options)
        .AddScoped<IGateawayAuthenticationService, T>()
            .AddScoped<GateawayAuthenticationMiddleware>();
}

