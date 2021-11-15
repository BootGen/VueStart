using System;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace VueStart
{
    class ErrorHandlerService
    {
        private readonly IConfiguration configuration;

        public ErrorHandlerService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void OnException(Exception e, string data)
        {
            Error error = e.InnerException == null ? ExceptionToError(e) : ExceptionToError(e.InnerException);
            error.Data = data;
            using (var dbContext = new ApplicationDbContext(configuration))
            {
                dbContext.Database.EnsureCreated();
                dbContext.Errors.Add(error);
                dbContext.SaveChanges();
            }
        }

        private static Error ExceptionToError(Exception e)
        {
            var st = new StackTrace(e, true);
            var frame = st.GetFrame(0);
            return new Error {
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