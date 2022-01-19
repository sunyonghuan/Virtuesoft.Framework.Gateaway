
//Gateaway 属性非必要,默认 小写 并移除gateaway字符
[Gateaway(Name ="user",Display ="用户接口")]
public class UserGateaway:GateawayBase
{
    ILogger<UserGateaway> Logger { get; }
    DataContext Db { get; }
    public UserGateaway(DataContext context,ILogger<UserGateaway> logger)
    {
        Logger=logger;
        Db = context;
    }

    //Gateaway 属性非必要,默认 小写 方法名
    [Gateaway(Name = "add", Display = "添加一个用户")]
    public async Task<object> Add()
    {
        try
        {
            var result= await Db.Accounts.AddAsync(new MemberAccount()
            {
                Name = $"N{new Random((int)DateTime.Now.Ticks).Next(1111, 9999)}",
                Role = Db.Roles.FirstOrDefault()
            });
            await Db.SaveChangesAsync();
            return Success(new {
                result.Entity.ID,
                result.Entity.No,
                result.Entity.Name,
                result.Entity.Phone,
                result.Entity.CreateTime
            });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, $"Add:{ex.Message}");
            throw;
        }

    }

    //Gateaway 属性非必要,默认 小写 方法名
    [Gateaway(Name="get", Display ="获取最新的用户")]
    public object Get()
    {
        var result = Db.Accounts
            .OrderByDescending(t => t.No)
            .Select(t => new
            {
                t.ID,
                t.Name,
                t.Phone,
                t.Age,
                t.No,
                t.Status,
                Role = new
                {
                    t.Role.ID,
                    t.Role.Name
                }
            })
            .FirstOrDefault();
        return Success(result);
    }

    //参数支持 [post]froms ,[post]json,[get]path;
    //需要注意 需要正确的 Content-Type
    [Gateaway(Name = "detail", Display = "根据ID 查询一个用户")]
    public dynamic Detail(string id) {
        var t = Db.Accounts.Include(t=>t.Role).FirstOrDefault(t=>t.ID==id);
        return new
        {
            t.ID,
            t.Name,
            t.Phone,
            t.Age,
            t.No,
            t.Status,
            Role = new
            {
                t.Role.ID,
                t.Role.Name
            }
        };
    }
}


