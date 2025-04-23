using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LabMaterials.DB
{
    public class PrimaryKeyManager
    {
        static int max_id;
        static int next_id = 0;
        static int incrementValue = 100;
        static object syncObj = new object();

        public static int GetNextId()
        {
            lock (syncObj)
            {
                if (next_id == 0 || (next_id == max_id - 1))
                {
                    var context = new LabDBContext();
                    PrimaryKey pk = context.PrimaryKeys.SingleOrDefault();
                    if (pk == null)
                    {
                        pk = new PrimaryKey();
                        pk.NextId = 1;
                        pk.TableName = "";
                        context.PrimaryKeys.Add(pk);
                        context.SaveChanges();
                    }

                    next_id = pk.NextId;
                    pk.NextId += incrementValue;
                    context.SaveChanges();
                    context.Dispose();
                    max_id = incrementValue + next_id;
                    return next_id;
                }
                else
                {
                    return ++next_id;
                }
            }
        }
    }

}
