using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

    public sealed class SQLCreateTable: IStatement
    {
        public IExpression Table;

        public SQLCreateTable(IExpression table)
        {
            Table = table;
        }

        public void Execute()
        {
            string database = Convert.ToString(Objects.GetVariable("ИСПБД")) + ".pycdb";
            string data = File.ReadAllText(database);
            dynamic jsonData = JsonConvert.DeserializeObject(data);
            string table = Convert.ToString(Table.Evaluated());

            try
            {
                var jObj = jsonData as JObject;
                jObj.Add(table, new JObject());
                File.WriteAllText(database, JsonConvert.SerializeObject(jObj));
            }
            catch (ArgumentException)
            {
                throw new Exception($"ТАБЛИЦА С ИМЕНЕНМ <{table}> УЖЕ СУЩЕСТВУЕТ");
            }
        }
    }
}
