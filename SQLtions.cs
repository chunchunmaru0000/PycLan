using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public override string ToString()
        {
            return $"СОЗДАТЬ БАЗУ ДАННЫХ <{Database}>";
        }
    }

    public sealed class SQLCreateTable: IStatement
    {
        public IExpression TableName;
        public Token[] Types;
        public Token[] Names;

        public SQLCreateTable(IExpression tableName, Token[] types, Token[] names)
        {
            TableName = tableName;
            Types = types;
            Names = names;
        }

        public void Execute()
        {
            string database = Convert.ToString(Objects.GetVariable("ИСПБД")) + ".pycdb";
            string data = File.ReadAllText(database);
            dynamic jsonData = JsonConvert.DeserializeObject(data);
            string tableName = Convert.ToString(TableName.Evaluated());

            try
            {
                var jObj = jsonData as JObject;
                jObj.Add(tableName, new JObject());
                
                var tableJobj = jObj[tableName] as JObject;
                string[] types = Types.Select(t => t.View).ToArray();
                string[] names = Names.Select(t => t.View).ToArray();

                tableJobj.Add("объявления", new JObject());
                var declareJobj = tableJobj["объявления"] as JObject;

                for (int i = 0; i < types.Length; i++)
                    declareJobj.Add(names[i], types[i]);

                Console.WriteLine(JsonConvert.SerializeObject(jObj));
                File.WriteAllText(database, JsonConvert.SerializeObject(jObj));
            } catch (ArgumentException) { throw new Exception($"ТАБЛИЦА С ИМЕНЕНМ <{tableName}> УЖЕ СУЩЕСТВУЕТ"); }
        }

        public override string ToString()
        {
            return $" СОЗДАТЬ ТАБЛИЦУ {TableName} {{ТИПЫ: {string.Join(", ", Types.Select(t => t.ToString()))};\n НАЗВАНИЯ: {string.Join(", ", Names.Select(t => t.ToString()))};}}";
        }
    }

     
}
