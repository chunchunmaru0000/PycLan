using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PycLan
{
    public sealed class SQLCreateDatabaseStatement : IStatement
    {
        public Token Database;

        public SQLCreateDatabaseStatement(Token database)
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
            File.WriteAllText($"{name}.pycdb", json, System.Text.Encoding.UTF8);
        }

        public override string ToString() => $"СОЗДАТЬ БАЗУ ДАННЫХ <{Database}>";
    }

    public sealed class SQLCreateTableStatement : IStatement
    {
        public Token TableName;
        public Token[] Types;
        public Token[] Names;

        public SQLCreateTableStatement(Token tableName, Token[] types, Token[] names)
        {
            TableName = tableName;
            Types = types;
            Names = names;
        }

        public void Execute()
        {
            string database = Convert.ToString(Objects.GetVariable("ИСПБД")) + ".pycdb";
            string data = File.ReadAllText(database, System.Text.Encoding.UTF8);
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

                tableJobj.Add("значения", new JArray());

                File.WriteAllText(database, JsonConvert.SerializeObject(jObj), System.Text.Encoding.UTF8);
            } catch (ArgumentException) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ТАБЛИЦА С ИМЕНЕНМ <{tableName}> УЖЕ СУЩЕСТВУЕТ");
                Console.ResetColor();
              //  throw new Exception($"ТАБЛИЦА С ИМЕНЕНМ <{tableName}> УЖЕ СУЩЕСТВУЕТ"); 
            }
        }

        public override string ToString() => $" СОЗДАТЬ ТАБЛИЦУ {TableName} {{ТИПЫ: {string.Join(", ", Types.Select(t => t.ToString()))};\n НАЗВАНИЯ: {string.Join(", ", Names.Select(n => n.ToString()))};}}";
    }

    public sealed class SQLInsertStatement : IStatement
    {
        public IExpression TableName;
        public IExpression[] Colons;
        public IExpression[] Values;

        public SQLInsertStatement(IExpression tableName, IExpression[] colons, IExpression[] values)
        {
            TableName = tableName;
            Colons = colons;
            Values = values;
        }

        public void Execute()
        {
            string database = Convert.ToString(Objects.GetVariable("ИСПБД")) + ".pycdb";
            string data = File.ReadAllText(database, System.Text.Encoding.UTF8);
            dynamic jsonData = JsonConvert.DeserializeObject(data);

            string tableName = Convert.ToString(TableName.Evaluated());

            try
            {
                JObject jObj = jsonData as JObject;
                JObject tableJobj = jObj[tableName] as JObject;
                JObject colonsJobj = tableJobj["колонки"] as JObject;
                JArray valuesJobj = tableJobj["значения"] as JArray;

                string[] colonsNames = colonsJobj.Properties().Select(n => n.Name).ToArray();
                string[] colonsTypes = colonsJobj.Properties().Select(n => n.Value.ToString()).ToArray();

                string[] colonsReaded = Colons.Select(c => Convert.ToString(c.Evaluated())).ToArray();
                object[] valuesReaded = Values.Select(v => v.Evaluated()).ToArray();

                int value = 0;
                JArray toBeAdded = new JArray();
                if (colonsReaded.Length > 0)
                    for (int i = 0; i < colonsNames.Length; i++)
                    {
                        JObject temp = new JObject();
                        temp.Add("колонка", colonsNames[i]);
                        temp.Add("значение", JToken.FromObject(colonsReaded.Contains(colonsNames[i]) ? valuesReaded[value++] is bool ? (bool)valuesReaded[value - 1] ? "Истина" : "Ложь" : valuesReaded[value - 1] : "НИЧЕГО"));
                        toBeAdded.Add(temp);
                    }
                else
                    for (int i = 0; i < colonsNames.Length; i++)
                    {
                        // Console.WriteLine(PrintStatement.ListString(colonsNames.Select(c => (object)c).ToList())); Console.WriteLine(PrintStatement.ListString(valuesReaded.Select(c => c).ToList())); Console.WriteLine(i); Console.WriteLine(value); Console.WriteLine(colonsNames[i]); Console.WriteLine(valuesReaded[value]);
                        JObject temp = new JObject();
                        temp.Add("колонка", colonsNames[i]);
                        temp.Add("значение", JToken.FromObject(valuesReaded[value++] is bool ? (bool)valuesReaded[value-1] ? "Истина" : "Ложь" : valuesReaded[value-1]));
                        toBeAdded.Add(temp);
                    }

                valuesJobj.Add(toBeAdded);
                File.WriteAllText(database, JsonConvert.SerializeObject(jObj), System.Text.Encoding.UTF8);
            } catch (Exception e)  { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(e); Console.ResetColor(); }
        }

        public override string ToString() => $"ДОБАВИТЬ В {TableName} КОЛОНКИ ({string.Join(", ", Colons.Select(c => c.ToString()))})\nЗНАЧЕНИЯ({string.Join(", ", Values.Select(v => v.ToString()))})";
    }

    public sealed class SQLSelectExpression : IExpression
    {
        List<IExpression> Selections;
        List<IExpression> Ats;
        List<IExpression> Aliases;
        List<IExpression> Froms;
        IExpression Condition;

        public SQLSelectExpression(List<IExpression> selections, List<IExpression> ats, List<IExpression> aliases, List<IExpression> froms, IExpression condition)
        {
            Selections = selections;
            Ats = ats;
            Aliases = aliases;
            Froms = froms;
            Condition = condition;
        }

        public object Evaluated()
        {
            string[] selections = Selections.Select(s => Convert.ToString(s.Evaluated())).ToArray();
            // ats
            string[] ats = new string[selections.Length];
            for (int i = 0; i < selections.Length; i++)
                ats[i] = Convert.ToString(Ats[i].Evaluated());
            // aliases
            string[] aliases = new string[selections.Length];
            for (int i = 0; i < selections.Length; i++)
                aliases[i] = Aliases[i] == Parser.Nothingness ? selections[i] : Convert.ToString(Aliases[i].Evaluated());
            //froms
            string[] froms = Froms.Select(f => Convert.ToString(f.Evaluated())).ToArray();
            // begin setup vars
            string database = Convert.ToString(Objects.GetVariable("ИСПБД")) + ".pycdb";
            string data = File.ReadAllText(database, System.Text.Encoding.UTF8);
            dynamic jsonData = JsonConvert.DeserializeObject(data);
            JObject jObj = jsonData as JObject;
            // selected dicts
            object[] selected = aliases.Select(a => new List<object> { a, null }).ToArray();
            // single from
            if (froms.Length == 1)
            {
                JObject tableJobj = jObj[froms[0]] as JObject;
                JArray values = tableJobj["значения"] as JArray;

                for (int i = 0; i < selections.Length; i++)
                    if (selections[i] == "всё") // to be fixed
                        ((List<object>)selected[i])[1] = values.Select(v => (object)v.ToList().Select(t => (object)t["значение"]).ToList()).ToList();
                    else
                    {
                        JToken[][] valuesArray = values.Select(v => v.ToArray()).ToArray();
                        List<object> toBeAdded = new List<object>();
                        foreach (JToken[] value in valuesArray)
                        {
                            foreach (JToken token in value)
                                if ((string)token["колонка"] == selections[i])
                                    toBeAdded.Add(token["значение"]);
                            ((List<object>)selected[i])[1] = toBeAdded;
                        }
                    }
            }
            else // multiple from
            {
                try
                {
                    JObject[] tableJobj = froms.Select(f => jObj[f] as JObject).ToArray();
                    JArray[] values = tableJobj.Select(t => t["значения"] as JArray).ToArray();
                    Dictionary<string, JArray> fromToValues = new Dictionary<string, JArray>();
                    for (int i = 0; i < froms.Length; i++)
                        fromToValues.Add(froms[i], values[i]);

                    for (int i = 0; i < selections.Length; i++)
                        if (selections[i] == "всё") // to be fixed
                            ((List<object>)selected[i])[1] = fromToValues[ats[i]].Select(v => (object)v.ToList().Select(t => (object)t["значение"]).ToList()).ToList();
                        else
                        {
                            JToken[][] valuesArray = fromToValues[ats[i]].Select(v => v.ToArray()).ToArray();
                            List<object> toBeAdded = new List<object>();
                            foreach (JToken[] value in valuesArray)
                            {
                                foreach (JToken token in value)
                                    if ((string)token["колонка"] == selections[i])
                                        toBeAdded.Add(token["значение"]);
                                ((List<object>)selected[i])[1] = toBeAdded;
                            }
                        }
                }
                catch (KeyNotFoundException) { throw new Exception($"НЕ СУЩЕСТВУЕТ СТОЛБЦА С НЕКОТОРЫМ НАЗВАНИЕМ\nЭТО МОЖЕТ БЫТЬ ПО ПРИЧИНЕ НЕ УКАЗАНИЯ ИСТОЧНИКА ПРИ ВЫБОРЕ ИЗ НЕСКОЛЬКИХ ТАБЛИЦ"); }
            }

            if (Condition == null)
                return selected.ToList();
            else
            {
                throw new NotImplementedException("ДАУН4");
            }
        }

        public override string ToString() => $"ВЫБРАТЬ {string.Join(", ", Selections)} ИЗ {string.Join(", ", Froms)}" + (Condition != null ? "ГДЕ {Condition}" : "");
    }
}
