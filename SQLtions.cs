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
        public Token Database;

        public SQLCreateDatabase(Token database)
        {
            Database = database;
        }

        public void Execute() 
        {
            string name;
            if (Objects.ContainsVariable(Database.View))
                name = Convert.ToString(Objects.GetVariable(Database.View));
            else
                name = Database.View;
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
        public Token TableName;
        public Token[] Types;
        public Token[] Names;

        public SQLCreateTable(Token tableName, Token[] types, Token[] names)
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

            string tableName;
            if (Objects.ContainsVariable(TableName.View))
                tableName = Convert.ToString(Objects.GetVariable(TableName.View));
            else
                tableName = TableName.View;

            try
            {
                JObject jObj = jsonData as JObject;
                jObj.Add(tableName, new JObject());

                JObject tableJobj = jObj[tableName] as JObject;
                string[] types = Types.Select(t => t.View).ToArray();
                string[] names = Names.Select(n => n.View).ToArray();

                tableJobj.Add("колонки", new JObject());
                JObject colonsJobj = tableJobj["колонки"] as JObject;

                for (int i = 0; i < types.Length; i++)
                    colonsJobj.Add(names[i], types[i]);

                tableJobj.Add("значения", new JObject());

                File.WriteAllText(database, JsonConvert.SerializeObject(jObj));
            } catch (ArgumentException) { throw new Exception($"ТАБЛИЦА С ИМЕНЕНМ <{tableName}> УЖЕ СУЩЕСТВУЕТ"); }
        }

        public override string ToString()
        {
            return $" СОЗДАТЬ ТАБЛИЦУ {TableName} {{ТИПЫ: {string.Join(", ", Types.Select(t => t.ToString()))};\n НАЗВАНИЯ: {string.Join(", ", Names.Select(n => n.ToString()))};}}";
        }
    }

    public sealed class SQLInsert : IStatement
    {
        public Token TableName;
        public Token[] Colons;
        public Token[] Values;

        public SQLInsert(Token tableName, Token[] colons, Token[] values)
        {
            TableName = tableName;
            Colons = colons;
            Values = values;
        }

        public void Execute()
        {
            string database = Convert.ToString(Objects.GetVariable("ИСПБД")) + ".pycdb";
            string data = File.ReadAllText(database);
            dynamic jsonData = JsonConvert.DeserializeObject(data);

            string tableName;
            if (Objects.ContainsVariable(TableName.View))
                tableName = Convert.ToString(Objects.GetVariable(TableName.View));
            else
                tableName = TableName.View;

     //       try
       //     {
                JObject jObj = jsonData as JObject;
                JObject tableJobj = jObj[tableName] as JObject;
                JObject valuesJobj = tableJobj["значения"] as JObject;
                JObject colonsJobj = tableJobj["колонки"] as JObject;

                string[] colonsNames = colonsJobj.Properties().Select(n => n.Name).ToArray();
                string[] colonsTypes = colonsJobj.Properties().Select(n => n.Value.ToString()).ToArray();

                string[] colonsReaded = Colons.Select(c => Objects.ContainsVariable(c.View) ? Convert.ToString(Objects.GetVariable(c.View)) : c.View).ToArray();
                string[] valuesReaded = Values.Select(v => Objects.ContainsVariable(v.View) ? Convert.ToString(Objects.GetVariable(v.View)) : v.View).ToArray();

                int value = 0;
                Dictionary<string, string> columnsToSave = new Dictionary<string, string>();
                
                if (colonsReaded.Length > 0)
                    for (int i = 0; i < colonsNames.Length; i++)
                        columnsToSave.Add(colonsNames[i], colonsReaded.Contains(colonsNames[i]) ? valuesReaded[value++] : "НИЧЕГО");
                else
                    for (int i = 0; i < colonsNames.Length; i++)
                        columnsToSave.Add(colonsNames[i], valuesReaded[value++]);

                foreach (var column in columnsToSave)
                    valuesJobj.Add(column.Value, column.Key);

                File.WriteAllText(database, JsonConvert.SerializeObject(jObj));
         //   } catch (Exception)  {  throw; }
        }

        public override string ToString()
        {
            return $"ДОБАВИТЬ В {TableName} КОЛОНКИ ({string.Join(", ", Colons.Select(c => c.ToString()))})\nЗНАЧАНИЯ({string.Join(", ", Values.Select(v => v.ToString()))})";
        }
    }
}
