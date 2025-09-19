using System.Data;
using Dapper;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.Data.DapperMappers;

public class CustomerJsonObjectTypeHandler : SqlMapper.TypeHandler<Settings>
{
    public override void SetValue(IDbDataParameter parameter, Settings value)
    {
        parameter.Value = (value == null)
            ? (object)DBNull.Value
            : JsonConvert.SerializeObject(value);
        parameter.DbType = DbType.String;
    }

    public override Settings Parse(object value)
    {
        return JsonConvert.DeserializeObject<Settings>(value.ToString()!)!;
    }
}