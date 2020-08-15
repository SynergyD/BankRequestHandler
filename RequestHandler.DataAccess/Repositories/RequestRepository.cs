using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Npgsql;
using RequestHandler.DataAccess.Entities;
using RequestHandler.DataAccess.Enums;

namespace RequestHandler.DataAccess.Repositories
{
    public class RequestRepository
    {
        private const string ConnectionString = "Server=localhost;Port=5432;Database=privatdb; User Id=postgres;Password=123";
        
        public int Save(Request request)
        {
            
            Array values = Enum.GetNames(typeof(Status));
            Random random = new Random();
            request.Status = (string)values.GetValue(random.Next(1, values.Length));
            
            using IDbConnection dbConnection = new NpgsqlConnection(ConnectionString);
            
            return dbConnection.ExecuteScalar<int>(SqlToInsertRequest(request).ToString());
        }

        
        public RequestDto Get(int id)
        {
            RequestDto request = new RequestDto();

            using (IDbConnection dbConnection = new NpgsqlConnection(ConnectionString))
            {
                request = dbConnection.Query<RequestDto>(SqlToGetById(id)).SingleOrDefault();
            }
            
            return request;
        }
        
        public List<RequestDtoo> GetByClient(RequestByClient requestByClient)
        {
            List<RequestDtoo> requests = new List<RequestDtoo>();
            
            using (IDbConnection dbConnection = new NpgsqlConnection(ConnectionString))
            {
                requests = dbConnection.Query<RequestDtoo>(SqlToGetByClient(requestByClient.Id,requestByClient.DepartmentAddress)).ToList();
            }
            
            return requests;
        }
        

        private string SqlToGetByClient(int client_id, string departmentAddress)
        {
            return $"select *from get_by_client_5({client_id}, '{departmentAddress}')";
        }

        private StringBuilder SqlToInsertRequest(Request request)
        {
            var sql = new StringBuilder();
            
            sql.Append("call insert_request2(");
            sql.Append($"'{request.ClientId}', ");
            sql.Append($"'{request.DepartmentAddress}', ");
            sql.Append($"'{request.Amount}', ");
            sql.Append($"'{request.IpAddress}', ");
            sql.Append($"'{request.Currency}', ");
            sql.Append($"'{request.Status}', 0)");

            return sql;
        }

        private string SqlToGetById(int id)
        {
            return $"call get_by_id_2({id});";
        }
    }
}