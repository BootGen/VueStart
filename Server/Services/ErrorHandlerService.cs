using System;
using System.Diagnostics;

namespace VueStart
{
    class ErrorHandlerService
    {
        private readonly ApplicationDbContext dbContext;

        public ErrorHandlerService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void OnException(Exception e, string data)
        {
            ServerError error = e.InnerException == null ? ExceptionToError(e) : ExceptionToError(e.InnerException);
            error.Data = data;
            dbContext.ServerErrors.Add(error);
            dbContext.SaveChanges();
        }

        private static ServerError ExceptionToError(Exception e)
        {
            var st = new StackTrace(e, true);
            var frame = st.GetFrame(0);
            return new ServerError {
                DateTime = DateTime.UtcNow,
                Message = e.Message,
                StackTrace = e.StackTrace,
                File = frame.GetFileName(),
                Line = frame.GetFileLineNumber(),
                Source = e.Source,
                HResult = e.HResult
            };
        }
    }
}