using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ts_get_safe
{
    class SafeParam
    {
        public string GenericDecl { get; set; }
        public string ParamDecl { get; set; }
        public string ObjName { get; set; }
        public string KeyName { get; set; }
        public string MyGenericConstraint { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("// this file has been auto-generated.");
            sb.AppendLine();
            sb.AppendLine("declare module \"ts-get-safe\" {");
            sb.AppendLine();
            sb.AppendLine("type GSArrEl<TKeys extends keyof TObj, TObj> = {[P in TKeys]: undefined[] & TObj[P]}[TKeys][number];");
            sb.AppendLine();

            GetPerms(6).ToList().ForEach(perm =>
            {
                var parameters = new List<SafeParam>();
                for (var i = 0; i < perm.Length; i++)
                {
                    var last = parameters.Count > 0 ? parameters.Last() : new SafeParam
                    {
                        KeyName = "keyof TObject",
                        MyGenericConstraint = "TObject",
                        ObjName = "TObject",
                    };
                    parameters.Add(GetParam(last, i, perm[i]));
                }
                sb.AppendLine(GetLine(parameters));
            });

            sb.AppendLine();
            sb.AppendLine("export default getSafe;");
            sb.AppendLine("}");
            sb.AppendLine();

            if (!Directory.Exists("lib"))
                Directory.CreateDirectory("lib");

            File.WriteAllText("lib/index.d.ts", sb.ToString());

            Console.WriteLine("Typings generated successfully...");
        }

        static string GetLine(IEnumerable<SafeParam> items)
        {
            string generic = String.Join(", ", items.Select(i => i.GenericDecl));
            string parms = String.Join(", ", items.Select(i => i.ParamDecl));
            string ret = items.Last().ObjName;

            var output = $"function getSafe<TObject, {generic}>(obj: TObject, {parms}): {ret};";
            return output;
        }

        static SafeParam GetParam(SafeParam last, int position, bool isArray)
        {
            string lastElementName = last.KeyName;
            string lastElementType = last.ObjName;

            if (isArray)
            {
                return new SafeParam
                {
                    GenericDecl = $"A{position} extends GSArrEl<{lastElementName}, {last.MyGenericConstraint}>",
                    ParamDecl = $"a{position}: number",
                    ObjName = "A" + position,
                    KeyName = "keyof A" + position,
                    MyGenericConstraint = "A" + position,
                };
            }
            else
            {
                return new SafeParam
                {
                    GenericDecl = $"P{position} extends keyof {lastElementType}",
                    ParamDecl = $"p{position}: P{position}",
                    ObjName = $"{lastElementType}[P{position}]",
                    KeyName = "P" + position,
                    MyGenericConstraint = lastElementType,
                };
            }
        }

        static IEnumerable<bool[]> GetPerms(int length)
        {
            string chars = "01";
            List<string> possible = new List<string>();
            void Dive(string prefix, int level)
            {
                level += 1;
                foreach (char c in chars)
                {
                    possible.Add(prefix + c);
                    if (level < length)
                    {
                        Dive(prefix + c, level);
                    }
                }
            }
            Dive("", 0);

            // IEnumerable<string> FillZeros(int len)
            // {
            //     var items = Enumerable.Range(0, len).Select(i => false).ToList();
            //     for (int m = 0; m < 2; m++)
            //     {
            //         for (int i = 0; i < len; i++)
            //         {
            //             yield return String.Join("", items.ToArray().Select(Convert.ToInt32));

            //             for (int x = i; x < len; x++)
            //             {
            //                 items[x] = !items[x];
            //                 yield return String.Join("", items.ToArray().Select(Convert.ToInt32));
            //                 items[x] = !items[x];
            //             }

            //             items[i] = !items[i];
            //         }
            //     }
            // };

            // var solid = Enumerable.Range(0, length + 1).SelectMany(FillZeros);
            // var b2 = Enumerable.Range(0, (int)Math.Pow(2, length)).Select(b10 => Convert.ToString(b10, 2));

            // var all = solid.Concat(b2).Distinct();

            return possible.Select(str => str.ToCharArray().Select(z => Convert.ToBoolean(int.Parse(z.ToString()))).ToArray());
        }
    }
}
