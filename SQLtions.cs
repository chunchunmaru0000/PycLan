using System;
using System.IO;
using Newtonsoft.Json;

namespace PycLan
{
    public sealed class SQLCreateDatabase : IStatement
    {
        public IExpression Database;

        public SQLCreateDatabase(IExpression database)
        {
            Database = database;
        }

        public void Execute() 
        {
            string name = Convert.ToString(Database.Evaluated());
            var database = new { бд = name };
            string json = JsonConvert.SerializeObject(database);
            File.WriteAllText($"{name}.pycdb", json);
        }
    }
}
