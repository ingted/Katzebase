﻿using NTDLS.Katzebase.Client.Payloads.RoundTrip;
using NTDLS.Katzebase.Engine.Interactions.Management;
using NTDLS.Katzebase.Engine.Query;
using NTDLS.ReliableMessaging;

namespace NTDLS.Katzebase.Engine.Interactions.APIHandlers
{
    /// <summary>
    /// Public class methods for handling API requests related to queries.
    /// </summary>
    public class QueryAPIHandlers : IRmMessageHandler
    {
        private readonly EngineCore _core;

        public QueryAPIHandlers(EngineCore core)
        {
            _core = core;

            try
            {
            }
            catch (Exception ex)
            {
                LogManager.Error($"Failed to instantiate query API handlers.", ex);
                throw;
            }
        }

        public KbQueryQueryExplainPlansReply ExecuteExplainPlans(RmContext context, KbQueryQueryExplainPlans param)
        {
            var session = _core.Sessions.GetSession(context.ConnectionId);
#if DEBUG
            Thread.CurrentThread.Name = $"KbAPI:{session.ProcessId}:{param.GetType().Name}";
            LogManager.Debug(Thread.CurrentThread.Name);
#endif

            var results = new KbQueryQueryExplainPlansReply();

            foreach (var statement in param.Statements)
            {
                session.SetCurrentQuery(statement);

                foreach (var preparedQuery in StaticQueryParser.PrepareBatch(statement, param.UserParameters))
                {
                    var intermediateResult = _core.Query.ExplainPlan(session, preparedQuery);

                    results.Add(intermediateResult);
                }
            }

            session.ClearCurrentQuery();

            return results;
        }

        public KbQueryQueryExplainPlanReply ExecuteExplainPlan(RmContext context, KbQueryQueryExplainPlan param)
        {
            var session = _core.Sessions.GetSession(context.ConnectionId);
#if DEBUG
            Thread.CurrentThread.Name = $"KbAPI:{session.ProcessId}:{param.GetType().Name}";
            LogManager.Debug(Thread.CurrentThread.Name);
#endif

            var results = new KbQueryQueryExplainPlanReply();

            session.SetCurrentQuery(param.Statement);

            foreach (var preparedQuery in StaticQueryParser.PrepareBatch(param.Statement, param.UserParameters))
            {
                var intermediateResult = _core.Query.ExplainPlan(session, preparedQuery);

                results.Add(intermediateResult);
            }

            session.ClearCurrentQuery();

            return results;
        }

        public KbQueryQueryExplainOperationsReply ExecuteExplainOperations(RmContext context, KbQueryQueryExplainOperations param)
        {
            var session = _core.Sessions.GetSession(context.ConnectionId);
#if DEBUG
            Thread.CurrentThread.Name = $"KbAPI:{session.ProcessId}:{param.GetType().Name}";
            LogManager.Debug(Thread.CurrentThread.Name);
#endif

            var results = new KbQueryQueryExplainOperationsReply();

            foreach (var statement in param.Statements)
            {
                session.SetCurrentQuery(statement);

                foreach (var preparedQuery in StaticQueryParser.PrepareBatch(statement, param.UserParameters))
                {
                    var intermediateResult = _core.Query.ExplainOperations(session, preparedQuery);

                    results.Add(intermediateResult);
                }
            }

            session.ClearCurrentQuery();

            return results;
        }

        public KbQueryQueryExplainOperationReply ExecuteExplainOperation(RmContext context, KbQueryQueryExplainOperation param)
        {
            var session = _core.Sessions.GetSession(context.ConnectionId);
#if DEBUG
            Thread.CurrentThread.Name = $"KbAPI:{session.ProcessId}:{param.GetType().Name}";
            LogManager.Debug(Thread.CurrentThread.Name);
#endif

            var results = new KbQueryQueryExplainOperationReply();

            session.SetCurrentQuery(param.Statement);

            foreach (var preparedQuery in StaticQueryParser.PrepareBatch(param.Statement, param.UserParameters))
            {
                var intermediateResult = _core.Query.ExplainOperations(session, preparedQuery);

                results.Add(intermediateResult);
            }

            session.ClearCurrentQuery();

            return results;
        }

        public KbQueryProcedureExecuteReply ExecuteStatementProcedure(RmContext context, KbQueryProcedureExecute param)
        {
            var session = _core.Sessions.GetSession(context.ConnectionId);
#if DEBUG
            Thread.CurrentThread.Name = $"KbAPI:{session.ProcessId}:{param.GetType().Name}";
            LogManager.Debug(Thread.CurrentThread.Name);
#endif

            session.SetCurrentQuery(param.Procedure.ProcedureName);
            var result = (KbQueryProcedureExecuteReply)_core.Query.ExecuteProcedure(session, param.Procedure);
            session.ClearCurrentQuery();

            return result;
        }

        public KbQueryQueryExecuteQueryReply ExecuteStatementQuery(RmContext context, KbQueryQueryExecuteQuery param)
        {
            var session = _core.Sessions.GetSession(context.ConnectionId);
#if DEBUG
            Thread.CurrentThread.Name = $"KbAPI:{session.ProcessId}:{param.GetType().Name}";
            LogManager.Debug(Thread.CurrentThread.Name);
#endif
            session.SetCurrentQuery(param.Statement);

            var results = new KbQueryQueryExecuteQueryReply();
            foreach (var preparedQuery in StaticQueryParser.PrepareBatch(param.Statement, param.UserParameters))
            {
                results.Add(_core.Query.ExecuteQuery(session, preparedQuery));
            }

            session.ClearCurrentQuery();

            return results;
        }

        public KbQueryQueryExecuteQueriesReply ExecuteStatementQueries(RmContext context, KbQueryQueryExecuteQueries param)
        {
            var session = _core.Sessions.GetSession(context.ConnectionId);
#if DEBUG
            Thread.CurrentThread.Name = $"KbAPI:{session.ProcessId}:{param.GetType().Name}";
            LogManager.Debug(Thread.CurrentThread.Name);
#endif
            var results = new KbQueryQueryExecuteQueriesReply();

            foreach (var statement in param.Statements)
            {
                session.SetCurrentQuery(statement);

                foreach (var preparedQuery in StaticQueryParser.PrepareBatch(statement, param.UserParameters))
                {
                    var intermediateResult = _core.Query.ExecuteQuery(session, preparedQuery);

                    results.Add(intermediateResult);
                }
            }

            session.ClearCurrentQuery();

            return results;
        }

        public KbQueryQueryExecuteNonQueryReply ExecuteStatementNonQuery(RmContext context, KbQueryQueryExecuteNonQuery param)
        {
            var session = _core.Sessions.GetSession(context.ConnectionId);
#if DEBUG
            Thread.CurrentThread.Name = $"KbAPI:{session.ProcessId}:{param.GetType().Name}";
            LogManager.Debug(Thread.CurrentThread.Name);
#endif

            session.SetCurrentQuery(param.Statement);

            var results = new KbQueryQueryExecuteNonQueryReply();
            foreach (var preparedQuery in StaticQueryParser.PrepareBatch(param.Statement, param.UserParameters))
            {
                results.Add(_core.Query.ExecuteNonQuery(session, preparedQuery));
            }

            session.ClearCurrentQuery();

            return results;
        }
    }
}
