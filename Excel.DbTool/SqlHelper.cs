using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Excel.DbTool
{
    public static class SqlHelper<T> where T :System.Data.IDbConnection,new()
    {
        public static Func<Action<System.Data.IDataReader, E>, List<E>> ExecuteReader<E>(string cnnstr, string cmdstr, params System.Data.IDataParameter[] parameters) where E : class, new()
        {
            return (Action<System.Data.IDataReader, E> handler) =>
            {
                List<E> lst = new List<E>();
                using (T cnn = new T())
                {
                    cnn.ConnectionString = cnnstr;
                    cnn.Open();
                    using (var cmd = cnn.CreateCommand())
                    {
                        cmd.CommandText = cmdstr;
                        if (parameters.Length > 0)
                            parameters.ToList().ForEach(x => cmd.Parameters.Add(x));
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                E entity = new E();
                                handler(reader, entity);
                                lst.Add(entity);
                            }
                        }
                    }
                }
                return lst;
            };
            
        }
        

        public static void ExecuteNonQuery(string cnnstr, string cmdstr,Func<System.Data.IDbCommand,bool> handler=null, params System.Data.IDataParameter[] parameters)
        {
            using (var cnn = new T())
            {
                cnn.ConnectionString = cnnstr;
                cnn.Open();
                using (var cmd = cnn.CreateCommand())
                {
                    cmd.CommandText = cmdstr;
                    if (parameters.Length > 0)
                        parameters.ToList().ForEach(x => cmd.Parameters.Add(x));
                    if (handler?.Invoke(cmd) ?? true)
                        cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
