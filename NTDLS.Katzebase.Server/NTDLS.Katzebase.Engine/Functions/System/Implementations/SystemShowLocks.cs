﻿using NTDLS.Katzebase.Client.Payloads;
using NTDLS.Katzebase.Engine.Atomicity;
using fs;
namespace NTDLS.Katzebase.Engine.Functions.System.Implementations
{
    internal static class SystemShowLocks
    {
        public static KbQueryResultCollection Execute(EngineCore core, Transaction transaction, SystemFunctionParameterValueCollection function)
        {
            var collection = new KbQueryResultCollection();
            var result = collection.AddNew();

            result.AddField("ProcessId");
            result.AddField("Granularity");
            result.AddField("Operation");
            result.AddField("Object Name");

            var txSnapshots = core.Transactions.Snapshot();

            var processId = function.GetNullable<ulong?>("processId");
            if (processId != null)
            {
                txSnapshots = txSnapshots.Where(o => o.ProcessId == processId).ToList();
            }

            foreach (var tx in txSnapshots)
            {
                foreach (var heldLockKey in tx.HeldLockKeys)
                {

                    var values = new List<fstring?>
                    {
                        fstring.NewS(heldLockKey.ProcessId.ToString()),
                        fstring.NewS(heldLockKey.ObjectLock.Granularity.ToString()),
                        fstring.NewS(heldLockKey.Operation.ToString()),
                        fstring.NewS(heldLockKey.ObjectName.ToString()),
                    };
                    result.AddRow(values);
                }
            }

            return collection;
        }
    }
}
